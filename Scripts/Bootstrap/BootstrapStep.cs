using System;
using Cysharp.Threading.Tasks;

namespace Bootstrap
{
    /// <summary>
    /// Bootstrap 단계를 정의하는 인터페이스
    /// </summary>
    public interface IBootstrapStep
    {
        string StepName { get; }
        
        /// <summary>
        /// Bootstrap 단계 실행
        /// </summary>
        UniTask Execute();
        
        /// <summary>
        /// 진행률 반환 (0 ~ 1)
        /// </summary>
        float GetProgress();
    }

    /// <summary>
    /// Bootstrap 단계 상태
    /// </summary>
    public enum BootstrapStepStatus
    {
        Pending,    // 대기 중
        Running,    // 실행 중
        Success,    // 성공
        Failed      // 실패
    }

    /// <summary>
    /// Bootstrap 단계 결과
    /// </summary>
    public class BootstrapStepResult
    {
        public BootstrapStepStatus Status { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public BootstrapStepResult(BootstrapStepStatus status, string message = "")
        {
            Status = status;
            Message = message;
        }
    }
}