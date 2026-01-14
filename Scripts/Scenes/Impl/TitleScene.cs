using Cysharp.Threading.Tasks;

using Bootstrap;
using Scene;
using UIController;
using UnityEngine;
using VContainer;

public class TitleScene : BaseScene
{
    private IBootstrapManager _bootstrapManager;
    private bool _bootstrapCompleted = false;

    /// <summary>
    /// Injects the bootstrap manager dependency into the scene.
    /// </summary>
    /// <param name="bootstrapManager">Instance of IBootstrapManager used to perform application bootstrap steps.</param>
    [Inject]
    public void Construct(IBootstrapManager bootstrapManager)
    {
        _bootstrapManager = bootstrapManager;
    }

    /// <summary>
    /// Initializes the scene, attaches message listeners, and begins the bootstrap sequence.
    /// </summary>
    /// <param name="sceneData">Optional data provided when the scene is started; may be null.</param>
    public override void OnSceneStart(SceneData sceneData = null)
    {
        base.OnSceneStart(sceneData);

        AddMessageListeners();
        // Bootstrap 시작
        StartBootstrap().Forget();
    }

    public override void RemoveScene()
    {
        _uiController.Clear();
        RemoveMessageListeners();
    }

    public override void BackKey()
    {
        // Title 씬에서는 뒤로가기 금지
        if (!_bootstrapCompleted)
            return;
            
        _uiController.Back().Forget();
    }

    /// <summary>
    /// Initiates the application bootstrap sequence: registers bootstrap steps, shows the title loading UI, runs the bootstrap process with progress and step callbacks, marks bootstrap completion, and navigates to the lobby on success.
    /// </summary>
    /// <remarks>
    /// Side effects:
    /// - Updates the UI to the title/loading controller.
    /// - Sets the internal <c>_bootstrapCompleted</c> flag to <c>true</c> when finished.
    /// - On successful completion, transitions the scene to the lobby.
    /// </remarks>
    private async UniTaskVoid StartBootstrap()
    {
        _bootstrapManager.Clear();

        // Bootstrap 단계 등록
        _bootstrapManager.RegisterSteps(
            new LoadConfigDataStep(),
            new LoadTableDataStep(),
            new ConnectServerStep(),
            new LoadUserDataStep(),
            new PreloadAssetStep()
        );

        // UI 표시 (로딩 화면)
        await _uiController.ChangeUIController(new UIControllerParam() 
        { 
            ControllerType = UIControllerType.Title 
        });

        // Bootstrap 실행
        bool success = await _bootstrapManager.StartBootstrap(
            onProgressChanged: OnBootstrapProgressChanged,
            onStepChanged: OnBootstrapStepChanged
        );

        _bootstrapCompleted = true;

        if (success)
        {
            Debug.Log("[TitleScene] Bootstrap completed successfully");
            // Lobby 씬으로 전환
            await UniTask.Delay(500); // UI 업데이트 대기
            _sceneManager.ChangeScene(ContentSceneType.Lobby, (result) => { });
        }
        else
        {
            Debug.LogError("[TitleScene] Bootstrap failed");
            // 오류 UI 표시
        }
    }

    private void OnBootstrapProgressChanged(float progress)
    {
        Debug.Log($"[TitleScene] Bootstrap Progress: {progress:P}");
        // UI 로딩 바 업데이트
        // Signals.Get<BootstrapProgressSignal>().Dispatch(progress);
    }

    private void OnBootstrapStepChanged(string stepName)
    {
        Debug.Log($"[TitleScene] Bootstrap Step: {stepName}");
        // UI 단계 표시 업데이트
        // Signals.Get<BootstrapStepChangedSignal>().Dispatch(stepName);
    }

    private void AddMessageListeners()
    {
    }

    private void RemoveMessageListeners()
    {
    }
}