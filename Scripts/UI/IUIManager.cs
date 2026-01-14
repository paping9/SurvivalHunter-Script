using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public interface IUIManager
    {
        /// <summary>
/// Begins opening the UI identified by <paramref name="uiId"/>, passing <paramref name="param"/> to the UI and invoking lifecycle callbacks at the start and end of open and close sequences.
/// </summary>
/// <param name="uiId">Identifier of the UI to open.</param>
/// <param name="param">Optional parameters to provide to the UI instance.</param>
/// <param name="onOpenStart">Optional callback invoked with the UI instance at the start of the open sequence.</param>
/// <param name="onOpenEnd">Optional callback invoked with the UI instance at the end of the open sequence.</param>
/// <param name="onCloseStart">Optional callback invoked with the UI instance at the start of the close sequence.</param>
/// <param name="onCloseEnd">Optional callback invoked with the UI instance at the end of the close sequence.</param>
/// <returns>A UniTask that completes when the open sequence finishes.</returns>
UniTask OpenAsync(UIID uiId, UIParam param = null, Action<UIBase> onOpenStart = null, Action<UIBase> onOpenEnd = null, Action<UIBase> onCloseStart = null, Action<UIBase> onCloseEnd = null);
        /// <summary>
/// Closes the UI identified by <paramref name="uiId"/> and updates popup handling.
/// </summary>
/// <param name="uiId">Identifier of the UI to close.</param>
/// <param name="bForceHide">If true, forcibly hides the UI regardless of normal transition or state checks.</param>
/// <param name="bNextPopup">If true, proceeds to show the next queued popup after closing this UI; if false, leaves popup processing paused.</param>
void Close(UIID uiId, bool bForceHide = false, bool bNextPopup = true);
        /// <summary>
/// Removes the UI instance associated with the specified identifier from the manager and deregisters it.
/// </summary>
/// <param name="uiId">Identifier of the UI to remove.</param>
void Remove(UIID uiId);
        /// <summary>
/// Converts a point from screen space (pixels) to the UI canvas coordinate space.
/// </summary>
/// <param name="vScreen">Point in screen coordinates (pixels), where (0,0) is the bottom-left of the screen.</param>
/// <returns>Point in canvas coordinates suitable for positioning UI elements.</returns>
Vector2 ConvertScreenToCanvasPoint(Vector2 vScreen);
        /// <summary>
/// Converts a 3D world-space position to a 2D position on the UI canvas.
/// </summary>
/// <param name="vWorld">The world-space position to convert.</param>
/// <returns>The corresponding position in canvas (UI) coordinates.</returns>
Vector2 ConvertWorldToCanvasPoint(Vector3 vWorld);
    }
}