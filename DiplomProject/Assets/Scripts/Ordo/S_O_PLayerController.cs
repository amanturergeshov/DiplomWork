using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool alchikKnockedOut = false;
    //**********************************************************************TOMPOY CONTROL PROPERTIES*******************************************************
    public LayerMask layer;
    public LayerMask layerForLaunch;
    public GameObject Tompoy;
    public Vector3 MouseClickPosition;
    public Vector3 LaunchVector;
    private bool TompoyClicked = false;
    private float ImpulseForce = 0;
    public List<GameObject> alchikObjects = new List<GameObject>();

    //****************************************************************************AIM LENGHT*****************************************************
    private float AimLineLenght = 0.03f;
    public GameObject AimLine;

    //**********************************************************************CAMERA PROPERTIES*******************************************************
    public float CameraRotationSpeed;

    private bool rotatingRight;
    private bool rotatingLeft;
    private Vector3 CameraHitPosition;
    private Camera mainCamera;

    //**********************************************************************TIMER*****************************************************************
    public float turnTimeLimit = 600f;
    private float turnTimer;
    private bool isTurnTimerRunning;
    public string TimerString;

    //************************************************************************DELEGATES*************************************************************
    public delegate void TurnChangeEvent(S_O_PLayerController newActivePlayer);
    public event TurnChangeEvent OnTurnChange;

    //**********************************************************************START*******************************************************
    private void Start()
    {
        turnTimer = turnTimeLimit;
        ResetTurn();
        UpdateTimerDisplay();

        mainCamera = Camera.main;
    }
    //*************************************************************TIMER FUNCTIONS****************************************************************
    void StartTurnTimer()
    {
        isTurnTimerRunning = true;
        StartCoroutine(TurnTimer());
    }
    void UpdateTimerDisplay()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(turnTimer);
        TimerString = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
    }

    IEnumerator TurnTimer()
    {
        while (turnTimer > 0 && isTurnTimerRunning)
        {
            yield return new WaitForSeconds(1f);
            turnTimer--;
            UpdateTimerDisplay();
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

                AimLine.SetActive(true);
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
            Vector3 tempTompoyPos = new Vector3(Tompoy.transform.position.x, CameraHitPosition.y, Tompoy.transform.position.z);
            Vector3 TestPos = (tempTompoyPos - CameraHitPosition);
            Vector3 TempPos = TestPos;

            ImpulseForce = Math.Clamp(((Vector3.Distance(Tompoy.transform.position, CameraHitPosition)) * 50), 1f, 100f);

            LaunchVector = Vector3.ClampMagnitude(TempPos * 5, 1f);

            DrawAimLine();
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

            AimLine.SetActive(false);


            isMyTurn = false;
            yield return new WaitForSeconds(2f);

            // Передача хода противнику только если не выбит ни один альчик
            if (!alchikKnockedOut)
            {
                GiveTurnToOponent();
                yield return new WaitForSeconds(1.5f);
                ReLocateTompoy();
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                isMyTurn = true;
                ReLocateTompoy();
            }


            // Сброс настроек хода
            ResetTurn();
        }
        else
        {
            Debug.LogError("Компонент Rigidbody не найден на объекте Tompoy.");
        }
    }

    //***********************************************************************AI LAUCH**********************************************************************
    IEnumerator LaunchTompoyTowardsRandomAlchik()
    {
        if (alchikObjects.Count > 0)
        {
            float delay = UnityEngine.Random.Range(2f, 3f);
            yield return new WaitForSeconds(delay);

            ImpulseForce = 110;
            GameObject randomAlchik = alchikObjects[UnityEngine.Random.Range(0, alchikObjects.Count)];

            Vector3 direction = randomAlchik.transform.position - Tompoy.transform.position;
            LaunchVector = direction.normalized;


            StartCoroutine(LaunchTompoy());
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

        Vector3 currentRotation = Tompoy.transform.rotation.eulerAngles;
        Tompoy.transform.rotation = Quaternion.Euler(0f, currentRotation.y, currentRotation.z);


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
        isMyTurn = false;
        isTurnTimerRunning = false;
        Oponent.isMyTurn = true;
        Oponent.StartTurnTimer();
        OnTurnChange?.Invoke(Oponent);
    }
    public void ResetTurn()
    {
        alchikKnockedOut = false;
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
        if (isAI == false && isMyTurn == true && TompoyClicked == true && ImpulseForce >= 25 && context.performed == true)
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
    //*********************************************************************DRAW AimLine*************************************************
    public void DrawAimLine()
    {
        AimLine.transform.forward = LaunchVector.normalized;

        AimLine.transform.localScale = new Vector3(0.25f, 0.25f, LaunchVector.magnitude * ImpulseForce * AimLineLenght);

        AimLine.transform.position = Tompoy.transform.position;
    }


    public void CompleteTurn()
    {
        ResetTurn();
        isMyTurn = false;
        isTurnTimerRunning = false;
        TompoyClicked = false;
    }
}
