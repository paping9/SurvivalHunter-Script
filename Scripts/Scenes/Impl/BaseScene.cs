using UnityEngine;
using Scene;

using UIController;

public class BaseScene : MonoBehaviour
{
    [SerializeField] private ContentSceneType _type;

    protected IUIControllerContainer _uiController;

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

    public virtual void Start()
    {
        RegistCurrentScene();
    }

    private void RegistCurrentScene()
    {
        SceneManager.Instance.SetCurrentContentsScene(this);
    }

    public virtual void OnSceneStart(IUIControllerContainer uiController, SceneData sceneData = null)
    {
        _uiController = uiController;
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
