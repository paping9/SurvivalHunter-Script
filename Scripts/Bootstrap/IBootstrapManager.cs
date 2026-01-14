using System;
using Cysharp.Threading.Tasks;

namespace Bootstrap
{
    public interface IBootstrapManager
    {
        bool IsBootstrapping { get; }
        float TotalProgress { get; }
        string CurrentStep { get; }
        void RegisterStep(IBootstrapStep step);
        void RegisterSteps(params IBootstrapStep[] steps);
        UniTask<bool> StartBootstrap(Action<float> onProgressChanged = null, Action<string> onStepChanged = null);
        void Clear();
    }
}
