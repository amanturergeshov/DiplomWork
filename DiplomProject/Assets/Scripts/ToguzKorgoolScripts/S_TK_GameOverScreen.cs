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

    public void Setup(string playerName, int score)
    {
        gameObject.SetActive(true);
        PlayerNameText.text = playerName.ToUpper();
        WinnerScore.text = "SCORE: " + score.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("ToguzKorgol");
        gameObject.SetActive(false);
    }
    private void Awake() {
        
        gameObject.SetActive(false);
    }
    public void ExitButton()
    {
        Debug.Log("Exiting game...");
        SceneManager.LoadScene("MainMenuScene");
    }
}
