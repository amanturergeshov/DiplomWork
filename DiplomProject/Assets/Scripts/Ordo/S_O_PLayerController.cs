using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_O_PLayerController : MonoBehaviour
{
    public LayerMask layer;
    public GameObject Tompoy;

    public Vector3 MouseClickPosition;
    public Vector3 LaunchVector;
    private bool TompoyClicked = false;
    private float ImpulseForce = 0;

    void Update()
    {

        if (TompoyClicked == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartRay();
            }
        }
        else
        {
            SetLaunchVector();
            if (Input.GetMouseButtonUp(0))
            {
                LaunchTompoy();
            }
        }
    }

    void StartRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, layer))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject == Tompoy)
            {
                MouseClickPosition = Input.mousePosition;
                TompoyClicked = true;
            }
            else
            {
                Debug.Log("Object is not Our Tompoy");
            }
        }
    }

    Vector3 TempPos;
    void SetLaunchVector()
    {
        float xCoordinates = (MouseClickPosition - Input.mousePosition).x;
        float yCoordinates = (MouseClickPosition - Input.mousePosition).y;
        float tempX = (xCoordinates / Screen.width);
        float tempZ = (yCoordinates / Screen.height);
        TempPos = new Vector3(xCoordinates, 0, yCoordinates);

        ImpulseForce = Math.Clamp(((Vector3.Distance(MouseClickPosition, Input.mousePosition))/3), 10f, 100f);
        Debug.Log(ImpulseForce);

        LaunchVector = Vector3.ClampMagnitude(TempPos * 5, 1f);
        // Debug.Log($"Vector: {LaunchVector}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Tompoy.transform.position, LaunchVector * ImpulseForce);
    }

    void LaunchTompoy()
    {
        if (TompoyClicked == true)
        {
            Rigidbody rb = Tompoy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(LaunchVector * ImpulseForce, ForceMode.Impulse);
                TompoyClicked = false;
            }
            else
            {
                Debug.LogError("Rigidbody component not found on Tompoy object.");
            }
        }
    }
}
