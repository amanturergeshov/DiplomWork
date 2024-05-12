using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class S_O_PLayerController_Old : MonoBehaviour
{
    //************************************************************************PRIMITIVE AI PROPERTIES*********************************************
    private bool AITurn;
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

    private float lastClickTime = 0f;
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
        if (isAI == false && isMyTurn == true)
        {
            PlayerTurn();
        }
        CameraRotate();
    }
    //******************************************************************PLAYER TURN*************************************************************
    void PlayerTurn()
    {

        if (TompoyClicked == false)
        {
            // Проверяем двойной клик
            if (Input.GetMouseButtonDown(0))
            {
                
                if (Time.time - lastClickTime < 0.2f) // Проверяем временной интервал между кликами (0.2 секунды)
                {
                    MoveTompoyToMousePosition(); // Если интервал между кликами мал, перемещаем Томпоя
                }
                else
                {
                    StartRay(); // Если не двойной клик, запускаем обычную логику
                }
                lastClickTime = Time.time; // Сохраняем время последнего клика
            }
        }
        else
        {
            SetLaunchVector();
            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(LaunchTompoy());
            }
        }
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

            ImpulseForce = Math.Clamp(((Vector3.Distance(Tompoy.transform.position, CameraHitPosition)) * 30), 10f, 100f);
            // Debug.Log(ImpulseForce);

            LaunchVector = Vector3.ClampMagnitude(TempPos * 5, 1f);
            // Debug.Log($"Vector: {LaunchVector}");

            // Debug.Log($"Vector: {TempPos.normalized * ImpulseForce}");
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
            // Choose a random alchik object
            GameObject randomAlchik = alchikObjects[UnityEngine.Random.Range(0, alchikObjects.Count)];

            // Calculate launch vector towards the random alchik
            Vector3 direction = randomAlchik.transform.position - Tompoy.transform.position;
            LaunchVector = direction.normalized;

            // Add a random delay between 2 to 3 seconds
            float delay = UnityEngine.Random.Range(2f, 3f);
            yield return new WaitForSeconds(delay);

            // Launch Tompoy after the delay
            StartCoroutine(LaunchTompoy());
            // GiveTurnToOponent();
        }
        else
        {
            Debug.LogWarning("No alchik objects found.");
        }
    }
    //**************************************************************************RELOCATE TOMPOY***********************************************
    public void ReLocateTompoy()
    {
        Tompoy.transform.position = MouseClickPosition;
    }
    //**************************************************************************RELOCATE TOMPOY***********************************************
    void MoveTompoyToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, layerForLaunch))
        {
            MouseClickPosition = hit.point;
            Tompoy.transform.position = MouseClickPosition; // Перемещаем Томпоя в точку клика
        }
    }

    public void GiveTurnToOponent()
    {
        Oponent.isMyTurn = true;


        OnTurnChange?.Invoke(Oponent);
    }

}
