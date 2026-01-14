using Message;

namespace DotAlram
{
    /// <summary>
    /// DotAlram 업데이트 메시지
    /// 1: TypeId (int)
    /// 2: Count
    /// 3: Value (sub-category)
    /// </summary>
    public class UpdateDotAlramMessage : ASignal<int, int, int> { }
}