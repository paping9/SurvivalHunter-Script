namespace Defs
{
    public enum StatusType
    {
        Health,        // 체력
        Mana,          // 마나
        EnergyShield,  // 에너지 쉴드
        
        AttackPower,   // 공격력
        CritRate,      // 치명타 확률
        CritDamage,    // 치명타 피해
        AttackSpeed,   // 공격 속도
        Accuracy,      // 명중률
        
        Defense,       // 방어력
        Evasion,       // 회피율
        BlockChance,   // 막기 확률
        
        MoveSpeed,     // 이동 속도
        CooldownReduction, // 쿨타임 감소
        
        Max
    }

    public enum StatusValueType
    {
        Point,      // 1 == 1
        Rate,       // 10000 == 100 %
    }
}