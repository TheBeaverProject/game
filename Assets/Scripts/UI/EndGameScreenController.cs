using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Scripts;
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
    public TextMeshProUGUI EloText;
    public TextMeshProUGUI EloGainText;

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

    public void SetNewElo(int currentElo, int gainloss)
    {
        EloGainText.text = gainloss > 0 ? $"+{gainloss}" : $"{gainloss}";
        EloText.text = $"Elo: {currentElo}";

        StartCoroutine(Utils.SmoothTransition(f => {}, 2, () =>
        {
            StartCoroutine(Utils.SmoothTransition(f =>
            {
                float val = Mathf.Round(Mathf.Lerp(0, gainloss, f));

                EloText.text = $"Elo: {currentElo + val}";
                EloGainText.text = gainloss - val > 0 ? $"+{gainloss - val}" : gainloss - val == 0 ? "" : $"{gainloss - val}";
            }, 1));
        }));
    }
    
    public void ReturnToMenu()
    {
        PhotonNetwork.Disconnect();
    }
}
