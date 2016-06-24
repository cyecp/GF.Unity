using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

public class GFEditor : EditorWindow
{
    //-------------------------------------------------------------------------
    static MD5 mMD5;
    static string mBundleVersion;
    static string mDataVersion;
    static string mAndroidBundleVersion;
    static string mAndroidDataVersion;
    static string mIOSBundleVersion;
    static string mIOSDataVersion;
    static string mPCBundleVersion;
    static string mPCDataVersion;
    static string mTargetPlatformRootPath;
    static string mAssetBundleResourcesPath;
    static string mRowAssetPath;
    static string mTargetPath;
    static string mABTargetPath;
    static string mAssetPath;
    static BuildTarget mInitBuildTarget;
    static BuildTarget mCurrentBuildTarget;
    static List<BuildTarget> mListNeedBuildPlatform;
    static Queue<BuildTarget> mQueueNeedBuildPlatform;
    static string mPatchInfoPath;
    static List<string> mDoNotPackFileExtention = new List<string> { ".meta", ".DS_Store" };

    //const string mNotPackAsset = "NotPackAsset";
    //const string mAssetBundleDirectory = "NeedPackAsset";
    const string mAssetBundleTargetDirectory = "ABPatch";
    const string mABPathInfoResourceDirectory = "GF.Unity/AutoPatcherInfo";
    const string mPatchiInfoName = "ABPatchInfo.xml";

    string mPackInfoTextName = "DataFileList.txt";
    string mDataTargetPath;
    bool mCurrentIsBuidAndroid = false;
    bool mCurrentIsBuidIOS = false;
    bool mCurrentIsBuidPC = false;
    bool mBuidAndroid = false;
    bool mBuidIOS = false;
    bool mBuidPC = false;
    List<string> mListAllABFile = new List<string>();


    //-------------------------------------------------------------------------
    [MenuItem("GF.Unity/AutoPatcher")]
    static void AutoPatcher()
    {
        _checkPath();
        _initCurrentBuildTarget();

        if (!Directory.Exists(mABTargetPath))
        {
            GFEditorInitProjectInfo test = GetWindow<GFEditorInitProjectInfo>("初始化项目信息");

            test.copyPatchInfo(mABTargetPath,
                mAssetPath + mABPathInfoResourceDirectory);
            return;
        }

        GFEditor dragon_pack = (GFEditor)EditorWindow.GetWindow(typeof(GFEditor));
        _getCurrentTargetPath();
        _checkPatchData();
        mMD5 = new MD5CryptoServiceProvider();
        mListNeedBuildPlatform = new List<BuildTarget>();
        mQueueNeedBuildPlatform = new Queue<BuildTarget>();
    }

    //-------------------------------------------------------------------------
    static void _initCurrentBuildTarget()
    {
#if UNITY_IPHONE || UNITY_IOS
        mInitBuildTarget = BuildTarget.iOS;
#elif UNITY_ANDROID
        mInitBuildTarget = BuildTarget.Android;
#elif UNITY_STANDALONE_WIN
        mInitBuildTarget = BuildTarget.StandaloneWindows;
#endif
        mCurrentBuildTarget = mInitBuildTarget;
    }

