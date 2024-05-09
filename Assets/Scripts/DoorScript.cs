using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [Header("References")]
    public DayCycle dayCycle;
    public Fader fader;
  
    [Header("Constants")]
    public float movementConstant;
    public float offset;
    public bool restDoor = false;

    enum DoorState
    {
        IdleBottom,
        IdleTop,
        MovingUp,
        MovingDown
    }
    private float timer;
    private DoorState state;
    private Vector3 originalPos;
    private Vector3 upPos;
    private void Start()
    {
        // Assign values to vars
        timer = 0f;
        state = DoorState.IdleBottom;
        originalPos = transform.position;
        upPos = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
    }
    private void Update()
    {
        switch (state)
        {
            case DoorState.IdleBottom:
                break;
            case DoorState.IdleTop:
                if (timer > 0) timer -= Time.deltaTime;
                else timer = 0f;

                if (timer == 0f)
                {
                    state = DoorState.MovingDown;
                    timer = .75f;
                }
                break;
            case DoorState.MovingUp:
                if (Mathf.Abs(transform.position.y - upPos.y) >=0.1f)
                    transform.position = Vector3.Lerp(transform.position, upPos, Time.deltaTime * movementConstant);
                else
                {
                    state = DoorState.IdleTop;
                    timer = 2f;
                }
                break;
            case DoorState.MovingDown:
                if (Mathf.Abs(transform.position.y - originalPos.y) >= 0.1f)
                    transform.position = Vector3.Lerp(transform.position, originalPos, Time.deltaTime * movementConstant);
                else state = DoorState.IdleBottom;
                break;
            default:
                Debug.Log("impossible thing happened");
                break;
        }
    }
    // Handle interactions with the doors
    public void OnPlayerInteract()
    {
        if (restDoor)
        {
            // If not tired, tell them to interact with the aliens
            if (dayCycle.exhaustionMeter >= 100f) fader.fadeToBlack(3);
            else NotifSys.system.notify("You are not tired enough to rest!\nGo interect with the aliens.");

        }
        else if (state == DoorState.IdleBottom)
        {
            state = DoorState.MovingUp;
            timer = 0.75f;
        }
    }  
}
