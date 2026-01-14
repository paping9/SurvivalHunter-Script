namespace UIController
{
    /// <summary>
    /// UIController 생성을 담당하는 Factory 인터페이스
    /// Logic 레이어에서 구체적인 Controller 타입을 알고 생성
    /// </summary>
    public interface IUIControllerFactory
    {
        /// <summary>
        /// Controller 타입 ID로 Controller 생성
        /// </summary>
        /// <param name="controllerTypeId">Controller 타입 ID (enum을 int로 캐스팅한 값)</param>
        /// <returns>생성된 Controller 인스턴스</returns>
        IUIController Create(int controllerTypeId);
    }
}
