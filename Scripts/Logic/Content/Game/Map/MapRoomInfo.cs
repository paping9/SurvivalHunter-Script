using System.Collections.Generic;
using Defs;
using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu(fileName = "MapRoomInfo", menuName = "SurvivalHunter/Map/MapRoomInfo")]
    public class MapRoomInfo : ScriptableObject
    {
        private string _roomId;
        [SerializeField] private Vector2Int _size = Vector2Int.one;
        [SerializeField] private ContentType _contentType;
        [SerializeField] private RoomType _roomType;

        [Header("스폰 / 트리거 / 기타")]
        [SerializeField] private  List<SpawnData> _spawnDatas = new();
        [SerializeField] private  List<TriggerData> _triggerDatas = new();
        [SerializeField] private  List<PropData> _propDatas = new();

        public Vector2Int MapSize => _size * MapRoomConfig.RoomTileSize;
        public Vector2Int CellSize => _size;
        public RoomType RoomType => _roomType;
        public ContentType ContentType => _contentType;
        
        
        public bool IsSide(int x, int y)
        {
            int width  = _size.x * MapRoomConfig.RoomTileSize;
            int height = _size.y * MapRoomConfig.RoomTileSize;
    
            return x == 0 || y == 0 || x == width - 1 || y == height - 1;
        }
        
        public TileDirectionType GetSideDirection(int x, int y)
        {
            int width  = _size.x * MapRoomConfig.RoomTileSize;
            int height = _size.y * MapRoomConfig.RoomTileSize;

            if (y == height - 1) return TileDirectionType.North;
            if (y == 0)          return TileDirectionType.South;
            if (x == width - 1)  return TileDirectionType.East;
            if (x == 0)          return TileDirectionType.West;
    
            return TileDirectionType.None;
        }
        
        public List<DoorInfo> CreateDefaultDoorInfos()
        {
            var doorList = new List<DoorInfo>(_size.x * 2 + _size.y * 2);
            var unit = MapRoomConfig.RoomTileSize;
            for (int x = 0; x < _size.x; x++)
            {
                int cx = x * unit + 3;
                
                doorList.Add(new DoorInfo(new Vector2Int(cx, 0), TileDirectionType.South));
                doorList.Add(new DoorInfo(new Vector2Int(cx, MapSize.y - 1), TileDirectionType.North));
            }

            // 좌/우 (West/East)
            for (int y = 0; y < _size.y; y++)
            {
                int cy = y * unit + 3;
                doorList.Add(new DoorInfo(new Vector2Int(0, cy), TileDirectionType.West));
                doorList.Add(new DoorInfo(new Vector2Int(_size.x * unit - 1, cy), TileDirectionType.East));
            }

            return doorList;
        }
        
        
#if UNITY_EDITOR
        public void SetContentType(ContentType type) => _contentType = type;
        public void SetRoomType(RoomType type) => _roomType = type;
        
        public void SetSize(Vector2Int size)
        {
            _size = size;
        }   
#endif
    }
}