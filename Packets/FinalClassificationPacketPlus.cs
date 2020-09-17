using System;
using System.Collections.Generic;
using Codemasters.F1_2020;
using TimHanewich.Toolkit;

namespace F1_2020_ResultOutput
{
    public class FinalClassificationPacketPlus : Packet
    {
        public byte NumberOfCars { get; set; }
        public FinalClassificationData[] FieldClassificationData { get; set; }

        public override void LoadBytes(byte[] bytes)
        {
            ByteArrayManager BAM = new ByteArrayManager(bytes);

            //Get header
            base.LoadBytes(BAM.NextBytes(24));

            //Get number of cars
            NumberOfCars = BAM.NextByte();

            //Get Final Classification Data
            List<FinalClassificationData> fcdata = new List<FinalClassificationData>();
            // int t = 0;
            for (int t = 0; t < 22; t++)
            {
                fcdata.Add(FinalClassificationData.Create(BAM.NextBytes(37)));
            }
            FieldClassificationData = fcdata.ToArray();
        }

        public class FinalClassificationData
        {
            public byte FinishingPosition { get; set; }
            public byte NumberOfLaps { get; set; }
            public byte StartingGridPosition { get; set; }
            public byte PointsScored { get; set; }
            public byte NumberOfPitStops { get; set; }
            public ResultStatus FinalResultStatus { get; set; }
            public float BestLapTimeSeconds { get; set; }
            public double TotalRaceTimeSeconds { get; set; }
            public byte PenaltiesTimeSeconds { get; set; }
            public byte NumberOfTyreStints { get; set; }
            public byte[] TyreStintsActual { get; set; }
            public byte[] TyreStintsVisual { get; set; }

            public static FinalClassificationData Create(byte[] bytes)
            {
                FinalClassificationData ToReturn = new FinalClassificationData();
                ByteArrayManager BAM = new ByteArrayManager(bytes);

                //Finishing position
                ToReturn.FinishingPosition = BAM.NextByte();

                //Number of laps
                ToReturn.NumberOfLaps = BAM.NextByte();

                //Starting grid position
                ToReturn.StartingGridPosition = BAM.NextByte();

                //Points scored
                ToReturn.PointsScored = BAM.NextByte();

                //Number of pit stops
                ToReturn.NumberOfPitStops = BAM.NextByte();

                //Result status
                byte nb = BAM.NextByte();
                if (nb == 0)
                {
                    ToReturn.FinalResultStatus = ResultStatus.Invalid;
                }
                else if (nb == 1)
                {
                    ToReturn.FinalResultStatus = ResultStatus.Inactive;
                }
                else if (nb == 2)
                {
                    ToReturn.FinalResultStatus = ResultStatus.Active;
                }
                else if (nb == 3)
                {
                    ToReturn.FinalResultStatus = ResultStatus.Finished;
                }
                else if (nb == 4)
                {
                    ToReturn.FinalResultStatus = ResultStatus.Disqualified;
                }
                else if (nb == 5)
                {
                    ToReturn.FinalResultStatus = ResultStatus.NotClassified;
                }
                else if (nb == 6)
                {
                    ToReturn.FinalResultStatus = ResultStatus.Retired;
                }

                //Best lap time in seconds
                ToReturn.BestLapTimeSeconds = BitConverter.ToSingle(BAM.NextBytes(4), 0);

                //Total race time in seconds
                ToReturn.TotalRaceTimeSeconds = BitConverter.ToDouble(BAM.NextBytes(8), 0);

                //Penalties time in seconds
                ToReturn.PenaltiesTimeSeconds = BAM.NextByte();

                //Number of tyre stints
                ToReturn.NumberOfTyreStints = BAM.NextByte();

                // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1
                // 7 = inter, 8 = wet
                // F1 Classic - 9 = dry, 10 = wet
                // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard
                // 15 = wet
                ToReturn.TyreStintsActual = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    ToReturn.TyreStintsActual[i] = BAM.NextByte();
                }

                // F1 visual (can be different from actual compound)
                // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet﻿
                // F1 Classic – same as above
                // F2 – same as above
                ToReturn.TyreStintsVisual = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    ToReturn.TyreStintsVisual[i] = BAM.NextByte();
                }

                return ToReturn;
            }
        }

        public enum ResultStatus
        {
            Invalid,
            Inactive,
            Active,
            Finished,
            Disqualified,
            NotClassified,
            Retired
        }
    }
}