public interface IPlayerManager
{
    void Add(S_PlayerList packet);
    void EnterGame(S_BroadcastEnterGame packet);
    void LeaveGame(S_BroadcastLeavGame packet);
    void Move(S_BroadcastMove packet);
}
