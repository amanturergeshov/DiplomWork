using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    [SerializeField] private string header;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button button;

    public UnityEvent<string, TextMeshProUGUI> onClick;

    private void Start()
    {
        button.onClick.AddListener(() => { onClick?.Invoke(header, description); });
    }
}
