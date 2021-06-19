using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneDominationDisplay : MonoBehaviour
{
    public Image redFillerA;
    public Image redFillerB;
    
    public Image blueFillerA;
    public Image blueFillerB;

    public TextMeshProUGUI AName;
    public TextMeshProUGUI BName;

    public void SetZoneB(float redAmount, float blueAmount)
    {
        blueFillerB.fillAmount = blueAmount;
        redFillerB.fillAmount = redAmount;

        BName.color = blueAmount >= 1 ? blueFillerB.color : redAmount >= 1 ? redFillerB.color : Color.white;
    }
    
    public void SetZoneA(float redAmount, float blueAmount)
    {
        blueFillerA.fillAmount = blueAmount;
        redFillerA.fillAmount = redAmount;
        
        AName.color = blueAmount >= 1 ? blueFillerA.color : redAmount >= 1 ? redFillerA.color : Color.white;
    }
}
