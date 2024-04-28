using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_UI_KorgoolsCount : MonoBehaviour
{
    public S_Lunka Lunka;
    public Text korgoolsCountText; // ссылка на компонент текста

    // Start is called before the first frame update
    void Start()
    {
        // Проверяем, что ссылка на компонент текста присутствует
        if (korgoolsCountText == null)
        {
            Debug.LogError("Text component reference is missing!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Проверяем, что ссылка на объект Lunka присутствует
        if (Lunka != null && korgoolsCountText != null)
        {
            if (Lunka.Tuz == true)
            {
                korgoolsCountText.text = "T";
            }
            else
            {
                // Обновляем текстовое значение компонента текста с помощью свойства text
                korgoolsCountText.text = Lunka.KorgoolsCount.ToString();
            }
        }
        else
        {
            Debug.LogWarning("Lunka reference or Text component reference is missing!");
        }
    }
}