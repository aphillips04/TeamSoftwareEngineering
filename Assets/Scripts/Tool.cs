using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public abstract string ToolType {  get; }

    // THIS WILL PROBABLY LATER BECOME A PARENT CLASS/ SCRIPTABLEOBJECT BUT NOT RIGHT NOW COS IM JUST MAKING A SIMPLE EXAMPLE
    public abstract void Use();
}

