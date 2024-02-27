using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAlien
{
    //this interface looks *very* blank for now but i'm assuming the alien will be able to take input from other sources than just whatever tool the player uses
    //at which point it will make a little more sense
    void React(Tool tool);

    //i'm not entirely sure whether to include the emotion scales in here - interfaces *have* to only specify public members and c# doesnt support multiple inheritance
    //so either each alien has a ":monobehaviour, IAlien" OR we have a parent alien class that ":monobehvaiour" and make each alien a child of that 
}
