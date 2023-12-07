using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tools
{
    Touch_Gently,
    Touch_Roughly,
    Feed_Treat,
    Feed_LiveAnimal,
    Oscilliscope,
}

    public abstract class Tool : MonoBehaviour
{
    public abstract Tools toolType {  get; }

    // THIS WILL PROBABLY LATER BECOME A PARENT CLASS/ SCRIPTABLEOBJECT BUT NOT RIGHT NOW COS IM JUST MAKING A SIMPLE EXAMPLE
    public abstract void Use();
}

