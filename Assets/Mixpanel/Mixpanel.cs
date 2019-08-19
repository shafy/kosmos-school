using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace mixpanel
{
    public static partial class Mixpanel
    {
        private const string MixpanelUnityVersion = "2.0.0";
        
        private const string MixpanelTrackUrl = "https://api.mixpanel.com/track";
        private const string MixpanelEngageUrl = "https://api.mixpanel.com/engage";
        
        internal static Value OnceProperties = Value.Object;
        internal static Value AutoTrackProperties;
        internal static Value AutoEngageProperties;

        internal static void DoTrack(string eventName, Value properties)
        {
            if (!IsTracking) return;
            if (AutoTrackProperties == null) AutoTrackProperties = CollectAutoTrackProperties();
            properties.Merge(AutoTrackProperties);
            // These auto properties can change in runtime so we don't bake them into AutoProperties
            properties["$screen_width"] = Screen.width;
            properties["$screen_height"] = Screen.height;
            properties.Merge(OnceProperties);
            OnceProperties = new Value();
            properties.Merge(SuperProperties);
            if (TimedEvents.TryGetValue(eventName, out Value startTime))
            {
                properties["$duration"] = CurrentTime() - (double)startTime;
                TimedEvents.Remove(eventName);
            }
            properties["token"] = MixpanelSettings.Instance.Token;
            properties["distinct_id"] = DistinctId;
            properties["time"] = CurrentTime();
            Value data = new Value { {"event", eventName}, {"properties", properties} };
            Enqueue(MixpanelTrackUrl, data);
        }

        internal static Value CollectAutoTrackProperties()
        {
            Value properties = new Value
            {
                {"mp_lib", "unity"},
                {"$lib_version", MixpanelUnityVersion},
                {"$os", SystemInfo.operatingSystemFamily.ToString()},
                {"$os_version", SystemInfo.operatingSystem},
                {"$manufacturer", ""},
                {"$model", SystemInfo.deviceModel},
                {"$app_version_string", Application.unityVersion},
                {"$app_build_number", Application.version},
                {"$carrier", ""},
                {"$wifi", Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork},
                {"$radio", GetRadio()},
                {"$brand", ""},
                {"$device", Application.platform.ToString()},
                {"$screen_dpi", Screen.dpi},
                {"$has_nfc", false},
                {"$has_telephone", false},
                {"$bluetooth_enabled", false},
                {"$bluetooth_version", "none"}
            };
            #if UNITY_IOS
            properties["$os"] = "Apple";
            properties["$os_version"] = Device.systemVersion;
            properties["$manufacturer"] = "Apple";
            properties["$ios_ifa"] = Device.advertisingIdentifier;
            #endif
            #if UNITY_ANDROID
            properties["$os"] = "Android";
            properties["$google_play_services"] = "";
            #endif
            return properties;
        }

        internal static void DoEngage(Value properties)
        {
            if (!IsTracking) return;
            if (AutoEngageProperties == null) AutoEngageProperties = CollectAutoEngageProperties();
            properties.Merge(AutoEngageProperties);
            properties["$token"] = MixpanelSettings.Instance.Token;
            properties["$distinct_id"] = DistinctId;
            Enqueue(MixpanelEngageUrl, properties);
        }
        
        internal static Value CollectAutoEngageProperties()
        {
            Value properties = new Value();
            properties["$lib_version"] = MixpanelUnityVersion;
            #if UNITY_IOS
            properties["$os"] = "Apple";
            properties["$os_version"] = Device.systemVersion;
            properties["$app_version_string"] = Application.unityVersion;
            properties["$app_build_number"] = Application.version;
            properties["$model"] = SystemInfo.deviceModel;
            properties["$ios_ifa"] = Device.advertisingIdentifier;
            properties["$ios_devices"] = PushDeviceTokenString;
            #endif
            #if UNITY_ANDROID
            properties["$os"] = "Android";
            properties["$os_version"] = SystemInfo.operatingSystem;
            properties["$manufacturer"] = "";
            properties["$brand"] = "";
            properties["$model"] = SystemInfo.deviceModel;
            properties["$app_version_string"] = Application.unityVersion;
            properties["$app_build_number"] = Application.version;
            properties["$android_devices"] = PushDeviceTokenString;
            #endif
            return properties;
        }

        internal static string GetRadio()
        {
            switch(Application.internetReachability)
            {
                case NetworkReachability.NotReachable :
                    return "none";
                case NetworkReachability.ReachableViaCarrierDataNetwork :
                    return "carrier";
                case NetworkReachability.ReachableViaLocalAreaNetwork :
                    return "wifi";
            }
            return "none";
        }

        internal static double CurrentTime()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            double currentEpochTime = (DateTime.UtcNow - epochStart).TotalSeconds;
            return currentEpochTime;
        }

        internal static string CurrentDateTime()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        internal static void Load()
        {
            LoadData();
            LoadBatches();
        }

        internal static void Save()
        {
            SaveData();
            SaveBatches();
        }
    }
}
