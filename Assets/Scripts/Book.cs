using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public enum BehaviourEnum
    {
        
    }
    public  Dictionary<BehaviourEnum, bool> SeenBehaviourFlags;
    public List<GameObject> AllPages;
    public GameObject ActivePage;
    public void Start()
    {
        SetPageIndex(0);
    }
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
