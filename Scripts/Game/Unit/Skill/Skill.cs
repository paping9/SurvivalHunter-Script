using System;
using System.Collections.Generic;

namespace Game.Unit
{
    public enum SkillType
    {
        Active,
        Passive
    }

    public class Skill
    {
        public int SkillId { get; private set; }
        public float Cooltime { get; private set; }

        public bool IsUseSkill()
        {
            return true;
        }

        public void UseSkill()
        {
            // 쿨타임 적용
        }

        // 조건에 따라 자동으로 발생하거나 제거되는

    }
}
