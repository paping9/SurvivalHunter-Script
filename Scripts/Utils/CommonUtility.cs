using UnityEngine;

namespace Utils
{
    public static class CommonUtility
    {
        public static Vector3 GetTileCenterPosition(Vector2Int[] tilePositions)
        {
            int startIndex = 0;
            int endIndex = tilePositions.Length - 1;
            
            return new Vector3((tilePositions[startIndex].x + tilePositions[endIndex].x) * 0.5f, 0f, (tilePositions[startIndex].y + tilePositions[endIndex].y) * 0.5f);
        }
    }
}