using System.Collections.Generic;
using Defs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Map
{
    [CreateAssetMenu(fileName = "MapTheme", menuName = "SurvivalHunter/Map/MapTheme")]
    public class MapThemeInfo : ScriptableObject
    {
        [SerializeField] private string[] _tileKeys;
        [SerializeField] private string[] _decoKeys;
        [SerializeField] private string[] _trapKeys;
        [SerializeField] private string[] _doorKeys;
        [SerializeField] private MapThemeDoorTileDictionary _wallDics;

        private Dictionary<MapObjectType, string[]> _keyMap = null;
        public MapThemeDoorTileDictionary WallDict => _wallDics;

        public IReadOnlyList<string> GetKeys(MapObjectType type)
        {
            return type switch
            {
                MapObjectType.Tile => _tileKeys,
                MapObjectType.Deco => _decoKeys,
                MapObjectType.Trap => _trapKeys,
                MapObjectType.Door => _doorKeys,
                _ => null
            };
        }

        public void SetKeys(MapObjectType type, List<string> keys)
        {
            switch (type)
            {
                case MapObjectType.Tile: _tileKeys = keys.ToArray(); break;
                case MapObjectType.Deco: _decoKeys = keys.ToArray(); break;
                case MapObjectType.Trap: _trapKeys = keys.ToArray(); break;
                case MapObjectType.Door: _doorKeys = keys.ToArray(); break;
            }

            if (_keyMap == null) _keyMap = new();
            _keyMap[type] = keys.ToArray();
        }
        
        public void SetWallKey(WallType type, string key)
        {
            if (_wallDics.ContainsKey(type))
                _wallDics[type] = key;
            else
                _wallDics.Add(type, key);
        }

        public string GetWallKey(WallType type)
        {
            if (_wallDics.ContainsKey(type)) return _wallDics[type];
            return "";
        }

        public void Init()
        {
            _keyMap = new Dictionary<MapObjectType, string[]>
            {
                { MapObjectType.Tile, _tileKeys },
                { MapObjectType.Deco, _decoKeys },
                { MapObjectType.Trap, _trapKeys },
                { MapObjectType.Door, _doorKeys }
            };
        }

        public void SetKey(MapObjectType type, int index, string newKey)
        {
            if (_keyMap == null) Init();
            if (_keyMap.TryGetValue(type, out var list) && index >= 0 && index < list.Length)
            {
                list[index] = newKey;
            }
        }

        // 🔧 런타임 또는 생성 시 사용할 Key 반환 함수들
        public string GetFloorKey()
        {
            if (_tileKeys == null || _tileKeys.Length == 0) return null;
            
            int index = Random.Range(0, _tileKeys.Length);
            return _tileKeys[index];
        }

        public string GetDoorKey()
        {
            if (_doorKeys == null || _doorKeys.Length == 0) return null;
            
            int index = Random.Range(0, _doorKeys.Length);
            
            return _doorKeys[index];
        }

    }
}