using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

public class EditorGFInternal : EditorWindow
{
    //-------------------------------------------------------------------------
    [MenuItem("GF/导出GF.Unity.unitypackage")]
    static void exportGFUnityPackage()
    {
        string[] arr_assetpathname = new string[4];
        arr_assetpathname[0] = "Assets/GF.Unity";
        arr_assetpathname[1] = "Assets/Plugins/GF.Common";
        arr_assetpathname[2] = "Assets/Plugins/GF.Sqlite";
        arr_assetpathname[3] = "Assets/Plugins/GF.UnityPlugins";
        
        AssetDatabase.ExportPackage(arr_assetpathname, "GF.Unity.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export GF.Unity.unitypackage Finished!");
    }

    //-------------------------------------------------------------------------
    [MenuItem("GF/导出GF.Json.unitypackage")]
    static void exportGFJsonPackage()
    {
        string[] arr_assetpathname = new string[1];
        arr_assetpathname[0] = "Assets/Plugins/GF.Json";
        AssetDatabase.ExportPackage(arr_assetpathname, "GF.Json.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export GF.Json.unitypackage Finished!");
    }
}
