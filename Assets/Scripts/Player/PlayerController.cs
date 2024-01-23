using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    //movement 
    [Tooltip("Constant for player movement")]
    public float MoveSpeed;
    [Tooltip("Constant for player movement")]
    public float MoveAcceleration;
    [Tooltip("Constant for player movement")]
    public float RotationSpeed;
    [Tooltip("Constant for player jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("Constant for player gravity")]
    public float Gravity = -9.81f;
    [Tooltip("Constant for player terminal velocity")]
    public float TerminalVelocity = 50f;

    [Tooltip("Reference to the empty on the player that the camera will follow")]
    public GameObject cinemachineTarget;

    [Tooltip("Layers that count as ground the player can jump off")]
    public LayerMask GroundLayers;

    //top and bottom limits for looking, shouldn't need changing in editor
    private float CineTopClamp = 90.0f;
    private float CineBottomClamp = -90.0f;

    private float cineTargetPitch;

    private float currentMoveSpeed;
    private float currentRotationSpeed;
    private float currentVerticalVelocity;


    //input
    private Vector2 movementInput;
    private Vector2 lookInput;
    private bool jumpInput;

    //components
    private CharacterController controller;
    private GameObject mainCamera;
    private Tool ActiveTool;
    private int _toolIndex;
    private int ToolIndex
    {
        get { return _toolIndex; }
        set
        {
            while (value < 0)
            {
                //not sure this is the best but it works
                value+= ToolInventory.Count;
            }
            _toolIndex = value % ToolInventory.Count;
        }
    }
    private PlayerUIManager UI;

    [Tooltip("List of all possible tools the player can have")]
    public List<Tool> ToolInventory;


    //debug
    Vector3 groundedSpherePos;
    // Start is called before the first frame update
    #region unityMethods
    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        UI = GetComponent<PlayerUIManager>();
        AddAllTools();
        this.ActiveTool = ToolInventory[0];
        UI.ToolInventory = ToolInventory;
        UI.InitHotbar(); // I wanted to do this in start() of UIManager but the inventory NEEDS to be initalised first 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Grounded());
        JumpAndGravity();
        Move();
        Look();
    }
    void FixedUpdate()
    {

    }
    #endregion

    #region InputRecievers
    public void OnUse()
    {
        //Debug.Log("Leftclick");
        UseTool();
    }
    public void OnSwitchTool()
    {
        //change tool
    }
    public void OnInteract()
    {
        Ray r = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.SphereCast(r, 2f, out RaycastHit hit, 10f))
        {
            hit.collider.SendMessage("PlayerInteract");
            //anything you can press "e" on will have a public funtion OnPlayerInteract that will do whatever it needs to when pressed
        }
    }
    public void OnHotbarScroll(InputValue val)
    {

        if (val.Get<float>() < 0)
        {
            SwitchTool(-1);
        }
        else if (val.Get<float>() > 0)
        {
            SwitchTool(1);
        }
        
    }
    public void OnMove(InputValue val)
    {
       // Debug.Log("Movement input received");
        movementInput = val.Get<Vector2>();
    }
    public void OnLook(InputValue val)
    {
        lookInput = val.Get<Vector2>();
    }
    public void OnJump(InputValue val)
    {
        jumpInput = val.isPressed;
    }
    #endregion
    private void SwitchTool(int slotsToMove)
    {
        ToolIndex += slotsToMove;
        UI.ActiveIndex = ToolIndex;
        ActiveTool = ToolInventory[ToolIndex];
        
    }
    private void Move()
    {
        float targetSpeed = MoveSpeed;
        if (movementInput == Vector2.zero) targetSpeed = 0.0f;

        float prevHorizSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        if (prevHorizSpeed < targetSpeed - 0.1f || prevHorizSpeed > targetSpeed + 0.1f)
        {
            currentMoveSpeed = Mathf.Lerp(prevHorizSpeed, targetSpeed, Time.deltaTime * MoveAcceleration);
        }
        else
        {
            currentMoveSpeed = targetSpeed;
        }
        Vector3 moveDirection = new Vector3(movementInput.x,0.0f,movementInput.y).normalized;
        if (movementInput != Vector2.zero) moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y; 
        controller.Move(moveDirection * (currentMoveSpeed * Time.deltaTime) + new Vector3(0f,currentVerticalVelocity) * Time.deltaTime);
    }
    private void Look()
    {
        cineTargetPitch += lookInput.y * RotationSpeed;
        currentRotationSpeed = lookInput.x * RotationSpeed;

        cineTargetPitch = ClampAngle(cineTargetPitch, CineBottomClamp, CineTopClamp);
        cinemachineTarget.transform.localRotation = Quaternion.Euler(cineTargetPitch, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * currentRotationSpeed);
    }
    private void JumpAndGravity()
    {
        if (Grounded())
        {
            //stopfall
            //allow jumping
            //clamp vertical velocity when grounded
            if (currentVerticalVelocity < 0.0f)
            {
                currentVerticalVelocity = -2f;
            }
            if (jumpInput)
            {
                //this maths is straight from FPSexample I haven't checked it at all

                // the square root of H * -2 * G = how much velocity needed to reach desired height
                currentVerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }
        else
        {
            //apply falling
            if (currentVerticalVelocity < TerminalVelocity)
            {
                currentVerticalVelocity += Gravity * Time.deltaTime;
            }
        }
    }
    private bool Grounded()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y -1f, transform.position.z);
        groundedSpherePos = spherePosition;
        return Physics.CheckSphere(spherePosition, 0.5f, GroundLayers, QueryTriggerInteraction.Ignore); 
    }
    public void UseTool()
    {
        Ray r = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        ActiveTool.Use(r);

    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    }
    private void AddAllTools()
    {
        foreach(Tools enumVal in Enum.GetValues(typeof(Tools)) )
        {
            Tool t = gameObject.AddComponent<Tool>();
            t.toolType = enumVal;
            ToolInventory.Add(t);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundedSpherePos, 0.5f);
    }
}