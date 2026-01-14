using Cysharp.Threading.Tasks;

namespace UIController
{
    /// <summary>
    /// UIController 인터페이스
    /// Logic 레이어에서 구체 타입을 구현
    /// </summary>
    public interface IUIController
    {
        UniTask OnEnter(UIControllerParam param);
        UniTask OnExit();
        bool Back();
        void Release();
        void Execute(float elapsedTime);
    }
}
