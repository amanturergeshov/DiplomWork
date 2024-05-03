using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class S_TK_Lunka : MonoBehaviour
{

    public Material TuzMaterial; // Ссылка на материал, который вы хотите присвоить создаваемому объекту
    public GameObject korgoolPrefab;
    public List<GameObject> Korgools;
    public bool CanBeTuz;
    public bool Tuz;

    private float xKorgoolOffset = 0.30f;
    private float zKorgoolOffset = 0.30f;
    public int index;
    public int KorgoolsCount;

    public int StartKorgoolsCount = 9;

    [HideInInspector]
    public float moveTime = 1f;
    private float RebuildMoveTime = 0.3f;

    public bool TakenLunka = false;

    public S_TK_Lunka NextLunka;
    private float SpawnHigh = 0.5f;

    // Делегат и событие для вызова функции ApplyScore
    public delegate void ScoreAppliedEventHandler(int score);
    public event ScoreAppliedEventHandler ScoreApplied;

    //  Делегат и событие для вызова метода CompleteTurn

    //*************************START************************************************
    void Start()
    {
        moveTime = 0.7f;
        SpawnObjects(StartKorgoolsCount);
    }

    //*************************************ON CLICKED************************************************
    public void OnClicked()
    {
        // Debug.Log(KorgoolsCount);


    }

    //******************************************REMOVE************************************************
    public void Remove()
    {
        if (KorgoolsCount == 1)
        {
            for (int i = 0; i < Korgools.Count; i++)
            {
                // Destroy(Korgools[i]);
            }
            Korgools.Clear();
            KorgoolsCount = 0;
        }
        else
        {
            if (Korgools.Count > 0)
            {
                GameObject LastKorgool = Korgools.Last();
                for (int i = 0; i < Korgools.Count - 1; i++)
                {
                    // Destroy(Korgools[i]);
                }
                Korgools.Clear();
                Korgools.Add(LastKorgool);
                KorgoolsCount = Korgools.Count();
            }
        }

    }

    //**********************************ADD KORGOOL************************************************
    public void AddKorgool(GameObject Korgool)
    {
        // KorgoolsCount++;
        Korgools.Add(Korgool);
        int TempKorgoolCount = KorgoolsCount + 1;
        // KorgoolsCount = 1;

        // Определяем позицию, к которой нужно двигаться
        Vector3 targetPosition = CalculateTargetPosition(TempKorgoolCount);

        // Запускаем корутину для плавного перемещения
        StartCoroutine(MoveTowardsTarget(Korgool, Korgool.transform, targetPosition));
    }
    //**********************************CALCULATE TARGET POSITION************************************
    private Vector3 CalculateTargetPosition(int korgoolIndex)
    {
        korgoolIndex--;
        int rowCount = 3; // Количество строк из метода SpawnObjects
        float xOffset = ((korgoolIndex % rowCount) - 1) * xKorgoolOffset; // Смещение по X, аналогично методу SpawnObjects
        float zOffset = ((korgoolIndex / rowCount) - 1) * zKorgoolOffset; // Смещение по Z, аналогично методу SpawnObjects
        return transform.position + Vector3.up * SpawnHigh + new Vector3(xOffset, 0f, zOffset);
    }
    //*******************************************MOVE************************************************
    private IEnumerator MoveTowardsTarget(GameObject Korgool, Transform objectTransform, Vector3 targetPosition)
    {
        yield return new WaitForSeconds(0.3f);
        float duration = moveTime; // Длительность анимации (в секундах)
        float elapsedTime = 0f;

        Vector3 startPosition = objectTransform.position;

        // Пока не достигнут конечный пункт, продолжаем двигаться
        while (elapsedTime < duration)
        {
            // Вычисляем прогресс анимации от 0 до 1
            float t = Mathf.Clamp01(elapsedTime / duration);

            if (Korgool)
            {
                // Применяем интерполяцию по дуге между начальной и конечной позициями
                objectTransform.position = Vector3.Slerp(startPosition, targetPosition, t);

                // Обновляем время
                elapsedTime += Time.deltaTime;
            }
            // Ждем один кадр
            yield return null;
        }

        // Устанавливаем точную целевую позицию (избегаем погрешности)
        objectTransform.position = targetPosition;

        KorgoolsCount = Korgools.Count;

        if (TakenLunka == true)
        {
            TakingKorgools();
        }
        else if (Tuz == true)
        {
            TakingKorgools();
        }
        else
        {
            if (Korgools.Count == 2)
            {
                CanBeTuz = true;
            }
            else
            {
                CanBeTuz = false;
            }
            StartCoroutine(RebuildKorgoolsSmoothly());
        }
    }


    //***************************************SPAWN OBJECTS************************************************
    void SpawnObjects(int count)
    {
        Vector3 spawnPosition = transform.position + Vector3.up * SpawnHigh; // Позиция, над лункой
        int rowCount = 3; // количество строк
        int numberOfObjects = count;
        int columnCount = Mathf.CeilToInt((float)numberOfObjects / rowCount); // количество столбцов, округленное вверх
        float middleRow = (rowCount - 1) / 2f;
        float middleColumn = (columnCount - 1) / 2f;

        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                int index = i * rowCount + j;
                if (index < numberOfObjects)
                {
                    // Вычисляем смещение относительно середины количества столбцов и строк
                    float xOffset = (j - middleRow) * xKorgoolOffset;
                    float zOffset = (i - middleColumn) * zKorgoolOffset;
                    Vector3 randomOffset = new Vector3(xOffset, 0f, zOffset); // смещение только по оси X и Z
                    GameObject newKorgool = Instantiate(korgoolPrefab, spawnPosition + randomOffset, Quaternion.identity);
                    Korgools.Add(newKorgool);
                }
            }
        }
        KorgoolsCount = Korgools.Count;
    }
    //***************************************TAKING KORGOOLS***************************************
    public void TakingKorgools()
    {
        // Вызываем событие для передачи счета
        ScoreApplied?.Invoke(KorgoolsCount);
        for (int i = 0; i < Korgools.Count; i++)
        {
            Destroy(Korgools[i]);
        }
        Korgools.Clear();
        KorgoolsCount = 0;
        TakenLunka = false;
    }
    //************************************************REBUILD*******************************************

    private IEnumerator RebuildKorgoolsSmoothly()
    {
        // Вычисляем количество рядов и столбцов
        int rowCount = 3;
        int columnCount = Mathf.CeilToInt((float)KorgoolsCount / rowCount);

        // Вычисляем смещение между корголами
        float xOffset = xKorgoolOffset;
        float zOffset = zKorgoolOffset;

        // Начальная позиция для первого коргола
        Vector3 startPosition = transform.position + Vector3.up * SpawnHigh - new Vector3((rowCount - 1) * xOffset / 2f, 0f, (columnCount - 1) * zOffset / 2f);

        // Индекс текущего коргола
        int korgoolIndex = 0;

        // Проходимся по всем рядам и столбцам
        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                // Проверяем, есть ли коргол в текущей позиции
                if (korgoolIndex < Korgools.Count)
                {
                    // Вычисляем целевую позицию для текущего коргола
                    Vector3 targetPosition = startPosition + new Vector3(j * xOffset, 0f, i * zOffset);

                    // Получаем текущий коргол
                    GameObject currentKorgool = Korgools[korgoolIndex];

                    // Запускаем анимацию перемещения текущего коргола к целевой позиции
                    StartCoroutine(MoveKorgoolSmoothly(currentKorgool, currentKorgool.transform, targetPosition));

                    // Увеличиваем индекс текущего коргола
                    korgoolIndex++;
                }
            }
        }

        // Ждем завершения анимации перемещения всех корголов
        yield return new WaitForSeconds(moveTime);

        // Очищаем список корголов (если это необходимо)
        for (int i = korgoolIndex; i < Korgools.Count; i++)
        {
            Destroy(Korgools[i]);
        }
        Korgools.RemoveRange(korgoolIndex, Korgools.Count - korgoolIndex);
    }

    // Метод для плавного перемещения коргола в новую позицию
    private IEnumerator MoveKorgoolSmoothly(GameObject Korgool, Transform objectTransform, Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        // Запоминаем начальную позицию коргола
        Vector3 startPosition = objectTransform.position;

        // Проходим анимацию до целевой позиции
        while (elapsedTime < RebuildMoveTime)
        {
            // Вычисляем прогресс анимации от 0 до 1
            float t = Mathf.Clamp01(elapsedTime / RebuildMoveTime);

            // Применяем интерполяцию между начальной и целевой позициями
            objectTransform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Увеличиваем время
            elapsedTime += Time.deltaTime;

            // Ждем один кадр
            yield return null;
        }

        // Устанавливаем точную целевую позицию (избегаем погрешности)
        objectTransform.position = targetPosition;
    }

    //*****************************************************SPAWN TUZ IN LUNKA*****************************************************
    public void SpawnTuzInLunka()
    {
        Vector3 spawnPosition = transform.position + Vector3.up * SpawnHigh; // Позиция, над лункой
        int rowCount = 3; // количество строк
        int numberOfObjects = 1;
        int columnCount = Mathf.CeilToInt((float)numberOfObjects / rowCount); // количество столбцов, округленное вверх
        float middleRow = (rowCount - 1) / 2f;
        float middleColumn = (columnCount - 1) / 2f;

        GameObject newKorgool = Instantiate(korgoolPrefab, spawnPosition, Quaternion.identity);
        // Присваиваем созданному объекту материал
        if (TuzMaterial != null)
        {
            Renderer renderer = newKorgool.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = TuzMaterial;
            }
        }
    }

}
