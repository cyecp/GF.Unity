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
    public IRpcSession Session { get { return mSession; } }

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

    //---------------------------------------------------------------------
    public void rpc(short method_id)
    {
        rpcBySession(Session, (ushort)method_id);
    }

    //---------------------------------------------------------------------
    public void rpc<T1>(short method_id, T1 obj1)
    {
        rpcBySession(Session, (ushort)method_id, obj1);
    }

    //---------------------------------------------------------------------
    public void rpc<T1, T2>(short method_id, T1 obj1, T2 obj2)
    {
        rpcBySession(Session, (ushort)method_id, obj1, obj2);
    }

    //---------------------------------------------------------------------
    public void rpc<T1, T2, T3>(short method_id, T1 obj1, T2 obj2, T3 obj3)
    {
        rpcBySession(Session, (ushort)method_id, obj1, obj2, obj3);
    }

    //---------------------------------------------------------------------
    public void rpc<T1, T2, T3, T4>(short method_id, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
    {
        rpcBySession(Session, (ushort)method_id, obj1, obj2, obj3, obj4);
    }

    //-------------------------------------------------------------------------
    void _onSocketConnected(object client, EventArgs args)
    {
        EbLog.Note("ClientSampleNetwork._onSocketConnected()");

        //rpc(MethodType.c2sAccountRequest, account_request);

        rpc(999);

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
