using System;
using Cysharp.Threading.Tasks;

namespace Bootstrap
{
    public interface IBootstrapManager
    {
        bool IsBootstrapping { get; }
        float TotalProgress { get; }
        string CurrentStep { get; }
        /// <summary>
/// Registers a bootstrap step to be executed as part of the bootstrap sequence.
/// </summary>
/// <param name="step">The bootstrap step to add to the execution sequence.</param>
void RegisterStep(IBootstrapStep step);
        /// <summary>
/// Registers multiple bootstrap steps to be executed during the bootstrap sequence.
/// </summary>
/// <param name="steps">The bootstrap steps to register, in the order they should be executed.</param>
void RegisterSteps(params IBootstrapStep[] steps);
        /// <summary>
/// Initiates the bootstrap sequence and executes registered steps in order.
/// </summary>
/// <param name="onProgressChanged">Called with overall progress (0 to 1) when bootstrap progress updates.</param>
/// <param name="onStepChanged">Called with the name or description of the current step when it changes.</param>
/// <returns>true if bootstrap completed successfully, false otherwise.</returns>
UniTask<bool> StartBootstrap(Action<float> onProgressChanged = null, Action<string> onStepChanged = null);
        /// <summary>
/// Clears all registered bootstrap steps and resets bootstrap state, including progress and the current step.
/// </summary>
void Clear();
    }
}