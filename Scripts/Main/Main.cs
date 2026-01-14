using UnityEngine;

using UIController;
using Scene;
using VContainer;

public class Main : MonoBehaviour
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

    public void OnDestroy()
    {
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
