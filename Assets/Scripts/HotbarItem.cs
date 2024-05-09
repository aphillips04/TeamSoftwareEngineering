using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HotbarItem : MonoBehaviour
{
    [Header("Visual Components")]
    public List<Sprite> AllIcons;
    public Image box;
    public Image icon;

    private bool isHighlighted = false;
    void Update()
    {
        // Change color to show player which tool they are using
        if (isHighlighted) box.color = Color.white;
        else box.color = Color.black;
    }
    
    public void SelectToolIcon(Tools toolType)
    {
        switch (toolType)
        {
            case Tools.Touch_Gently:
                icon.sprite = AllIcons[0];
                break;
            case Tools.Touch_Roughly:
                icon.sprite = AllIcons[1];
                break;
            case Tools.Feed_Treat:
                icon.sprite = AllIcons[2];
                break;
            case Tools.Feed_LiveAnimal:
                icon.sprite = AllIcons[3];
                break;
            case Tools.Item_Oscilliscope:
                icon.sprite = AllIcons[4];
                break;
        }
    }
    public void Activate()
    {
        //Debug.Log("Higlighted new box");
        isHighlighted = true;
    }
    public void Deactivate()
    {
        //Debug.Log("Unhighlighted box");
        isHighlighted = false;
    }
}
