using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class S_PlayerController : MonoBehaviour
{
    // Переменные 
    public bool isAI;
    public bool isMyTurn;
    float delayBetweenMove = 0.35f;
    public S_PlayerController Oponent;
    public LayerMask layer;
    public List<GameObject> OurLunki;
    private S_Lunka ClickedLunka;
    public GameObject OurScoreLunka;
    public bool PlayerHasTuz = false;
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
            isMyTurn = false;
            StartCoroutine(AIThinking());
        }

        Debug.Log("isAI: " + isAI + ", isMyTurn: " + isMyTurn);


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
        if (ClickedLunka.KorgoolsCount == 1)//Первый возможный ход
        {
            isMyTurn = false;
            Lunka.AddKorgool(ClickedLunka.Korgools[0]);
            ClickedLunka.Remove();
            StartCoroutine(CompleteTurn(Lunka));
        }
        else
        {
            if (ClickedLunka.KorgoolsCount > 0)//Второй возможный ход
            {
                isMyTurn = false;
                int IterationCount = ClickedLunka.KorgoolsCount;
                List<GameObject> KorgoolsToMove = new List<GameObject>(ClickedLunka.Korgools);
                ClickedLunka.Remove();

                // Удаляем из KorgoolsToMove элементы, которые присутствуют в ClickedLunka.Korgools
                KorgoolsToMove.RemoveAll(korgool => ClickedLunka.Korgools.Contains(korgool));
                for (int i = 0; i < IterationCount - 1; i++)
                {
                    Lunka.AddKorgool(KorgoolsToMove[i]);
                    if (i < KorgoolsToMove.Count - 1)
                    {
                        Lunka = Lunka.NextLunka;
                    }

                    yield return new WaitForSeconds(delayBetweenMove);
                }
                StartCoroutine(CompleteTurn(Lunka));
            }
        }

    }
    //*****************************************************DELEGATE**************************************
    void HandleScoreApplied(int score)
    {
        // Вызываем нужную функцию при получении счета
        OurScoreLunka.GetComponent<S_Schetchik>().ApplyScore(score);
    }
    //**************************************AI THINKING IMITATION********************************************
    IEnumerator AIThinking()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
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
            StartCoroutine(Turn(Lunka));
        }
    }
    //********************************************COMPLETE TURN***************************************************
    IEnumerator CompleteTurn(S_Lunka Lunka)
    {
        if (!OurLunki.Contains(Lunka.gameObject) && Lunka.KorgoolsCount % 2 == 0)
        {
            // OurScoreLunka.GetComponent<S_Schetchik>().ApplyScore(Lunka.KorgoolsCount);
            Lunka.TakenLunka = true;//Убираем чтоб добавились очки
        }
        else if (!OurLunki.Contains(Lunka.gameObject) && Lunka.CanBeTuz == true && PlayerHasTuz == false)
        {
            Lunka.TakenLunka = true;//Убираем чтоб добавились очки
            PlayerHasTuz = true;
            Lunka.Tuz = true;
            Lunka.CanBeTuz = false;
            Lunka.SpawnTuzInLunka();
        }
        yield return new WaitForSeconds(Lunka.moveTime + 0.5f);
        Oponent.isMyTurn = true;
    }
}
