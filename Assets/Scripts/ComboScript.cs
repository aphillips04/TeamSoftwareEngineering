using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboScript : MonoBehaviour
{
    public List<string> placeholderTexts;
    public GameObject hiddenVer;
    public GameObject shownVer;
    public bool discovered = false;
    public bool correct = false;
    void Start()
    {
        shownVer.SetActive(false);
        hiddenVer.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (discovered)
        {
            hiddenVer.SetActive(false);
            shownVer.SetActive(true);
            //show
            //
        }
    }
}