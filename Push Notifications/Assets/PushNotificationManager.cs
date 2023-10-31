using System.Collections;
using UnityEngine;
#if UNITY_IPHONE
using Unity.Notifications.iOS;
#elif UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

public class PushNotificationManager : MonoBehaviour
{
    string pushToken;
    string iOSDeviceToken;

    private void OnEnable()
    {
#if UNITY_IOS
        iOSNotificationCenter.OnRemoteNotificationReceived += RemoteNotificationReceived_IOS;
#elif UNITY_ANDROID
            AndroidNotificationCenter.OnNotificationReceived += RemoteNotificationReceived_Android;
            var request = new PermissionRequest();
#endif
    }

    private void OnDisable()
    {
#if UNITY_IOS
        iOSNotificationCenter.OnRemoteNotificationReceived -= RemoteNotificationReceived_IOS;
#elif UNITY_ANDROID
        AndroidNotificationCenter.OnNotificationReceived -= RemoteNotificationReceived_Android;
#endif
    }

    private void RegisterForPush()
    {
        if (string.IsNullOrEmpty(pushToken)) return;

        Debug.Log("<color=cyan>[PushNotification Manager] Registering for Push Notifications...</color>");

        // The process for registering for iOS push notifications and Android
        // push notifcations is different, so we branch at this point.
        //
        // Documentation for iOS: https://docs.unity3d.com/Packages/com.unity.mobile.notifications@1.4/manual/iOS.html
        // Documentation for Android: https://docs.unity3d.com/Packages/com.unity.mobile.notifications@1.4/manual/Android.html
#if UNITY_IOS

        RequestPushAuthorization_IOS();
#elif UNITY_ANDROID

        RequestPushAuthorization_Android();
#endif
    }

    #region iOS
#if UNITY_IPHONE

    // This Corotuine is created to handle if upon logging in the user has
    // changed their notifcation status. This specifically runs after logging in
    // AND after the user can either confirmed or denied the notifcation request/
    // The permission dialog should only displayed once in the app's lifetime.
    IEnumerator UpdateAuthorizationRequest()
    {
        // must be called before trying to obtain the push token
        // an asynchronous call with no callback into native iOS code that takes a moment or two before
        // the token is available. (so spin and wait, or call this one early on)
        // this will always return null if your app is not signed
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization: \n";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log("<color=blue>[PushNotification Manager]" + res + "</color>");

            iOSDeviceToken = req.DeviceToken;
        }
    }

    private void RequestPushAuthorization_IOS()
    {

        if (iOSDeviceToken != null)
        {
     
        }
        else
        {
            // If we logged in, but failed to get the iOSDeviceToken for some reason.
            // Scenario when this would happen:
            //      -   user previously declined push notification on launch, but changed our notification
            //          permissions in their setting app
            var settings = iOSNotificationCenter.GetNotificationSettings();
            if (settings.AuthorizationStatus != AuthorizationStatus.Denied)
            {
                StartCoroutine(UpdateAuthorizationRequest());
            }
        }
    }

    private void RemoteNotificationReceived_IOS(iOSNotification notification)
    {
        // This sends out the activation if we have the application active
    }
#endif
    #endregion

    #region Android
#if UNITY_ANDROID
    // After logging in successfully we can request a push token. Most of the heavy lifting is done
    // via Firebase. After Firebases initialized, a token is sent to the android device kicking off
    // registering for push notifications.
    private void RequestPushAuthorization_Android()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

    private void RemoteNotificationReceived_Android(AndroidNotificationIntentData data)
    {
        // This sends out the activation if we have the application active
    }
#endif
    #endregion
}
