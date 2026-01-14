using System.Collections.Generic;
using Game.Domain;

namespace Game.Repositories
{
    public class MissionRepository : IMissionRepository
    {
        private readonly Dictionary<int, Mission> _missions = new Dictionary<int, Mission>();

        public Mission GetMissionById(int id)
        {
            _missions.TryGetValue(id, out var mission);
            return mission;
        }

        public IEnumerable<Mission> GetAllMissions() => _missions.Values;

        public void AddMission(Mission mission) => _missions[mission.Id] = mission;

        public void UpdateMission(Mission mission) => _missions[mission.Id] = mission;

        public void DeleteMission(int id) => _missions.Remove(id);
    }
}