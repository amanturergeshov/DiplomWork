using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_O_GameManager : MonoBehaviour
{
    //*******************************************************************TOSS UP PROPERTIES***********************************************************************
    public List<S_O_PLayerController> OurTompoys;
    public float timerDuration = 1f;
    public float launchForce = 100f;
    public float maxRotationSpeed = 5600f;
    public bool gameEnded = false;

    S_O_PLayerController TossUpWinner = null;
    public GameObject PlayZone;

    //*******************************************************************SPAWN PROPERTIES***********************************************************************
    public GameObject prefabToSpawn;
    public int numberOfObjects = 100;
    public float radiusStep = 1f;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    //*******************************************************************ROUND PROPERTIES***********************************************************************
    public int CurrentRound = -1;
    public int MaxRound = 2;
    public int[] RoundsWinner = new int[3]; //0 - никто ещё не выиграл, 1- выиграл первый игрок, 2- выиграл второй игрок (для UI)

    //*******************************************************************ROUND PROPERTIES***********************************************************************
    public delegate void RoundWinnerDelegate();
    public event RoundWinnerDelegate OnRoundWinnerDetermined;
    //*******************************************************************ROUND PROPERTIES***********************************************************************
    public S_O_GameOverScreen GameOverScreen;



    //*******************************************************************START***********************************************************************
    void Start()
    {
        if (OurTompoys.Count == 2)
        {
            for (int i = 0; i < RoundsWinner.Length; i++)
            {
                RoundsWinner[i] = 0; // Заполняем массив нулями
            }
            StartCoroutine(TossUp());
        }
        else
        {
            Debug.LogError("OurTompoys should contain exactly 2 Tompoys.");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleGameOverScreen();
        }
    }

    void ToggleGameOverScreen()
    {
        if (GameOverScreen.gameObject.activeSelf)
        {
            // Если меню уже отображается, скрываем его
            Debug.Log("HIDE!");
            GameOverScreen.Hide();
        }
        else
        {
            // Если меню скрыто, вызываем его
            GameOverScreen.Setup("Nobody", OurTompoys[0].OurScore.score);
        }
    }

    //*******************************************************************SPAWN ALCHIKS***********************************************************************
    void SpawnObjectsInCircle()
    {
        int spawnedObjectCount = 0;
        float currentRadius = 0f;

        while (spawnedObjectCount < numberOfObjects)
        {
            int objectsOnCurrentRadius = Mathf.CeilToInt(2 * Mathf.PI * currentRadius / radiusStep);

            for (int i = 0; i < objectsOnCurrentRadius && spawnedObjectCount < numberOfObjects; i++)
            {
                float angle = (360f / objectsOnCurrentRadius) * i;
                float angleRad = Mathf.Deg2Rad * angle;

                Vector3 spawnPosition = new Vector3(
                    Mathf.Cos(angleRad) * currentRadius,
                    0, // Y координата на 0
                    Mathf.Sin(angleRad) * currentRadius
                );

                Quaternion spawnRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0); // Rotation по y любая, но по x и z равна 0
                GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
                spawnedObjects.Add(spawnedObject); // Добавление объекта в список

                spawnedObjectCount++;
            }

            currentRadius += radiusStep;
        }
    }

    //*******************************************************************TOSS UP***********************************************************************
    IEnumerator TossUp()
    {
        while (!gameEnded)
        {
            PositionTompoys();
            yield return StartCoroutine(LaunchTompoysAfterTimer());
            yield return new WaitForSeconds(5f);
            CheckForTossEnd();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void PositionTompoys()
    {
        OurTompoys[0].Tompoy.transform.position = new Vector3(-1, 0, 0);
        OurTompoys[0].Tompoy.transform.rotation = Quaternion.identity;

        OurTompoys[1].Tompoy.transform.position = new Vector3(1, 0, 0);
        OurTompoys[1].Tompoy.transform.rotation = Quaternion.identity;
    }

    IEnumerator LaunchTompoysAfterTimer()
    {
        yield return new WaitForSeconds(timerDuration);

        foreach (var Tompoy in OurTompoys)
        {
            Rigidbody rb = Tompoy.Tompoy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * launchForce, ForceMode.VelocityChange);

                Vector3 randomRotationAxis = new Vector3(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)
                ).normalized;

                float randomRotationSpeed = UnityEngine.Random.Range(-maxRotationSpeed, maxRotationSpeed);

                rb.AddTorque(randomRotationAxis * randomRotationSpeed, ForceMode.VelocityChange);
            }
            else
            {
                Debug.LogError("Rigidbody is missing on the Tompoy: " + Tompoy.name);
            }
        }
    }

    void CheckForTossEnd()
    {
        int counter = 0;
        foreach (var Tompoy in OurTompoys)
        {
            Debug.Log(Tompoy + " " + Mathf.RoundToInt(Tompoy.Tompoy.transform.rotation.eulerAngles.z));
            if (Mathf.Round(Tompoy.Tompoy.transform.rotation.eulerAngles.z) == 0f || Mathf.Round(Tompoy.Tompoy.transform.rotation.eulerAngles.z) % 180 == 0f)
            {
                TossUpWinner = Tompoy;
                counter++;
            }
        }
        if (counter == 1)
        {
            gameEnded = true;
            StartRound(TossUpWinner);
        }
    }

    //*******************************************************************START ROUND***********************************************************************
    public void StartRound(S_O_PLayerController TossUpWinner)
    {
        CurrentRound++;
        if (CurrentRound > MaxRound)
        {
            StartCoroutine(DetermineGameWinner());
        }
        else
        {
            foreach (var Tompoy in OurTompoys)
            {
                Tompoy.ReLocateTompoy();
            }
            SpawnObjectsInCircle();
            PlayZone.SetActive(true);
            PlayZone.GetComponent<S_O_PlayZone>().InsideAlchikObjects = spawnedObjects;

            foreach (var Tompoy in OurTompoys)
            {
                Tompoy.alchikObjects = spawnedObjects;
            }
            TossUpWinner.Oponent.GiveTurnToOponent();
        }
    }

    IEnumerator Restart()
    {
        var roundWinner = CheckWinnerOfRound();
        spawnedObjects.Clear();
        foreach (var Tompoy in OurTompoys)
        {
            Tompoy.OurScore.ResetScore();
            Tompoy.isMyTurn = false;
            Tompoy.CompleteTurn();
        }

        if (roundWinner != null)
        {
            roundWinner.OurScore.AddWinRounds();
            TossUpWinner = roundWinner.Oponent; // Начинает раунд проигравший игрок
        }
        else
        {
            TossUpWinner = TossUpWinner.Oponent;
        }
        yield return new WaitForSeconds(1f);
        StartRound(TossUpWinner);
    }

    public S_O_PLayerController CheckWinnerOfRound()
    {
        if (CurrentRound < RoundsWinner.Length)
        {
            S_O_PLayerController winner = null;
            if (OurTompoys[0].OurScore.score > OurTompoys[1].OurScore.score)
            {
                RoundsWinner[CurrentRound] = 1;
                winner = OurTompoys[0];
            }
            else if (OurTompoys[1].OurScore.score > OurTompoys[0].OurScore.score)
            {
                RoundsWinner[CurrentRound] = 2;
                winner = OurTompoys[1];
            }
            else
            {
                RoundsWinner[CurrentRound] = 0;
            }

            // Триггерим событие после определения победителя
            OnRoundWinnerDetermined?.Invoke();

            return winner;
        }
        else
        {
            return null;
        }
    }


    public void CallRestart()
    {
        StartCoroutine(Restart());
    }

    IEnumerator DetermineGameWinner()
    {
        yield return new WaitForSeconds(1f);
        if (OurTompoys[0].OurScore.WinRounds > OurTompoys[1].OurScore.WinRounds)
        {
            // На случай победы первого игрока
            GameOverScreen.Setup(OurTompoys[0].PlayerName, OurTompoys[0].OurScore.WinRounds);
        }
        else if (OurTompoys[1].OurScore.WinRounds > OurTompoys[0].OurScore.WinRounds)
        {
            // На случай победы второго игрока
            GameOverScreen.Setup(OurTompoys[1].PlayerName, OurTompoys[1].OurScore.WinRounds);
        }
        else
        {
            GameOverScreen.Setup("Draw", 0);
        }
    }
}
