using System;
using System.Collections.Generic;
using Defs;
using UnityEngine;
using Utils.Pool;

namespace Game.Map
{
    public class MapRoom : MonoBehaviour
    {
        private IGenericPoolManager _poolManager;
        private Dictionary<MapObjectType, List<MapObject>> _objects = new();
        
        public MapRoomInfo Info { get; private set; }
        public Room Room { get; private set; }
        public List<MapObject> Tiles => _objects[MapObjectType.Tile];
        public List<MapObject> Doors => _objects[MapObjectType.Door];
        public List<MapObject> Walls => _objects[MapObjectType.Wall];

        private void Start()
        {
            _poolManager = SystemLocator.Get<IGenericPoolManager>();
        }

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
            if(_objects.TryGetValue(objectType,  out var list) == false)
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
                    var poolObject = obj.GetComponent<PoolingComponent>();
                    if(poolObject != null)
                        poolObject.Return();
                    else
                        DestroyImmediate(obj.gameObject);
                }
            }
        }
    }
}