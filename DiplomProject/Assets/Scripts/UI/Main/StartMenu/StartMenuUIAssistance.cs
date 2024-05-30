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

    private TextMeshProUGUI currentThemeDescription;
    private string currentThemeGameSceneName;

    private void Start()
    {
        for(int i = 0; i < themeButtons.Length; i++)
        {
            themeButtons[i].onClick.AddListener(UpdateInfo);
        }

        startGameButton.onClick.AddListener(StartGame);
    }

    private void UpdateInfo(TextMeshProUGUI description, string sceneName)
    {
        currentThemeDescription = description;
        currentThemeGameSceneName = sceneName;
        descritpionTextComponent.text = description.text;
    }

    private void StartGame()
    {
        if (string.IsNullOrEmpty(currentThemeDescription.text))
        {
            Debug.LogError($"����� �� �������� {currentThemeDescription} �� ����������. ��� �� �� ������ ����. ������� ���������� Build Settings, �� ����� ��� ����� � ������ ?");
            return;
        }
        SceneManager.LoadScene(currentThemeGameSceneName);
    }
}
