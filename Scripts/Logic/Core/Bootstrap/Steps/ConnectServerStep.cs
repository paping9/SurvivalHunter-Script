using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bootstrap
{
    /// <summary>
    /// Bootstrap - ConnectServer
    /// </summary>
    public class ConnectServerStep : IBootstrapStep
    {
        public string StepName => "Step: ConnectServer";

        public async UniTask Execute()
        {
            await UniTask.Delay(300);
            Debug.Log("[Bootstrap] Server connected");
        }

        public float GetProgress()
        {
            return 1f;
        }
    }
}