using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_O_GameManager : MonoBehaviour
{
    public List<S_O_PLayerController> OurPlayers;
    public float timerDuration = 2f;
    public float launchForce = 100f;
    public float maxRotationSpeed = 5600f;
    private bool gameEnded = false;

    void Start()
    {
        if (OurPlayers.Count == 2)
        {
            StartCoroutine(GameLoop());
        }
        else
        {
            Debug.LogError("OurPlayers should contain exactly 2 players.");
        }
    }

    IEnumerator GameLoop()
    {
        while (!gameEnded)
        {
            PositionPlayers();
            yield return StartCoroutine(LaunchPlayersAfterTimer());

            // Ждем пока один из игроков не упадет с ротацией по Z = 90 градусов
            yield return new WaitForSeconds(5f);
            CheckForGameEnd();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void PositionPlayers()
    {
        OurPlayers[0].Tompoy.transform.position = new Vector3(-1, 0, 0);
        OurPlayers[0].Tompoy.transform.rotation = Quaternion.identity;

        OurPlayers[1].Tompoy.transform.position = new Vector3(1, 0, 0);
        OurPlayers[1].Tompoy.transform.rotation = Quaternion.identity;
    }

    IEnumerator LaunchPlayersAfterTimer()
    {
        yield return new WaitForSeconds(timerDuration);

        foreach (var player in OurPlayers)
        {
            Rigidbody rb = player.Tompoy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * launchForce, ForceMode.VelocityChange);

                Vector3 randomRotationAxis = new Vector3(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)
                ).normalized;

                float randomRotationSpeed = UnityEngine.Random.Range(maxRotationSpeed-100, maxRotationSpeed);

                rb.AddTorque(randomRotationAxis * randomRotationSpeed, ForceMode.VelocityChange);
            }
            else
            {
                Debug.LogError("Rigidbody is missing on the player: " + player.name);
            }
        }
    }

    void CheckForGameEnd()
    {
        int counter = 0;
        S_O_PLayerController winner=null;
        foreach (var player in OurPlayers)
        {
            if (Math.Round(player.Tompoy.transform.rotation.z) == 0 || Math.Round(player.Tompoy.transform.rotation.z) == 180)
            {
                winner = player;
                counter++;
            }
        }
        if (counter == 1)
        {
            Debug.Log(" winner ");
            if (winner)
            {
                Debug.Log(winner);
            }
            // gameEnded = true;
        }
    }
}
