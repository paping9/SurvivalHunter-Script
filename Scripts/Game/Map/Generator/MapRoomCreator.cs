using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Defs;
using Unity.VisualScripting;
using UnityEngine;
using Utils.Pool;

namespace Game.Map
{
    public class MapRoomCreator : IMapRoomCreator
    {
        private readonly IGenericPoolManager _genericPoolManager;
        private MapRoomDatabase _roomDatabase;
        
        public MapRoomCreator(IGenericPoolManager genericPoolManager)
        {
            _genericPoolManager = genericPoolManager;
        }

        public void Initialize(MapRoomDatabase roomDatabase)
        {
            _roomDatabase = roomDatabase;
        }
        
        public void ReleaseRoom()
        {
            
        }

        public MapRoom CreateRoom(ContentType contentType, Room room, MapThemeInfo mapThemeInfo, Transform parent = null)
        {
            MapRoomInfo roomInfo = _roomDatabase.GetRandom(contentType, room.RoomType, new Vector2Int(room.Width, room.Height));
            if (roomInfo == null)
            {
                return null;
            }
            var doorInfoMap = room.CreateDoorsFromRoom();

            return CreateRoom(roomInfo, mapThemeInfo, doorInfoMap, parent);
        }
        
        public MapRoom CreateRoom(MapRoomInfo roomInfo, MapThemeInfo themeInfo, List<DoorInfo> doorInfoMap, Transform parent = null)
        {
            GameObject root = new GameObject($"Room_{roomInfo.name}");
            var room = root.GetOrAddComponent<MapRoom>();
            if(parent != null) root.transform.SetParent(parent);

            Vector2Int size = roomInfo.MapSize;
            int[,] tempMap = new int[size.y, size.x];

            foreach (DoorInfo doorInfo in doorInfoMap)
            {
                doorInfo.WriteDoorInfo(ref tempMap);
                CreateDoor(doorInfo, themeInfo, room).Forget();
            }
            
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector2Int tilePos = new Vector2Int(x, y);
                    
                    CreateFloorTile(tilePos, themeInfo, room).Forget();
                    
                    if (roomInfo.IsSide(x, y) && tempMap[y, x] == 0)
                    {
                        if (IsCornerWall(x, y, size, tempMap))
                        {
                            CreateWall(tilePos, TileDirectionType.None, WallType.Corner, themeInfo, room).Forget();
                            tempMap[y, x] = 1;
                            continue;
                        }
                        
                        var direction = roomInfo.GetSideDirection(x, y);
                        Vector2Int offset = direction.ToOffset();
                        
                        bool next1Empty = InBounds(x + offset.x, y + offset.y, size) && tempMap[x + offset.x, y + offset.y] == 0;
                        bool next2Empty = InBounds(x + 2 * offset.x, y + 2 * offset.y, size) && tempMap[x + 2 * offset.x, y + 2 * offset.y] == 0;

                        if (next1Empty)
                        {
                            bool useTwo = next2Empty && UnityEngine.Random.value < 0.5f;

                            var wallType = useTwo ? WallType.TwoUnit : WallType.OneUnit;
                            CreateWall(tilePos, direction, wallType, themeInfo, room).Forget();
                            tempMap[y, x] = 1;
                            if (useTwo) tempMap[y + offset.y, x + offset.x] = 1;
                        }
                    }
                }
            }

