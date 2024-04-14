using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_Lunka : MonoBehaviour
{
    public GameObject korgoolPrefab;
    public List<GameObject> Korgools;
    public bool CanBeTuz;
    public bool Tuz;
    public int index;
    public int KorgoolsCount;

    public bool TakenLunka=false;

    public S_Lunka NextLunka;

//*************************START************************************************
    void Start()
    {
        SpawnObjects(9);
    }

//*************************************ON CLICKED************************************************
    public void OnClicked()
    {
        // Debug.Log(KorgoolsCount);

        
    }

//******************************************REMOVE************************************************
    public void Remove()
    {
        if(KorgoolsCount==1)
        {
            for(int i=0; i<Korgools.Count; i++)
            {
                // Destroy(Korgools[i]);
            }
            Korgools.Clear();
            KorgoolsCount = 0;
        }
        else
        {
            if(Korgools.Count>0)
            {
                GameObject LastKorgool = Korgools.Last();
                for(int i=0; i<Korgools.Count-1; i++)
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
    KorgoolsCount++;
    Korgools.Add(Korgool);
    int TempKorgoolCount = KorgoolsCount;
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
    float xOffset = ((korgoolIndex % rowCount) - 1) * 0.25f; // Смещение по X, аналогично методу SpawnObjects
    float zOffset = ((korgoolIndex / rowCount) - 1) * 0.25f; // Смещение по Z, аналогично методу SpawnObjects
    return transform.position + Vector3.up * 1.5f + new Vector3(xOffset, 0f, zOffset);
}
//*******************************************MOVE************************************************
private IEnumerator MoveTowardsTarget(GameObject Korgool, Transform objectTransform, Vector3 targetPosition)
{
    float duration = 1f; // Длительность анимации (в секундах)
    float elapsedTime = 0f;

    Vector3 startPosition = objectTransform.position;

    // Пока не достигнут конечный пункт, продолжаем двигаться
    while (elapsedTime < duration)
    {
        // Вычисляем прогресс анимации от 0 до 1
        float t = Mathf.Clamp01(elapsedTime / duration);

        if(Korgool)
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
        
    if(TakenLunka==true)
    {
        TakingKorgools();
    }
}


//***************************************SPAWN OBJECTS************************************************
    void SpawnObjects(int count)
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 1.5f; // Позиция, над лункой
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
                    float xOffset = (j - middleRow) * 0.25f;
                    float zOffset = (i - middleColumn) * 0.25f;
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
        for(int i=0; i<Korgools.Count; i++)
            {
                Destroy(Korgools[i]);
            }
        Korgools.Clear();
        KorgoolsCount = 0;
        TakenLunka=false;
    }
}
