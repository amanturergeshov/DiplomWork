using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class S_PlayerController : MonoBehaviour
{
    // Переменные 
    public bool isAI;
    public bool isMyTurn;
    public S_PlayerController Oponent;
    public LayerMask layer;
    public List<GameObject> OurLunki;
    private S_Lunka ClickedLunka;
    public GameObject OurScoreLunka;
    void Start()
    {
        // Находим все объекты с компонентом S_Lunka на сцене
        S_Lunka[] allLunki = FindObjectsOfType<S_Lunka>();

        // Подписываемся на событие ScoreApplied для каждой лунки, если она не содержится в OurLunki
        foreach (S_Lunka lunka in allLunki)
        {
            if (!OurLunki.Contains(lunka.gameObject))
            {
                lunka.ScoreApplied += HandleScoreApplied;
            }
        }
    }


    // Update is called once per frame

    void Update()
    {
        if (isMyTurn == true && isAI == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClickLunka();
            }
        }
        else if (isMyTurn == true && isAI == true)
        {
            Debug.Log(isMyTurn);
            Oponent.isMyTurn = true;
            isMyTurn = false;
            StartCoroutine(AIThinking());
        }
    }

    void ClickLunka()
    {
        //Пускаем луч с камеры
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, layer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != null && OurLunki.Contains(hitObject))
            {
                ClickedLunka = hitObject.GetComponent<S_Lunka>();
                StartCoroutine(Turn(ClickedLunka));
                Oponent.isMyTurn = true;
                isMyTurn = false;
            }
            else
            {
                // Объект не содержится в списке OurLunki
                Debug.Log("Object is not in OurLunki list.");
            }
        }
    }
    /// <summary>
    /// TURN MOVE
    /// </summary>
    IEnumerator Turn(S_Lunka ClickedLunka)
    {
        S_Lunka Lunka = ClickedLunka.NextLunka;
        if (ClickedLunka.KorgoolsCount == 1)
        {
            Lunka.AddKorgool(ClickedLunka.Korgools[0]);

            Debug.Log(Lunka.index + " and " + Lunka.KorgoolsCount);
        }
        else
        {
            if (ClickedLunka.KorgoolsCount > 0)
            {
                for (int i = 0; i < ClickedLunka.KorgoolsCount - 1; i++)
                {
                    Debug.Log(i);
                    Lunka.AddKorgool(ClickedLunka.Korgools[i]);
                    Debug.Log(Lunka.index + " and " + Lunka.KorgoolsCount);
                    if (i < ClickedLunka.KorgoolsCount - 2)
                    {
                        Lunka = Lunka.NextLunka;
                    }

                    yield return new WaitForSeconds(0.35f);
                }
            }
        }
        if (!OurLunki.Contains(Lunka.gameObject) && Lunka.KorgoolsCount % 2 == 0)
        {
            // OurScoreLunka.GetComponent<S_Schetchik>().ApplyScore(Lunka.KorgoolsCount);
            Lunka.TakenLunka = true;//Убираем чтоб добавились очки
        }
        ClickedLunka.Remove();
    }
    //*****************************************************DELEGATE**************************************
    void HandleScoreApplied(int score)
    {
        // Вызываем нужную функцию при получении счета
        OurScoreLunka.GetComponent<S_Schetchik>().ApplyScore(score);
    }

    IEnumerator AIThinking()
    {
        yield return new WaitForSeconds(Random.Range(3, 5));
        S_Lunka Lunka = null;
        while (true)
        {
            int index = Random.Range(0, OurLunki.Count);
            if (OurLunki[index].GetComponent<S_Lunka>().KorgoolsCount > 0)
            {
                Lunka = OurLunki[index].GetComponent<S_Lunka>();
                break;
            }
        }
        if (Lunka)
        {
            // Debug.Log("AD");
            StartCoroutine(Turn(Lunka));
        }
    }
}
