using System;

namespace Data
{
    public class StatValue : IEquatable<StatValue>
    {
        public StatType StatType { get; private set; }
        public int Value { get; private set; }
        private int _value;
        private int _ratio;

        public bool Equals(StatValue other)
        {
            return StatType == other.StatType;
        }

        public override bool Equals(object obj)
        {
            return obj is StatValue value && Equals(value);
        }

        public void Calculate()
        {
            Value = _value * _ratio;
        }

        public static bool operator ==(StatValue a, StatValue b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(StatValue a, StatValue b)
        {
            return !a.Equals(b);
        }

        public static StatValue operator +(StatValue a, StatValue b)
        {
            if (a.StatType != b.StatType)
            {
                throw new System.ArgumentException($"a:{a}, b:{b} is not equal Kind.");
            }

            var stat = new StatValue()
            {
                StatType = a.StatType,
                _value = a._value + b._value,
                _ratio = a._ratio + b._ratio,
            };

            stat.Calculate();

            return stat;
        }

        public static StatValue operator -(StatValue a, StatValue b)
        {
            if (a.StatType != b.StatType)
            {
                throw new System.ArgumentException($"a:{a}, b:{b} is not equal Kind.");
            }

            var stat = new StatValue()
            {
                StatType = a.StatType,
                _value = a._value - b._value,
                _ratio = a._ratio - b._ratio,
            };

            stat.Calculate();

            return stat;
        }

        public override int GetHashCode()
        {
            return this.StatType.GetHashCode();
        }
    }
}
