using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using VContainer;

namespace Bootstrap
{
    /// <summary>
    /// 테이블 데이터 로드
    /// </summary>
    public class LoadTableDataStep : IBootstrapStep
    {
        private ITableDataManager _tableDataManager;

        [Inject]
        public void Construct(ITableDataManager tableDataManager)
        {
            _tableDataManager = tableDataManager;
        }

        public string StepName => "로딩: 테이블 데이터";

        public async UniTask Execute()
        {
            await _tableDataManager.InitializeAsync();
            Debug.Log("[Bootstrap] Table data loaded");
        }

        public float GetProgress()
        {
            return 1f;
        }
    }
}