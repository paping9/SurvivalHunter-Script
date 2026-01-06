using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundle
{
    public class AddressableLabels
    {
        [SerializeField] private List<string> _labels = new List<string>();
        public List<string> Labels { get => _labels; }

        public void CacheLableData(List<string> labels)
        {
            _labels = labels;
        }
    }
}
