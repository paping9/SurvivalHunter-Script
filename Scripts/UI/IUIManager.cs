using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public interface IUIManager
    {
        UniTask OpenAsync(UIID uiId, UIParam param = null, Action<UIBase> onOpenStart = null, Action<UIBase> onOpenEnd = null, Action<UIBase> onCloseStart = null, Action<UIBase> onCloseEnd = null);
        void Close(UIID uiId, bool bForceHide = false, bool bNextPopup = true);
        void Remove(UIID uiId);
        Vector2 ConvertScreenToCanvasPoint(Vector2 vScreen);
        Vector2 ConvertWorldToCanvasPoint(Vector3 vWorld);
    }
}