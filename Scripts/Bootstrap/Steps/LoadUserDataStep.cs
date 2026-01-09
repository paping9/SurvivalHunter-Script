using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bootstrap
{
    /// <summary>
    /// 사용자 데이터 로드
    /// </summary>
    public class LoadUserDataStep : IBootstrapStep
    {
        public string StepName => "로드: 사용자 정보";

        public async UniTask Execute()
        {
            // 사용자 계정, 인벤토리, 통계 등 로드
            await UniTask.Delay(200); // 시뮬레이션
            Debug.Log("[Bootstrap] User data loaded");
        }

        public float GetProgress()
        {
            return 1f;
        }
    }
}