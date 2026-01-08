using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bootstrap
{
    /// <summary>
    /// 서버 연결
    /// </summary>
    public class ConnectServerStep : IBootstrapStep
    {
        public string StepName => "연결: 게임 서버";

        public async UniTask Execute()
        {
            // 서버 연결 로직
            await UniTask.Delay(300); // 시뮬레이션
            Debug.Log("[Bootstrap] Server connected");
        }

        public float GetProgress()
        {
            return 1f;
        }
    }
}