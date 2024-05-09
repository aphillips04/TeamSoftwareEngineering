using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationSystem : MonoBehaviour
{
    private GameObject background;
    private GameObject textBox;
    private float timer;
    private float length;
    void Start()
    {
        NotifSys.system = this; // Assign static reference for easier access elsewhere

        // Assign variables and update to correct states
        background = gameObject.transform.GetChild(0).gameObject;
        textBox = gameObject.transform.GetChild(1).gameObject;
        gameObject.SetActive(false);
        textBox.GetComponent<TextMeshProUGUI>().color = Color.white;
    }
    void Update()
    {
        // If the gameObject is not active, do not time
        if (!gameObject.activeSelf) return;
        timer += Time.deltaTime;
        if (timer > length) gameObject.SetActive(false);
        
    }
    // Interface to activate notification bar
    public void notify(string message, float period = 3)
    {
        // Set variables
        length = period;
        timer = 0;

        // Update text and show
        gameObject.SetActive(true);
        textBox.GetComponent<TextMeshProUGUI>().text = message;

        // Force update canvas to run content size fitter, calculating new height of textbox
        Canvas.ForceUpdateCanvases();

        // Update background to same dimensions as textbox
        Vector2 size = textBox.GetComponent<RectTransform>().sizeDelta;
        background.GetComponent<RectTransform>().sizeDelta = size;
    }
}

// Static reference for easier access to notification system
public static class NotifSys
{
    public static NotificationSystem system;
}
