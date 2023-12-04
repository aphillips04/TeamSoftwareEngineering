using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed;
    public float MoveAcceleration;
    public float RotationSpeed;

    public GameObject cinemachineTarget;
    public float CineTopClamp = 90.0f;
    public float CineBottomClamp = -90.0f;

    private float cineTargetPitch;

    private float currentMoveSpeed;
    private float currentRotationSpeed;

    private Vector2 movementInput;
    private Vector2 lookInput;
    private CharacterController controller;
    private GameObject mainCamera;

    private ToolPoke ActiveTool;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        this.ActiveTool = GetComponent<ToolPoke>();
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
    public void UseTool()
    {
        Ray r = new Ray(transform.position,mainCamera.transform.forward);
        ActiveTool.Use(r);

    }
    public void OnUse()
    {
        //Debug.Log("Leftclick");
        UseTool();
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
