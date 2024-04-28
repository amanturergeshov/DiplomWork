using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class S_Schetchik : MonoBehaviour
{
    // Start is called before the first frame update
    public int score;
    public S_PlayerController OwnerPlayer;
    public List<GameObject> Korgools;
    [NonSerialized]
    public bool isWinner = false;
    public int WinScore = 81;
    private float xKorgoolOffset = 0.28f;
    private float zKorgoolOffset = 0.28f;
    public GameObject korgoolPrefab;

    //********************Нужно перенести в PlayerController**********************
    public S_GameOverScreen GameOverScreen;

    //**************************************************************************
    public void ApplyScore(int num)
    {
        Clear();
        score = score + num;
        SpawnObjects(score);
        CheckWin();
    }

    void SpawnObjects(int count)
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f; // Позиция, над лункой
        int rowCount = 45; // количество строк
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
    }

    public void Clear()
    {
        for (int i = 0; i < Korgools.Count; i++)
        {
            Destroy(Korgools[i]);
        }
        Korgools.Clear();
    }

    public void CheckWin()
    {
        if (score > WinScore)
        {
            isWinner = true;
            GameOver();
        }
    }

    //********************Нужно перенести в PlayerController**********************
    public void GameOver()
    {
        GameOverScreen.Setup(OwnerPlayer.PlayerName);
        // OwnerPlayer.isMyTurn = false;
        // OwnerPlayer.Oponent.isMyTurn = false;
        OwnerPlayer.OnGameOver();
        OwnerPlayer.Oponent.OnGameOver();
    }
}
