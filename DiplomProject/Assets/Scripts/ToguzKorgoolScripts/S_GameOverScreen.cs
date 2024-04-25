using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_GameOverScreen : MonoBehaviour
{
    public Text PlayerNameText;
    public void Setup(String PlayerName)
    {
        gameObject.SetActive(true);
        PlayerNameText.text = PlayerName.ToUpper();
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("ToguzKorgol");
    }

    public void ExitButton()
    {
        Debug.Log("MainMenu is not ready!");
    }
}
