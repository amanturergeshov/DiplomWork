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
    public Vector2 offset;
    public int KorgoolsCount;

    public S_Lunka NextLunka;

    void Start()
    {
        SpawnObjects(9);
    }

    public void OnClicked()
    {
        // Debug.Log(KorgoolsCount);

        
    }
    public void Remove()
    {
        if(KorgoolsCount==1)
        {
            for(int i=0; i<Korgools.Count; i++)
            {
                Destroy(Korgools[i]);
            }
            Korgools.Clear();
            KorgoolsCount = 0;
        }
        else
        {
            GameObject LastKorgool = Korgools.Last();
            for(int i=0; i<Korgools.Count-1; i++)
            {
                Destroy(Korgools[i]);
            }
            Korgools.Clear();
            Korgools.Add(LastKorgool);
            KorgoolsCount = Korgools.Count();
        }
        
    }
    public void AddKorgool()
    {
        KorgoolsCount++;
        int TempKorgoolCount = KorgoolsCount;
        KorgoolsCount=1;

        Clear();

        SpawnObjects(TempKorgoolCount);
    }

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
    public void Clear()
    {
        for(int i=0; i<Korgools.Count; i++)
            {
                Destroy(Korgools[i]);
            }
        Korgools.Clear();
        KorgoolsCount = 0;
    }
}
