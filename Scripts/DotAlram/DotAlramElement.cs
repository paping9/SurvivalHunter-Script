using Defs;
using Message;
using VContainer;

namespace Alram
{
    public class DotAlramElement
    {
        public DotAlramType Type { get; private set; }
        public AlramLevel AlramLevel { get; private set; }
        public int Count { get; private set; }
        public int Value { get; private set; }

        private ISignalHub _signalHub;

        public DotAlramElement(DotAlramType type, AlramLevel level, int count, int value, ISignalHub signalHub)
        {
            Type = type;
            AlramLevel = level;
            Count = count;
            Value = value;
            _signalHub = signalHub;
        }

        public void UpdateCount(int count)
        {
            Count = count;
            _signalHub.Get<UpdateDotAlramMessage>().Dispatch(Type, Count, Value);
        }
    }
}