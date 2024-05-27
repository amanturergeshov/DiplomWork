using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_O_GameManager : MonoBehaviour
{
    public List<S_O_PLayerController> OurTompoys;
    public float timerDuration = 1f;
    public float launchForce = 100f;
    public float maxRotationSpeed = 5600f;
    private bool gameEnded = false;

    public GameObject PlayZone;
    public GameObject prefabToSpawn; // Префаб для спавна
    public int numberOfObjects = 100; // Общее количество объектов для спавна
    public float radiusStep = 1f; // Шаг радиуса для увеличения

    private List<GameObject> spawnedObjects = new List<GameObject>(); // Список для хранения заспавненных объектов

    void Start()
    {
        if (OurTompoys.Count == 2)
        {
            StartCoroutine(TossUp());
        }
        else
        {
            Debug.LogError("OurTompoys should contain exactly 2 Tompoys.");
        }

    }

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
        S_O_PLayerController winner = null;
        foreach (var Tompoy in OurTompoys)
        {
            if (Math.Round(Tompoy.Tompoy.transform.rotation.z) == 0 || Math.Round(Tompoy.Tompoy.transform.rotation.z) == 180)
            {
                winner = Tompoy;
                counter++;
            }
        }
        if (counter == 1)
        {
            Debug.Log("Winner");
            if (winner)
            {
                Debug.Log(winner);
            }
            gameEnded = true;
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

            winner.Oponent.GiveTurnToOponent();
        }
    }
}