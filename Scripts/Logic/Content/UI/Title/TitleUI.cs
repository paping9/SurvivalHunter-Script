using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UIController;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class TitleUI : UIBase
    {
        [SerializeField] private Button _enterButton = null;

        public override void Init()
        {
            //_enterButton
        }

        public override void Remove()
        {

        }
    }
}
