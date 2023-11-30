using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed;

    private float currentSpeed;

    private Vector2 movementInput;
    private Vector2 lookInput;
    private CharacterController controller;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {

    }
    public void OnMove(InputValue val)
    {
        movementInput = val.Get<Vector2>();
    }
    public void OnLook(InputValue val)
    {
        lookInput = val.Get<Vector2>();
    }
    private void Move()
    {
        Vector3 moveDirection = new Vector3(movementInput.x,0.0f,movementInput.y).normalized;
        controller.Move(moveDirection * (currentSpeed * Time.deltaTime));
    }
    private void Look()
    {

    }
}