            return room;
        }
        
        private async UniTask CreateFloorTile(Vector2Int pos, MapThemeInfo themeInfo, MapRoom room)
        {
            var floorKey = themeInfo.GetFloorKey();
            var tile = await LoadMapObject(floorKey, 500);
            tile.name = $"Floor_{pos.x}_{pos.y}";

            var worldPos = new Vector3(pos.x * tile.TileSize.x + 0.5f, 0f, pos.y * tile.TileSize.z + 0.5f);

            tile.transform.position = worldPos;
            tile.transform.SetParent(room.transform, worldPositionStays: false);
            
            room.AddMapObject(MapObjectType.Tile, tile);
        }

        private async UniTask CreateDoor(DoorInfo doorInfo, MapThemeInfo themeInfo, MapRoom room)
        {
            var doorKey = themeInfo.GetDoorKey();
            var tile =  await LoadMapObject(doorKey, 10);
            tile.name = $"Door_{doorInfo.Direction}";
            
            tile.transform.position = doorInfo.GetPosition();
            tile.transform.rotation = Quaternion.Euler(doorInfo.GetRotation());
            tile.transform.SetParent(room.transform, worldPositionStays: false);
            
            room.AddMapObject(MapObjectType.Door, tile);
        }

        private async UniTask CreateWall(Vector2Int pos, TileDirectionType direction, WallType wallType, MapThemeInfo themeInfo, MapRoom room)
        {
            var tileKey = themeInfo.GetWallKey(wallType);
            if (string.IsNullOrEmpty(tileKey)) return;
            
            var tile = await LoadMapObject(tileKey, 500);
            tile.name = $"Wall_{pos.x}_{pos.y}";
            
            var worldPos = new Vector3(pos.x * tile.TileSize.x + 0.5f, 0f, pos.y * tile.TileSize.z + 0.5f);
            
            tile.transform.position = worldPos;
            tile.transform.rotation = Quaternion.Euler(direction.ToRotation());
            tile.transform.SetParent(room.transform, worldPositionStays: false);
            
            room.AddMapObject(MapObjectType.Wall, tile);
        }
        
        //private async UniTask Create
        private bool IsCornerWall(int x, int y, Vector2Int size, int[,] map)
        {
            // 상하좌우 중 2개 이상이 막혀있고, 현재 위치는 열려있으면 코너로 간주
            int count = 0;
            if (!InBounds(x - 1, y, size) || map[y, x - 1] != 0) count++;
            if (!InBounds(x + 1, y, size) || map[y, x + 1] != 0) count++;
            if (!InBounds(x, y - 1, size) || map[y - 1, x] != 0) count++;
            if (!InBounds(x, y + 1, size) || map[y + 1, x] != 0) count++;

            return count >= 2;
        }

        private bool InBounds(int x, int y, Vector2Int size)
        {
            return x >= 0 && y >= 0 && x < size.x && y < size.y;
        }
        
        private async UniTask<MapObject> LoadMapObject(string key, int size)
        {
            if (Application.isPlaying)
            {
                return await _genericPoolManager.Get<MapObject>(key, size);
            }
            else
            {
#if UNITY_EDITOR
                // 에디터에서는 직접 에셋 로드
                string path = AssetBundle.AddressableAssetPath.CacheAssetPath.TryGetValue(key, out var p) ? p : null;
                if (!string.IsNullOrEmpty(path))
                {
                    var loaded = UnityEditor.AssetDatabase.LoadAssetAtPath<MapObject>(path);
                    var clone = GameObject.Instantiate(loaded);
                    return clone;
                }

                Debug.LogWarning($"[Editor] Addressable key not found: {key}");
                return null;
#endif
            }
        }
        
        private void InstantiateAtEdge(MapObject prefab, Vector2Int pos, TileDirectionType dir, Transform parent)
        {
            if (prefab == null) return;

            Vector3 position = GetEdgeWorldPosition(pos, dir);
            Quaternion rotation = GetRotationForDirection(dir);
            GameObject.Instantiate(prefab, position, rotation, parent);
        }
        
        private Vector3 GetEdgeWorldPosition(Vector2Int pos, TileDirectionType dir)
        {
            return dir switch
            {
                TileDirectionType.North => new Vector3(pos.x, 0, pos.y + 0.5f),
                TileDirectionType.South => new Vector3(pos.x, 0, pos.y - 0.5f),
                TileDirectionType.East  => new Vector3(pos.x + 0.5f, 0, pos.y),
                TileDirectionType.West  => new Vector3(pos.x - 0.5f, 0, pos.y),
                _ => Vector3.zero
            };
        }
        
        private Quaternion GetRotationForDirection(TileDirectionType dir)
        {
            return dir switch
            {
                TileDirectionType.North => Quaternion.Euler(0, 0, 0),
                TileDirectionType.South => Quaternion.Euler(0, 180, 0),
                TileDirectionType.East  => Quaternion.Euler(0, 90, 0),
                TileDirectionType.West  => Quaternion.Euler(0, 270, 0),
                _ => Quaternion.identity
            };
        }
    }
}