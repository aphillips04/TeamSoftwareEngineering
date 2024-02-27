using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAlien
{
    //this interface looks *very* blank for now but i'm assuming the alien will be able to take input from other sources than just whatever tool the player uses
    //at which point it will make a little more sense
    public void React(Tool tool);
}
