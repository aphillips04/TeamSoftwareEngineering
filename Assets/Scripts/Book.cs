using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

public class Book : MonoBehaviour
{
    public enum BehaviourEnum
    {
        
    }
    public  Dictionary<BehaviourEnum, bool> SeenBehaviourFlags;//idk how this will work
    //this is based on the OLD idea for how the book works -- needs updating to the new system
    public List<GameObject> AllPages;
    public GameObject ActivePage;
    public List<GameObject> AllButtons;
    public GameObject leftAnchor;
    public GameObject rightAnchor;
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
        GameObject currentButton = AllButtons[index];
        for (int i = 0; i < AllPages.Count; i++)
        {
            GameObject p = AllPages[i];
            GameObject button = AllButtons[i];
            p.SetActive(false);
            button.transform.SetAsFirstSibling();
        }
        for (int i = 0; i < index; i++)
        {
            GameObject button = AllButtons[i];
            button.transform.position =new Vector3(leftAnchor.transform.position.x,button.transform.position.y,button.transform.position.z);
        }
        for (int i = index; i < AllButtons.Count; i++)
        {
            GameObject button = AllButtons[i];
            button.transform.position = new Vector3(rightAnchor.transform.position.x, button.transform.position.y, button.transform.position.z);
        }
        currentButton.transform.SetAsLastSibling();
        ActivePage.SetActive(true);
    }
    public List<ComboScript> GetCurrentCombos()
    {
        PageScript script = ActivePage.GetComponent<PageScript>();
        return script.ComboScriptInstances;
    }
}
