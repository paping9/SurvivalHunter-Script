using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Message;
using Scene;
using UIController;

public class LobbyScene : BaseScene
{
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

    private void AddMessageListeners()
    {
        Signals.Get<ChangeHomeMenuSignal>().AddListener(OnChangeHomeMenu);
    }

    private void RemoveMessageListeners()
    {
        Signals.Get<ChangeHomeMenuSignal>().RemoveListener(OnChangeHomeMenu);
    }

    private void OnChangeHomeMenu(UIControllerType uiControllerType)
    {

    }
}
