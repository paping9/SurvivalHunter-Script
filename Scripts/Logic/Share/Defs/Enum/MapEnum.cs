using UnityEngine;

namespace Defs
{
    public enum MapObjectType
    {
        Tile,
        Wall,
        Deco,
        Trap,
        Door,
        
        Max,
    }

    public enum WallType
    {
        OneUnit,      // 1칸짜리 기본 벽
        TwoUnit,      // 2칸짜리 벽
        Corner,       // 모서리용 벽
        TShape,       // T자형 벽 (optional)
        Cross         // +자형 (optional)
    }

    public enum MapRotationType
    {
        R_0,
        R_90,
        R_180,
        R_270,
    }
    
    public enum TileDirectionType
    {
        None,
        North, 
        South, 
        East, 
        West
    }

    public enum ContentType
    {
        Stage,
    }
    
    public static class TileExtensions
    {
        public static Vector2Int ToOffset(this TileDirectionType dir)
        {
            return dir switch
            {
                TileDirectionType.North => new Vector2Int(0, 1),
                TileDirectionType.South => new Vector2Int(0, -1),
                TileDirectionType.East  => new Vector2Int(1, 0),
                TileDirectionType.West  => new Vector2Int(-1, 0),
                _ => Vector2Int.zero
            };
        }

        public static Vector3 ToRotation(this TileDirectionType dir)
        {
            return dir switch
            {
                TileDirectionType.North => new Vector3(0, 0, 0),
                TileDirectionType.South => new Vector3(0, 180, 0),
                TileDirectionType.East => new Vector3(0, 90, 0),
                TileDirectionType.West => new Vector3(0, 270, 0),
                _ => Vector3.zero
            };
        }
    }
}