using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GF.Common;

public class ClientSampleAutoPatcher<TDef> : Component<TDef> where TDef : DefSampleAutoPatcher, new()
{
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientSampleAutoPatcher.init()");

        EntityMgr.getDefaultEventPublisher().addHandler(Entity);

        AutoPatcherConfig autopatcher_cfg = new AutoPatcherConfig();
        autopatcher_cfg.RemoteVersionInfoUrl = "https://www.cragon.cn/download/Cragon/SampleAutoPatcher/VersionInfo.xml";

        var et_autopatcher = EntityMgr.createEntity<EtAutoPatcher>(null, Entity);
        var co_autopatcher = et_autopatcher.getComponent<ClientAutoPatcher<DefAutoPatcher>>();

        co_autopatcher.OnAutoPatcherGetServerVersionCfg =
            () =>
            {
                //UiMbLoading loading = UiMgr.Instance.createUi<UiMbLoading>(_eUiLayer.Loading);
                //loading.setTips("检测版本中");

                EbLog.Note("请求获取版本信息");
            };

        co_autopatcher.OnAutoPatcherGetServerVersionCfgResult =
            (r, error) =>
            {
                //UiMbLoading loading = UiMgr.Instance.createUi<UiMbLoading>(_eUiLayer.Loading);
                //loading.setTips("下载版本信息失败::Error::" + mServerVersionWWW.error);

                if (r == AutoPatcherResult.Success)
                {
                    EbLog.Note("获取版本信息成功");
                }
                else
                {
                    EbLog.Note("获取版本信息失败！ Error=" + error);
                }
            };

        co_autopatcher.OnAutoPatcherIsNeedBundlePatcher =
            (is_need, local_bundle_version, remote_bundle_version) =>
            {
                if (!is_need)
                {
                    //UiMgr.Instance.destroyCurrentUi<UiMbLoading>();
                    EbLog.Note("Bundle版本相同，无需更新");
                }
                else
                {
                    //UiMbLoading loading = UiMgr.Instance.createUi<UiMbLoading>(_eUiLayer.Loading);
                    //loading.setTips("更新版本");
                    //UiMbUpdate update = UiMgr.Instance.createUi<UiMbUpdate>(_eUiLayer.Loading);
                    //update.setUpdateInfo("更新版本," + mVersionConfig.LocalVersionConfig.bundle_version + "版本到" + mVersionConfig.RemoteVersionConfig.bundle_version + "版本,是否更新?", _updateBundle, _cancelUpdate);

                    string info = string.Format("Bundle版本不同，从{0}更新到{1}", local_bundle_version, remote_bundle_version);
                    EbLog.Note(info);
                }
            };

        co_autopatcher.OnAutoPatcherIsNeedDataPatcher =
            (is_need, local_data_version, remote_data_version) =>
            {
                if (!is_need)
                {
                    //UiMgr.Instance.destroyCurrentUi<UiMbLoading>();
                    EbLog.Note("Data版本相同，无需更新");
                }
                else
                {
                    //UiMbLoading loading = UiMgr.Instance.createUi<UiMbLoading>(_eUiLayer.Loading);
                    //loading.setTips("更新数据");
                    //UiMbUpdate update = UiMgr.Instance.createUi<UiMbUpdate>(_eUiLayer.Loading);
                    //update.setUpdateInfo("更新数据," + mVersionConfig.LocalVersionConfig.data_version + "版本到" + mVersionConfig.RemoteVersionConfig.data_version + "版本,是否更新?", _updateData, _cancelUpdate);

                    string info = string.Format("Data版本不同，从{0}更新到{1}", local_data_version, remote_data_version);
                    EbLog.Note(info);
                }
            };

        co_autopatcher.OnAutoPatcherGetRemoteDataFileList =
            () =>
            {
                //UiMgr.Instance.destroyCurrentUi<UiMbUpdate>();
                //UiMbLoading loading = UiMgr.Instance.createUi<UiMbLoading>(_eUiLayer.Loading);
                //loading.setTips("更新数据中");

                EbLog.Note("请求获取数据文件列表");
            };

        co_autopatcher.OnAutoPatcherGetRemoteDataFileListResult =
            (r, error) =>
            {
                //UiMbLoading loading = UiMgr.Instance.createUi<UiMbLoading>(_eUiLayer.Loading);
                //loading.setLoadProgress(mUpdateDataWWW.progress, "更新数据错误 Error::" + mUpdateDataWWW.error);

                if (r == AutoPatcherResult.Success)
                {
                    EbLog.Note("获取数据文件列表成功");
                }
                else
                {
                    EbLog.Note("获取数据文件列表失败！ Error=" + error);
                }
            };


        co_autopatcher.OnAutoPatcherDataPatcher =
            (info) =>
            {
                //UiMbLoading loading = UiMgr.Instance.createUi<UiMbLoading>(_eUiLayer.Loading);
                //loading.setLoadProgress(progress, msg);
                //EbLog.Note(msg + "   " + progress);

                EbLog.Note(info);
            };

        co_autopatcher.OnAutoPatcherFinished =
            () =>
            {
                EbLog.Note("自动更新结束");

                //public OnAutoPatcherFinished OnAutoPatcherFinished { get; set; }
                //CoApp.BundleVersion = mCheckVersion.mVersionConfig.LocalVersionConfig.bundle_version;
                //CoApp.DataVersion = mCheckVersion.mVersionConfig.LocalVersionConfig.data_version;

                //// 创建EtLogin
                //EntityMgr.createEntity<EtLogin>(null, CoApp.Entity);
                EntityMgr.destroyEntity(et_autopatcher);
            };

        co_autopatcher.launchAutoPatcher(autopatcher_cfg);
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("ClientSampleAutoPatcher.release()");
    }

    //-------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
    }

    //-------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }
}
