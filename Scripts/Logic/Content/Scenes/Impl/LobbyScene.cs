using Cysharp.Threading.Tasks;

using Message;
using Scene;
using UIController;
using VContainer;

public class LobbyScene : BaseScene
{
    private ISignalHub _signalHub;

    [Inject]
    public void Construct(ISignalHub signalHub)
    {
        _signalHub = signalHub;
    }

    public override void OnSceneStart(SceneData sceneData = null)
    {
        base.OnSceneStart(sceneData);
        // UIController 에 Home UI 호출.
        _uiController.ChangeUIController(
            (int)UIControllerType.Home,
            new UIControllerContentParam() { ControllerType = UIControllerType.Home }
        ).Forget();
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
        //_signalHub.Get<ChangeHomeMenuSignal>().AddListener(OnChangeHomeMenu);
    }

    private void RemoveMessageListeners()
    {
        //_signalHub.Get<ChangeHomeMenuSignal>().RemoveListener(OnChangeHomeMenu);
    }

    private void OnChangeHomeMenu(UIControllerType uiControllerType)
    {

    }
}
