using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HotbarItem : MonoBehaviour
{
    public List<Sprite> AllIcons;

    public Image box;
    public Image icon;



    public bool isHighlighted;
    public void Start()
    {
        
    }
    public void Update()
    {
        if (isHighlighted)
        {
            box.color = Color.white;
        }
        else
        {
            box.color = Color.black;
        }
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
            case Tools.Oscilliscope:
                icon.sprite = AllIcons[4];
                break;
        }
    }
}
