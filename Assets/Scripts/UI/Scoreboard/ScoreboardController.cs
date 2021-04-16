using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    public ScoreboardPlayerController[] BlueTeamPlayers;
    public ScoreboardPlayerController[] RedTeamPlayers;
    public GameObject Container;

    public void Set(IEnumerable<PlayerData> bluePlayers, IEnumerable<PlayerData> redPlayers)
    {
        Debug.Log("Setting Scoreboard");
        
        int i = 0;
        foreach (var playerData in bluePlayers)
        {
            Debug.Log($"Blue: {playerData.name} - {playerData.kills}/{playerData.assists}/{playerData.deaths} - {playerData.points}");
            BlueTeamPlayers[i].Set(playerData.name, playerData.kills, playerData.deaths, playerData.assists, playerData.points);
        }

        i = 0;
        foreach (var playerData in redPlayers)
        {
            Debug.Log($"Red: {playerData.name} - {playerData.kills}/{playerData.assists}/{playerData.deaths} - {playerData.points}");
            RedTeamPlayers[i].Set(playerData.name, playerData.kills, playerData.deaths, playerData.assists, playerData.points);
        }
    }
}
