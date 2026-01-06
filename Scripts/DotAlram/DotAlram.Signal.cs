using Defs;
using Message;

namespace Alram
{
    /// <summary>
    /// 1: Type
    /// 2: count
    /// 3: id
    /// </summary>
    public class UpdateDotAlramMessage : ASignal<DotAlramType, int, int> { }
}