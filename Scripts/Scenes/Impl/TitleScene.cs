using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Bootstrap;
using Message;
using Scene;
using UIController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TitleScene : BaseScene
{
    private BootstrapManager _bootstrapManager;
    private bool _bootstrapCompleted = false;

    public override void OnSceneStart(SceneData sceneData = null)
    {
        base.OnSceneStart(sceneData);
        
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

    private async UniTaskVoid StartBootstrap()
    {
        _bootstrapManager = BootstrapManager.Instance;
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

        AddMessageListeners();
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
