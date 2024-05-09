using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Cavases")]
    public Canvas MainUI;
    public Canvas BookUI;
    
    [Header("Hotbar")]
    public GameObject HotbarPrefab;
    public Vector2 HotbarCenter;
    public Transform HotbarTransform;
    public float BoxWidth;
    public List<Tool> ToolInventory;
    public List<GameObject> Hotbar;
    private int _activeIndex;
    public int ActiveIndex
    {
        get { return _activeIndex; }
        set
        {
            // Update active hotbar cell as well as value
            Hotbar[_activeIndex].SendMessage("Deactivate");
            _activeIndex = value;
            Hotbar[_activeIndex].SendMessage("Activate");
        }
    }

    [Header("Progress Bar")]
    public GameObject ProgressBarPrefab;
    public float ProgressSpeed = 0.1f;
    private Image ExhaustionFill;
    private float targetFill;
    private bool CursorLock = true;
    private DayCycle cycle;
    // Start is called before the first frame update
    void Start()
    {
        cycle = GetComponent<DayCycle>();
        //Debug.Log("Hello World!");
        MainUI.enabled = true;
        InitExhaustionBar();
        SetExhaustionBar(cycle.exhaustionMeter);

        ToggleUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Update hotbar
        DrawHotbar();
        UpdateExhaustionBar( 100 - cycle.exhaustionMeter);
        if (ExhaustionFill.fillAmount < targetFill) ExhaustionFill.fillAmount += ProgressSpeed * Time.deltaTime;
        else if (ExhaustionFill.fillAmount > targetFill) ExhaustionFill.fillAmount -= ProgressSpeed * Time.deltaTime;
    }
    // Create hotbar cell for each tool in inventory
    public void InitHotbar()
    {
        foreach (Tool tool in ToolInventory)
        {
            GameObject g = Instantiate(HotbarPrefab, MainUI.transform);
            g.transform.SetAsFirstSibling();
            g.SendMessage("SelectToolIcon", tool.toolType);
            Hotbar.Add(g);
        }
        Hotbar[_activeIndex].SendMessage("Activate");
    }
    // Recalculate position and reposition each hotbar cell
    void DrawHotbar()
    {
        int HotbarCount = ToolInventory.Count;
        // Find half width of hotbar, biased low
        float HotbarHalfWidth;
        if (HotbarCount % 2 == 0) HotbarHalfWidth = HotbarCount * BoxWidth * 0.5f;
        else HotbarHalfWidth = Mathf.Floor(HotbarCount / 2) * BoxWidth;
        
        // Set position of each cell
        float HotbarStart = HotbarCenter.x - HotbarHalfWidth;
        for (int i = 0; i < HotbarCount; i++)
            Hotbar[i].transform.localPosition = HotbarTransform.transform.localPosition + new Vector3(HotbarStart + BoxWidth * i, 0,0);
    }
    // Create exhaustion bar on UI
    public void InitExhaustionBar()
    {
        Color background = new Color(.267f, .267f, .267f);
        Color fill = Color.white;
        // Instantiate and colour progress bar on UI
        GameObject bar = Instantiate(ProgressBarPrefab, MainUI.transform);
        foreach (Image child in bar.GetComponentsInChildren<Image>())
        {
            switch (child.name)
            {
                case "Background":
                    child.color = background;
                    break;
                case "Fill":
                    ExhaustionFill = child;
                    child.color = fill;
                    break;
                default:
                    break;
            }
        }
        if (ExhaustionFill == null) Debug.LogError("ExhaustionFill is null");
        
    }
    // Update the value displayed by the exhaustion bar
    public void UpdateExhaustionBar(float exhaustion)
    {
        targetFill = (float)Math.Round(exhaustion / 100, 2, MidpointRounding.AwayFromZero);
    }
    // Intially set the value displayed by the exhaustion bar
    public void SetExhaustionBar(float exhaustion)
    {
        targetFill = (float)Math.Round(exhaustion / 100, 2, MidpointRounding.AwayFromZero);
        // Forcibly set the fill to the target fill - skipping the animation
        ExhaustionFill.fillAmount = targetFill;
    }
    // Update variables related to which UI is currently active
    public void ToggleUI()
    {
        if (MainUI.enabled)
        {
            MainUI.enabled = false;
            BookUI.enabled = true;
            CursorLock = false;
        }
        else
        {
            MainUI.enabled = true;
            BookUI.enabled = false;
            CursorLock = true;
        }
        Cursor.lockState = GetCursorMode();
    }
    // Internalise the cursor lock state
    public CursorLockMode GetCursorMode()
    {
        return CursorLock ? CursorLockMode.Locked : CursorLockMode.Confined;
    }
}
