using System.Collections;
using System.Collections.Generic;
using Scripts;
using TMPro;
using UnityEngine;

public class CountdownDisplay : MonoBehaviour
{
    public TextMeshProUGUI CountdownText;

    public void UpdateTime(double timeInSeconds)
    {
        int minutes = (int) timeInSeconds / 60;
        int seconds = (int) timeInSeconds - (minutes * 60);

        CountdownText.text = $"{minutes.ToString("00")}:{seconds.ToString("00")}";
    }
}
