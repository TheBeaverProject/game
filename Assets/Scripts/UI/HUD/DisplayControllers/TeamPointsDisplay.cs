using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class TeamPointsDisplay : MonoBehaviour
{
    public TextMeshProUGUI bluePointsText;
    public TextMeshProUGUI redPointsText;
    public Slider slider;

    public void SetPoints(float bluePoints, float redPoints)
    {
        bluePointsText.text = bluePoints.ToString();
        redPointsText.text = redPoints.ToString();
        
        if (bluePoints + redPoints == 0)
        {
            slider.value = 50;
            return;
        }

        var value = (bluePoints / (bluePoints + redPoints)) * 100;

        slider.value = value;
    }
}
