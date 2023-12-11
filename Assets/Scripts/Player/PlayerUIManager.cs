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
    public List<GameObject> Hotbar;
    public float BoxWidth;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Hello World!");
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawHotbar();
 
    }
    void DrawHotbar()
    {
        int HotbarCount = ToolInventory.Count;
        float HotbarHalfWidth;
        if (HotbarCount % 2 == 0)
        {
             HotbarHalfWidth = HotbarCount * BoxWidth * 0.5f;
            
        }
        else
        {
            HotbarHalfWidth =  Mathf.Floor(HotbarCount / 2) * BoxWidth;
        }
        float HotbarStart = HotbarCenter.x - HotbarHalfWidth;
        for (int i = 0; i < HotbarCount; i++)
        {
            Hotbar[i].transform.localPosition = new Vector2(HotbarStart + BoxWidth * i, HotbarCenter.y);
        }
    }
    public void InitHotbar()
    {
        foreach (Tool tool in ToolInventory)
        {
            GameObject g = Instantiate(HotbarPrefab, UI.transform);
            Hotbar.Add(g);
        }
    }
    
}
