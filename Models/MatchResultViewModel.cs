using Codemasters.F1_2020;

namespace F1_2020_ResultOutput.Models
{
    public class MatchResultViewModel
    {
        public PlayerResult[] PlayerResults { get; set; }
        public FinalClassificationPacketPlus FCP { get; set; }
        public ParticipantPacket PP { get; set; }

        public double RaceTimeSeconds { get; set; }
        public int TotalLaps { get; set; }
        public int NumberOfPlayers { get; set; }

        public MatchResultViewModel()
        {
            PlayerResults = new PlayerResult[22];
            for (int i = 0; i < 22; i++)
            {
                PlayerResults[i] = new PlayerResult();
            }
            FCP = new FinalClassificationPacketPlus(); // replaced with new packet class
            PP = new ParticipantPacket();
        }

        public void LoadQualiData(byte[] bytes)
        {
            FCP.LoadBytes(bytes);
            for (int i = 0; i < 22; i++)
            {
                var time = FCP.FieldClassificationData[i].BestLapTimeSeconds;
                if (time > 1)
                    PlayerResults[i].QualiTimeString = StringConverter.DoubleToStringTime(time);               
            }
        }

        public void LoadRaceData(byte[] bytes)
        {
            FCP.LoadBytes(bytes);
            var data = FCP.FieldClassificationData;

            for (int i = 0; i < 22; i++)
            {
                if (data[i].FinishingPosition == 1)
                {
                    RaceTimeSeconds = data[i].TotalRaceTimeSeconds + data[i].PenaltiesTimeSeconds;
                    TotalLaps = data[i].NumberOfLaps;
                    break;
                }
            }
            NumberOfPlayers = FCP.NumberOfCars;

            for (int i = 0; i < 22; i++)
            {
                var item = data[i];
                var player = PlayerResults[i];
                if (item.FinishingPosition == 0) { continue; } // driver does not exist               

                // player.Name = "_unnamed_";

                player.GridPosition = item.StartingGridPosition;

                player.FinishPosition = item.FinishingPosition;

                #region deal with TyreStintsString
                foreach (var tyre in item.TyreStintsVisual)
                {
                    switch (tyre)
                    {
                        case 16:
                            player.TyreStintsString += "S";
                            break;
                        case 17:
                            player.TyreStintsString += "M";
                            break;
                        case 18:
                            player.TyreStintsString += "H";
                            break;
                        case 7:
                            player.TyreStintsString += "I";
                            break;
                        case 8:
                            player.TyreStintsString += "W";
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                if (item.BestLapTimeSeconds > 1)
                    player.FastestLapString = StringConverter.DoubleToStringTime(item.BestLapTimeSeconds);

                #region deal with FinishTimeString
                // Finished = 3 DNF = 4
                if ((int)item.FinalResultStatus == 4)
                {
                    player.FinishTimeString = "DNF";
                }
                else
                {
                    if (item.NumberOfLaps < TotalLaps)
                    {
                        int delta = TotalLaps - item.NumberOfLaps;
                        player.FinishTimeString = $"+{delta} Lap{(delta > 1 ? "s" : "")}";
                    }
                    else if (item.FinishingPosition == 1)
                    {
                        player.FinishTimeString = StringConverter.DoubleToStringTime(RaceTimeSeconds);
                    }
                    else
                    {
                        if (item.TotalRaceTimeSeconds > 1)
                            player.FinishTimeString = $"+{StringConverter.DoubleToStringTime(item.TotalRaceTimeSeconds + item.PenaltiesTimeSeconds - RaceTimeSeconds)}";
                        else
                            player.FinishTimeString = "DNS";
                    }
                }
                #endregion

                if (item.PenaltiesTimeSeconds > 0)
                    player.PenaltyString = $"+{item.PenaltiesTimeSeconds} sec.";

                player.Point = item.PointsScored;
            }
        }

        public void LoadParticipantData(byte[] bytes)
        {
            PP.LoadBytes(bytes);
            for (int i = 0; i < 22; i++)
            {
                PlayerResults[i].Name = PP.FieldParticipantData[i].Name.Replace("\0","");
            }
        }

        public class PlayerResult
        {
            public string Name { get; set; }
            public string QualiTimeString { get; set; }
            public int GridPosition { get; set; }
            public int FinishPosition { get; set; }
            public string TyreStintsString { get; set; }
            public string FastestLapString { get; set; }
            public string FinishTimeString { get; set; }
            public string PenaltyString { get; set; }
            public int Point { get; set; }

            public PlayerResult()
            {
                Name = "";
                QualiTimeString = "";
                TyreStintsString = "";
                FastestLapString = "";
                FinishTimeString = "";
                PenaltyString = "";
            }
        }
    }
}