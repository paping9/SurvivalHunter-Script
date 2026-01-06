using System.Collections.Generic;
using UnityEngine;

namespace Utils.Collections
{
    public abstract class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private List<TKey> _keyData = new List<TKey>();

        [SerializeField, HideInInspector]
        private List<TValue> _valueData = new List<TValue>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for ( int i = 0; i < _keyData.Count && i < _valueData.Count; i++ )
            {
                this.Add( _keyData[i], _valueData[i] );
                //this[_keyData[i]] = _valueData[i];
            }

            _OnAfterDeserialize();

            _keyData.Clear();
            _valueData.Clear();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _keyData.Clear();
            _valueData.Clear();

            foreach ( var item in this )
            {
                _keyData.Add( item.Key );
                if ( item.Value is ISerializationCallbackReceiver receiverValue )
                {
                    receiverValue.OnBeforeSerialize();
                }
                _valueData.Add( item.Value );
            }

            _OnBeforeSerialize();
        }

        protected virtual void _OnAfterDeserialize() { }
        protected virtual void _OnBeforeSerialize() { }
    }
}