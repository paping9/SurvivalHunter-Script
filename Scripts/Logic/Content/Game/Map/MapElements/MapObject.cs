using Defs;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Pool;

namespace Game.Map
{
    public class MapObject : MonoBehaviour
    {
        [SerializeField] private MapObjectType _mapObjectType = MapObjectType.Max;
        [SerializeField] private Vector3 _tileSize = Vector3.one;
        [SerializeField] private MapRotationType _mapRotation = MapRotationType.R_0;

        public Vector3 TileSize => _tileSize;
        public MapObjectType MapObjectType => _mapObjectType;
        
        public void SetRotation(MapRotationType rotationType)
        {
            _mapRotation = rotationType;
            
            switch (_mapRotation)
            {
                case MapRotationType.R_0: this.transform.rotation = Quaternion.identity; break;
                case MapRotationType.R_90: this.transform.rotation = Quaternion.AngleAxis(90, Vector3.up); break;
                case MapRotationType.R_180: this.transform.rotation = Quaternion.AngleAxis(180, Vector3.up); break;
                case MapRotationType.R_270: this.transform.rotation = Quaternion.AngleAxis(270, Vector3.up); break;
            }
        }
    }
}