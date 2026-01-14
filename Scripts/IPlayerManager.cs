public interface IPlayerManager
{
    /// <summary>
/// Adds players described by the provided S_PlayerList packet to the player manager.
/// </summary>
/// <param name="packet">Packet containing one or more player entries to add to the manager.</param>
void Add(S_PlayerList packet);
    /// <summary>
/// Handle a player entering the game using data from the provided enter-game broadcast packet.
/// </summary>
/// <param name="packet">Broadcast packet containing the player's identity and initial state (position, appearance, and related spawn data).</param>
void EnterGame(S_BroadcastEnterGame packet);
    /// <summary>
/// Handle a player's departure from the game.
/// </summary>
/// <param name="packet">Broadcast packet containing the departing player's identifier and related state information.</param>
void LeaveGame(S_BroadcastLeavGame packet);
    /// <summary>
/// Handle a player movement broadcast and update the manager's state for that player.
/// </summary>
/// <param name="packet">Broadcast packet containing the player's identifier and the new position and rotation data.</param>
void Move(S_BroadcastMove packet);
}