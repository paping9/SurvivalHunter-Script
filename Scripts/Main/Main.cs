using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

using UIController;
using Scene;
using Utils;

public class Main : SingletonMB<Main>
{
    private IUIControllerContainer _uiController;

    private void Start()
    {
        CreateController();
        InitializeManager();
        StartTitle();
    }

    private void CreateController()
    {
        _uiController = new UIControllerContainer();
    }

    private void InitializeManager()
    {
        SceneManager.Instance.Init(_uiController);
    }

    private void StartTitle()
    {
        SceneManager.Instance.ChangeScene(ContentSceneType.Title, (result) => { });
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
