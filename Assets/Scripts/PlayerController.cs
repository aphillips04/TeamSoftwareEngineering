using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //movement 
    [Tooltip("Constant for player movement")]
    public float MoveSpeed;
    [Tooltip("Constant for player movement")]
    public float MoveAcceleration;
    [Tooltip("Constant for player movement")]
    public float RotationSpeed;

    [Tooltip("Reference to the empty on the player that the camera will follow")]
    public GameObject cinemachineTarget;

    //top and bottom limits for looking, shouldn't need changing in editor
    private float CineTopClamp = 90.0f;
    private float CineBottomClamp = -90.0f;

    private float cineTargetPitch;

    private float currentMoveSpeed;
    private float currentRotationSpeed;

    //input
    private Vector2 movementInput;
    private Vector2 lookInput;

    //components
    private CharacterController controller;
    private GameObject mainCamera;
    private Tool ActiveTool;

    [Tooltip("Array of all possible tools the player can have")]
    public Tool[] ToolInventory;
    // Start is called before the first frame update
    #region unityMethods
    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        this.ActiveTool = ToolInventory[0];
    }

    // Update is called once per frame
    void Update()
    {
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
    #endregion
    private void SwitchTool(int slotsToMove)
    {
        // PLEASE REMEMBER THIS *WILL* CAUSE OUTOFRANGE EXCEPTIONS
        // CHANGE IT SOON
        ActiveTool = ToolInventory[slotsToMove];
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
        controller.Move(moveDirection * (currentMoveSpeed * Time.deltaTime));
    }
    private void Look()
    {
        cineTargetPitch += lookInput.y * RotationSpeed;
        currentRotationSpeed = lookInput.x * RotationSpeed;

        cineTargetPitch = ClampAngle(cineTargetPitch, CineBottomClamp, CineTopClamp);
        cinemachineTarget.transform.localRotation = Quaternion.Euler(cineTargetPitch, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * currentRotationSpeed);
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
}
