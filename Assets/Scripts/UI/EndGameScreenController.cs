using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class EndGameScreenController : MonoBehaviour
{
    public enum Result
    {
        Win,
        Loss,
        Draw,
    }
    
    public TextMeshProUGUI ResultText;

    public void SetResult(Result result)
    {
        switch (result)
        {
            case Result.Win:
                ResultText.text = "Victory";
                break;
            case Result.Loss:
                ResultText.text = "Defeat";
                break;
            case Result.Draw:
                ResultText.text = "Draw";
                break;
        }
    }
    
    public void ReturnToMenu()
    {
        PhotonNetwork.Disconnect();
    }
}
