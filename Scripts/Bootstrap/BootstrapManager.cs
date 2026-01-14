using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Bootstrap
{
    /// <summary>
    /// 게임 시작 시 필요한 초기화 작업을 관리
    /// </summary>
    public class BootstrapManager : IBootstrapManager
    {
        private readonly List<IBootstrapStep> _bootstrapSteps = new List<IBootstrapStep>();
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

        /// <summary>
        /// Assigns the dependency injection container used to inject dependencies into bootstrap steps.
        /// </summary>
        /// <param name="container">The object resolver used to inject dependencies into registered bootstrap steps.</param>
        [Inject]
        public void Construct(IObjectResolver container)
        {
            _container = container;
        }

        /// <summary>
        /// Bootstrap 단계 등록
        /// <summary>
        /// Registers a bootstrap step for execution during startup. If a dependency injection container is available, the step will be injected before registration.
        /// Duplicate steps are ignored; passing null has no effect.
        /// </summary>
        /// <param name="step">The bootstrap step to register.</param>
        public void RegisterStep(IBootstrapStep step)
        {
            if (step == null) return;

            if (!_bootstrapSteps.Contains(step))
            {
                _container?.Inject(step);
                _bootstrapSteps.Add(step);
            }
        }

        /// <summary>
        /// ���� Bootstrap �ܰ� ���
        /// <summary>
        /// Registers multiple bootstrap steps in the given order. Non-null steps are added to the bootstrap sequence and will be injected via the configured container if available.
        /// </summary>
        /// <param name="steps">Array of bootstrap steps to register; null elements are ignored.</param>
        public void RegisterSteps(params IBootstrapStep[] steps)
        {
            foreach (var step in steps)
            {
                RegisterStep(step);
            }
        }

        /// <summary>
        /// Bootstrap ����
        /// <summary>
        /// Starts the registered bootstrap steps sequence and reports progress and step changes via callbacks.
        /// </summary>
        /// <param name="onProgressChanged">Callback invoked with overall progress value in the range [0, 1] after each step completes.</param>
        /// <param name="onStepChanged">Callback invoked with the name of the step that is starting.</param>
        /// <returns>`true` if all bootstrap steps completed successfully, `false` otherwise.</returns>
        public UniTask<bool> StartBootstrap(Action<float> onProgressChanged = null, Action<string> onStepChanged = null)
        {
            return StartBootstrapInternal(onProgressChanged, onStepChanged);
        }

        /// <summary>
        /// Executes registered bootstrap steps in sequence while reporting current step and overall progress.
        /// </summary>
        /// <param name="onProgressChanged">Callback invoked with the overall progress value (0 to 1) after each step and upon completion.</param>
        /// <param name="onStepChanged">Callback invoked with the current step's name immediately before that step starts.</param>
        /// <returns>`true` if all steps completed successfully; `false` if a bootstrap was already running or a step failed.</returns>
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

                    // �� �ܰ��� ����� ���
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

        /// <summary>
        /// Updates the manager's overall bootstrap progress and notifies the progress callback when steps are registered.
        /// </summary>
        /// <remarks>
        /// Calculates progress as (current step index + 1) divided by the total number of registered steps, stores the result in <c>_totalProgress</c>, and invokes <c>_onProgressChanged</c> if it is set. No action is taken when there are no registered steps.
        /// </remarks>
        private void UpdateProgress()
        {
            if (_bootstrapSteps.Count > 0)
            {
                // �Ϸ�� �ܰ� + ���� �ܰ��� �����
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