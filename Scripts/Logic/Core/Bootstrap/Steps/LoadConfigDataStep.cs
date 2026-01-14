using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bootstrap
{
    /// <summary>
    /// 게임 설정 데이터 로드
    /// </summary>
    public class LoadConfigDataStep : IBootstrapStep
    {
        public string StepName => "로드: 설정 데이터";

        public async UniTask Execute()
        {
            await UniTask.Delay(100); // 시뮬레이션
            Debug.Log("[Bootstrap] Config data loaded");
        }

        public float GetProgress()
        {
            return 1f;
        }
    }
}