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
    public float CameraRotationSpeed;

    public bool rotatingRight;
    public bool rotatingLeft;

    public LayerMask layerForLaunch;

    private Vector3 CameraHitPosition;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
    }
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
        CameraRotate();

    }

    void CameraRotate()
    { // Вращение камеры в зависимости от нажатой клавиши
        if (Input.GetKeyDown(KeyCode.E))
        {
            rotatingRight = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            rotatingLeft = true;
        }

        // Остановка вращения камеры при отпускании клавиши
        if (Input.GetKeyUp(KeyCode.E))
        {
            rotatingRight = false;
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            rotatingLeft = false;
        }

        // Вращение камеры вокруг объекта
        if (rotatingRight)
        {
            transform.Rotate(0, CameraRotationSpeed * Time.deltaTime, 0);
        }
        else if (rotatingLeft)
        {
            transform.Rotate(0, -CameraRotationSpeed * Time.deltaTime, 0);
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
                // MouseClickPosition = Input.mousePosition;
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

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, layerForLaunch))
        {
            CameraHitPosition = hit.point;
            Vector3 TestPos = (Tompoy.transform.position - CameraHitPosition);
            Vector3 TempPos = TestPos;

            ImpulseForce = Math.Clamp(((Vector3.Distance(Tompoy.transform.position, CameraHitPosition))*30), 10f, 100f);
            Debug.Log(ImpulseForce);

            LaunchVector = Vector3.ClampMagnitude(TempPos * 5, 1f);
            // Debug.Log($"Vector: {LaunchVector}");

            // Debug.Log($"Vector: {TempPos.normalized * ImpulseForce}");
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Tompoy.transform.position, Tompoy.transform.position + LaunchVector * ImpulseForce);
        Gizmos.DrawWireSphere(CameraHitPosition, 1f);
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
