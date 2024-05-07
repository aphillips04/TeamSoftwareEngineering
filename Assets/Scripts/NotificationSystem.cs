using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationSystem : MonoBehaviour
{
    [SerializeField] private GameObject notifications;
    private GameObject background;
    private GameObject textBox;
    private float timer;
    private float length;
    void Start()
    {
        NotifSys.system = this;

        background = notifications.transform.GetChild(0).gameObject;
        textBox = notifications.transform.GetChild(1).gameObject;
        notifications.SetActive(false);
        textBox.GetComponent<TextMeshProUGUI>().color = Color.white;
    }
    private void Update()
    {
        if (!notifications.activeSelf) return;
        timer += Time.deltaTime;
        if (timer > length)
        {
            notifications.SetActive(false);
        }
        
    }
    public void notify(string message, float period = 3)
    {
        // Set variables
        length = period;
        timer = 0;

        // Update text and show
        notifications.SetActive(true);
        textBox.GetComponent<TextMeshProUGUI>().text = message;

        // Force update canvas to run content size fitter, calculating new height
        Canvas.ForceUpdateCanvases();

        // Update background to same dimensions as textbox
        Vector2 size = textBox.GetComponent<RectTransform>().sizeDelta;
        Debug.Log(size);
        background.GetComponent<RectTransform>().sizeDelta = size;
    }
}

public static class NotifSys
{
    public static NotificationSystem system;
}
