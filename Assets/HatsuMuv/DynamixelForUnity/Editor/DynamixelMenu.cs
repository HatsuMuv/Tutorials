#if UNITY_64
using HatsuMuv.DynamixelForUnity.x64;
#else
        using HatsuMuv.DynamixelForUnity.x86;
#endif


using HatsuMuv.DynamixelForUnity.Example;
using UnityEditor;
using UnityEngine;
using System.Windows;

public class DynamixelMenu : MonoBehaviour
{
    [MenuItem("DynamixelForUnity/Get DeviceID for Registration (Copy DeviceID to Clipboard)",false,1)]
    public static void GetDeviceID()
    {
        string deviceID = HatsuMuvUtilities.GetDeviceID();
        print("\"<color=yellow>[DynamixelForUnity] Your Device ID is " + deviceID + "</color>" );
        GUIUtility.systemCopyBuffer = deviceID;

        //設定した文字列をダイアログで伝える
        EditorUtility.DisplayDialog("Device ID Copied", "Following device ID has been copied to your clipboard!\n"+ deviceID, "OK");

    }

    [MenuItem("DynamixelForUnity/Extend Your Offline Mode (Require DynamixelForUnity in scene)", false, 2)]
    public static void ExtendOfflineMode()
    {
        DynamixelForUnity d4u = GameObject.FindObjectOfType<DynamixelForUnity>();


        if (d4u == null)
        {
            Debug.LogError("\"<color=red>[DynamixelForUnity] DynamixelForUnity component should be added in scene</color>");
            return;
        }
        if (d4u.UserEmail == null)
        {
            Debug.LogError("\"<color=red>[DynamixelForUnity] Your DynamixelForUnity component dont have Email! Please input your Email!</color>");
            return;
        }
        if (d4u.SNCode == "")
        {
            Debug.LogError("\"<color=red>[DynamixelForUnity] Your DynamixelForUnity component dont have SN! Please Input your SN!</color>");
            return;
        }


        d4u.Verificate(true); // force renew license, need internet connection
    }


    [MenuItem("DynamixelForUnity/Version", false, 2)]
    public static void ShowVersion()
    {
        EditorUtility.DisplayDialog("Version", "DynamixelForUnity " + DynamixelForUnity.Version, "OK");
    }


    [MenuItem("DynamixelForUnity/Purchasing License/Japanese")]
    public static void ShowPurchasingPageJP()
    {
        Application.OpenURL("https://www.hatsumuv.com/jp/service/dynamixelforunity");
    }

    [MenuItem("DynamixelForUnity/Purchasing License/English")]
    public static void ShowPurchasingPageEN()
    {
        Application.OpenURL("https://www.hatsumuv.com/en/service/dynamixelforunity");
    }

}
