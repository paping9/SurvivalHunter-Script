using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Map
{
    public class MakeMazeTest : MonoBehaviour
    {
        [Header("Maze Parameters")]
        public int gridSize = 20;
        public int minDist = 20;
        public int maxDist = 30;
        public int minWayPoint = 3;
        public int maxWayPoint = 8;
        public int wayPointDist = 3;
        public int minStart = 4;
        public int maxStart = 8;
        
        [Header("Test")] 
        public bool isViewId = false;
        public bool isMainPath = false;
        public bool isBranchPath = false;
        
        private MazeResult _mazeResult;
        
        void OnDrawGizmos()
        {
            if (_mazeResult.MainPath == null || _mazeResult.MainPath.Count == 0)
                return;

            if (isMainPath)
            {
                // Draw Main Path (검정 선)
                Gizmos.color = Color.black;
                foreach (List<Vector2Int> mainPath in _mazeResult.MainPath.Values)
                {
                    for (int i = 0; i < mainPath.Count - 1; i++)
                    {
                        Vector3 a = new Vector3(mainPath[i].x + 0.5f, 0, mainPath[i].y + 0.5f);
                        Vector3 b = new Vector3(mainPath[i + 1].x + 0.5f, 0, mainPath[i + 1].y + 0.5f);
                        Gizmos.DrawLine(a, b);
                    }
                }
            }

            if (isBranchPath)
            {
                // Draw Branch Paths (주황색 선)
                Gizmos.color = new Color(1f, 0.5f, 0f);
                foreach (List<Vector2Int> branch in _mazeResult.BranchPaths.Values)
                {
                    for (int i = 0; i < branch.Count - 1; i++)
                    {
                        Vector3 a = new Vector3(branch[i].x + 0.5f, 0, branch[i].y + 0.5f);
                        Vector3 b = new Vector3(branch[i + 1].x + 0.5f, 0, branch[i + 1].y + 0.5f);
                        Gizmos.DrawLine(a, b);
                    }
                }
            }
            
            foreach (Room room in _mazeResult.Rooms)
            {
                Vector3 center = new Vector3(room.TopLeft.x + room.Width / 2f, 0, room.TopLeft.y + room.Height / 2f);
                Vector3 size = new Vector3(room.Width, 0.1f, room.Height );

                // RoomType에 따른 색상 설정
                switch (room.RoomType)
                {
                    case RoomType.StartRoom:
                        Gizmos.color = Color.green;
                        break;
                    case RoomType.EndRoom:
                        Gizmos.color = Color.magenta;
                        break;
                    case RoomType.WayPointRoom:
                        Gizmos.color = Color.blue;
                        break;
                    case RoomType.MainRoom:
                        Gizmos.color = Color.red;
                        break;
                    case RoomType.BranchRoom:
                        Gizmos.color = new Color(1f, 0.5f, 0f); // 주황색
                        break;
                }

                Gizmos.DrawWireCube(center, size);

                
                // 인접한 Room과 선 연결
                Gizmos.color = Color.white;
                foreach (var connection in room.Connections)
                {
                    Vector3 roomCenter = new Vector3(room.TopLeft.x + room.Width / 2f, 0, room.TopLeft.y + room.Height / 2f);
                    Vector3 connectionPoint = new Vector3(connection.Value.x, 0, connection.Value.y);
    
                    Gizmos.DrawLine(roomCenter, connectionPoint);
                    Gizmos.DrawSphere(connectionPoint, 0.1f); // 연결 위치에 작은 구 표시
                }
                
#if UNITY_EDITOR
                if(isViewId)
                    Handles.Label(center, $"ID: {room.Id} ({room.RoomType})");
#endif
            }
            
            // Draw Waypoints as blue cubes
            Gizmos.color = Color.blue;
            foreach (Vector2Int wp in _mazeResult.Waypoints)
            {
                Vector3 pos = new Vector3(wp.x + 0.5f, 0, wp.y + 0.5f);
                Gizmos.DrawCube(pos, Vector3.one * 0.8f);
            }
            
            // Draw S and E as spheres
            foreach (var start in _mazeResult.StartPoints)
            {
                Gizmos.color = Color.green;
                Vector3 sPos = new Vector3(start.x + 0.5f, 0, start.y + 0.5f);
                Gizmos.DrawSphere(sPos, 0.5f);
            }
            
            Gizmos.color = Color.magenta;
            Vector3 ePos = new Vector3(_mazeResult.End.x + 0.5f, 0, _mazeResult.End.y + 0.5f);
            Gizmos.DrawSphere(ePos, 0.5f);
            
        }

        [ContextMenu("MazeGenerate")]
        public MazeResult StartMazeGenerate()
        {
            _mazeResult = MazeGenerator.GenerateMaze(gridSize, minDist, maxDist, minWayPoint, maxWayPoint, wayPointDist);
            return _mazeResult;
        }
        
        [ContextMenu("MazeGenerate-Pvp")]
        public MazeResult StartMazeGeneratePvp()
        {
            _mazeResult = MazeGenerator.GenerateMazePvp(gridSize, minDist, maxDist, minStart, maxStart, minWayPoint, maxWayPoint, wayPointDist);
            return _mazeResult;
        }
    }
}