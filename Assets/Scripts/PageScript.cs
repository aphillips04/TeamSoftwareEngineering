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
    public GameObject AlienName;
    public GameObject AlienDescription;
    private TextMeshPro NameText;
    private TextMeshPro DescText;
    // Start is called before the first frame update
    void Start()
    {
        NameText = AlienName.GetComponent<TextMeshPro>();
        DescText = AlienDescription.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SelectPages(string name)
    {
        SelectLeftPage(name);
        SelectRightPage(name);
    }

    private void SelectRightPage(string name)
    {
        //fill out combos
        //yep
        //storing combos could be tricky ill come back to this later
    }

    private void SelectLeftPage(string name)//make sure to call this when instantiating pages
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
