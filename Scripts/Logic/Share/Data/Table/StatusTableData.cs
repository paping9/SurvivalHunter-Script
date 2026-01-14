using System;
using Defs;
using UnityEditor;
using UnityEngine;

namespace Data.Table
{
    [Serializable]
    public class StatusData : TableRaw
    {
        public StatusType StatusType;
        public StatusValueType ValueType;
        public int Value;
    }

    
}