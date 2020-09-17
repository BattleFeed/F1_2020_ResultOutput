using System;
using System.Collections.Generic;
using System.Text;
using Codemasters.F1_2020;
using F1_2020_ResultOutput;

namespace F1_2020_ResultOutput
{
    class FinalClassificationDataComparer : IComparer<FinalClassificationPacketPlus.FinalClassificationData>
    {
        public int Compare(FinalClassificationPacketPlus.FinalClassificationData d1, FinalClassificationPacketPlus.FinalClassificationData d2)
        {
            if (d1 == null || d2 == null) throw new ArgumentNullException("argument error.");
            return d1.FinishingPosition.CompareTo(d2.FinishingPosition); //sort rule
        }
    }
}