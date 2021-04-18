using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    public ScoreboardPlayerController[] BlueTeamPlayers;
    public ScoreboardPlayerController[] RedTeamPlayers;
    public GameObject Container;

    /// <summary>
    /// Populates the scoreboard with the given lists
    /// </summary>
    /// <param name="bluePlayers">playerData of the blue team</param>
    /// <param name="redPlayers">playerData of the red team</param>
    public void Set(IEnumerable<PlayerData> bluePlayers, IEnumerable<PlayerData> redPlayers)
    {
        int i = 0;
        foreach (var playerData in bluePlayers)
        {
            if (i < 5)
            {
                BlueTeamPlayers[i].Set(playerData.name, playerData.kills, playerData.deaths, playerData.assists, playerData.points);
            }

            i++;
        }

        i = 0;
        foreach (var playerData in redPlayers)
        {
            if (i < 5)
            {
                RedTeamPlayers[i].Set(playerData.name, playerData.kills, playerData.deaths, playerData.assists, playerData.points);
            }

            i++;
        }
    }
}
