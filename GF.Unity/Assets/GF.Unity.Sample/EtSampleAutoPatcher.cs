using System;
using System.Collections.Generic;
using GF.Common;

public class EtSampleAutoPatcher : EntityDef
{
    //-------------------------------------------------------------------------
    public override void declareAllComponent(byte node_type)
    {
        declareComponent<DefSampleAutoPatcher>();
    }
}
