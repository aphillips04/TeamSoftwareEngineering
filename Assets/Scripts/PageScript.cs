using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Xml.Linq;

public class PageScript : MonoBehaviour
{
    [Header("Components")]
    public GameObject ComboStart;

    [Header("State variables")]
    public List<ComboScript> ComboScriptInstances;

    [Header("Constants")]
    public List<GameObject> Combos;
    public float comboScale=1;
    public float comboYoffset;
    public float comboXoffset;
    // Start is called the first time the object is activated
    void Start()
    {
        BuildPage();
    }
    // Places all the combos on the page
    private void BuildPage()
    {
        for (int i=0; i < Combos.Count;i++)
        {
            // Instantiate the combo on the page and hold on to a reference to it
            GameObject newObj = GameObject.Instantiate(Combos[i],ComboStart.transform);
            ComboScriptInstances.Add(newObj.GetComponent<ComboScript>());
            // Reset the scale
            newObj.transform.localScale = Vector3.one * comboScale;
            // Set its position based on its index
            newObj.transform.position = ComboStart.transform.position;
            newObj.transform.localPosition += new Vector3((i % 2) * comboXoffset ,i/2 * comboYoffset,0);
        }
    }
    // Sets a combo as visible by "name"
    public void ActivateCombo(string name)
    {
        foreach (ComboScript cs in ComboScriptInstances)
        {
            if (cs.name == name)
            {
                cs.discovered = true;
            }
        }
    }
    // Count the number of correctly selected dropdown options on the current page
    public int CheckNumCorrect()
    {
        int output = 0;
        foreach (ComboScript cs in ComboScriptInstances)
        {
            if (cs.CheckCorrect()) output++;
        }
        return output;
    }
}
