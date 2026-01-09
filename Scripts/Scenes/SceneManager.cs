using AssetBundle;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UIController;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using VContainer;

namespace Scene
{
    public enum ContentSceneType
    {
        Title = 0,
        Lobby,
        Game,
        Empty,

        MAX
    }

    public class SceneStackData
    {
        private ContentSceneType _type;
        private string _name;
        
        public ContentSceneType Type { get { return _type; } }
        public string Name { get { return _name; } }

        public SceneStackData(ContentSceneType eType, string strName)
        {
            _type = eType;
            _name = strName;
        }
    }

    public interface ISceneManager
    {
        ContentSceneType CurrentSceneType { get; }
        float LoadingSceneProgress { get; }
        BaseScene CurrentContentScene { get; }
        void Init();
        void ClearLoadingState();
        void SetCurrentContentsScene(BaseScene currentContentsScene);
        void LoadEnviromentScene(string sceneName, Action<bool> result);
        void ChangeScene(ContentSceneType changeContentsScene, Action<bool> changeDone, SceneData sceneData = null);
        bool ReturnScene(System.Action<bool> changeDone = null);
        ContentSceneType GetPrevSceneBySceneStack();
        ContentSceneType GetPrevScene();
        bool IsCurrentSceneExist();
        bool IsOpenScene(ContentSceneType sceneType);

    }

    public class SceneManager : ISceneManager
    {
        private Stack<SceneStackData>   _sceneStack         = new Stack<SceneStackData>();
        private SceneStackData          _curScene           = null;
        private ContentSceneType        _prevSceneType      = ContentSceneType.MAX;
        private SceneData               _sceneData          = null;

        private BaseScene _currentContentScene = null;
        public BaseScene CurrentContentScene
        {
            get => _currentContentScene;
        }

        private bool _isAsyncOperationStarted = false;
        private float _asyncOperationProgress = 0;

        public ContentSceneType CurrentSceneType
        {
            get => _curScene.Type;
        }

        private string _currentCustomEnvironmentSceneName = "";
        public string CustomEnvironmentSceneName { get => _currentCustomEnvironmentSceneName; }

        public float LoadingSceneProgress
        {
            get
            {
                if (_isAsyncOperationStarted)
                    return _asyncOperationProgress;
                else
                    return 0;
            }
        }

        public void Init()
        {

        }

        public void ClearLoadingState()
        {
            _isAsyncOperationStarted = false;
            _asyncOperationProgress = 0;
        }

        public void SetCurrentContentsScene(BaseScene currentContentsScene)
        {
            if (_curScene != null)
                _prevSceneType = _curScene.Type;

            _curScene = new SceneStackData(currentContentsScene.Type, currentContentsScene.SceneName);
            _currentContentScene = currentContentsScene;
            currentContentsScene.OnSceneStart(_sceneData);
        }

        public void LoadEnviromentScene(string sceneName, Action<bool> result)
        {
            if (string.IsNullOrEmpty(sceneName) == true)
            {
                sceneName = "Environment_Default";
            }

            if (string.IsNullOrEmpty(_currentCustomEnvironmentSceneName) == false)
            {
                UnloadScene(_currentCustomEnvironmentSceneName, (bool b) => {
                    _currentCustomEnvironmentSceneName = sceneName;
                    LoadScene(_currentCustomEnvironmentSceneName, result, null);
                });
            }
            else
            {
                _currentCustomEnvironmentSceneName = sceneName;
                LoadScene(_currentCustomEnvironmentSceneName, result, null);
            }
        }

        public void ChangeScene(ContentSceneType changeContentsScene, Action<bool> changeDone, SceneData sceneData = null)
        {
            SceneStackData fromScene = _curScene;

            _sceneData = sceneData;
            _isAsyncOperationStarted = false;
            _asyncOperationProgress = 0;

            if (null != _curScene)
            {
                if (string.IsNullOrEmpty(_currentCustomEnvironmentSceneName) == false)
                {
                    UnloadScene(_currentCustomEnvironmentSceneName, (bool b) => {
                        _currentCustomEnvironmentSceneName = "";
                        UnloadScene(_curScene.Name, (result) => {
                            if (result == true)
                            {
                                ChangeSceneStart(changeContentsScene, fromScene, changeDone);
                            }
                        });
                    });
                }
                else
                {
                    UnloadScene(_curScene.Name, (result) => {
                        if (result == true)
                        {
                            ChangeSceneStart(changeContentsScene, fromScene, changeDone);
                        }
                    });
                }
            }
            else
            {
                ChangeSceneStart(changeContentsScene, fromScene, changeDone);
            }
        }

