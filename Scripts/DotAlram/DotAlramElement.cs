using Defs;
using Message;

namespace Alram
{
    public class DotAlramElement
    {
        public DotAlramType Type { get; private set; }
        public AlramLevel AlramLevel { get; private set; }
        public int Count { get; private set; }
        public int Value { get; private set; }

        public DotAlramElement(DotAlramType type, AlramLevel level, int count, int value)
        {
            Type = type;
            AlramLevel = level;
            Count = count;
            Value = value;
        }

        public void UpdateCount(int count)
        {
            Count = count;
            Signals.Get<UpdateDotAlramMessage>().Dispatch(Type, Count, Value);
        }
    }
}