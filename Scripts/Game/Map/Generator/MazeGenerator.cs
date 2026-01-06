using System;
using System.Collections.Generic;
using System.Linq;
using Defs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Extension;

namespace Game.Map
{
    #region Data Structures

    public enum RoomType
    {
        StartRoom,
        EndRoom,
        WayPointRoom,
        MainRoom,
        BranchRoom
    }
    
    [Serializable]
    public class Room
    {
        public int Id { get; private set; }
        public RoomType RoomType { get; private set; }
        public Vector2Int TopLeft { get; private set; }
        public int Width{ get; private set; }
        public int Height{ get; private set; }
        public HashSet<Vector2Int> Cells{ get; private set; }
        public Dictionary<Room, Vector2> Connections { get; private set; }

        public Room(Vector2Int topLeft, int width, int height, HashSet<Vector2Int> cells, int id, RoomType roomType)
        {
            this.Id = id;
            this.RoomType = roomType;
            this.TopLeft = topLeft;
            this.Width = width;
            this.Height = height;
            this.Cells = cells;
            this.Connections = new();
        }
        
        public List<DoorInfo> CreateDoorsFromRoom()
        {
            int unit = MapRoomConfig.RoomTileSize;
            List<DoorInfo> doors = new();

            foreach (var kvp in Connections)
            {
                Room other = kvp.Key;
                TileDirectionType dir = GetDirectionTo(kvp.Value);
                if(dir == TileDirectionType.None)
                    continue;
                
                int index = 0;
                
                if (dir == TileDirectionType.South || dir == TileDirectionType.North)
                {
                    index = (int)kvp.Value.x - TopLeft.x;
                }
                else
                {
                    index = (int)kvp.Value.y - TopLeft.y;
                }
                
                // 기준 좌표 계산
                Vector2Int pos = GetDoorStartPosition(this, dir, index, unit);
                doors.Add(new DoorInfo(pos, dir));
            }

            return doors;
        }
        
        private TileDirectionType GetDirectionTo(Vector2 position)
        {
            if(position.x == TopLeft.x) return TileDirectionType.West;
            if(position.x == TopLeft.x + Width) return TileDirectionType.East;
            if (position.y == TopLeft.y) return TileDirectionType.South;
            if(position.y == TopLeft.y + Height) return TileDirectionType.North;

            return TileDirectionType.None;
        }
        
        private Vector2Int GetDoorStartPosition(Room room, TileDirectionType dir, int index, int unit)
        {
            return dir switch
            {
                TileDirectionType.North => new Vector2Int(unit * index + 3, room.Height * unit - 1),
                TileDirectionType.South => new Vector2Int(unit * index + 3, 0),
                TileDirectionType.East  => new Vector2Int(room.Width * unit - 1, unit * index + 3),
                TileDirectionType.West  => new Vector2Int(0, unit * index + 3),
                _ => Vector2Int.zero
            };
        }
    }

    [Serializable]
    public struct MazeResult
    {
        public List<Vector2Int> StartPoints;
        public Vector2Int End;
        public List<Vector2Int> Waypoints;
        public Dictionary<int, List<Vector2Int>> MainPath;
        public Dictionary<int, List<Vector2Int>> BranchPaths;
        public List<Room> Rooms; // 하나로 통합

        public MazeResult(Vector2Int start, Vector2Int end, Dictionary<int, List<Vector2Int>> mainPath, List<Vector2Int> waypoints,
            Dictionary<int, List<Vector2Int>> branchPaths, List<Room> rooms)
        {
            this.StartPoints = new List<Vector2Int>() { start };
            this.End = end;
            this.MainPath = mainPath;
            this.Waypoints = waypoints;
            this.BranchPaths = branchPaths;
            this.Rooms = rooms;
        }
        
