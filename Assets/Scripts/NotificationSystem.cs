using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class NotificationSystem : MonoBehaviour
{
    public GameObject notifications;
    private GameObject textBox;
    private float timer;
    private float length;
    void Start()
    {
        NotifSys.system = this;
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
        notifications.SetActive(true);
        textBox.GetComponent<TextMeshProUGUI>().text = message;
        timer = 0;
        length = period;
    }
}

public static class NotifSys
{
    public static NotificationSystem system;
}
