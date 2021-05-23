using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardPlayerController : MonoBehaviour
{
    public TextMeshProUGUI pName;
    public TextMeshProUGUI kills;
    public TextMeshProUGUI deaths;
    public TextMeshProUGUI assists;
    public TextMeshProUGUI points;

    /// <summary>
    /// Sets the player's info on the corresponding field
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="kills">kills num</param>
    /// <param name="deaths">deaths num</param>
    /// <param name="assists">assists num</param>
    /// <param name="points">points num</param>
    public void Set(string name = "", int kills = -1, int deaths = -1, int assists = -1, int points = -1)
    {
        this.gameObject.SetActive(true);
        if (name != "")
            this.pName.text = name;
        if (kills != -1)
            this.kills.text = kills.ToString();
        if (deaths != -1)
            this.deaths.text = deaths.ToString();
        if (assists != -1)
            this.assists.text = assists.ToString();
        if (points != -1)
            this.points.text = points.ToString();
    }
}
