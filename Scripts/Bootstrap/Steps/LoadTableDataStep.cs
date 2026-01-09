using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bootstrap
{
    /// <summary>
    /// 테이블 데이터 로드 (게임 밸런스, 아이템 등)
    /// </summary>
    public class LoadTableDataStep : IBootstrapStep
    {
        public string StepName => "로드: 테이블 데이터";

        public async UniTask Execute()
        {
            await UniTask.Delay(200); // 시뮬레이션
            Debug.Log("[Bootstrap] Table data loaded");
        }

        public float GetProgress()
        {
            return 1f;
        }
    }
}