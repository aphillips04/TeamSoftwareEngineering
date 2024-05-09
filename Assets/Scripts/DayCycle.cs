using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DayCycle : MonoBehaviour
{
    public Book BookUI;
    [HideInInspector] public int exhaustionMeter = 0;
    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    public void EndDay()
    {
        exhaustionMeter = 0;
        gameObject.transform.Rotate(-gameObject.transform.rotation.eulerAngles); // Normal to the door is 0 degrees
        
        // Set player position in middle of door
        CharacterController controller = gameObject.GetComponent<CharacterController>();
        controller.enabled = false;
        controller.transform.position = new Vector3(0.13f, 3.5f, -29.0f);
        controller.enabled = true;
        
        // Notify the current number of correctly assigned emotions
        PageScript ps;
        if (BookUI.ActivePage.TryGetComponent<PageScript>(out ps)) NotifSys.system.notify("Number of correct answers on current page: " + ps.CheckNumCorrect(), 5);
        else NotifSys.system.notify("You must rest with an alien's page selected to check your answers!");
    }
}
