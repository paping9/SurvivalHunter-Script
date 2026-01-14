using System;
using System.Collections.Generic;

namespace Game
{
    public enum StateType
    {
        Idle,
        Move,
        Skill,
        Dash,
        
        KnockBack,
        KnockDown,
    }
    public class UnitState : IUnitComponent
    {
        private StateType _preState = StateType.Idle;
        private StateType _curState = StateType.Idle;
        private BaseUnit _owner = null;
        public void Init(BaseUnit unit)
        {
            _owner = unit;
        }
        
        public void Release()
        {
            _owner = null;
        }

        public void ChangeState(StateType stateType)
        {
            _preState = _curState;
            _curState = stateType;
        }

        public void GameUpdate(float elapsedTime)
        {

        }
        
        public void FixedUpdate(float elapsedTime)
        {

        }

        // cc기에 당했거나 스킬 사용 중이거나 CutScene 등 상태
        public bool IsWalkAble()
        {
            return _curState == StateType.Idle || _curState == StateType.Move;
        }

        // cc기에 당했거나 스킬 사용 중이거나 CutScene 등 상태 
        public bool IsSkillAble()
        {
            return false;
        }

        // 회피 스킬 사용중이거나 특수한 경우에는 무적
        public bool IsInvincibility()
        {
            return false;
        }
    }
}
