using System;

namespace Defs
{
    public enum UnitType
    {
        None = 0,
        
        Hero,
        Monster,
        Prop,
        Gimmick,
        _Max_
    }

    public enum BuffType
    {
        None = 0,
        // buff
        Attack      = 1 << 0,
        Defence     = 1 << 1,
        Hp          = 1 << 2,
        Speed       = 1 << 3,
        Taunt       = 1 << 4,
        
        // debuff
        Stun        = 1 << 16,
        Groggy      = 1 << 17,
        KnockBack   = 1 << 18,
        KnockDown   = 1 << 19,
        Silence     = 1 << 20,
        Frozen      = 1 << 21,
    }

    public enum ElementType
    {
        None = 0, 
        Fire,
        Water,
    }

    public enum ActionButtonType : byte
    {
        None = 0,
        Dash = 1,
        Jump,
        Skill01,
        Skill02,
        Skill03,
        Skill04,
        Skill05
    }
    
    public static class UnitEnumHelperClass
    {
        public static bool HasBuff(this BuffType statusType, BuffType compare)
        {
            return statusType.HasFlag(compare);
        }
    }
}