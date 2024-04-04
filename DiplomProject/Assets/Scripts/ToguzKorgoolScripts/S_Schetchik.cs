using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class S_Schetchik : MonoBehaviour
{
    // Start is called before the first frame update
    public int score;
    
    public List<GameObject> Korgools;
    public GameObject korgoolPrefab;

    public void ApplyScore(int num)
    {
        Clear();
        score = score + num;
        SpawnObjects(score);
    }

    void SpawnObjects(int count)
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 1.5f; // Позиция, над лункой
        int rowCount = 50; // количество строк
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
    }

    public void Clear()
    {
        for(int i=0; i<Korgools.Count; i++)
            {
                Destroy(Korgools[i]);
            }
        Korgools.Clear();
    }
}
