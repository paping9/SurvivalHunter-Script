using System.Collections.Generic;
using UnityEngine;
using Defs;

namespace Game.Map
{
    [System.Serializable]
    public class MapRoomEntry
    {
        public ContentType ContentType;
        public RoomType RoomType;
        public Vector2Int Size;
        public List<MapRoomInfo> Rooms = new();
    }
    
    [CreateAssetMenu(fileName = "MapRoomDatabase", menuName = "Game/MapRoomDatabase")]
    public class MapRoomDatabase : ScriptableObject
    {
        [SerializeField] private List<MapRoomEntry> _entries = new();

        public MapRoomInfo GetRandom(ContentType content, RoomType type, Vector2Int size)
        {
            foreach (var entry in _entries)
            {
                if (entry.ContentType == content && entry.RoomType == type && entry.Size == size)
                {
                    if (entry.Rooms.Count > 0)
                        return entry.Rooms[Random.Range(0, entry.Rooms.Count)];
                }
            }
            return null;
        }
        
        public void Clear()
        {
            _entries.Clear();
        }

        public void AddRoom(ContentType content, RoomType type, Vector2Int size, MapRoomInfo room)
        {
            var entry = _entries.Find(e => e.ContentType == content && e.RoomType == type && e.Size == size);
            if (entry == null)
            {
                entry = new MapRoomEntry
                {
                    ContentType = content,
                    RoomType = type,
                    Size = size
                };
                _entries.Add(entry);
            }
            if (!entry.Rooms.Contains(room))
            {
                entry.Rooms.Add(room);
            }
        }
    }
}