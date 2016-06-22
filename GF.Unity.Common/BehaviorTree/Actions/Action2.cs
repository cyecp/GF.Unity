using System;
using System.Collections.Generic;
using System.Text;

namespace GF.Unity.Common
{
    public class Action2 : BehaviorComponent
    {
        //---------------------------------------------------------------------
        public Action2(BehaviorTree bt)
            : base(bt)
        {
        }

        //---------------------------------------------------------------------
        public override BehaviorReturnCode Behave()
        {
            return BehaviorReturnCode.Success;
        }
    }
}
