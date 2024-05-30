using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThemeButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private string sceneName;

    public UnityEvent<TextMeshProUGUI, string> onClick;

    private void Start()
    {
        button.onClick.AddListener(() => { onClick?.Invoke(description, sceneName); });
    }
}
