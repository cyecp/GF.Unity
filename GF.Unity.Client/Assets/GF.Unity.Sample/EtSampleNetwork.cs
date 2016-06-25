using System;
using System.Collections.Generic;
using GF.Unity.Common;

public class EtSampleNetwork : EntityDef
{
    //-------------------------------------------------------------------------
    public override void declareAllComponent(byte node_type)
    {
        declareComponent<DefSampleNetwork>();
    }
}