        private void ChangeSceneStart(ContentSceneType changeContentsScene, SceneStackData fromScene, Action<bool> changeDoneCallback)
        {
            _isAsyncOperationStarted = false;
            _asyncOperationProgress = 0;

            ChangeLoadSceneStart(changeContentsScene, fromScene, changeDoneCallback);
        }

        private void ChangeLoadSceneStart(ContentSceneType changeContentsScene, SceneStackData fromScene, Action<bool> changeDoneCallback)
        {
            LoadScene(SceneHelper.GetSceneName(changeContentsScene), (loadResult) =>
            {
                if (loadResult)
                {

                }
                else
                {

                }

                if (null != changeDoneCallback)
                {
                    changeDoneCallback(loadResult);
                }
            },
            (progress) =>
            {
                _asyncOperationProgress = progress;
            });
            _isAsyncOperationStarted = true;
        }

        private void UnloadScene(string sceneName, System.Action<bool> result)
        {
            if (_currentContentScene != null)
            {
                _currentContentScene.RemoveScene();
            }

            UnloadSceneAsycn(sceneName, result).Forget();
        }

        private void LoadScene(string sceneName, System.Action<bool> result, System.Action<float> progress)
        {
            LoadSceneAsync(sceneName, result, progress).Forget();
        }

        private async UniTask UnloadSceneAsycn(string sceneName, System.Action<bool> result)
        {
            await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);

            await UniTask.DelayFrame(1);

            result?.Invoke(true);

            await UniTask.DelayFrame(1);
        }

        private async UniTask LoadSceneAsync(string sceneName, System.Action<bool> result, System.Action<float> progress)
        {
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            while (asyncOperation.isDone == false)
            {
                progress?.Invoke(asyncOperation.progress);

                await UniTask.DelayFrame(1);
            }

            progress?.Invoke(1);

            await UniTask.DelayFrame(1);

            result?.Invoke(true);

            await UniTask.DelayFrame(1);
        }


        #region SCENE_FUNCTION
        /// <summary>
        /// 씬 돌아가기
        /// </summary>
        public bool ReturnScene(System.Action<bool> changeDone = null)
        {
            if (_sceneStack.Count > 0)
            {
                SceneStackData nextScene = _sceneStack.Pop();
                ChangeScene(nextScene.Type, changeDone);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 바로 전의 Scene이 어떤 Scene인지 알려줌(없으면 현재 Scene을 알려줌)
        /// </summary>
        public ContentSceneType GetPrevSceneBySceneStack()
        {
            if (_sceneStack.Count > 0)
            {
                SceneStackData prevScene = _sceneStack.Peek();
                return prevScene.Type;
            }
            else
            {
                return _curScene.Type;
            }
        }

        public ContentSceneType GetPrevScene()
        {
            return _prevSceneType;
        }

        /// <summary>
        /// 현재 Scene이 설정되어있는지 알려줌
        /// </summary>
        public bool IsCurrentSceneExist()
        {
            return _curScene != null;
        }

        /// <summary>
        /// 씬스택에 저장합니다.
        /// 단, 이미 저장이 되어있는 씬이라면 그 중간의 씬들은 리스트에서 제거합니다.
        /// ex) A - B - C - D - (B 저장시 C,D는 제거됩니다)
        /// </summary>
        /// <param name="prevScene"></param>
        private void InsertSceneStack(SceneStackData prevScene, ContentSceneType newSceneType)
        {
            if (prevScene == null)
            {
                return;
            }

            if (newSceneType == ContentSceneType.Empty)
            {
                _sceneStack.Clear();
            }
            _sceneStack.Push(prevScene);
        }

        public bool IsOpenScene(ContentSceneType sceneType)
        {
            foreach (var scene in _sceneStack)
            {
                if (scene.Type == sceneType)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

    }
}
