
using System.Collections.Generic;
using Game.Domain;

namespace Game.Repositories
{
    public interface IMissionRepository
    {
        Mission GetMissionById(int id);
        IEnumerable<Mission> GetAllMissions();
        void AddMission(Mission mission);
        void UpdateMission(Mission mission);
        void DeleteMission(int id);
    }
}