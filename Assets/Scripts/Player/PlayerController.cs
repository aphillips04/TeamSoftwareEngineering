using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using System.Threading;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
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
    private float currentMoveSpeed;
    private float currentRotationSpeed;
    private float currentVerticalVelocity;

    [Header("Camera")]
    [Tooltip("Reference to the empty on the player that the camera will follow")]
    public GameObject cinemachineTarget;
    // top and bottom limits for looking, shouldn't need changing in editor
    private float CineTopClamp = 90.0f;
    private float CineBottomClamp = -90.0f;
    private float cineTargetPitch;

    [Header("Layers")]
    [Tooltip("Layers that count as ground the player can jump off")]
    public LayerMask GroundLayers;
    [Tooltip("Layers that count as the player itself")]
    public LayerMask PlayerLayers;

    [Header("Input")]
    private Vector2 movementInput;
    private Vector2 lookInput;
    private bool jumpInput;

    [Header("Components")]
    public Image canvas;
    public GameObject player;
    private GameObject mainCamera;
    private CharacterController controller;
    private PlayerUIManager UI;
    private DayCycle dayCycle;

    [Header("Tools")]
    public Tool ActiveTool;
    [Tooltip("List of all possible tools the player can have")]
    public List<Tool> ToolInventory;
    private int _toolIndex;
    private int ToolIndex
    {
        get { return _toolIndex; }
        set
        {
            while (value < 0) value += ToolInventory.Count;
            _toolIndex = value % ToolInventory.Count;
        }
    }
    
    Vector3 groundedSpherePos; // TODO! NEEDS MOVING TO APPROPRIATE SECTION ABOVE
    #region unityMethods
    // Start is called before the first frame update
    void Start()
    {
        // Assign components
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        controller = GetComponent<CharacterController>();
        UI = GetComponent<PlayerUIManager>();
        dayCycle = GetComponent<DayCycle>();

        // Init tools
        AddAllTools();
        ActiveTool = ToolInventory[0];
        UI.ToolInventory = ToolInventory;
        UI.InitHotbar(); // Must be done here instead of UIManager as it is dependant on the inventory being initialised first
     }

    // Update is called once per frame
    void Update()
    {
        JumpAndGravity();
        DoMovement();
        DoLook();
    }
    #endregion

    #region InputRecievers
    // Handle exhaustion and tool use
    public void OnUse()
    {
        // Don't interact if within book
        if (UI.BookUI.enabled) return;
        
        // Handle exhaustion
        if (dayCycle.exhaustionMeter == 100) { NotifSys.system.notify("You are too tired to perform any more actions!\nYou should rest!"); return; }
        dayCycle.OnAction();

        // Use the active tool with a ray cast forward from camera
        ActiveTool.Use(new Ray(mainCamera.transform.position, mainCamera.transform.forward));
    }
    // Casts a ray to a max distances and checks for collison with interactable objects
    public void OnInteract()
    {
        // Don't interact if within book
        if (UI.BookUI.enabled) return;
     
        Ray r = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(r.origin, r.direction, Color.green, 2.5f);
        // All interactable objects must have an 'OnPlayerInteract' inteface
        if (Physics.Raycast(r, out RaycastHit hit, 10f, ~PlayerLayers)) hit.collider.SendMessage("OnPlayerInteract");
    }
    // Scrolls through hotbar
    public void OnHotbarScroll(InputValue val)
    {
        if (val.Get<float>() < 0) SwitchTool(-1);
        else if (val.Get<float>() > 0) SwitchTool(1);
    }
    // Update movement input
    public void OnMove(InputValue val)
    {
        movementInput = val.Get<Vector2>();
    }
    // Update look input
    public void OnLook(InputValue val)
    {
        lookInput = val.Get<Vector2>();
    }
    // Update jump input
    public void OnJump(InputValue val)
    {
        jumpInput = val.isPressed;
    }
    // Switch active UI
    public void OnOpenBook()
    {
        UI.ToggleUI();
    }
    #endregion
    // Iterate through tools by given amount
    private void SwitchTool(int slotsToMove)
    {
        ToolIndex += slotsToMove;
        UI.ActiveIndex = ToolIndex;
        ActiveTool = ToolInventory[ToolIndex];
    }
    // Handle player movement
    private void DoMovement()
    {
        // Do not allow movement if the cursor is confined
        if (Cursor.lockState == CursorLockMode.Confined) return;

        // Define target speed
        float targetSpeed = MoveSpeed;
        if (movementInput == Vector2.zero) targetSpeed = 0.0f;

        // Calculate current speed
        float prevHorizSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        if (prevHorizSpeed < targetSpeed - 0.1f || prevHorizSpeed > targetSpeed + 0.1f)
            currentMoveSpeed = Mathf.Lerp(prevHorizSpeed, targetSpeed, Time.deltaTime * MoveAcceleration);
        else currentMoveSpeed = targetSpeed;

        // Calculate direction of movement
        Vector3 moveDirection = new Vector3(movementInput.x,0.0f,movementInput.y).normalized;
        if (movementInput != Vector2.zero) moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y; 
        controller.Move(moveDirection * (currentMoveSpeed * Time.deltaTime) + new Vector3(0f,currentVerticalVelocity) * Time.deltaTime);
    }
    // Hande camera movement
    private void DoLook()
    {
        // Do not allow looking if the cursor is confined
        if (Cursor.lockState == CursorLockMode.Confined) return;

        // Update pitch and rotation speed by input
        cineTargetPitch += lookInput.y * RotationSpeed;
        currentRotationSpeed = lookInput.x * RotationSpeed;

        // Confine target to defined limits and rotate to it
        cineTargetPitch = ClampAngle(cineTargetPitch, CineBottomClamp, CineTopClamp);
        cinemachineTarget.transform.localRotation = Quaternion.Euler(cineTargetPitch, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * currentRotationSpeed);
    }
    // Handle jumping and the affects of gravity
    private void JumpAndGravity()
    {
        if (Grounded())
        {
            // Stop falling, allow jumping and clamp vertical velocity when grounded
            if (currentVerticalVelocity < 0.0f) currentVerticalVelocity = -2f;
            // Math taken from FPSexample [TODO! reference]
            // square root of H multipled by -2 multiplied by gravity = velocity required to reach desired height
            if (jumpInput) currentVerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
        // Apply falling
        else if (currentVerticalVelocity < TerminalVelocity) currentVerticalVelocity += Gravity * Time.deltaTime;
    }
    // Checks if player is currently on the ground
    private bool Grounded()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y -1f, transform.position.z);
        groundedSpherePos = spherePosition;
        return Physics.CheckSphere(spherePosition, 0.5f, GroundLayers, QueryTriggerInteraction.Ignore); 
    }
    // Locks an angle within a supplied range
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        return Mathf.Clamp(lfAngle % 360, lfMin, lfMax);
    }
    // Sets cursor lock state based on focus state
    private void OnApplicationFocus(bool hasFocus)
    {
        // Ensure UI is not null (happens as this function is run before Start() can assign UI)
        if (UI == null) UI = GetComponent<PlayerUIManager>();
        Cursor.lockState = hasFocus ? UI.GetCursorMode() : CursorLockMode.None; 
    }
    // Load all tools into inventory
    private void AddAllTools()
    {
        foreach (Tools enumVal in Enum.GetValues(typeof(Tools)))
        {
            if (enumVal == Tools.Item_Oscilliscope) continue; // Static tool placed in world, so skip
            Tool t = gameObject.AddComponent<Tool>();
            t.toolType = enumVal;
            ToolInventory.Add(t);
        }
    }
    // Displays visual markers in the editor for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundedSpherePos, 0.5f);
    }
}
