using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Scripts;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    public ScoreboardPlayerController[] BlueTeamPlayers;
    public ScoreboardPlayerController[] RedTeamPlayers;
    public GameObject Container;

    public void Init()
    {
        foreach (var scoreboardPlayerController in BlueTeamPlayers)
        {
            scoreboardPlayerController.gameObject.SetActive(false);
        }
        
        foreach (var scoreboardPlayerController in RedTeamPlayers)
        {
            scoreboardPlayerController.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Populates the scoreboard with the given lists
    /// </summary>
    /// <param name="bluePlayers">playerData of the blue team</param>
    /// <param name="redPlayers">playerData of the red team</param>
    public void Set(IEnumerable<PlayerData> bluePlayers, IEnumerable<PlayerData> redPlayers)
    {
        Init();
        
        int i = 0;
        foreach (var playerData in bluePlayers)
        {
            if (i < 5)
            {
                BlueTeamPlayers[i].Set(playerData.name, playerData.icon, playerData.kills, playerData.deaths, playerData.assists, playerData.points);

                if (playerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber && !playerData.alive)
                {
                    BlueTeamPlayers[i].pName.faceColor = new Color32(186, 165, 0, 255);
                }
                if (!playerData.alive)
                {
                    BlueTeamPlayers[i].pName.faceColor = new Color32(100, 100, 100, 255);
                }
                else if (playerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    BlueTeamPlayers[i].pName.faceColor = new Color32(246, 235, 20, 255);
                }
                else
                {
                    BlueTeamPlayers[i].pName.faceColor = new Color32(255, 255, 255, 255);
                }
            }

            i++;
        }

        i = 0;
        foreach (var playerData in redPlayers)
        {
            if (i < 5)
            {
                RedTeamPlayers[i].Set(playerData.name, playerData.icon, playerData.kills, playerData.deaths, playerData.assists, playerData.points);
                
                if (playerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber && !playerData.alive)
                {
                    RedTeamPlayers[i].pName.faceColor = new Color32(186, 165, 0, 255);
                }
                if (!playerData.alive)
                {
                    RedTeamPlayers[i].pName.faceColor = new Color32(100, 100, 100, 255);
                }
                else if (playerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    RedTeamPlayers[i].pName.faceColor = new Color32(246, 235, 20, 255);
                }
                else
                {
                    RedTeamPlayers[i].pName.faceColor = new Color32(255, 255, 255, 255);
                }
            }

            i++;
        }
    }

    /// <summary>
    /// Populates the scoreboard as a FFA List
    /// </summary>
    /// <param name="playerData"></param>
    public void SetAsFFA(List<PlayerData> playersData)
    {
        int i = 0;
        int v = 0;
        Init();

        for (int j = 0; j < playersData.Count; j++)
        {
            var playerData = playersData[i];
            
            if (j < 5)
            {
                BlueTeamPlayers[i].Set(playerData.name, playerData.icon, playerData.kills, playerData.deaths, playerData.assists, playerData.points);

                if (playerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber && !playerData.alive)
                {
                    BlueTeamPlayers[i].pName.faceColor = new Color32(186, 165, 0, 255);
                }
                if (!playerData.alive)
                {
                    BlueTeamPlayers[i].pName.faceColor = new Color32(100, 100, 100, 255);
                }
                else if (playerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    BlueTeamPlayers[i].pName.faceColor = new Color32(246, 235, 20, 255);
                }
                else
                {
                    BlueTeamPlayers[i].pName.faceColor = new Color32(255, 255, 255, 255);
                }
                
                i++;
            }

            if (j >= 5 && j < 10)
            {
                RedTeamPlayers[v].Set(playerData.name, playerData.icon, playerData.kills, playerData.deaths, playerData.assists, playerData.points);

                if (playerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber && !playerData.alive)
                {
                    RedTeamPlayers[i].pName.faceColor = new Color32(186, 165, 0, 255);
                }
                if (!playerData.alive)
                {
                    RedTeamPlayers[i].pName.faceColor = new Color32(100, 100, 100, 255);
                }
                else if (playerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    RedTeamPlayers[i].pName.faceColor = new Color32(246, 235, 20, 255);
                }
                else
                {
                    RedTeamPlayers[i].pName.faceColor = new Color32(255, 255, 255, 255);
                }
                v++;
            }
        }
    }
}
