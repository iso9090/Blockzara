using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AdMobTestConfig
{
    const string TestAndroidAppId = "ca-app-pub-3940256099942544~3347511713";
    const string TestIosAppId = "ca-app-pub-3940256099942544~1458002511";

    static AdMobTestConfig()
    {
        EditorApplication.delayCall += Apply;
    }

    static void Apply()
    {
        var type = System.Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor");
        if (type == null) return;
        var load = type.GetMethod("LoadInstance", BindingFlags.NonPublic | BindingFlags.Static);
        var settings = load == null ? null : load.Invoke(null, null) as ScriptableObject;
        if (settings == null) return;
        type.GetProperty("GoogleMobileAdsAndroidAppId").SetValue(settings, TestAndroidAppId);
        type.GetProperty("GoogleMobileAdsIOSAppId").SetValue(settings, TestIosAppId);
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }
}
