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

                float randomRotationSpeed = UnityEngine.Random.Range(maxRotationSpeed - 100, maxRotationSpeed);

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
            Debug.Log(" winner ");
            if (winner)
            {
                Debug.Log(winner);
            }
            gameEnded = true;
            foreach (var Tompoy in OurTompoys)
            {
                Tompoy.ReLocateTompoy();
            }
            winner.Oponent.GiveTurnToOponent();
            PlayZone.SetActive(true);
        }
    }
}
