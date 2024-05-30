using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoMenuUIAssistance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerTextComponent;
    [SerializeField] private TextMeshProUGUI descriptionTextComponent;
    [SerializeField] private InfoButton[] infoButtons;

    private void Start()
    {
        for(int i = 0; i < infoButtons.Length; i++)
        {
            infoButtons[i].onClick.AddListener(UpdateInfo);
        }
    }

    private void UpdateInfo(string header, TextMeshProUGUI description)
    {
        headerTextComponent.text = header;
        descriptionTextComponent.text = description.text;
    }
}
