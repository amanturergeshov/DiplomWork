using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameButton : MonoBehaviour
{
    [SerializeField] private string header;
    [SerializeField] private string sceneName;
    [SerializeField] private Sprite gameSprite;
    [SerializeField] private Button button;

    public UnityEvent<string, string, Sprite> onClick;
    private void Start()
    {
        button.onClick.AddListener(() => { onClick?.Invoke(header, sceneName, gameSprite); });
    }
}
