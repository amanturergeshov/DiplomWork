using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_O_PLayerController : MonoBehaviour
{
    //************************************************************************PRIMITIVE AI PROPERTIES*********************************************
    public bool isAI;
    //********************************************************************PLAYER PROPERTIES************************************************
    public string PlayerName;
    public bool isMyTurn;
    public S_O_Score OurScore;
    public S_O_PLayerController Oponent;
    //**********************************************************************TOMPOY CONTROL PROPERTIES*******************************************************
    public LayerMask layer;
    public LayerMask layerForLaunch;
    public GameObject Tompoy;
    public Vector3 MouseClickPosition;
    public Vector3 LaunchVector;
    private bool TompoyClicked = false;
    private float ImpulseForce = 0;
    public List<GameObject> alchikObjects = new List<GameObject>();

    //**********************************************************************CAMERA PROPERTIES*******************************************************
    public float CameraRotationSpeed;

    private bool rotatingRight;
    private bool rotatingLeft;
    private Vector3 CameraHitPosition;
    private Camera mainCamera;
    //************************************************************************DELEGATES*************************************************************
    public delegate void TurnChangeEvent(S_O_PLayerController newActivePlayer);
    public event TurnChangeEvent OnTurnChange;

    //**********************************************************************START*******************************************************
    private void Start()
    {
        if (isMyTurn == false)
        {
            GiveTurnToOponent();
        }

        mainCamera = Camera.main;

        //Запоминаем все альчики на сцене
        GameObject[] alchikArray = GameObject.FindGameObjectsWithTag("Alchik");
        foreach (GameObject obj in alchikArray)
        {
            alchikObjects.Add(obj);
        }
    }
    //**********************************************************************UPDATE*******************************************************
    void Update()
    {
        if (isAI == true && isMyTurn == true)
        {
            isMyTurn = false;
            StartCoroutine(LaunchTompoyTowardsRandomAlchik());
        }
        if (isAI == false && isMyTurn == true && TompoyClicked == true)
        {
            SetLaunchVector();
        }
        CameraRotate();
    }


    //**********************************************************************CAMERA ROTATE*******************************************************
    void CameraRotate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            rotatingRight = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            rotatingLeft = true;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            rotatingRight = false;
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            rotatingLeft = false;
        }
        if (rotatingRight)
        {
            transform.Rotate(0, CameraRotationSpeed * Time.deltaTime, 0);
        }
        else if (rotatingLeft)
        {
            transform.Rotate(0, -CameraRotationSpeed * Time.deltaTime, 0);
        }
    }

    //**********************************************************************START RAY*******************************************************
    void StartRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, layer))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject == Tompoy)
            {
                MouseClickPosition = Tompoy.transform.position;
                TompoyClicked = true;
            }
            else
            {
                Debug.Log("Object is not Our Tompoy");
            }
        }
    }

    //**********************************************************************SET LAUCH VECTOR*******************************************************
    void SetLaunchVector()
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, layerForLaunch))
        {
            CameraHitPosition = hit.point;
            Vector3 TestPos = (Tompoy.transform.position - CameraHitPosition);
            Vector3 TempPos = TestPos;

            ImpulseForce = Math.Clamp(((Vector3.Distance(Tompoy.transform.position, CameraHitPosition)) * 50), 1f, 100f);

            LaunchVector = Vector3.ClampMagnitude(TempPos * 5, 1f);
        }

    }

    //**********************************************************************DEBUG GIZMO*******************************************************
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Tompoy.transform.position, Tompoy.transform.position + LaunchVector * ImpulseForce);
        Gizmos.DrawWireSphere(CameraHitPosition, 1f);
    }

    //**********************************************************************LAUNCH TOMPOY*******************************************************
    IEnumerator LaunchTompoy()
    {
        Rigidbody rb = Tompoy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(LaunchVector * ImpulseForce, ForceMode.Impulse);
            TompoyClicked = false;

            isMyTurn = false;

            yield return new WaitForSeconds(2f);
            GiveTurnToOponent();
            // isMyTurn = true;
            yield return new WaitForSeconds(1.5f);
            ReLocateTompoy();

        }
        else
        {
            Debug.LogError("Rigidbody component not found on Tompoy object.");
        }

    }

    //***********************************************************************AI LAUCH**********************************************************************
    IEnumerator LaunchTompoyTowardsRandomAlchik()
    {
        if (alchikObjects.Count > 0)
        {
            ImpulseForce = 80;
            GameObject randomAlchik = alchikObjects[UnityEngine.Random.Range(0, alchikObjects.Count)];

            Vector3 direction = randomAlchik.transform.position - Tompoy.transform.position;
            LaunchVector = direction.normalized;

            float delay = UnityEngine.Random.Range(2f, 3f);
            yield return new WaitForSeconds(delay);

            StartCoroutine(LaunchTompoy());
        }
        else
        {
            Debug.LogWarning("No alchik objects found.");
        }
    }
    //**************************************************************************RELOCATE TOMPOY***********************************************
    public void ReLocateTompoy()
    {
        Vector3 playZoneCenter = new Vector3(0, 0, 0);

        float moveRadius = 5f;

        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * moveRadius;

        Vector3 newPosition = playZoneCenter + new Vector3(randomPoint.x, 1f, randomPoint.y);

        Tompoy.transform.position = newPosition;

    }
    //**************************************************************************RELOCATE TOMPOY***********************************************
    void MoveTompoyToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, layerForLaunch))
        {
            // Проверяем, есть ли на объекте скрипт S_O_PlayerZone
            S_O_PlayZone playerZoneScript = hit.collider.GetComponent<S_O_PlayZone>();
            if (playerZoneScript == null)
            {
                // Если скрипта нет, перемещаем томпоя в точку клика
                MouseClickPosition = hit.point;
                Tompoy.transform.position = MouseClickPosition;
            }
            else
            {
                // Если есть, не перемещаем томпоя
                Debug.Log("Clicked on object with S_O_PlayerZone script. Tompoy cannot be moved.");
            }
        }
    }

    public void GiveTurnToOponent()
    {
        Oponent.isMyTurn = true;

        OnTurnChange?.Invoke(Oponent);
    }


    //*********************************************************************INPUTS*************************************************
    public void OnTapLMB(InputAction.CallbackContext context)
    {
        if (isAI == false && isMyTurn == true && TompoyClicked == false && context.performed == true)
        {
            MoveTompoyToMousePosition();
        }
    }

    public void OnReleaseLMB(InputAction.CallbackContext context)
    {
        Debug.Log("I'm Trying!");
        if (isAI == false && isMyTurn == true && TompoyClicked == true && ImpulseForce >= 10 && context.performed == true)
        {
            Debug.Log(ImpulseForce);
            StartCoroutine(LaunchTompoy());
        }
        else
        {
            TompoyClicked = false;
        }
    }

    public void OnPressLMB(InputAction.CallbackContext context)
    {

        if (isAI == false && isMyTurn == true && TompoyClicked == false && context.performed == true)
        {
            StartRay();
        }

    }

}
