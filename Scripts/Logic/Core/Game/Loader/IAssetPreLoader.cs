
using Cysharp.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// Asset Bundle Data 를 미리 로드해 둔다.
    /// Prefab 화 까지는 하지 않음.
    /// </summary>
    public interface IAssetPreLoader
    {
        UniTask Load();
    }
}
