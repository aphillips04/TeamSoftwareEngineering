using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DayCycle : MonoBehaviour
{
    private PlayerController playerController;
    public int exhaustionMeter = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAction()
    {
        exhaustionMeter += 25;
    }

    public void EndDay()
    {
        if (exhaustionMeter == 100)
        {

        }
    }
}
