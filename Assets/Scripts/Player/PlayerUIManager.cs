using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject HotbarPrefab;
    public Canvas UI;
    public Vector2 HotbarCenter;
    public List<Tool> ToolInventory;
    private GameObject asda;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Hello World!");
        DrawHotbar();
    }

    // Update is called once per frame
    void Update()
    {
 
        asda.transform.localPosition = HotbarCenter;
    }
    void DrawHotbar()
    {
       // foreach(Tool tool in ToolInventory)
        {
            asda = Instantiate(HotbarPrefab, UI.transform);
            asda.SendMessage("SelectToolIcon", Tools.Touch_Gently);
            //asda.transform.position = HotbarCenter;
        }
        
    }
    
}
