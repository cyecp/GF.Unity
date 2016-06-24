using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class IAsyncAssetLoader
{
    //-------------------------------------------------------------------------    
    public string AssetPath { get; set; }
    public Dictionary<AsyncAssetLoadGroup, List<RequestLoadAssetInfo>> MapRequestLoadAssetInfo { get; set; }
    public AsyncAssetLoaderMgr AsyncAssetLoaderMgr { get; set; }

    //-------------------------------------------------------------------------
    public IAsyncAssetLoader(AsyncAssetLoaderMgr mgr)
    {
        AsyncAssetLoaderMgr = mgr;
    }

    //-------------------------------------------------------------------------
    public abstract void createAssetLoad(string asset_path, string asset_name,
        AsyncAssetLoadGroup async_assetloadgroup, Action<UnityEngine.Object> loaded_action);

    //-------------------------------------------------------------------------
    public abstract void checkAssetLoadDone();

    //-------------------------------------------------------------------------
    public abstract string assetLoadError();

    //-------------------------------------------------------------------------
    internal abstract void destory();

    //-------------------------------------------------------------------------
    internal abstract void assetLoadDone();
}

//-------------------------------------------------------------------------
public class RequestLoadAssetInfo
{
    public string AssetName { get; set; }
    public Action<UnityEngine.Object> LoadedAction { get; set; }
}
