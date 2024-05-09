using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboScript : MonoBehaviour
{
    [Header("")]
    public List<string> placeholderTexts;
    public string comboName;
    public GameObject hiddenVer;
    public GameObject shownVer;
    public TMP_Dropdown dropdown;
    public int correctIndex;
    public bool discovered = false;
  
    void Start()
    {
        shownVer.SetActive(discovered);
        hiddenVer.SetActive(!discovered);
    }
    // Update is called once per frame
    void Update()
    {
        hiddenVer.SetActive(!discovered);
        shownVer.SetActive(discovered);
    }
    // Check if the selected value is correct
    public bool CheckCorrect()
    {
        return dropdown.value == correctIndex;
    }
}
