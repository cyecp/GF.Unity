using System;
using System.Collections.Generic;

public class AsyncAssetLoaderMgr
{
    //-------------------------------------------------------------------------
    Dictionary<string, IAsyncAssetLoader> mMapIAsyncAssetLoader;
    Dictionary<string, IAsyncAssetLoader> mMapNeedRemoveAssetLoader;

    //-------------------------------------------------------------------------
    public AsyncAssetLoaderMgr()
    {
        mMapIAsyncAssetLoader = new Dictionary<string, IAsyncAssetLoader>();
        mMapNeedRemoveAssetLoader = new Dictionary<string, IAsyncAssetLoader>();
    }

    //-------------------------------------------------------------------------
    public AsyncAssetLoadGroup createAsyncAssetLoadGroup()
    {
        return new AsyncAssetLoadGroup(this);
    }

    //-------------------------------------------------------------------------
    public void update(float time)
    {
        foreach (var i in mMapIAsyncAssetLoader)
        {
            i.Value.checkAssetLoadDone();
        }

        foreach (var i in mMapNeedRemoveAssetLoader)
        {
            if (mMapIAsyncAssetLoader.ContainsKey(i.Key))
            {
                mMapIAsyncAssetLoader.Remove(i.Key);
            }
        }

        mMapNeedRemoveAssetLoader.Clear();
    }

    //-------------------------------------------------------------------------
    public void _destroyAsyncAssetLoader(string asset_path)
    {
        if (mMapIAsyncAssetLoader.ContainsKey(asset_path))
        {
            mMapNeedRemoveAssetLoader[asset_path] = mMapIAsyncAssetLoader[asset_path];
        }
    }

    //-------------------------------------------------------------------------
    public void _asyncLoadAsset(_eAsyncAssetLoadType async_assetloadtype,
        string asset_path, string asset_name,
        Action<UnityEngine.Object> loaded_action, AsyncAssetLoadGroup group)
    {
        IAsyncAssetLoader asynce_assetloader = null;
        mMapIAsyncAssetLoader.TryGetValue(asset_path, out asynce_assetloader);
        if (asynce_assetloader == null)
        {
            switch (async_assetloadtype)
            {
                case _eAsyncAssetLoadType.WWW:
                    asynce_assetloader = new WWWAsyncAssetLoader(this);
                    break;
                case _eAsyncAssetLoadType.LoacalAB:
                    asynce_assetloader = new LocalABAsyncAssetLoader(this);
                    break;
                default:
                    break;
            }
        }

        asynce_assetloader.createAssetLoad(asset_path, asset_name, group, loaded_action);

        mMapIAsyncAssetLoader[asset_path] = asynce_assetloader;
    }
}

//-------------------------------------------------------------------------
public enum _eAsyncAssetLoadType
{
    WWW,
    LoacalAB,
}