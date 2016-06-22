using System;
using System.Collections.Generic;
using GF.Unity.Common;

public class EtNode : EntityDef
{
    //---------------------------------------------------------------------
    public override void declareAllComponent(byte node_type)
    {
        declareComponent<DefNode>();
    }
}
