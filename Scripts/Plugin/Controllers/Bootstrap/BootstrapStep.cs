using System;
using Cysharp.Threading.Tasks;

namespace Bootstrap
{
    public interface IBootstrapStep
    {
        string StepName { get; }
        
        UniTask Execute();
        float GetProgress();
    }

    public enum BootstrapStepStatus
    {
        Pending,    
        Running,    
        Success,    
        Failed      
    }

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