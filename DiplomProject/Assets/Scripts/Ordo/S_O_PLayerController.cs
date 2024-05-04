using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_O_PLayerController : MonoBehaviour
{
    public LayerMask layer;
    public GameObject Tompoy;

    public Vector3 MouseClickPosition;
    public Vector3 LaunchVector;
    private bool TompoyClicked = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TompoyClicked == false)
            {
                StartRay();
            }
        }
        if (Input.GetMouseButtonUp(0) && TompoyClicked == true)
        {
            SetLaunchVector();
            LaunchTompoy();
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

    void SetLaunchVector()
    {
        float xCoordinates = (MouseClickPosition - Input.mousePosition).x;
        float yCoordinates = (MouseClickPosition - Input.mousePosition).y;
        float tempX = (xCoordinates / Screen.width);
        float tempZ = (yCoordinates / Screen.height);
        Vector3 TempPos = new Vector3(tempX, 0, tempZ);

        LaunchVector = Vector3.ClampMagnitude(TempPos * 5, 1f);
    }

    void LaunchTompoy()
    {
        Rigidbody rb = Tompoy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(LaunchVector * 35, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogError("Rigidbody component not found on Tompoy object.");
        }
        TompoyClicked = false;
    }
}
