using System;
using System.Collections.Generic;
using System.Text;
using Codemasters.F1_2020;

namespace F1_2020_ResultOutput
{
    class FinalClassificationDataComparer : IComparer<FinalClassificationPacket.FinalClassificationData>
    {
        public int Compare(FinalClassificationPacket.FinalClassificationData d1, FinalClassificationPacket.FinalClassificationData d2)
        {
            if (d1 == null || d2 == null) throw new ArgumentNullException("argument error.");
            return d1.FinishingPosition.CompareTo(d2.FinishingPosition); //sort rule
        }
    }
}