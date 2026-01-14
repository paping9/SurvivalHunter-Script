using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Message;
using Scene;
using UIController;
using VContainer;

public class LobbyScene : BaseScene
{
    private ISignalHub _signalHub;

    /// <summary>
    /// Injects the scene's ISignalHub dependency.
    /// </summary>
    /// <param name="signalHub">The signal hub used to subscribe and unsubscribe scene-level signals.</param>
    [Inject]
    public void Construct(ISignalHub signalHub)
    {
        _signalHub = signalHub;
    }

    /// <summary>
    /// Initializes the lobby scene, switches the UI to the Home controller, and registers message listeners.
    /// </summary>
    /// <param name="sceneData">Optional scene initialization data; pass null to use default initialization.</param>
    public override void OnSceneStart(SceneData sceneData = null)
    {
        base.OnSceneStart(sceneData);
        // UIController 에 Home UI 호출.
        _uiController.ChangeUIController(new UIControllerParam() { ControllerType = UIControllerType.Home }).Forget();
        AddMessageListeners();
    }

    public override void RemoveScene()
    {
        _uiController.Clear();
        RemoveMessageListeners();
    }

    public override void BackKey()
    {
        _uiController.Back().Forget();
    }

    /// <summary>
    /// Subscribes the lobby's OnChangeHomeMenu callback to the ChangeHomeMenuSignal from the signal hub.
    /// </summary>
    private void AddMessageListeners()
    {
        _signalHub.Get<ChangeHomeMenuSignal>().AddListener(OnChangeHomeMenu);
    }

    /// <summary>
    /// Unsubscribes the scene's ChangeHomeMenu listener from the signal hub.
    /// </summary>
    /// <remarks>
    /// Removes OnChangeHomeMenu as a listener for ChangeHomeMenuSignal so the scene no longer receives menu-change notifications.
    /// </remarks>
    private void RemoveMessageListeners()
    {
        _signalHub.Get<ChangeHomeMenuSignal>().RemoveListener(OnChangeHomeMenu);
    }

    /// <summary>
    /// Handles ChangeHomeMenuSignal events that indicate a change in the home UI menu.
    /// </summary>
    /// <param name="uiControllerType">The target UI controller type identifying which home menu should become active.</param>
    private void OnChangeHomeMenu(UIControllerType uiControllerType)
    {

    }
}