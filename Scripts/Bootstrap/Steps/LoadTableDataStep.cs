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

        /// <summary>
        /// Injects the table data manager used by this bootstrap step.
        /// </summary>
        /// <param name="tableDataManager">The <see cref="ITableDataManager"/> responsible for initializing and providing table data.</param>
        [Inject]
        public void Construct(ITableDataManager tableDataManager)
        {
            _tableDataManager = tableDataManager;
        }

        public string StepName => "로딩: 테이블 데이터";

        /// <summary>
        /// Initializes table data through the configured table data manager.
        /// </summary>
        /// <returns>A UniTask that completes when table data initialization finishes.</returns>
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