        public MazeResult(List<Vector2Int> starts, Vector2Int end, Dictionary<int, List<Vector2Int>> mainPath, List<Vector2Int> waypoints,
            Dictionary<int, List<Vector2Int>> branchPaths, List<Room> rooms)
        {
            this.StartPoints = new List<Vector2Int>(starts);
            this.End = end;
            this.MainPath = mainPath;
            this.Waypoints = waypoints;
            this.BranchPaths = branchPaths;
            this.Rooms = rooms;
        }
    }

    #endregion
    
    public static partial class MazeGenerator
    {
        public const int StartRoomID        = 1;
        public const int EndRoomID          = 10;
        public const int WayPointRoomID     = 100;
        public const int MainRoomID         = 1000;
        public const int BranchRoomID       = 2000;
        
        #region Maze Generation Functions
        
        // Start를 중앙 30% 영역에서 선택하고, 
        // S에서부터 랜덤하게 (10~20) 칸 이동하여 경로를 생성한 후, 마지막 셀을 E로 지정하는 함수
        public static (Vector2Int start, Vector2Int end, List<Vector2Int> path) ChooseStartEndCenter(int gridSize, int minSteps = 10, int maxSteps = 20)
        {
            int centerSize = Mathf.Max(1, Mathf.RoundToInt(gridSize * 0.3f));
            int rowMin = (gridSize - centerSize) / 2;
            int colMin = (gridSize - centerSize) / 2;
            Vector2Int start = new Vector2Int(UnityEngine.Random.Range(rowMin, rowMin + centerSize), UnityEngine.Random.Range(colMin, colMin + centerSize));
            
            List<Vector2Int> path = GenerateRandomWalkPathFixed(start, gridSize, minSteps, maxSteps);
            
            Vector2Int end = path[path.Count - 1];
            
            return (start, end, path);
        }

        // S에서 시작해, 10~20 칸 동안 랜덤하게 이동하는 경로를 생성합니다.
        // 이 함수는 DFS나 복잡한 로직 없이 단순 랜덤 워크 방식으로 경로를 생성합니다.
        public static List<Vector2Int> GenerateRandomWalkPathFixed(Vector2Int start, int gridSize, int minSteps = 10, int maxSteps = 20)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            path.Add(start);
            Vector2Int current = start;
            List<Vector2Int> directions = new List<Vector2Int> { new Vector2Int(-1,0), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(0,1) };
            int steps = UnityEngine.Random.Range(minSteps, maxSteps + 1);
            for (int i = 0; i < steps; i++)
            {
                List<Vector2Int> validMoves = new List<Vector2Int>();
                foreach (Vector2Int d in directions)
                {
                    Vector2Int nxt = current + d;
                    if (nxt.x >= 0 && nxt.x < gridSize && nxt.y >= 0 && nxt.y < gridSize && !path.Contains(nxt))
                        validMoves.Add(nxt);
                }
                if (validMoves.Count == 0)
                    return GenerateRandomWalkPathFixed(start, gridSize, minSteps, maxSteps);
                current = validMoves[UnityEngine.Random.Range(0, validMoves.Count)];
                path.Add(current);
            }
            return path;
        }
        
        // 3. WayPoint 생성 (3~7개), 기존에 생성된 S, E, WayPoints 와 거리가 Dist 이하면 생성 x
        public static List<Vector2Int> GenerateRandomWaypoints(int gridSize, int numPoints, List<Vector2Int> preserve, List<Vector2Int> mainPath, int dist)
        {
            HashSet<Vector2Int> points = new HashSet<Vector2Int>();
            while (points.Count < numPoints)
            {
                Vector2Int p = new Vector2Int(UnityEngine.Random.Range(0, gridSize), UnityEngine.Random.Range(0, gridSize));

                if (preserve.AnyWithinDistance(p, dist) || points.AnyWithinDistance(p, dist) || mainPath.AnyWithinDistance(p, dist))
                    continue;
                
                points.Add(p);
            }
            return new List<Vector2Int>(points);
        }
        
