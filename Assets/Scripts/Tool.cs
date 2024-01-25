using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum Tools
{
    Touch_Gently,
    Touch_Roughly,
    Feed_Treat,
    Feed_LiveAnimal,
    Item_Oscilliscope,
}

public class Tool : MonoBehaviour
{
    public Tools toolType;

    public void Use(Ray r)
    {
        Debug.Log(string.Format("Used tool {0}", toolType));
        // Debug.DrawRay(r.origin,r.direction,Color.red,2f);
        // if (Physics.Raycast(r, out RaycastHit hit, 10f))
        // {
        //     if (hit.collider.CompareTag("Alien"))
        //     {
        //         hit.collider.SendMessage("React", this);
        //     }
        // }

        GameObject alien = GameObject.FindGameObjectWithTag("Alien");
        alien.SendMessage("React", this);
    }
}

