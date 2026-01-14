using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Map
{
    public static partial class MazeGenerator
    {
        public static (List<Vector2Int> starts, Vector2Int end, List<List<Vector2Int>> paths) ChooseStartEndReversed(int gridSize, int minStarts = 4, int maxStarts = 8, int minSteps = 10, int maxSteps = 20)
        {
            // 🔹 End는 중심부 15% 영역에 배치
            int centerSize = Mathf.Max(1, Mathf.RoundToInt(gridSize * 0.15f));
            int rowMin = (gridSize - centerSize) / 2;
            int colMin = (gridSize - centerSize) / 2;
            
            Vector2Int end = new Vector2Int(
                UnityEngine.Random.Range(rowMin, rowMin + centerSize),
                UnityEngine.Random.Range(colMin, colMin + centerSize)
            );

            int numStarts = UnityEngine.Random.Range(minStarts, maxStarts + 1);
            List<Vector2Int> starts = new List<Vector2Int>();
            HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

            // 🔹 사사분면 정의
            int midX = gridSize / 2;
            int midY = gridSize / 2;
            List<BoundsInt> quadrants = new List<BoundsInt>
            {
                new BoundsInt(0, 0, 0, midX, midY, 0), // 1사분면 (좌상)
                new BoundsInt(midX, 0, 0, midX, midY, 0), // 2사분면 (우상)
                new BoundsInt(0, midY, 0, midX, midY, 0), // 3사분면 (좌하)
                new BoundsInt(midX, midY, 0, midX, midY, 0) // 4사분면 (우하)
            };

            // 🔹 각 사사분면에 최소 1개 이상의 Start 배치
            List<Vector2Int> quadrantStarts = new List<Vector2Int>();
            foreach (var quad in quadrants)
            {
                Vector2Int start = new Vector2Int(
                    UnityEngine.Random.Range(quad.xMin, quad.xMax),
                    UnityEngine.Random.Range(quad.yMin, quad.yMax)
                );
                starts.Add(start);
                usedPositions.Add(start);
            }

            // 🔹 추가 Start 배치 (랜덤)
            while (starts.Count < numStarts)
            {
                Vector2Int start = new Vector2Int(
                    UnityEngine.Random.Range(0, gridSize),
                    UnityEngine.Random.Range(0, gridSize)
                );

                if (!usedPositions.Contains(start) && !IsInCentralRegion(start, gridSize, 0.3f)) // 중앙 30% 제외
                {
                    starts.Add(start);
                    usedPositions.Add(start);
                }
            }

            // 🔹 각 Start 지점에서 End까지 경로 생성
            List<List<Vector2Int>> paths = new List<List<Vector2Int>>();
            foreach (var start in starts)
            {
                paths.Add(GenerateRandomWalkPathFixed(start, end, gridSize, minSteps, maxSteps));
            }

            return (starts, end, paths);
        }
        
        // 🔹 특정 지점이 중앙부(비율%) 안에 있는지 확인
        private static bool IsInCentralRegion(Vector2Int point, int gridSize, float centerRatio)
        {
            int centerSize = Mathf.RoundToInt(gridSize * centerRatio);
            int rowMin = (gridSize - centerSize) / 2;
            int colMin = (gridSize - centerSize) / 2;
            return point.x >= rowMin && point.x < rowMin + centerSize && point.y >= colMin && point.y < colMin + centerSize;
        }
        
        public static List<Vector2Int> GenerateRandomWalkPathFixed(Vector2Int start, Vector2Int end, int gridSize, int minSteps = 10, int maxSteps = 20)
        {
            List<Vector2Int> path = new List<Vector2Int> { start };
            Vector2Int current = start;

            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(-1, 0), new Vector2Int(1, 0), // 좌우 이동
                new Vector2Int(0, -1), new Vector2Int(0, 1)  // 상하 이동
            };

            int steps = UnityEngine.Random.Range(minSteps, maxSteps + 1);
            for (int i = 0; i < steps; i++)
            {
                List<Vector2Int> validMoves = new List<Vector2Int>();

                foreach (Vector2Int d in directions)
                {
                    Vector2Int nxt = current + d;

                    // 🔹 경로가 Grid 안에 있어야 하며, 이미 지나간 경로가 아니어야 함
                    if (nxt.x >= 0 && nxt.x < gridSize && nxt.y >= 0 && nxt.y < gridSize && !path.Contains(nxt))
                    {
                        validMoves.Add(nxt);
                    }
                }

                if (validMoves.Count == 0)
                    break;

                // 🔹 End 방향으로 조금 더 유도
                validMoves.Sort((a, b) => (a - end).sqrMagnitude.CompareTo((b - end).sqrMagnitude));

                current = validMoves[0];
                path.Add(current);

                // 🔹 목표 지점에 도달하면 종료
                if (current == end)
                    break;
            }

            return path;
        }
        
        public static MazeResult GenerateMazePvp(int gridSize, int minDist = 10, int maxDist = 20, int minStarts = 4, int maxStarts = 8, int minWayPoint = 3, int maxWayPoint = 8, int wayPointDist = 3)
        {
            // 🔹 여러 개의 Start 지점과 중앙 End 지점 배치
            (List<Vector2Int> starts, Vector2Int end, List<List<Vector2Int>> paths) = ChooseStartEndReversed(gridSize, minStarts, maxStarts, minDist, maxDist);
            List<Vector2Int> mainPath = new List<Vector2Int>();
            foreach (var tmpPath in paths)
            {
                mainPath.AddRange(tmpPath);
            }
            
            // 2. S, E 를 preserve 로 설정
            HashSet<(Vector2Int, int, RoomType)> preserve = new HashSet<(Vector2Int, int, RoomType)>();
            Dictionary<int, List<Vector2Int>> mainPaths = new();
            int id = StartRoomID;
            int index = 0;
            foreach (var start in starts)
            {
                mainPaths.Add(id, paths[index++]);
                preserve.Add((start, id++, RoomType.StartRoom));
            }
            
            preserve.Add((end, EndRoomID, RoomType.EndRoom));

            // 3. WayPoint 랜덤 생성
            int numWaypoints = UnityEngine.Random.Range(minWayPoint, maxWayPoint);
            int dist = wayPointDist;
            List<Vector2Int> waypoints = GenerateRandomWaypoints(gridSize, numWaypoints, preserve.Select(x => x.Item1).ToList(), mainPath, dist);
            
            int wayPointId = WayPointRoomID;
            preserve.UnionWith(waypoints.Select(wp => (wp, wayPointId++, RoomType.WayPointRoom)));

            
            // 4. Branch 생성: MainPath 및 BranchPath 위치에 점들 과 연결
            Dictionary<int, List<Vector2Int>> branchPaths = new();
            
            List<Vector2Int> path = new List<Vector2Int>();
            path.AddRange(mainPath);
            
            foreach (Vector2Int wp in waypoints)
            {
                Vector2Int nearest = GetNearestOnPath(wp, path);
                List<Vector2Int> branch = ConnectPoints(wp, nearest);
                var preserveData = preserve.Where(x => x.Item1 == wp).FirstOrDefault();
                branchPaths.Add(preserveData.Item2, path);
                path.AddRange(branch);
            }

            // 5. Create preserve rooms (S, WayPoint)
            List<Room> preserveRooms;
            bool[,] occupancy;
            (preserveRooms, occupancy) = CreatePreserveRooms(gridSize, preserve);

            // 6. Main Rooms 생성
            List<Room> mainRooms;
            id = MainRoomID;
            (mainRooms, occupancy) = ExpandPathToRooms(mainPath, gridSize, 
                new HashSet<Vector2Int>(preserve.Select(x => x.Item1)), 
                end, occupancy, includeEnd: false,
                ref id, RoomType.MainRoom);

            // 7. Branch Rooms 생성: Branch의 시작점이 이미 Main Room에 있으면 skip
            id = BranchRoomID;
            List<List<Room>> branchRooms = new List<List<Room>>();
            foreach (List<Vector2Int> branch in branchPaths.Values)
            {
                if (IsInAnyRoom(branch[0], mainRooms))
                    continue;
                List<Room> br;
                (br, occupancy) = ExpandPathToRooms(branch, gridSize,
                    new HashSet<Vector2Int>(preserve.Select(x => x.Item1)), 
                    end, occupancy, includeEnd: true,
                    ref id, RoomType.BranchRoom);
                
                branchRooms.Add(br);
            }

            List<Room> allRooms = new List<Room>();
            allRooms.AddRange(preserveRooms);
            allRooms.AddRange(mainRooms);
            
            foreach (var rooms in branchRooms)
            {
                allRooms.AddRange(rooms);
            }
            
            FindAdjacentRooms(allRooms);
            
            return new MazeResult(starts, end, mainPaths, waypoints, branchPaths, allRooms);
        }
    }
}