using Scene;
using UIController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BaseScene : MonoBehaviour
{
    [SerializeField] private ContentSceneType _type;

    protected IUIControllerContainer _uiController;
    protected ISceneManager _sceneManager;

    public ContentSceneType Type
    {
        get => _type;
    }

    public virtual string ThemaName
    {
        get => _type.ToString();
    }

    public virtual string SceneName
    {
        get => ThemaName;
    }

    [Inject]
    public void Construct(IUIControllerContainer uiController, ISceneManager sceneManager)
    {
        _uiController = uiController;
        _sceneManager = sceneManager;
    }


    public virtual void Start()
    {
        RegistCurrentScene();
    }

    private void RegistCurrentScene()
    {
        _sceneManager?.SetCurrentContentsScene(this);
    }

    public virtual void OnSceneStart(SceneData sceneData = null)
    {
    }

    public virtual void BackKey()
    {
        BackScene();
    }

    public virtual void RemoveScene()
    {

    }

    protected void BackScene()
    {

    }
}
