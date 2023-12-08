using UnityEngine;
using System;


public class Alien : MonoBehaviour
{
    // Alien configuration variables
    public enum AlienType { Star, Insect, Mammal }
    public AlienType alienType;
    [Range(0, 10)]
    public int happiness = 5, calmness = 5; // Higher is happier, higher is calmer

    /// <summary>
    /// Start is the initial setup function, called before the first frame update
    /// </summary>
    void Start()
    {
        return;
    }

    /// <summary>
    /// Update is the objects game loop function, called once per frame
    /// </summary>
    void Update()
    {
        if (alienType == AlienType.Star)
        {
            // Set the colour of the alien based on its happiness
            Color skinColour = new Color(
                .65f + (happiness *  .035f), // The change is calculated through: (end - start) / (steps - 1)
                .00f + (happiness *  .100f), // There are 11 steps of from 0 to 11
                .65f + (happiness * -.065f)  // The start value was RGB(166, 0, 166), end value was RGB(255, 255, 0) 
            );
            GetComponent<MeshRenderer>().material.color = skinColour;

            // Set the spin speed of the alien based on its calmness
        }
        else if (alienType == AlienType.Insect)
        {
            // Set how much teeth shown based on its happiness

            // Set how much it moves around based on its calmness
        }
        else if (alienType == AlienType.Mammal)
        {
            // Set its purring or growling based on its happiness

            // Set how much its tail is coiled based on its calmness
        }
    }

    /// <summary>
    /// How the alien responds to the players action
    /// </summary>
    void React(Tool tool)
    {
        if (tool.toolType == Tools.Touch_Gently)
        {
            if (alienType == AlienType.Star)
                happiness = Math.Min(10, happiness + 1);
            else if (alienType == AlienType.Insect)
                happiness = Math.Max(0, happiness - 1);
            else if (alienType == AlienType.Mammal) // NOT DECIDED YET
                return;
        }
        else if (tool.toolType == Tools.Touch_Roughly)
        {
            if (alienType == AlienType.Star)
                happiness = Math.Min(10, happiness - 1);
            else if (alienType == AlienType.Insect)
                happiness = Math.Min(10, happiness + 1);
            else if (alienType == AlienType.Mammal) // NOT DECIDED YET
                return;
        }
        else if (tool.toolType == Tools.Feed_Treat)
        {
            if (alienType == AlienType.Star)
                calmness = Math.Min(10, calmness + 1);
            else if (alienType == AlienType.Insect)
                calmness = Math.Max(0, calmness - 1);
            else if (alienType == AlienType.Mammal) // NOT DECIDED YET
                return;
        }
        else if (tool.toolType == Tools.Feed_LiveAnimal)
        {
            if (alienType == AlienType.Star)
                calmness = Math.Min(10, calmness - 1);
            else if (alienType == AlienType.Insect)
                calmness = Math.Min(10, calmness + 1);
            else if (alienType == AlienType.Mammal) // NOT DECIDED YET
                return;
        }
        else if (tool.toolType == Tools.Oscilliscope)
        {
        }
    }
}
