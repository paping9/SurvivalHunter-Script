using System.Collections.Generic;
using Defs;
using UnityEngine;

namespace Game
{
    public class UnitManager : IUnitManager
    {
        private readonly Dictionary<int, BaseUnit> _unitContainer = new Dictionary<int, BaseUnit>();
        private int _playerId;
        /// <summary>
        /// Unit Table Id
        /// Status Info
        /// Costume Info
        /// Skill Info
        /// </summary>
        public void CreateUnit()
        {

        }

        public void SpawnUnit(int unitId, Vector3 position)
        {

        }

        public void DespawnUnit(int unitId)
        {

        }

        public T GetUnit<T>(int unitId) where T : BaseUnit
        {
            return _unitContainer[unitId] as T;
        }

        public void OnGameUpdate(float elapsedTime, float now)
        {

        }

        public void Move(int unitId, Vector3 startPosition, Vector3 nextPosition, double time)
        {
            if (_playerId == unitId) return;
            
        }

        public void StopMove(int unitId, Vector3 position, Vector3 direction, double time)
        {
            if (_playerId == unitId) return;
            
        }
        
        public void ActionSkill(int unitId, ActionButtonType buttonType, Vector3 position, Vector3 direction, double startTime)
        {
            if (_playerId == unitId) return;
            
        }
    }
}
