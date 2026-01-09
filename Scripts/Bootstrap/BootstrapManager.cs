using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Utils;
using VContainer;

namespace Bootstrap
{
    /// <summary>
    /// 게임 시작 시 필요한 초기화 작업을 관리
    /// 데이터 로드, 서버 연결 등을 순차적으로 처리
    /// </summary>
    public class BootstrapManager : Singleton<BootstrapManager>
    {
        private List<IBootstrapStep> _bootstrapSteps = new List<IBootstrapStep>();
        private int _currentStepIndex = -1;
        private float _totalProgress = 0;
        private bool _isBootstrapping = false;
        private Action<float> _onProgressChanged;
        private Action<string> _onStepChanged;
        
        private IObjectResolver _container;

        public bool IsBootstrapping => _isBootstrapping;
        public float TotalProgress => _totalProgress;
        public string CurrentStep => _currentStepIndex >= 0 && _currentStepIndex < _bootstrapSteps.Count 
            ? _bootstrapSteps[_currentStepIndex].StepName 
            : "";

        public void SetContainer(IObjectResolver container)
        {
            _container = container;
        }

        /// <summary>
        /// Bootstrap 단계 등록
        /// </summary>
        public void RegisterStep(IBootstrapStep step)
        {
            if (step == null) return;
            
            if (!_bootstrapSteps.Contains(step))
            {
                _bootstrapSteps.Add(step);
            }
        }

        /// <summary>
        /// 여러 Bootstrap 단계 등록
        /// </summary>
        public void RegisterSteps(params IBootstrapStep[] steps)
        {
            foreach (var step in steps)
            {
                RegisterStep(step);
            }
        }

        /// <summary>
        /// Bootstrap 시작
        /// </summary>
        public UniTask<bool> StartBootstrap(Action<float> onProgressChanged = null, Action<string> onStepChanged = null)
        {
            return StartBootstrapInternal(onProgressChanged, onStepChanged);
        }

        private async UniTask<bool> StartBootstrapInternal(Action<float> onProgressChanged, Action<string> onStepChanged)
        {
            if (_isBootstrapping)
            {
                UnityEngine.Debug.LogWarning("Bootstrap is already running");
                return false;
            }

            _isBootstrapping = true;
            _currentStepIndex = -1;
            _onProgressChanged = onProgressChanged;
            _onStepChanged = onStepChanged;

            try
            {
                for (int i = 0; i < _bootstrapSteps.Count; i++)
                {
                    _currentStepIndex = i;
                    var step = _bootstrapSteps[i];

                    _onStepChanged?.Invoke(step.StepName);
                    UnityEngine.Debug.Log($"[Bootstrap] Starting: {step.StepName}");

                    await step.Execute();

                    // 각 단계의 진행률 계산
                    UpdateProgress();
                }

                UnityEngine.Debug.Log("[Bootstrap] All steps completed successfully");
                _totalProgress = 1f;
                _onProgressChanged?.Invoke(_totalProgress);
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[Bootstrap] Failed at step {CurrentStep}: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                _isBootstrapping = false;
                _currentStepIndex = -1;
            }
        }

        private void UpdateProgress()
        {
            if (_bootstrapSteps.Count > 0)
            {
                // 완료된 단계 + 현재 단계의 진행률
                float stepsProgress = (float)(_currentStepIndex + 1) / _bootstrapSteps.Count;
                _totalProgress = stepsProgress;
                _onProgressChanged?.Invoke(_totalProgress);
            }
        }

        public void Clear()
        {
            _bootstrapSteps.Clear();
            _currentStepIndex = -1;
            _totalProgress = 0;
            _isBootstrapping = false;
        }
    }
}