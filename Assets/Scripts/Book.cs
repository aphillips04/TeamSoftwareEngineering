using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;
public class Book : MonoBehaviour
{
    public GameObject ExitPopup;
    public List<GameObject> AllButtons;
    [Header("Pages")]
    public List<GameObject> AllPages;
    public GameObject ActivePage;
    [Header("Anchors")]
    public GameObject leftAnchor;
    public GameObject rightAnchor;
    public void Start()
    {
        ExitPopup.SetActive(false);
        SetPageIndex(0);
    }
    //this works on a list of GameObjects so "Page" can be of any type - so long as page exists in the hierarchy this is usable

    public void SetPageIndex(int index)
    {
        // Do not allow invalid page index
        if (index < 0 || index >= AllPages.Count) return;
        
        // Get current page and button
        ActivePage = AllPages[index];
        GameObject currentButton = AllButtons[index];

        // Deactivate pages and reset button order
        for (int i = 0; i < AllPages.Count; i++)
        {
            GameObject p = AllPages[i];
            GameObject button = AllButtons[i];
            p.SetActive(false);
            button.transform.SetAsFirstSibling();
        }
        // Move previous pages buttons to left side
        for (int i = 0; i < index; i++)
        {
            GameObject button = AllButtons[i];
            button.transform.position = new Vector3(leftAnchor.transform.position.x,button.transform.position.y,button.transform.position.z);
        }
        // Move current and following pages buttons to right side
        for (int i = index; i < AllButtons.Count; i++)
        {
            GameObject button = AllButtons[i];
            button.transform.position = new Vector3(rightAnchor.transform.position.x, button.transform.position.y, button.transform.position.z);
        }
        
        // Bring button to front to highlight it as
        // current page and activate current page
        currentButton.transform.SetAsLastSibling();
        ActivePage.SetActive(true);
    }
    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }
}
