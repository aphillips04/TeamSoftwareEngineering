using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Alien;


public class PlayerUIManager : MonoBehaviour
{
    public Canvas UI;
    [Header("Hotbar")]
    public GameObject HotbarPrefab;
    public Vector2 HotbarCenter;
    public float BoxWidth;
    public List<Tool> ToolInventory;
    public List<GameObject> Hotbar;
    private int _activeIndex;
    public int ActiveIndex
    {
        get { return _activeIndex; }
        set
        {
            Hotbar[_activeIndex].SendMessage("Deactivate");
            _activeIndex = value;
            Hotbar[_activeIndex].SendMessage("Activate");
        }
    }
    [Header("Progress Bar")]
    public GameObject ProgressBarPrefab;
    private Image RelationshipFill;
    private float targetFill;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Hello World!");
    }

    // Update is called once per frame
    void Update()
    {
        DrawHotbar();
        if (RelationshipFill != null)
        {
            if (RelationshipFill.fillAmount < targetFill)
            {
                RelationshipFill.fillAmount += 0.1f * Time.deltaTime;
            } else if (RelationshipFill.fillAmount > targetFill)
            {
                RelationshipFill.fillAmount -= 0.1f * Time.deltaTime;
            }
        }
    }
    public void InitHotbar()
    {
        foreach (Tool tool in ToolInventory)
        {
            GameObject g = Instantiate(HotbarPrefab, UI.transform);
            g.SendMessage("SelectToolIcon", tool.toolType);
            Hotbar.Add(g);
        }
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
            HotbarHalfWidth = Mathf.Floor(HotbarCount / 2) * BoxWidth;
        }
        float HotbarStart = HotbarCenter.x - HotbarHalfWidth;
        for (int i = 0; i < HotbarCount; i++)
        {
            Hotbar[i].transform.localPosition = new Vector2(HotbarStart + BoxWidth * i, HotbarCenter.y);
        }

    }

    public void InitRelationshipBar(AlienType alienType)
    {
        Color background = new Color(.267f, .267f, .267f);
        Color fill;
        switch (alienType)
        {
            case AlienType.Star:
                fill = Color.green;
                break;
            case AlienType.Insect:
                fill = Color.yellow;
                break;
            case AlienType.Mammal:
                fill = Color.red;
                break;
            default:
                fill = Color.white;
                break;
        }

        GameObject bar = Instantiate(ProgressBarPrefab, UI.transform);
        foreach (Image child in bar.GetComponentsInChildren<Image>())
        {
            switch (child.name)
            {
                case "Background":
                    child.color = background;
                    break;
                case "Fill":
                    RelationshipFill = child;
                    child.color = fill;
                    break;
                default:
                    break;
            }
        }
        if (RelationshipFill == null)
        {
            Debug.LogError("RelationshipFill is null");
        }
    }
    public void UpdateRelationshipBar(float relationship)
    {
        targetFill = (float)Math.Round(relationship / 10, 2, MidpointRounding.AwayFromZero);
    }
    public void SetRelationshipBar(float relationship)
    {
        targetFill = (float)Math.Round(relationship / 10, 2, MidpointRounding.AwayFromZero);
        RelationshipFill.fillAmount = targetFill;
    }
}
