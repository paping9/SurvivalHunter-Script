using System;

using Cysharp.Threading.Tasks;

namespace UIController
{
    public interface IUIController
    {
        UIControllerType ControllerType { get; }
        UniTask OnEnter(UIControllerParam param);
        UniTask OnExit();
        bool Back();
        void Release();
        void Execute(float elapsedTime);
    }
}
