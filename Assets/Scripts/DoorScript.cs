using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private PlayerController playerController;
    public float movementConstant;
    public float offset;
    [Header("")]
    public bool restDoor = false;
  
    enum DoorState
    {
        idleBottom,
        idleTop,
        MovingUp,
        MovingDown
    }
    private float timer;
    private DoorState state;
    private Vector3 originalPos;
    private Vector3 upPos;
    private void Start()
    {
        timer = 0f;
        state = DoorState.idleBottom;
        originalPos = transform.position;
        upPos = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
    }
    private void Update()
    {
        switch (state)
        {
            case DoorState.idleBottom:
                //do nothing
                break;
            case DoorState.idleTop:
                UpdateTimer();
                if (timer == 0f)
                {
                    //if next state
                    state = DoorState.MovingDown;
                    timer = .75f;
                }
                break;
            case DoorState.MovingUp:
                //UpdateTimer();
                if (Mathf.Abs(transform.position.y - upPos.y) >=0.1f)
                {
                    transform.position = Vector3.Lerp(transform.position, upPos, Time.deltaTime * movementConstant);
                }
                else
                {
                    state = DoorState.idleTop;
                    timer = 2f;
                }
                /*
                if (timer == 0f)
                {
                    //if next state
                    state = DoorState.idleTop;
                    timer = 2f;
                }
                else
                {
                    //move door
                    transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * movementConstant + transform.position.z);
                }
                */
                break;
            case DoorState.MovingDown:
                // UpdateTimer();
                if (Mathf.Abs(transform.position.y - originalPos.y) >= 0.1f)
                {
                    transform.position = Vector3.Lerp(transform.position, originalPos, Time.deltaTime * movementConstant);
                }
                else
                {
                    state = DoorState.idleBottom;
                }
                /*
                if (timer == 0f)
                {
                    //if next state
                    state = DoorState.idleBottom;
                }
                else
                {
                    //move door
                    transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * movementConstant + transform.position.z);
                }
                */
                break;
            default:
                Debug.Log("impossible thing happened");
                break;
        }
        
    }
    private void UpdateTimer()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0f;
        }
    }
    public void OnPlayerInteract()
    {
        Debug.Log("Player interacted with door");
        if (restDoor)
        {
            playerController.fadeToBlack();
            float counter = 3;
            counter -= Time.deltaTime;
            //transform.rotation = new Quaternion(0, 180, 0, 0);
            Vector3 rotation = new Vector3(0, 180, 0);
            playerController.player.transform.eulerAngles = rotation;
            Debug.Log("REST DOOR USED");
            // TODO! rest door stuff

        }
        else if (state == DoorState.idleBottom)
        {
            state = DoorState.MovingUp;
            timer = 0.75f;
        }
    }
   
}