        // 메인 경로 상에서 point와의 Manhattan 거리가 최소인 셀 찾기
        public static Vector2Int GetNearestOnPath(Vector2Int point, List<Vector2Int> path)
        {
            Vector2Int nearest = new Vector2Int();
            int minDist = int.MaxValue;
            foreach (Vector2Int cell in path)
            {
                int d = Mathf.Abs(point.x - cell.x) + Mathf.Abs(point.y - cell.y);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = cell;
                }
            }
            return nearest;
        }
        
        // L-자 형태 연결
        public static List<Vector2Int> ConnectPoints(Vector2Int point, Vector2Int target)
        {
            List<Vector2Int> connPath = new List<Vector2Int> { point };
            if (UnityEngine.Random.value < 0.5f)
            {
                int step = target.y > point.y ? 1 : -1;
                for (int c = point.y + step; c != target.y + step; c += step)
                    connPath.Add(new Vector2Int(point.x, c));
                step = target.x > point.x ? 1 : -1;
                for (int r = point.x + step; r != target.x + step; r += step)
                    connPath.Add(new Vector2Int(r, target.y));
            }
            else
            {
                int step = target.x > point.x ? 1 : -1;
                for (int r = point.x + step; r != target.x + step; r += step)
                    connPath.Add(new Vector2Int(r, point.y));
                step = target.y > point.y ? 1 : -1;
                for (int c = point.y + step; c != target.y + step; c += step)
                    connPath.Add(new Vector2Int(target.x, c));
            }
            return connPath;
        }
        #endregion
        
        #region Room Expansion Functions
        
        // S, E 와 WayPoint에 대해 1x1 Room 생성
        public static (List<Room> preserveRooms, bool[,] occupancy) CreatePreserveRooms(int gridSize, HashSet<(Vector2Int, int, RoomType)> preservePoints)
        {
            bool[,] occ = new bool[gridSize, gridSize];
            List<Room> rooms = new List<Room>();
            int waypointCounter = 10;
            
            foreach ((Vector2Int pt, int id, RoomType roomType) in preservePoints)
            {
                occ[pt.x, pt.y] = true;
                rooms.Add(new Room(pt, 1, 1, new HashSet<Vector2Int> { pt }, id, roomType));
            }
            return (rooms, occ);
        }
        
        // includeEnd: false이면 E 위치에서 Room 생성하지 않음.
        public static (List<Room> rooms, bool[,] occupancy) ExpandPathToRooms(List<Vector2Int> path
                                                            , int gridSize
                                                            , HashSet<Vector2Int> preservePoints
                                                            , Vector2Int end
                                                            , bool[,] occ
                                                            , bool includeEnd
                                                            , ref int id
                                                            , RoomType roomType)
        {
            List<Room> rooms = new List<Room>();
            HashSet<Vector2Int> expanded = new HashSet<Vector2Int>();

            foreach (Vector2Int cell in path)
            {
                if (!includeEnd && cell == end)
                    continue;
                if (occ[cell.x, cell.y])
                    continue;
                if (expanded.Contains(cell))
                    continue;

                int w = 1, h = 1;
                HashSet<Vector2Int> roomCells = new HashSet<Vector2Int>();

                if (preservePoints.Contains(cell))
                {
                    roomCells.Add(cell);
                }
                else
                {
                    List<(int, int, HashSet<Vector2Int>)> possible = new List<(int, int, HashSet<Vector2Int>)>();
                    for (int W = 3; W >= 1; W--)
                    {
                        for (int H = 3; H >= 1; H--)
                        {
                            if (cell.x + W <= gridSize && cell.y + H <= gridSize)
                            {
                                bool conflict = false;
                                HashSet<Vector2Int> cellsInRoom = new HashSet<Vector2Int>();

                                for (int i = cell.x; i < cell.x + W; i++)
                                {
                                    for (int j = cell.y; j < cell.y + H; j++)
                                    {
                                        Vector2Int pos = new Vector2Int(i, j);
                                        cellsInRoom.Add(pos);
                                        if (occ[i, j])
                                        {
                                            conflict = true;
                                            break;
                                        }
                                    }
                                    if (conflict) break;
                                }

                                if (!conflict)
                                {
                                    // 정사각형 방일 경우 우선적으로 선택할 확률을 높임
                                    int weight = (W == H) ? 3 : 1;
                                    for (int k = 0; k < weight; k++)
                                        possible.Add((W, H, cellsInRoom));
                                }
                            }
                        }
                    }

                    if (possible.Count > 0)
                    {
                        var choice = possible[UnityEngine.Random.Range(0, possible.Count)];
                        w = choice.Item1;
                        h = choice.Item2;
                        roomCells = choice.Item3;
                    }
                    else
                    {
                        roomCells.Add(cell);
                    }
                }

                Room room = new Room(cell, w, h, roomCells, id++, roomType);
                rooms.Add(room);

                foreach (Vector2Int pos in roomCells)
                {
                    occ[pos.x, pos.y] = true;
                }

                expanded.UnionWith(roomCells);
            }
            return (rooms, occ);
        }
        
        // End는 별도로 Room 생성 안하도록 할 것이므로, 여기서는 E를 preservePoints에 포함하지 않음.
        public static bool IsInAnyRoom(Vector2Int point, List<Room> rooms)
        {
            foreach (Room room in rooms)
            {
                if (room.Cells.Contains(point))
                    return true;
            }
            return false;
        }
        
        public static void FindAdjacentRooms(List<Room> rooms)
        {
            foreach (var room in rooms)
            {
                foreach (var otherRoom in rooms)
                {
                    if (room == otherRoom) continue;

                    // 인접한 경우 (Manhattan 거리 1~2 이내)
                    if (AreRoomsAdjacent(room, otherRoom, out Vector2 connectionPoint))
                    {
                        room.Connections[otherRoom] = connectionPoint;
                    }
                }
            }
        }
        
        // 두 개의 방이 한쪽 면이 완전히 맞닿아 있는지 확인하고, 연결 지점을 계산
        private static bool AreRoomsAdjacent(Room a, Room b, out Vector2 connectionPoint)
        {
            connectionPoint = Vector2.zero;

            // 방 A의 경계를 가져옴
            int aLeft = a.TopLeft.x;
            int aRight = a.TopLeft.x + a.Width;
            int aTop = a.TopLeft.y;
            int aBottom = a.TopLeft.y + a.Height;

            // 방 B의 경계를 가져옴
            int bLeft = b.TopLeft.x;
            int bRight = b.TopLeft.x + b.Width;
            int bTop = b.TopLeft.y;
            int bBottom = b.TopLeft.y + b.Height;

            float centerA_X = aLeft + a.Width / 2f;
            float centerA_Y = aTop + a.Height / 2f;
            float centerB_X = bLeft + b.Width / 2f;
            float centerB_Y = bTop + b.Height / 2f;

            // 동쪽 면이 맞닿아 있는 경우
            if (aRight == bLeft && aTop < bBottom && aBottom > bTop)
            {
                float connectY = Mathf.Max(aTop, bTop) + (Mathf.Min(aBottom, bBottom) - Mathf.Max(aTop, bTop)) / 2f;
                connectionPoint = new Vector2(aRight, connectY);
                return true;
            }

            // 서쪽 면이 맞닿아 있는 경우
            if (bRight == aLeft && bTop < aBottom && bBottom > aTop)
            {
                float connectY = Mathf.Max(aTop, bTop) + (Mathf.Min(aBottom, bBottom) - Mathf.Max(aTop, bTop)) / 2f;
                connectionPoint = new Vector2(aLeft, connectY);
                return true;
            }

            // 남쪽 면이 맞닿아 있는 경우
            if (aBottom == bTop && aLeft < bRight && aRight > bLeft)
            {
                float connectX = Mathf.Max(aLeft, bLeft) + (Mathf.Min(aRight, bRight) - Mathf.Max(aLeft, bLeft)) / 2f;
                connectionPoint = new Vector2(connectX, aBottom);
                return true;
            }

            // 북쪽 면이 맞닿아 있는 경우
            if (bBottom == aTop && bLeft < aRight && bRight > aLeft)
            {
                float connectX = Mathf.Max(aLeft, bLeft) + (Mathf.Min(aRight, bRight) - Mathf.Max(aLeft, bLeft)) / 2f;
                connectionPoint = new Vector2(connectX, aTop);
                return true;
            }

            return false;
        }
        
        #endregion
        
        #region Public Maze Generation

        // Public으로 호출하면 MazeResult Struct를 반환
        public static MazeResult GenerateMaze(int gridSize, int minDist = 20, int maxDist = 30, int minWayPoint = 3, int maxWayPoint = 8, int wayPointDist = 3)
        {
            // 1. S, E 선택 (S는 중앙 30% 영역에서)
            (Vector2Int start, Vector2Int end, List<Vector2Int> mainPath) = ChooseStartEndCenter(gridSize, minDist, maxDist);

            // 2. S, E 를 preserve 로 설정
            HashSet<(Vector2Int, int, RoomType)> preserve = new HashSet<(Vector2Int, int, RoomType)>();
            preserve.Add((start, StartRoomID, RoomType.StartRoom));
            preserve.Add((end, EndRoomID, RoomType.EndRoom));

            Dictionary<int, List<Vector2Int>> mainPaths = new();
            mainPaths.Add(StartRoomID, mainPath);
            
            // 3. WayPoint 랜덤 생성
            int numWaypoints = UnityEngine.Random.Range(minWayPoint, maxWayPoint);
            int dist = wayPointDist;
            List<Vector2Int> waypoints = GenerateRandomWaypoints(gridSize, numWaypoints, preserve.Select(x => x.Item1).ToList(), mainPath, dist);
            
            int wayPointId = WayPointRoomID;
            preserve.UnionWith(waypoints.Select(wp => (wp, wayPointId++, RoomType.WayPointRoom)));
            
            // 4. Branch 생성: MainPath 및 BranchPath 위치에 점들 과 연결
            Dictionary<int, List<Vector2Int>> branchPaths = new();
            
            List<Vector2Int> path = new List<Vector2Int>(mainPath);
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
            int id = MainRoomID;
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
            
            return new MazeResult(start, end, mainPaths, waypoints, branchPaths, allRooms);
        }

        #endregion
        
        #region Logging

        public static void LogGrid(int gridSize, Vector2Int start, Vector2Int end, List<Room> mainRooms, List<List<Room>> branchRooms, List<Vector2Int> waypointPoints)
        {
            string log = "";
            string[,] grid = new string[gridSize, gridSize];
            for (int i = 0; i < gridSize; i++)
            for (int j = 0; j < gridSize; j++)
                grid[i, j] = "E";
            foreach (Room room in mainRooms)
            {
                foreach (Vector2Int pos in room.Cells)
                {
                    grid[pos.x, pos.y] = "MR";
                }
            }
            foreach (List<Room> branch in branchRooms)
            {
                foreach (Room room in branch)
                {
                    foreach (Vector2Int pos in room.Cells)
                    {
                        if (grid[pos.x, pos.y] == "E")
                            grid[pos.x, pos.y] = "BR";
                    }
                }
            }
            foreach (Vector2Int wp in waypointPoints)
            {
                grid[wp.x, wp.y] = "W";
            }
            grid[start.x, start.y] = "S";
            grid[end.x, end.y] = "E";
            for (int i = 0; i < gridSize; i++)
            {
                string row = "";
                for (int j = 0; j < gridSize; j++)
                {
                    row += grid[i, j].PadRight(4) + " ";
                }
                log += row + "\n";
            }
            Debug.Log(log);
        }

        #endregion
    }
}