using System;
using Defs;
using UnityEngine;
using Utils;

namespace Game.Map
{
    [System.Serializable]
    public class DoorInfo
    {
        private Vector2Int[] _cellPosition;           // 기준 셀 (도어 시작 위치)
        public TileDirectionType Direction { get; private set; } // 문이 바라보는 방향 (방 기준)

        public DoorInfo(Vector2Int pos, TileDirectionType dir, int size = 4)
        {
            Direction = dir;
            _cellPosition = new Vector2Int[size];
            
            Vector2Int offset = GetOffset();
            for (int i = 0; i < size; i++)
            {
                _cellPosition[i] = pos + offset * i;
            }
        }

        public void WriteDoorInfo(ref int[,] mapTiles)
        {
            foreach (var cell in _cellPosition)
            {
                try
                {
                    mapTiles[cell.y, cell.x] = (int)MapObjectType.Door;
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log(e);
                }
                
            }
        }
        
        public Vector2Int GetOffset()
        {
            return Direction switch
            {
                TileDirectionType.North => new Vector2Int(1, 0),
                TileDirectionType.South => new Vector2Int(1, 0),
                TileDirectionType.East  => new Vector2Int(0, 1),
                TileDirectionType.West  => new Vector2Int(0, 1),
                _ => Vector2Int.zero
            };
        }

        public Vector3 GetRotation()
        {
            return Direction.ToRotation();
        }

        public Vector3 GetPosition()
        {
            return CommonUtility.GetTileCenterPosition(_cellPosition) + new Vector3(0.5f, 0f, 0.5f);
        }
        
        public bool HasDoor(int x, int y)
        {
            foreach (var cellPos in _cellPosition)
            {
                if (cellPos.x == x && cellPos.y == y) return true;
            }
            
            return false;
        }
    }
}