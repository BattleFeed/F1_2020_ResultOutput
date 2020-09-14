using System;
using System.Collections.Generic;
using Codemasters.F1_2020;

namespace F1_2020_ResultOutput
{
    class FinalClassificationOutput
    {
        public static void OutputResult(byte[] bytes)
        {
            var fcp = new FinalClassificationPacket();
            fcp.LoadBytes(bytes);
            Console.WriteLine($"Number of Cars : {fcp.NumberOfCars}");
            Console.WriteLine("Pos.\tGrid\tBest\t\tTime\t\tPenalty\tPoints");

            var data = fcp.FieldClassificationData;
            Array.Sort(data, new FinalClassificationDataComparer()); // sort the array (winner to last)
            double winnerTime = 0; // winner's total time
            int totalLaps = 0; // match total laps
            foreach (var item in data)
            {
                if (item.FinishingPosition == 0) { continue; } // driver does not exist

                string bestLapTime;
                if (item.BestLapTimeSeconds < 1) // abnormal best lap
                {
                    bestLapTime = "-";
                }
                else
                {
                    bestLapTime = StringConverter.FloatToStringTime(item.BestLapTimeSeconds);
                }

                string raceTime;
                if (item.FinalResultStatus == FinalClassificationPacket.ResultStatus.Disqualified)
                {
                    raceTime = "DSQ";
                }
                else if (item.FinalResultStatus == FinalClassificationPacket.ResultStatus.Retired)
                {
                    raceTime = "DNF";
                }
                else
                {
                    if (item.NumberOfLaps < totalLaps)
                    {
                        int delta = totalLaps - item.NumberOfLaps;
                        raceTime = $"+{delta} Lap" + (delta > 1 ? "s" : "");
                    }
                    else if (item.FinishingPosition == 1)
                    {
                        winnerTime = item.TotalRaceTimeSeconds + item.PenaltiesTimeSeconds;
                        raceTime = StringConverter.DoubleToStringTime(winnerTime);
                        totalLaps = item.NumberOfLaps;
                    }
                    else
                    {
                        raceTime = "+" + StringConverter.DoubleToStringTime(item.TotalRaceTimeSeconds + item.PenaltiesTimeSeconds - winnerTime);
                    }
                }

                Console.WriteLine($"{item.FinishingPosition}\t{item.StartingGridPosition}\t{bestLapTime.PadRight(16)}" +
                    $"{raceTime.PadRight(16)}{item.PenaltiesTimeSeconds}\t{item.PointsScored}");
            }
        }
    }
}
