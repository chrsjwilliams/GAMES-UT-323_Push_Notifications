using System;
#if UNITY_IPHONE
using Unity.Notifications.iOS;
#elif UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

public class PushNotificationTester : MonoBehaviour
{

    public void SendPushNotification()
    {
        Debug.Log("SENDING PUSH NOTIFICATION");

        string title = "Test Notification";
        string message = "This is the message body!";

#if UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(hours:0, minutes: 0,seconds: 1),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = "_notification_01",
            Title = title,
            Body = message,
            Subtitle = "This is a subtitle, something, something important...",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#elif UNITY_ANDROID
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = message;
        notification.FireTime = System.DateTime.Now;

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
#endif
    }
}
