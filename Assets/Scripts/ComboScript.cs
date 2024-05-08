using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboScript : MonoBehaviour
{
    public List<string> placeholderTexts;
    public string comboName;
    public GameObject hiddenVer;
    public GameObject shownVer;
    public TMP_Dropdown dropdown;
    public int correctIndex;
    public bool discovered = false;
    public bool correct = false;
    void Start()
    {
        shownVer.SetActive(discovered);
        hiddenVer.SetActive(!discovered);
    }

    // Update is called once per frame
    void Update()
    {
       // if (discovered)
        {
            hiddenVer.SetActive(!discovered);
            shownVer.SetActive(discovered);
            //show
            //
        }
    }
    private void CheckCorrect()
    {
        if (dropdown.value == correctIndex) 
        {
            correct = true;
        }
        else
        {
            correct = false;
        }
    }
}
