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
    public LayerMask ignorePlayerMask;
    public void Start()
    {
        ignorePlayerMask = ~LayerMask.GetMask("Player");
    }
    // Ray cast check for alien hits to use the tool
    public void Use(Ray r)
    {
        Debug.Log(string.Format("Used tool {0}", toolType));
        Debug.DrawRay(r.origin, r.direction * 40f, Color.red, 2f);
        // Cast ray to a maximum distance, checking for collision with
        // either the Alien or Eye but ignoring collisons with the player
        if (
            Physics.Raycast(r, out RaycastHit hit, 40f, ignorePlayerMask) &&
            (hit.collider.CompareTag("Alien") || hit.collider.CompareTag("StarEye"))
        ) hit.collider.SendMessage("React", this);
    }
}

