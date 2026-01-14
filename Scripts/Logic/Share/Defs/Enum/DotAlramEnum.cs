namespace Defs
{
    public enum DotAlramCategory
    {
        Root,
        Skill,
        Status,
        Inventory,
        Shop,
    }

    /// <summary>
    /// DotAlram 타입 정의
    /// 사용 시: (int)DotAlramType.NewSkill 형태로 캐스팅하여 DotAlramController에 전달
    /// </summary>
    public enum DotAlramType
    {
        // 예시:
        // NewSkill = 1,
        // StatusUp = 2,
        // NewItem = 3,
        // ShopDiscount = 4,
    }
}