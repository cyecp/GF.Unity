using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

public class GFEditorInternal : EditorWindow
{
    //-------------------------------------------------------------------------
    [MenuItem("GF.Unity/导出GF.Unity.Client.unitypackage")]
    static void exportGFUnityPackage()
    {
        string[] arr_assetpathname = new string[4];
        arr_assetpathname[0] = "Assets/GF.Unity.Client";
        arr_assetpathname[1] = "Assets/Plugins/GF.Common";
        arr_assetpathname[2] = "Assets/Plugins/GF.Sqlite";
        arr_assetpathname[3] = "Assets/Plugins/GF.UnityPlugins";

        AssetDatabase.ExportPackage(arr_assetpathname, "GF.Unity.Client.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export GF.Unity.Client.unitypackage Finished!");
    }

    //-------------------------------------------------------------------------
    [MenuItem("GF.Unity/导出GF.Unity.Json.unitypackage")]
    static void exportGFJsonPackage()
    {
        string[] arr_assetpathname = new string[1];
        arr_assetpathname[0] = "Assets/Plugins/GF.Unity.Json";
        AssetDatabase.ExportPackage(arr_assetpathname, "GF.Unity.Json.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export GF.Unity.Json.unitypackage Finished!");
    }
}
