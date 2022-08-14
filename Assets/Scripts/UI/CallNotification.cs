using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallNotification : MonoBehaviour
{
    public bool onStart = true;

    public string title, body, tip;
    public Sprite icon;

    private void Start()
    {
        if(onStart)
        {
            Notification.Instance.Show(title, body, tip, icon);
        }
    }
}
