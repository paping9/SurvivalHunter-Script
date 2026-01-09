using System;
using System.Collections.Generic;
using Defs;
using UnityEngine;
using Utils.Pool;
using VContainer; // 1. 네임스페이스 추가

namespace Game.Map
{
    public class MapRoom : MonoBehaviour
    {
        private IGenericPoolManager _poolManager;
        private Dictionary<MapObjectType, List<MapObject>> _objects = new();

        public MapRoomInfo Info { get; private set; }
        public Room Room { get; private set; }

        // 프로퍼티들은 그대로 유지
        public List<MapObject> Tiles => _objects.ContainsKey(MapObjectType.Tile) ? _objects[MapObjectType.Tile] : new List<MapObject>();
        public List<MapObject> Doors => _objects.ContainsKey(MapObjectType.Door) ? _objects[MapObjectType.Door] : new List<MapObject>();
        public List<MapObject> Walls => _objects.ContainsKey(MapObjectType.Wall) ? _objects[MapObjectType.Wall] : new List<MapObject>();

        // 2. [Inject] 메서드 추가 (생성자 주입과 비슷한 역할)
        // VContainer가 이 객체를 만들 때 자동으로 호출해줍니다.
        [Inject]
        public void Construct(IGenericPoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        // 3. Start() 제거 (이제 Construct에서 처리됨)
        // 만약 초기화 로직이 더 있다면 Start()를 써도 되지만, 의존성 가져오는 코드는 없어야 합니다.

        private void OnDestroy()
        {
            _poolManager = null;
            Room = null;
            Info = null;
        }

        public void SetData(Room room, MapRoomInfo info)
        {
            Room = room;
            Info = info;
        }

        public void AddMapObject(MapObjectType objectType, MapObject obj)
        {
            if (_objects.TryGetValue(objectType, out var list) == false)
                _objects.Add(objectType, list = new List<MapObject>());

            list.Add(obj);
        }

        public void ReleaseAll()
        {
            using var iter = _objects.GetEnumerator();
            while (iter.MoveNext())
            {
                var tiles = iter.Current.Value;
                foreach (var obj in tiles)
                {
                    // Null check 습관
                    if (obj == null) continue;

                    var poolObject = obj.GetComponent<PoolingComponent>();
                    if (poolObject != null)
                        poolObject.Return();
                    else
                        // DestroyImmediate는 에디터용입니다. 런타임에선 Destroy 권장.
                        Destroy(obj.gameObject);
                }
            }
            _objects.Clear(); // 리스트 클리어 추가 권장
        }
    }
}