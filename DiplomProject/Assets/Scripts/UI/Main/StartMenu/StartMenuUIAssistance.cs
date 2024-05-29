using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUIAssistance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descritpionTextComponent;
    [SerializeField] private Button startGameButton;

    [SerializeField] private ThemeButton[] themeButtons;

    private string currentThemeDescription;
    private string currentThemeGameSceneName;

    private void Start()
    {
        for(int i = 0; i < themeButtons.Length; i++)
        {
            themeButtons[i].onClick.AddListener(UpdateInfo);
        }

        startGameButton.onClick.AddListener(StartGame);
    }

    private void UpdateInfo(string description, string sceneName)
    {
        currentThemeDescription = description;
        currentThemeGameSceneName = sceneName;
        descritpionTextComponent.text = description;
    }

    private void StartGame()
    {
        if (string.IsNullOrEmpty(currentThemeDescription))
        {
            Debug.LogError($"Сцена по названию {currentThemeDescription} не существует. Где то ты наебал меня. Проверь пожалуйста Build Settings, ты кинул эту сцену в список ?");
            return;
        }
        SceneManager.LoadScene(currentThemeGameSceneName);
    }
}
