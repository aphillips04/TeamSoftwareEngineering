using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Tools
{
    Touch_Gently,
    Touch_Roughly,
    Feed_Treat,
    Feed_LiveAnimal,
    Item_Oscilliscope,
}

public  class Tool : MonoBehaviour
{
    public Tools toolType;

    public void Use(Ray r)
    {
        if (Physics.SphereCast(r, 2f, out RaycastHit hit, 10f))
        {
            if (hit.collider.CompareTag("Alien"))
            {
                hit.collider.SendMessage("React", this);
            }
        }
    }
}

