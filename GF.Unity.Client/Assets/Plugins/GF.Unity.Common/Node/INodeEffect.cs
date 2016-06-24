using System;
using System.Collections.Generic;
using System.Text;
using EventDataXML;
using GF.Unity.Common;

public interface INodeEffect
{
    //-------------------------------------------------------------------------
    string getId();

    //-------------------------------------------------------------------------
    void setEntity(CNode node);

    //-------------------------------------------------------------------------
    void excute(Group xml_group);
}