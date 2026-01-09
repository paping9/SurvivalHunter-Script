using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

using UIController;
using Scene;
using Utils;
using VContainer;
using VContainer.Unity;
using UI;

public class Main : SingletonMB<Main>
{
    private IUIControllerContainer _uiController;
    private ISceneManager _sceneManager;

    [Inject]
    public void Construct(IUIControllerContainer uiController, ISceneManager sceneManager)
    {
        _uiController = uiController;
        _sceneManager = sceneManager;
    }

    private void Start()
    {
        InitializeManager();
        StartTitle();
    }

    private void InitializeManager()
    {
        _sceneManager.Init();
    }

    private void StartTitle()
    {
       _sceneManager.ChangeScene(ContentSceneType.Title, (result) => { });
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void ResetGame()
    {

    }
}
