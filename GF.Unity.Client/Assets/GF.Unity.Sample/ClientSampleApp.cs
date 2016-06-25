using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GF.Unity.Common;

public class ClientSampleApp<TDef> : Component<TDef> where TDef : DefSampleApp, new()
{
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientSampleApp.init()");

        EntityMgr.getDefaultEventPublisher().addHandler(Entity);

        // AutoPatcher示例
        //EntityMgr.createEntity<EtSampleAutoPatcher>(null, Entity);

        // Network示例
        EntityMgr.createEntity<EtSampleNetwork>(null, Entity);
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("ClientSampleApp.release()");
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
