using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PageScript : MonoBehaviour
{
    public string AlienNamestr;
    public string AlienDescstr;
    public List<ComboScript> ComboScriptInstances;
    public GameObject AlienName;
    public GameObject AlienDescription;
    public GameObject ComboStart;
    public float comboScale=1;
    public float comboYoffset;
    public float comboXoffset;
    public List<GameObject> Combos;
    private TextMeshProUGUI NameText;
    private TextMeshProUGUI DescText;
    // Start is called before the first frame update
    void Start()
    {
        NameText = AlienName.GetComponent<TextMeshProUGUI>();
        DescText = AlienDescription.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BuildPages()
    {
        
        BuildLeftPage();
        BuildRightPage();
    }

    private void BuildRightPage()
    {
        for (int i=0; i < Combos.Count;i++)
        {
            GameObject newObj = GameObject.Instantiate(Combos[i],ComboStart.transform);
            ComboScriptInstances.Add(newObj.GetComponent<ComboScript>());
            newObj.transform.localScale = Vector3.one * comboScale;
            newObj.transform.position = ComboStart.transform.position;
            newObj.transform.localPosition += new Vector3((i % 2) * comboXoffset ,i/2 * comboYoffset,0);
        }
    }

    private void BuildLeftPage()//make sure to call this when instantiating pages
    {
        NameText.text = AlienNamestr;
        DescText.text = AlienDescstr;
    }
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
    public int CheckNumCorrect()
    {
        int output = 0;
        foreach (ComboScript cs in ComboScriptInstances)
        {
            if (cs.correct)
            {
                output++;
            }
        }
        return output;
    }
}
