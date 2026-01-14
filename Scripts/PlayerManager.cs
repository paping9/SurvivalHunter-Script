using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : IPlayerManager
{
    private MyPlayer _myPlayer;
    private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();

    /// <summary>
    /// Spawns Player GameObjects from the given player list, positions them, and registers them with the manager.
    /// </summary>
    /// <param name="packet">A player-list packet whose entries specify each player's position, player id, and whether the entry represents the local player. Local player entries are added as a MyPlayer and stored in _myPlayer; other entries are added as Player and stored in _players.</param>
    public void Add(S_PlayerList packet)
    {
        var gameObject = Resources.Load("Player");

        foreach(var player in packet.players)
        {
            GameObject playerGo = GameObject.Instantiate(gameObject) as GameObject;

            playerGo.transform.position = new Vector3(player.posX, player.posY, player.posZ);

            if (player.isSelf)
            {
                _myPlayer = playerGo.AddComponent<MyPlayer>();
                _myPlayer.PlayerId = player.palyerId;
            }
            else _players.Add(player.palyerId, playerGo.AddComponent<Player>());

        }
    }

    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (_myPlayer.PlayerId == packet.palyerId) return;

        if (_players.ContainsKey(packet.palyerId)) return;

        var gameObject = Resources.Load("Player");
        GameObject playerGo = GameObject.Instantiate(gameObject) as GameObject;
        playerGo.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        _players.Add(packet.palyerId, playerGo.AddComponent<Player>());
    }

    public void LeaveGame(S_BroadcastLeavGame packet)
    {
        if(_myPlayer.PlayerId == packet.playerId)
        {
            GameObject.DestroyImmediate(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else
        {
            Player player = null;

            if(_players.TryGetValue(packet.playerId, out player))
            {
                GameObject.DestroyImmediate(player.gameObject);
                _players.Remove(packet.playerId);
            }
        }
    }

    public void Move(S_BroadcastMove packet)
    {
        if (_myPlayer.PlayerId == packet.palyerId)
        {
            _myPlayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        }
        else
        {
            Player player = null;

            if (_players.TryGetValue(packet.palyerId, out player))
            {
                player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
            }
        }
    }
}