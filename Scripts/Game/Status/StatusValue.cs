using System;
using Defs;

namespace Game.Status
{
    public struct StatusValue
    {
        public StatusKind Kind { get; private set; }
        public double Value { get; private set; }

        public StatusValue(StatusKind kind)
        {
            this.Kind = kind;
            this.Value = 0.0;
        }

        public StatusValue(StatusKind kind, double value)
        {
            this.Kind = kind;
            this.Value = value;
        }

        public static StatusValue operator +(StatusValue a, StatusValue b)
        {
            if (a.Kind != b.Kind)
            {
                throw new System.ArgumentException($"a:{a}, b:{b} is not equal Kind.");
            }

            return new StatusValue()
            {
                Kind = a.Kind,
                Value = a.Value + b.Value,
            };
        }

        public static StatusValue operator -(StatusValue a, StatusValue b)
        {
            if (a.Kind != b.Kind)
            {
                throw new System.ArgumentException($"a:{a}, b:{b} is not equal Kind.");
            }

            return new StatusValue()
            {
                Kind = a.Kind,
                Value = a.Value - b.Value,
            };
        }
        
        public static StatusValue operator *(StatusValue a, double b)
        {
            return new StatusValue()
            {
                Kind = a.Kind,
                Value = a.Value * b,
            };
        }
        
        public static StatusValue operator *(StatusValue a, StatusValue b)
        {
            return new StatusValue()
            {
                Kind = a.Kind,
                Value = a.Value * b.Value,
            };
        }

        public override string ToString()
        {
            return $"{{{Kind.Type}, {Kind.ValueType}}}:{Value}";
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Kind.Type;
                hashCode = (hashCode * 397) ^ (int)Kind.ValueType;
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                return hashCode;
            }
        }
    }
    
    public readonly struct StatusKind : IEquatable<StatusKind>
    {
        public StatusType Type { get; }
        public StatusValueType ValueType { get; }

        public StatusKind(StatusType type, StatusValueType valueType)
        {
            Type = type;
            ValueType = valueType;
        }
        
        public static bool operator ==(StatusKind a, StatusKind b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(StatusKind a, StatusKind b)
        {
            return !a.Equals(b);
        }

        public bool Equals(StatusKind other)
        {
            return this.Type == other.Type
                   && this.ValueType == other.ValueType;
        }

        public override bool Equals(object obj)
        {
            return obj is StatusKind kind && Equals(kind);
        }

        public override int GetHashCode()
        {
            return (this.Type, this.ValueType).GetHashCode();
        }

        public override string ToString() => $"{Type}, {ValueType}";
    }
}