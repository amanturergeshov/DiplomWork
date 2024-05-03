using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_TK_GameOverScreen : MonoBehaviour
{
    public Text PlayerNameText;
    public Text WinnerScore;
    public void Setup(String PlayerName, int score)
    {
        gameObject.SetActive(true);
        PlayerNameText.text = PlayerName.ToUpper();
        WinnerScore.text = "SCORE: "+ score.ToString();
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("ToguzKorgol");
        gameObject.SetActive(false);
    }

    public void ExitButton()
    {
        Debug.Log("MainMenu is not ready!");
    }
}
