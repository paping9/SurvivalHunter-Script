using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Message;
using Scene;
using UIController;

public class TitleScene : BaseScene
{
    public override void OnSceneStart(IUIControllerContainer uiController, SceneData sceneData = null)
    {
        base.OnSceneStart(uiController);
        // UIController 에 Home UI 호출.
        _uiController.ChangeUIController(new UIControllerParam() { ControllerType = UIControllerType.Title }).Forget();
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
    }

    private void RemoveMessageListeners()
    {
    }

}
