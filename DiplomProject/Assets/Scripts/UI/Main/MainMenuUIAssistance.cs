using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIAssistance : MonoBehaviour
{
    [SerializeField] private PanelProperties[] panelProperties;
    [SerializeField] private Button[] closePanelButtons;
    [SerializeField] private Button quitButton;

    private PanelProperties previosPanel;
    private PanelProperties currentPanel;

    private Dictionary<string, PanelProperties> panelsLibrary = new Dictionary<string, PanelProperties>();

    private void Start()
    {
        for(int i = 0; i < panelProperties.Length; i++)
        {
            panelsLibrary.Add(panelProperties[i].Name, panelProperties[i]);
        }

        for(int i = 0; i < closePanelButtons.Length; i++)
        {
            closePanelButtons[i].onClick.AddListener(() => OpenPanel("main"));
        }

        quitButton.onClick.AddListener(QuitGame);
        // По дефолту пиздим мейн меню
        OpenPanel("main");
    }
    /// <summary>
    /// Остальная хуйня вызывается в кнопках
    /// </summary>
    /// <param name="key"></param>
    public void OpenPanel(string key)
    {
        if (panelsLibrary.ContainsKey(key))
        {
            previosPanel = currentPanel;
            currentPanel = panelsLibrary[key];
        }
        else
        {
            Debug.Log($"Не нашли панельку: {key}");
        }

        SetActiveCurrentPanel(true);
    }
    private void SetActiveCurrentPanel(bool value)
    {
        if(previosPanel != null)
            previosPanel.Panel.gameObject.SetActive(!value);
        if(currentPanel != null)
            currentPanel.Panel.gameObject.SetActive(value);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    [Serializable]
    public class PanelProperties
    {
        public string Name;
        public GameObject Panel;
    }
}
