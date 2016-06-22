using System;
using System.Collections.Generic;

public class AsyncAssetLoadGroup
{
    //-------------------------------------------------------------------------
    public bool IsCancel { get; private set; }
    AsyncAssetLoaderMgr AsyncAssetLoaderMgr { get; set; }

    //-------------------------------------------------------------------------
    public AsyncAssetLoadGroup(AsyncAssetLoaderMgr mgr)
    {
        IsCancel = false;
        AsyncAssetLoaderMgr = mgr;
    }

    //-------------------------------------------------------------------------
    public void asyncLoadAssetBundle(string asset_path, string asset_name,
        Action<UnityEngine.Object> loaded_action)
    {
        AsyncAssetLoaderMgr._asyncLoadAsset(_eAsyncAssetLoadType.LoacalAB, asset_path, asset_name, loaded_action, this);
    }

    //-------------------------------------------------------------------------
    public void asyncLoadWWWAsset(string asset_path, string asset_name,
        Action<UnityEngine.Object> loaded_action)
    {
        AsyncAssetLoaderMgr._asyncLoadAsset(_eAsyncAssetLoadType.WWW, asset_path, asset_name, loaded_action, this);
    }

    //-------------------------------------------------------------------------
    public void destroy()
    {
        IsCancel = true;
    }
}
