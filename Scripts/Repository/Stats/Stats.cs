using System;
using System.Collections.Generic;

namespace Data
{
    public class Stats
    {
        private Dictionary<StatType, StatValue> _stats = new Dictionary<StatType, StatValue>();
        public int this[StatType type]
        {
            get
            {
                return _stats[type].Value;
            }
        }
    }
}
