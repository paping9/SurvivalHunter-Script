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

        /// <summary>
        /// Initializes a DotAlramElement with the given type, severity level, count, value, and signal hub for dispatching updates.
        /// </summary>
        /// <param name="type">The dot alarm type.</param>
        /// <param name="level">The alarm severity level.</param>
        /// <param name="count">The initial count of dots.</param>
        /// <param name="value">The numeric value associated with the element.</param>
        /// <param name="signalHub">The signal hub used to obtain and dispatch update messages.</param>
        public DotAlramElement(DotAlramType type, AlramLevel level, int count, int value, ISignalHub signalHub)
        {
            Type = type;
            AlramLevel = level;
            Count = count;
            Value = value;
            _signalHub = signalHub;
        }

        /// <summary>
        /// Sets the element's count and notifies subscribers of the change.
        /// </summary>
        /// <param name="count">The new count value to assign to this element.</param>
        /// <remarks>After updating, an UpdateDotAlramMessage is dispatched via the configured signal hub with the element's Type, Count, and Value.</remarks>
        public void UpdateCount(int count)
        {
            Count = count;
            _signalHub.Get<UpdateDotAlramMessage>().Dispatch(Type, Count, Value);
        }
    }
}