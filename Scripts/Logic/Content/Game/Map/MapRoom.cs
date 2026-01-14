using System;
using System.Collections.Generic;
using Defs;
using UnityEngine;
using Utils.Pool;
using VContainer;

namespace Game.Map
{
    public class MapRoom : MonoBehaviour
    {
        private IGenericPoolManager _poolManager;
        private Dictionary<MapObjectType, List<MapObject>> _objects = new();

        public MapRoomInfo Info { get; private set; }
        public Room Room { get; private set; }

        // ������Ƽ���� �״�� ����
        public List<MapObject> Tiles => _objects.ContainsKey(MapObjectType.Tile) ? _objects[MapObjectType.Tile] : new List<MapObject>();
        public List<MapObject> Doors => _objects.ContainsKey(MapObjectType.Door) ? _objects[MapObjectType.Door] : new List<MapObject>();
        public List<MapObject> Walls => _objects.ContainsKey(MapObjectType.Wall) ? _objects[MapObjectType.Wall] : new List<MapObject>();

        // 2. [Inject] �޼��� �߰� (������ ���԰� ����� ����)
        // VContainer�� �� ��ü�� ���� �� �ڵ����� ȣ�����ݴϴ�.
        [Inject]
        public void Construct(IGenericPoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        // 3. Start() ���� (���� Construct���� ó����)
        // ���� �ʱ�ȭ ������ �� �ִٸ� Start()�� �ᵵ ������, ������ �������� �ڵ�� ����� �մϴ�.

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
                    // Null check ����
                    if (obj == null) continue;

                    var poolObject = obj.GetComponent<PoolingComponent>();
                    if (poolObject != null)
                        poolObject.Return();
                    else
                        // DestroyImmediate�� �����Ϳ��Դϴ�. ��Ÿ�ӿ��� Destroy ����.
                        Destroy(obj.gameObject);
                }
            }
            _objects.Clear(); // ����Ʈ Ŭ���� �߰� ����
        }
    }
}