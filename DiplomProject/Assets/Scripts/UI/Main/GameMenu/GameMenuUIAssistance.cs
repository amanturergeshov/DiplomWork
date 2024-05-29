using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuUIAssistance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerTextComponent;
    [SerializeField] private Image gameImageComponent;
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameButton[] gameButtons;

    private string currentGameSceneName;

    private void Start()
    {
        for(int i = 0; i < gameButtons.Length; i++)
        {
            gameButtons[i].onClick.AddListener(UpdateGameInfo);
        }

        startGameButton.onClick.AddListener(StartGame);
    }

    private void UpdateGameInfo(string header, string sceneName, Sprite gameSprite)
    {
        currentGameSceneName = sceneName;

        headerTextComponent.text = header;
        gameImageComponent.sprite = gameSprite;
    }

    private void StartGame()
    {
        SceneManager.LoadScene(currentGameSceneName);
    }
}
