using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public enum BehaviourEnum
    {
        
    }
    public  Dictionary<BehaviourEnum, bool> SeenBehaviourFlags;//idk how this will work
    //this is based on the OLD idea for how the book works -- needs updating to the new system
    public List<GameObject> AllPages;
    public GameObject ActivePage;
    public void Start()
    {
        SetPageIndex(0);
        for (int i = 0; i < AllPages.Count; i++)
        {
            GameObject page = AllPages[i];
            PageScript script = page.GetComponent<PageScript>();
            if (script == null) {
                Debug.Log("PAGESCRIPT NULL PANIC!");
            }
            script.BuildPages(script.AllAlienNames[i]);
        }
    }
    //this works on a list of GameObjects so "Page" can be of any type - so long as page exists in the hierarchy this is usable
    public void SetPageIndex(int index)
    {
        if (index <0 || index >= AllPages.Count)
            return;
        
        ActivePage = AllPages[index];
        foreach (var p in AllPages)
        {
            p.SetActive(false);
        }
        ActivePage.SetActive(true);
    }
}
