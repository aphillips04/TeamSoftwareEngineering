using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PageScript : MonoBehaviour
{
    public int numAliens;
    public List<string> AllAlienNames;
    public List<string> AllAlienDescriptions;
    public List<GameObject> ComboInstances;
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
    public void BuildPages(string name)
    {
        
        BuildLeftPage(name);
        BuildRightPage(name);
    }

    private void BuildRightPage(string name)
    {
        for (int i=0; i < Combos.Count;i++)
        {
            GameObject newObj = GameObject.Instantiate(Combos[i],ComboStart.transform);
            ComboInstances.Add(newObj);
            newObj.transform.localScale = Vector3.one * comboScale;
            newObj.transform.position = ComboStart.transform.position;
            newObj.transform.localPosition += new Vector3((i % 2) * comboXoffset ,i/2 * comboYoffset,0);
        }
    }

    private void BuildLeftPage(string name)//make sure to call this when instantiating pages
    {
        if (!AllAlienNames.Contains(name))
        {
            Debug.Log("SelectLeftPage was given an invalid alien name!");
            NameText.text = "ERROR";
            DescText.text = "Invalid alien name given!";
            return;
        }
        int idx = AllAlienNames.IndexOf(name);
        NameText.text = AllAlienNames[idx];
        DescText.text = AllAlienDescriptions[idx];
    }
}
