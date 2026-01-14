using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bootstrap
{
    /// <summary>
    /// 필수 에셋 사전 로드 (UI, 프리팹 등)
    /// </summary>
    public class PreloadAssetStep : IBootstrapStep
    {
        public string StepName => "로드: 필수 에셋";

        public async UniTask Execute()
        {
            // Addressable 또는 AssetBundle에서 필수 에셋 로드
            await UniTask.Delay(150); // 시뮬레이션
            Debug.Log("[Bootstrap] Essential assets preloaded");
        }

        public float GetProgress()
        {
            return 1f;
        }
    }
}