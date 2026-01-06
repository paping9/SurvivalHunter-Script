using System;
using UnityEngine;

namespace Data.Table
{
    public class TableRaw
    {
        public int ID;
    }
    
    public class BaseTableData<T> : ScriptableObject where T : TableRaw
    {
        [SerializeField] public T[] _data;
        [HideInInspector] [SerializeField] public string TableType;
        
        protected void OnEnable()
        {
            _data ??= Array.Empty<T>();
            TableType = typeof(T).Name;
        }

        public T GetData(int index)
        {
            if (_data == null || _data.Length == 0 || index < 0 || index >= _data.Length) return null;

            var data = _data[index];
            if (data is not T)
                throw new InvalidCastException($"Data at index {index} is not of type {typeof(T)}.");
            
            return data;
        }

        public T[] GetDataArray() => _data;

        public void SetData(T[] data)
        {
            _data = data;
        }
    }
}