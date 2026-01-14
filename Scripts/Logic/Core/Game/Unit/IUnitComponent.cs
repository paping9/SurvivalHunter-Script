namespace Game
{
    public interface IUnitComponent
    {
        void Init(BaseUnit unit);
        void Release();
        void GameUpdate(float elapsedTime);
        void FixedUpdate(float elapsedTime);
    }
}
