using Defs;
using UnityEngine;

namespace Game
{
    public interface IUnitManager
    {
        void CreateUnit();
        void SpawnUnit(int unitId, Vector3 position);
        void DespawnUnit(int unitId);
        T GetUnit<T>(int unitId) where T : BaseUnit;
        void OnGameUpdate(float elapsedTime, float now);
        void Move(int unitId, Vector3 startPosition, Vector3 nextPosition, double time);
        void StopMove(int unitId, Vector3 position, Vector3 direction, double time);
        void ActionSkill(int unitId, ActionButtonType buttonType, Vector3 position, Vector3 direction, double startTime);
    }
}
