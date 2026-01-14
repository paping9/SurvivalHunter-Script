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

    /// <summary>
    /// Switches the active scene to the Title scene.
    /// </summary>
    private void StartTitle()
    {
       _sceneManager.ChangeScene(ContentSceneType.Title, (result) => { });
    }

    /// <summary>
    /// Unity callback invoked when the GameObject is destroyed; currently no cleanup is performed.
    /// </summary>
    public void OnDestroy()
    {
    }

    /// <summary>
    /// Per-frame update method invoked by Unity to advance game logic and handle frame-based processing.
    /// </summary>
    /// <remarks>
    /// Currently has no implementation; left intentionally empty for future per-frame logic.
    /// </remarks>
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