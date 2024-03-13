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
    public LayerMask ignorePlayerMask = LayerMask.GetMask("Player");
    public void Use(Ray r)
    {
        //Debug.Log(string.Format("Used tool {0}", toolType));
        //Debug.DrawRay(r.origin, r.direction * 40f, Color.red, 2f);
        //if (Physics.Raycast(r, out RaycastHit hit, 40f,ignorePlayerMask))
        //{
        //    //this is still broken - the layer masking didn't work - i'm not entirely sure whats going on
        //    Debug.Log(hit.collider.gameObject.layer);
        //    Debug.Log(hit.collider.tag);
        //    Debug.Log(hit.collider.name);
        //    if (hit.collider.CompareTag("Alien") || hit.collider.CompareTag("StarEye"))
        //    {
        //        hit.collider.SendMessageUpwards("React", this);
        //    }
        //}

        GameObject alien = GameObject.FindGameObjectWithTag("Alien");
        alien.SendMessage("React", this);
    }
}

