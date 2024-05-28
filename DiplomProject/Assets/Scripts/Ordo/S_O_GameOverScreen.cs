using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_O_GameOverScreen : MonoBehaviour
{
    public Text PlayerNameText;
    public Text WinnerScore;

    public void Setup(string playerName, int score)
    {
        gameObject.SetActive(true);
        PlayerNameText.text = playerName.ToUpper();
        WinnerScore.text = "WIN ROUNDS: " + score.ToString();
    }

    public void RestartButton()
    {
        Debug.Log("CLICK");
        SceneManager.LoadScene("Ordo");
        gameObject.SetActive(false);
    }

    public void ExitButton()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