    //-------------------------------------------------------------------------
    static void _checkPath()
    {
        string current_dir = System.Environment.CurrentDirectory;
        current_dir = current_dir.Replace(@"\", "/");
        mAssetPath = current_dir + "/Assets/";

        mABTargetPath = Path.Combine(current_dir, mAssetBundleTargetDirectory);
        mABTargetPath = mABTargetPath.Replace(@"\", "/");
        mPatchInfoPath = Path.Combine(mABTargetPath, mPatchiInfoName);
        mPatchInfoPath = mPatchInfoPath.Replace(@"\", "/");
        //Debug.LogError("mRealTargetPath:: " + mRealTargetPath);      
    }

    //-------------------------------------------------------------------------
    static void _getCurrentTargetPath()
    {
        if (mCurrentBuildTarget == BuildTarget.Android)
        {
            mTargetPlatformRootPath = mABTargetPath + "/ANDROID/";
        }
        else if (mCurrentBuildTarget == BuildTarget.iOS)
        {
            mTargetPlatformRootPath = mABTargetPath + "/IOS/";
        }
        else if (mCurrentBuildTarget == BuildTarget.StandaloneWindows)
        {
            mTargetPlatformRootPath = mABTargetPath + "/PC/";
        }
    }

    //-------------------------------------------------------------------------
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前平台程序包版本号:", mBundleVersion);
        bool add_bundleversion = GUILayout.Button("程序包版本号加一");
        if (add_bundleversion)
        {
            _changeBundleData(true);
        }
        bool minus_bundleversion = GUILayout.Button("程序包版本号减一");
        if (minus_bundleversion)
        {
            _changeBundleData(false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前平台资源版本号:", mDataVersion);
        bool add_dataeversion = GUILayout.Button("资源版本号加一");
        if (add_dataeversion)
        {
            _changeDataData(true);
        }
        bool minus_dataeversion = GUILayout.Button("资源版本号减一");
        if (minus_dataeversion)
        {
            _changeDataData(false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("AB资源所在路径:");
        Rect ab_resourcesrect = EditorGUILayout.GetControlRect(GUILayout.Width(500));
        mAssetBundleResourcesPath = EditorGUI.TextField(ab_resourcesrect, mAssetBundleResourcesPath);
        if ((Event.current.type == EventType.DragUpdated ||
          Event.current.type == EventType.DragExited) &&
          ab_resourcesrect.Contains(Event.current.mousePosition))
        {
            string path = DragAndDrop.paths[0];
            if (!string.IsNullOrEmpty(path))
            {
                DragAndDrop.AcceptDrag();
                mAssetBundleResourcesPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("裸资源所在路径:");
        Rect ab_rowrect = EditorGUILayout.GetControlRect(GUILayout.Width(500));
        mRowAssetPath = EditorGUI.TextField(ab_rowrect, mRowAssetPath);
        if ((Event.current.type == EventType.DragUpdated ||
          Event.current.type == EventType.DragExited) &&
          ab_rowrect.Contains(Event.current.mousePosition))
        {
            string path = DragAndDrop.paths[0];
            if (!string.IsNullOrEmpty(path))
            {
                DragAndDrop.AcceptDrag();
                mRowAssetPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("目标路径:", mTargetPlatformRootPath);

        EditorGUILayout.BeginHorizontal();
        mBuidAndroid = EditorGUILayout.Toggle("是否打AndroidAB", mBuidAndroid);
        if (mCurrentIsBuidAndroid != mBuidAndroid)
        {
            mCurrentIsBuidAndroid = mBuidAndroid;
            _checkIfNeePackPlatform(mCurrentIsBuidAndroid, BuildTarget.Android);
        }
        mBuidIOS = EditorGUILayout.Toggle("是否打IOSAB", mBuidIOS);
        if (mCurrentIsBuidIOS != mBuidIOS)
        {
            mCurrentIsBuidIOS = mBuidIOS;
            _checkIfNeePackPlatform(mCurrentIsBuidIOS, BuildTarget.iOS);
        }
        mBuidPC = EditorGUILayout.Toggle("是否打PCAB", mBuidPC);
        if (mCurrentIsBuidPC != mBuidPC)
        {
            mCurrentIsBuidPC = mBuidPC;
            _checkIfNeePackPlatform(mCurrentIsBuidPC, BuildTarget.StandaloneWindows);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool copy_asset = GUILayout.Button("复制AB资源到persistentPath", GUILayout.Width(200));
        if (copy_asset)
        {
            _copyOrDeleteToClient(true);
        }
        bool delete_asset = GUILayout.Button("删除persistentPath中的AB资源", GUILayout.Width(200));
        if (delete_asset)
        {
            _copyOrDeleteToClient(false);
        }
        EditorGUILayout.EndHorizontal();

        //bool check_path = GUILayout.Button("重设路径", GUILayout.Width(200));
        //if (check_path)
        //{
        //    _checkPath();
        //    _checkPatchData();
        //}

        bool click_build_asset = GUILayout.Button("打AssetBundle包(压缩)", GUILayout.Width(200));
        if (click_build_asset)
        {
            _startBuild();
        }
    }

    ////-------------------------------------------------------------------------
    //string _checkDragText(Rect rect)
    //{
    //    string path = "";
    //    if ((Event.current.type == EventType.DragUpdated ||
    //        Event.current.type == EventType.DragExited) &&
    //        rect.Contains(Event.current.mousePosition))
    //    {
    //        path = DragAndDrop.paths[0];
    //        if (!string.IsNullOrEmpty(path))
    //        {
    //            DragAndDrop.AcceptDrag();
    //        }
    //    }

    //    return path;
    //}

    //-------------------------------------------------------------------------
    void _checkIfNeePackPlatform(bool is_currentneed, BuildTarget build_target)
    {
        if (is_currentneed)
        {
            _setNeedPackPlatform(build_target);
        }
        else
        {
            _removeNeedPackPlatform(build_target);
        }
    }

    //-------------------------------------------------------------------------
    void _setNeedPackPlatform(BuildTarget build_target)
    {
        if (!mListNeedBuildPlatform.Contains(build_target))
        {
            mListNeedBuildPlatform.Add(build_target);
        }
    }

    //-------------------------------------------------------------------------
    void _removeNeedPackPlatform(BuildTarget build_target)
    {
        if (mListNeedBuildPlatform.Contains(build_target))
        {
            mListNeedBuildPlatform.Remove(build_target);
        }
    }

    //-------------------------------------------------------------------------
    void _startBuild()
    {
        foreach (var i in mListNeedBuildPlatform)
        {
            mQueueNeedBuildPlatform.Enqueue(i);
        }

        mListNeedBuildPlatform.Clear();

        _startCurrentBuild();
    }

    //-------------------------------------------------------------------------
    void _startCurrentBuild()
    {
        if (mQueueNeedBuildPlatform.Count > 0)
        {
            mCurrentBuildTarget = mQueueNeedBuildPlatform.Dequeue();
            _packAssetBundleCompress();
        }
        else
        {
            ShowNotification(new GUIContent("打包完成!"));

            EditorUserBuildSettings.SwitchActiveBuildTarget(mInitBuildTarget);
            changeAssetBundleResourcePath();
            changeRowResourcePath();
        }
    }

    //-------------------------------------------------------------------------
    void _packResources(string pack_infopath)
    {
        StreamWriter sw;
        string info = pack_infopath + "/" + mPackInfoTextName;

        if (!File.Exists(info))
        {
            sw = File.CreateText(info);
        }
        else
        {
            sw = new StreamWriter(info);
        }

        using (sw)
        {
            _checkPackInfo(sw, pack_infopath);
        }

        _startCurrentBuild();
    }

    //-------------------------------------------------------------------------
    void _checkPackInfo(StreamWriter sw, string path)
    {
        string[] files = Directory.GetFiles(path);
        foreach (var i in files)
        {
            string directory_name = Path.GetDirectoryName(i);
            directory_name = directory_name.Replace(@"\", "/");
            directory_name = directory_name.Substring(directory_name.LastIndexOf("/") + 1);
            string file_name = Path.GetFileName(i);
            string file_namewithoutex = Path.GetFileNameWithoutExtension(i);
            if (file_name.Equals(mPackInfoTextName) || file_name.Equals(directory_name) || file_namewithoutex.Equals(directory_name))
            {
                continue;
            }

            string file_extension = Path.GetExtension(i);
            if (mDoNotPackFileExtention.Contains(file_extension))
            {
                continue;
            }

            string file_directory = Path.GetDirectoryName(i);
            string target_path = file_directory.Replace(mTargetPath, "");
            target_path = target_path.Replace(@"\", "/");
            string file_path = i;
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(target_path + "/" + file_name + " ");

                using (FileStream sr = File.OpenRead(file_path))
                {
                    byte[] new_bytes = mMD5.ComputeHash(sr);
                    foreach (var bytes in new_bytes)
                    {
                        sb.Append(bytes.ToString("X2"));
                    }
                }

                sw.WriteLine(sb.ToString());
            }
        }

        string[] directorys = Directory.GetDirectories(path);
        foreach (var i in directorys)
        {
            _checkPackInfo(sw, i);
        }
    }

    //-------------------------------------------------------------------------
    void _copyOrDeleteToClient(bool is_copy)
    {
        string persistent_data_path =
#if UNITY_STANDALONE_WIN && UNITY_EDITOR
        Application.persistentDataPath + "/PC/";
#elif UNITY_ANDROID && UNITY_EDITOR
 Application.persistentDataPath + "/ANDROID/";
#elif UNITY_IPHONE && UNITY_EDITOR
        Application.persistentDataPath + "/IOS/";
#endif
        persistent_data_path = persistent_data_path.Replace(@"\", "/");
        if (is_copy)
        {
            if (!Directory.Exists(persistent_data_path))
            {
                Directory.CreateDirectory(persistent_data_path);
            }

            try
            {
                copyFile(mTargetPath, persistent_data_path, mTargetPath);
                ShowNotification(new GUIContent("复制AB到本地成功!"));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        else
        {
            try
            {
                _deleteFile(persistent_data_path);
                ShowNotification(new GUIContent("删除本地AB成功!"));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    //-------------------------------------------------------------------------
    void _deleteFile(string directory_path)
    {
        if (Directory.Exists(directory_path))
        {
            string[] files = Directory.GetFiles(directory_path);
            foreach (var i in files)
            {
                File.Delete(i);
            }

            string[] directorys = Directory.GetDirectories(directory_path);

            foreach (var i in directorys)
            {
                _deleteFile(i);
            }

            Directory.Delete(directory_path);
        }
    }

    //-------------------------------------------------------------------------
    void _packAssetBundleCompress()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(mCurrentBuildTarget);

        _getCurrentTargetPath();
        _checkPatchData();

        _deleteFile(mTargetPlatformRootPath);

        Caching.CleanCache();

        _getAllFiles(mAssetBundleResourcesPath);

        if (!Directory.Exists(mTargetPath))
        {
            Directory.CreateDirectory(mTargetPath);
        }

        foreach (var obj in mListAllABFile)
        {
            if (File.Exists(obj))
            {
                string path = Path.GetFullPath(obj);
                path = path.Replace(@"\", "/");
                path = path.Replace(mAssetPath, "");
                path = mTargetPath + "/" + path;
                string obj_dir = path.Replace(Path.GetFileName(obj), "");
                if (!Directory.Exists(obj_dir))
                {
                    Directory.CreateDirectory(obj_dir);
                }
                var names = AssetDatabase.GetDependencies(obj);

                AssetBundleBuild abb;
                abb.assetBundleName = Path.GetFileNameWithoutExtension(obj) + ".ab";
                abb.assetBundleVariant = "";
                int asset_index = 0;
                List<string> list_needbuildassetname = new List<string>();
                //list_needbuildassetname.Add(obj.Replace(mAssetPath, "Assets/"));
                foreach (var j in names)
                {
                    //Debug.Log("Asset: " + j);
                    if (j.EndsWith(".cs") || j.EndsWith(".ttf")) continue;
                    if (list_needbuildassetname.Contains(j))
                    {
                        continue;
                    }
                    list_needbuildassetname.Add(j);
                }
                abb.assetNames = new string[list_needbuildassetname.Count];

                foreach (var i in list_needbuildassetname)
                {
                    abb.assetNames[asset_index++] = i;
                }

                AssetBundleBuild[] arr_abb = new AssetBundleBuild[1];
                arr_abb[0] = abb;

                _buildAssetBundleCompressed(arr_abb, obj_dir, mCurrentBuildTarget, false);
                //#if UNITY_STANDALONE_WIN
                //                _buildAssetBundleCompressed(arr_abb, obj_dir, BuildTarget.StandaloneWindows64, false);
                //#elif UNITY_IOS||UNITY_IPHONE
                //                _buildAssetBundleCompressed(arr_abb, obj_dir, BuildTarget.iOS, false);
                //#elif UNITY_ANDROID
                //                _buildAssetBundleCompressed(arr_abb, obj_dir, BuildTarget.Android, false);
                //#endif
            }
        }

        if (Directory.Exists(mRowAssetPath))
        {
            copyFile(mRowAssetPath, mTargetPath, "Assets/");
        }

        Debug.Log("裸资源复制完毕!");

        _packResources(mTargetPath);
    }

    //-------------------------------------------------------------------------
    static void _buildAssetBundleCompressed(AssetBundleBuild[] arr_abb, string path, BuildTarget target, bool build_all = true)
    {
        if (build_all)
        {
            //BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
            //  BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, target);
        }
        else
        {
            BuildPipeline.BuildAssetBundles(path, arr_abb, BuildAssetBundleOptions.ForceRebuildAssetBundle, target);
        }

        EbLog.Note("Build AssetBundle BuildTarget=" + target);
    }

    //-------------------------------------------------------------------------
    void _getAllFiles(string directory_path)
    {
        string[] ab_file = Directory.GetFiles(directory_path);
        foreach (var i in ab_file)
        {
            string extension = Path.GetExtension(i);
            //Debug.LogError(i + "   " + extension);
            if (mDoNotPackFileExtention.Contains(extension))
            {
                continue;
            }

            mListAllABFile.Add(i);
        }

        string[] directorys = Directory.GetDirectories(directory_path);
        foreach (var i in directorys)
        {
            _getAllFiles(i);
        }
    }

    //-------------------------------------------------------------------------
    public static void copyFile(string path, string target_rootpath, string need_replacepath)
    {
        string[] files = Directory.GetFiles(path);
        foreach (var i in files)
        {
            if (!File.Exists(i))
            {
                continue;
            }

            string file_extension = Path.GetExtension(i);
            if (mDoNotPackFileExtention.Contains(file_extension))
            {
                continue;
            }

            string file_name = Path.GetFileName(i);
            string file_directory = Path.GetDirectoryName(i);
            file_directory = file_directory.Replace(@"\", "/");
            string target_path = file_directory.Replace(need_replacepath, "");
            string file_path = i;
            string target_p = target_rootpath + "/" + target_path;
            if (!Directory.Exists(target_p))
            {
                Directory.CreateDirectory(target_p);
            }
            File.Copy(file_path, target_p + "/" + file_name, true);
        }

        string[] directorys = Directory.GetDirectories(path);
        foreach (var i in directorys)
        {
            copyFile(i, target_rootpath, need_replacepath);
        }
    }

    //-------------------------------------------------------------------------
    static void _checkPatchData()
    {
        XmlDocument abpath_xml = new XmlDocument();
        abpath_xml.Load(mPatchInfoPath);

        XmlElement root = null;
        root = abpath_xml.DocumentElement;

        foreach (XmlElement i in root.ChildNodes)
        {
            if (i.Name.Equals("BundleData"))
            {
                mAndroidBundleVersion = i.GetAttribute("BDAndroid");
                mIOSBundleVersion = i.GetAttribute("BDIOS");
                mPCBundleVersion = i.GetAttribute("BDWindowsPC");
                if (mCurrentBuildTarget == BuildTarget.iOS)
                {
                    mBundleVersion = mIOSBundleVersion;
                }
                else if (mCurrentBuildTarget == BuildTarget.Android)
                {
                    mBundleVersion = mAndroidBundleVersion;
                }
                else if (mCurrentBuildTarget == BuildTarget.StandaloneWindows)
                {
                    mBundleVersion = mPCBundleVersion;
                }
            }
            else if (i.Name.Equals("DataData"))
            {
                mAndroidDataVersion = i.GetAttribute("DDAndroid");
                mIOSDataVersion = i.GetAttribute("DDIOS");
                mPCDataVersion = i.GetAttribute("DDWindowsPC");
                if (mCurrentBuildTarget == BuildTarget.iOS)
                {
                    mDataVersion = mIOSDataVersion;
                }
                else if (mCurrentBuildTarget == BuildTarget.Android)
                {
                    mDataVersion = mAndroidDataVersion;
                }
                else if (mCurrentBuildTarget == BuildTarget.StandaloneWindows)
                {
                    mDataVersion = mPCDataVersion;
                }
            }
            else if (i.Name.Equals("AssetBundleResourcePath"))
            {
                mAssetBundleResourcesPath = i.GetAttribute("ABValue");//mAssetPath + mAssetBundleDirectory;
            }
            else if (i.Name.Equals("RawResourcePath"))
            {
                mRowAssetPath = i.GetAttribute("RawValue");//mAssetPath + mNotPackAsset;
            }
        }

        PlayerSettings.bundleIdentifier = "com." + PlayerSettings.companyName + "." + PlayerSettings.productName;
        PlayerSettings.bundleVersion = mBundleVersion;
        mTargetPath = mTargetPlatformRootPath + "DataVersion_" + mDataVersion;
    }

    //-------------------------------------------------------------------------
    public static void changeBundleData(string new_bundle, bool change_allplatform = false)
    {
        _checkPatchData();
        string ab_pathinfo = mABTargetPath + "/" + mPatchiInfoName;
        string file = "";
        using (StreamReader sr = new StreamReader(ab_pathinfo))
        {
            file = sr.ReadToEnd();
            string replace_oldvalue = "";
            string replace_newvalue = "";
            if (change_allplatform)
            {
                replace_oldvalue = "BDIOS=\"" + mIOSBundleVersion + "\"";
                replace_newvalue = "BDIOS=\"" + new_bundle + "\"";
                file = file.Replace(replace_oldvalue, replace_newvalue);

                replace_oldvalue = "BDAndroid=\"" + mAndroidBundleVersion + "\"";
                replace_newvalue = "BDAndroid=\"" + new_bundle + "\"";
                file = file.Replace(replace_oldvalue, replace_newvalue);

                replace_oldvalue = "BDWindowsPC=\"" + mPCBundleVersion + "\"";
                replace_newvalue = "BDWindowsPC=\"" + new_bundle + "\"";
                file = file.Replace(replace_oldvalue, replace_newvalue);
            }
            else
            {
#if UNITY_IPHONE || UNITY_IOS
            replace_oldvalue = "BDIOS=\"" + mBundleVersion + "\"";
            replace_newvalue = "BDIOS=\"" + new_bundle + "\"";
#elif UNITY_ANDROID
                replace_oldvalue = "BDAndroid=\"" + mBundleVersion + "\"";
                replace_newvalue = "BDAndroid=\"" + new_bundle + "\"";
#elif UNITY_STANDALONE_WIN
                replace_oldvalue = "BDWindowsPC=\"" + mDataVersion + "\"";
                replace_newvalue = "BDWindowsPC=\"" + new_bundle + "\"";
#endif
                file = file.Replace(replace_oldvalue, replace_newvalue);
            }
        }

        using (StreamWriter sw = new StreamWriter(ab_pathinfo))
        {
            sw.Write(file);
        }

        _checkPatchData();
    }

    //-------------------------------------------------------------------------
    public static void changeDataData(string new_data, bool change_allplatform = false)
    {
        _checkPatchData();
        string ab_pathinfo = mABTargetPath + "/" + mPatchiInfoName;
        string file = "";
        using (StreamReader sr = new StreamReader(ab_pathinfo))
        {
            file = sr.ReadToEnd();
            string replace_oldvalue = "";
            string replace_newvalue = "";
            if (change_allplatform)
            {
                replace_oldvalue = "DDIOS=\"" + mIOSDataVersion + "\"";
                replace_newvalue = "DDIOS=\"" + new_data + "\"";
                file = file.Replace(replace_oldvalue, replace_newvalue);

                replace_oldvalue = "DDAndroid=\"" + mAndroidDataVersion + "\"";
                replace_newvalue = "DDAndroid=\"" + new_data + "\"";
                file = file.Replace(replace_oldvalue, replace_newvalue);

                replace_oldvalue = "DDWindowsPC=\"" + mPCDataVersion + "\"";
                replace_newvalue = "DDWindowsPC=\"" + new_data + "\"";
                file = file.Replace(replace_oldvalue, replace_newvalue);
            }
            else
            {
#if UNITY_IPHONE || UNITY_IOS
            replace_oldvalue = "DDIOS=\"" + mDataVersion + "\"";
            replace_newvalue = "DDIOS=\"" + new_data + "\"";
#elif UNITY_ANDROID
                replace_oldvalue = "DDAndroid=\"" + mDataVersion + "\"";
                replace_newvalue = "DDAndroid=\"" + new_data + "\"";
#elif UNITY_STANDALONE_WIN
                replace_oldvalue = "DDWindowsPC=\"" + mDataVersion + "\"";
                replace_newvalue = "DDWindowsPC=\"" + new_data + "\"";
#endif
                file = file.Replace(replace_oldvalue, replace_newvalue);
            }
        }

        using (StreamWriter sw = new StreamWriter(ab_pathinfo))
        {
            sw.Write(file);
        }

        _checkPatchData();
    }

    //-------------------------------------------------------------------------
    public static void changeAssetBundleResourcePath()
    {
        string file = "";
        using (StreamReader sr = new StreamReader(mPatchInfoPath))
        {
            file = sr.ReadToEnd();
            string ab_value = "ABValue=\"";
            int replace_index = file.IndexOf(ab_value) + ab_value.Length;
            string left_file = file.Substring(replace_index);
            string replace_oldvalue = left_file.Substring(0, left_file.IndexOf("\""));
            string replace_newvalue = mAssetBundleResourcesPath;

            if (string.IsNullOrEmpty(replace_oldvalue))
            {
                file = file.Insert(replace_index, replace_newvalue);
            }
            else
            {
                file = file.Replace(replace_oldvalue, replace_newvalue);
            }
        }

        using (StreamWriter sw = new StreamWriter(mPatchInfoPath))
        {
            sw.Write(file);
        }
    }

    //-------------------------------------------------------------------------
    public static void changeRowResourcePath()
    {
        string file = "";
        using (StreamReader sr = new StreamReader(mPatchInfoPath))
        {
            file = sr.ReadToEnd();
            string ab_value = "RawValue=\"";
            int replace_index = file.IndexOf(ab_value) + ab_value.Length;
            string left_file = file.Substring(replace_index);
            string replace_oldvalue = left_file.Substring(0, left_file.IndexOf("\""));
            string replace_newvalue = mRowAssetPath;

            if (string.IsNullOrEmpty(replace_oldvalue))
            {
                file = file.Insert(replace_index, replace_newvalue);
            }
            else
            {
                file = file.Replace(replace_oldvalue, replace_newvalue);
            }
        }

        using (StreamWriter sw = new StreamWriter(mPatchInfoPath))
        {
            sw.Write(file);
        }
    }

    //-------------------------------------------------------------------------
    void _changeBundleData(bool add)
    {
        int bundle = int.Parse(mBundleVersion.Replace(".", ""));
        if (add)
        {
            bundle += 1;
        }
        else
        {
            bundle -= 1;
        }
        string bundle_version = bundle.ToString();
        bundle_version = bundle_version.Insert(1, ".").Insert(4, ".");
        changeBundleData(bundle_version);
    }

    //-------------------------------------------------------------------------
    void _changeDataData(bool add)
    {
        int data = int.Parse(mDataVersion.Replace(".", ""));
        if (add)
        {
            data += 1;
        }
        else
        {
            data -= 1;
        }
        string data_version = data.ToString();
        data_version = data_version.Insert(1, ".").Insert(4, ".");
        changeDataData(data_version);
    }
}
