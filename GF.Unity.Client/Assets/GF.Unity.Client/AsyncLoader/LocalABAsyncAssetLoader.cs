using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalABAsyncAssetLoader : IAsyncAssetLoader
{
    //-------------------------------------------------------------------------
    AssetBundleCreateRequest mAssetBundleCreateRequest;

    //-------------------------------------------------------------------------
    public LocalABAsyncAssetLoader(AsyncAssetLoaderMgr mgr)
        : base(mgr)
    {
        MapRequestLoadAssetInfo = new Dictionary<AsyncAssetLoadGroup, List<RequestLoadAssetInfo>>();
    }

    //-------------------------------------------------------------------------
    public override void checkAssetLoadDone()
    {
        bool is_done = false;

        if (mAssetBundleCreateRequest == null)
        {
            is_done = false;
            destory();
        }
        else
        {
            is_done = mAssetBundleCreateRequest.isDone;
        }

        if (is_done)
        {
            if (string.IsNullOrEmpty(assetLoadError()))
            {
                assetLoadDone();
            }
            else
            {
                Debug.LogError(assetLoadError());
                destory();
            }
        }
    }

    //-------------------------------------------------------------------------
    public override string assetLoadError()
    {
        string load_error = "";
        if (mAssetBundleCreateRequest == null)
        {
            load_error = "LoadABAsync Is Null! AssetPath: " + AssetPath;
        }
        else
        {
            load_error = "";
        }

        return load_error;
    }

    //-------------------------------------------------------------------------
    public override void createAssetLoad(string asset_path, string asset_name, AsyncAssetLoadGroup async_assetloadgroup, Action<UnityEngine.Object> loaded_action)
    {
        AssetPath = asset_path;

        RequestLoadAssetInfo request_loadassetinfo = new RequestLoadAssetInfo();
        request_loadassetinfo.AssetName = asset_name;
        request_loadassetinfo.LoadedAction = loaded_action;

        List<RequestLoadAssetInfo> list_requestloadasssetinfo = null;
        MapRequestLoadAssetInfo.TryGetValue(async_assetloadgroup, out list_requestloadasssetinfo);

        if (list_requestloadasssetinfo == null)
        {
            list_requestloadasssetinfo = new List<RequestLoadAssetInfo>();
        }

        list_requestloadasssetinfo.Add(request_loadassetinfo);

        MapRequestLoadAssetInfo[async_assetloadgroup] = list_requestloadasssetinfo;

        if (mAssetBundleCreateRequest == null)
        {
            mAssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(asset_path);
        }
    }

    //-------------------------------------------------------------------------
    internal override void assetLoadDone()
    {
        bool must_copyasset = false;
        if (MapRequestLoadAssetInfo.Count > 1)
        {
            must_copyasset = true;
        }

        AssetBundle ab = mAssetBundleCreateRequest.assetBundle;

        foreach (var i in MapRequestLoadAssetInfo)
        {
            if (i.Key.IsCancel)
            {
                continue;
            }

            foreach (var asset_loadrequest in i.Value)
            {
                UnityEngine.Object load_asset = ab.LoadAsset(asset_loadrequest.AssetName);
                if (must_copyasset)
                {
                    asset_loadrequest.LoadedAction(GameObject.Instantiate(load_asset));
                }
                else
                {
                    asset_loadrequest.LoadedAction(load_asset);
                }
            }
        }

        MapRequestLoadAssetInfo.Clear();
        destory();
    }

    //-------------------------------------------------------------------------
    internal override void destory()
    {
        AsyncAssetLoaderMgr._destroyAsyncAssetLoader(AssetPath);

        if (mAssetBundleCreateRequest != null && mAssetBundleCreateRequest.assetBundle != null)
        {
            mAssetBundleCreateRequest.assetBundle.Unload(false);
        }
        mAssetBundleCreateRequest = null;
    }
}
