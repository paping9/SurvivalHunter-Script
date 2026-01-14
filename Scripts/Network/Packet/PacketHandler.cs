using System;
using DummyClient;
using ServerCore;

using UnityEngine;

public class PacketHandler
{

    /// <summary>
    /// Handles an incoming S_BroadcastEnterGame packet for a server session.
    /// </summary>
    /// <param name="session">The packet session; expected to be a <see cref="ServerSession"/>.</param>
    /// <param name="packet">The incoming packet; expected to be an <see cref="S_BroadcastEnterGame"/> instance.</param>
    /// <remarks>
    /// This method casts the inputs to their expected concrete types. It does not perform further processing currently.
    /// </remarks>
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame enter = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.EnterGame(enter);
    }

    /// <summary>
    /// Handles an incoming S_BroadcastLeavGame packet from a session.
    /// </summary>
    /// <remarks>
    /// The method casts the provided packet to S_BroadcastLeavGame and the session to ServerSession; it currently performs no further action (no-op).
    /// </remarks>
    /// <param name="session">The session that sent the packet.</param>
    /// <param name="packet">The received packet, expected to be an S_BroadcastLeavGame instance.</param>
    public static void S_BroadcastLeavGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeavGame pkt = packet as S_BroadcastLeavGame;
        ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.LeaveGame(pkt);
    }

    /// <summary>
    /// Handles an incoming player list packet and integrates its data into server state.
    /// </summary>
    /// <param name="session">The originating server session.</param>
    /// <param name="packet">The packet containing player list data; expected to be an <see cref="S_PlayerList"/> instance.</param>
    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList pkt = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.Add(pkt);
    }

    /// <summary>
    /// Handles an incoming S_BroadcastMove packet and associates it with the originating server session. Currently no game-state update is performed (packet processing is disabled).
    /// </summary>
    /// <param name="session">The session that sent the packet (expected to be a ServerSession).</param>
    /// <param name="packet">The received packet (expected to be an S_BroadcastMove).</param>
    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove pkt = packet as S_BroadcastMove;
        ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.Move(pkt);
    }
}