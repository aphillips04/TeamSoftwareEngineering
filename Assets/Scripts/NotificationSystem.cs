using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class NotificationSystem : MonoBehaviour
{
    public GameObject textBox;
    void Start()
    {
        NotifSys.system = this;
    }
    public void notify(string message)
    {
        textBox.GetComponent<TextMeshProUGUI>().text = message;
    }
}

public static class NotifSys
{
    public static NotificationSystem system;
}
