using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GF.Unity.Common;

public class ClientSampleNetwork<TDef> : Component<TDef> where TDef : DefSampleNetwork, new()
{
    //-------------------------------------------------------------------------
    EntityRpcSessionSuperSocketC mSession;

    //-------------------------------------------------------------------------
    public RpcSession Session { get { return mSession; } }

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientSampleNetwork.init()");

        EntityMgr.getDefaultEventPublisher().addHandler(Entity);

        connectBase("192.168.0.10", 5882);
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        disconnect();

        EbLog.Note("ClientSampleNetwork.release()");
    }

    //-------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
        if (mSession != null)
        {
            mSession.update(elapsed_tm);
        }
    }

    //-------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }

    //-------------------------------------------------------------------------
    public void connectBase(string base_ip, int base_port)
    {
        if (mSession != null)
        {
            mSession.close();
            mSession = null;
        }

        mSession = new EntityRpcSessionSuperSocketC(EntityMgr);
        mSession.OnSocketConnected = _onSocketConnected;
        mSession.OnSocketClosed = _onSocketClosed;
        mSession.OnSocketError = _onSocketError;
        mSession.connect(base_ip, base_port);
    }

    //-------------------------------------------------------------------------
    public void disconnect()
    {
        EbLog.Note("ClientSampleNetwork.disconnect()");

        if (mSession != null)
        {
            mSession.close();
            mSession = null;
        }
    }
    
    //-------------------------------------------------------------------------
    void _onSocketConnected(object client, EventArgs args)
    {
        EbLog.Note("ClientSampleNetwork._onSocketConnected()");

        //rpc(MethodType.c2sAccountRequest, account_request);

        //rpc(999);

        //byte[] data = Encoding.UTF8.GetBytes("Hello world");
        //if (session != null) session.send(method_id, data);
    }

    //-------------------------------------------------------------------------
    void _onSocketClosed(object client, EventArgs args)
    {
        EbLog.Note("ClientSampleNetwork._onSocketClosed()");

        _onSocketClose();
    }

    //-------------------------------------------------------------------------
    void _onSocketError(object rec, SocketErrorEventArgs args)
    {
        EbLog.Note("ClientSampleNetwork._onSocketError()");

        _onSocketClose();
    }

    //-------------------------------------------------------------------------
    void _onSocketClose()
    {
        mSession = null;
    }
}
