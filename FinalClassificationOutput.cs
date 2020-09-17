using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace F1_2020_ResultOutput
{
    class FinalClassificationOutput
    {
        public static void OutputResult(byte[] bytes)
        {
            string templatePath = @"template.xlsx";
            string filePath = DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
            // var fcp = new FinalClassificationPacket();
            var fcp = new FinalClassificationPacketPlus();
            fcp.LoadBytes(bytes);
            var driverAmount = fcp.NumberOfCars;
            var data = fcp.FieldClassificationData;
            // Array.Sort(data, new FinalClassificationDataComparer()); // sort the array (winner to last)
            

            #region initialize workbook and set font colors
            var stream = File.OpenRead(templatePath);
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);
            stream.Close();

            //var whiteFont = workbook.CreateFont();
            //whiteFont.FontName = "Segoe UI Symbol";
            //whiteFont.FontHeightInPoints = 11;
            //whiteFont.Color = 0;

            //var redFont = workbook.CreateFont();
            //redFont.FontName = "Segoe UI Symbol";
            //redFont.FontHeightInPoints = 11;
            //redFont.Color = 2;

            //var greenFont = workbook.CreateFont();
            //greenFont.FontName = "Segoe UI Symbol";
            //greenFont.FontHeightInPoints = 11;
            //greenFont.Color = 3;

            //var purpleFont = workbook.CreateFont();
            //purpleFont.FontName = "Titillium Web SemiBold";
            //purpleFont.FontHeightInPoints = 11;
            //purpleFont.Color = 20;
            #endregion

            // float fastestLapOfTheRace = 0;
            double winnerTime = 0; // winner's total time
            int totalLaps = 0; // match total laps
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].FinishingPosition == 1)
                {
                    // fastestLapOfTheRace = data[i].BestLapTimeSeconds;                  
                    winnerTime = data[i].TotalRaceTimeSeconds;
                    totalLaps = data[i].NumberOfLaps;
                }
            }

            Console.WriteLine($"Number of Cars : {driverAmount}");
            //Console.WriteLine("Pos.\tGrid\tBest\t\tTime\t\tPenalty\tPoints");

            foreach (var item in data)
            {
                if (item.FinishingPosition == 0) { continue; } // driver does not exist

                #region deal with bestLapTimeString
                string bestLapTime;
                if (item.BestLapTimeSeconds < 1) // abnormal best lap
                {
                    bestLapTime = "-";
                }
                else
                {
                    bestLapTime = StringConverter.FloatToStringTime(item.BestLapTimeSeconds);
                }
                #endregion

                #region deal with raceTimeString
                string raceTime;
                if (item.FinalResultStatus == FinalClassificationPacketPlus.ResultStatus.Disqualified)
                {
                    raceTime = "DSQ";
                }
                else if (item.FinalResultStatus == FinalClassificationPacketPlus.ResultStatus.Retired)
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
                    }
                    else
                    {
                        raceTime = "+" + StringConverter.DoubleToStringTime(item.TotalRaceTimeSeconds + item.PenaltiesTimeSeconds - winnerTime);
                    }
                }
                #endregion

                #region deal with visual tyre stints
                string tyreStints = "";
                foreach (var tyre in item.TyreStintsVisual)
                {
                    switch (tyre)
                    {
                        case 16:
                            tyreStints += "S";
                            break;
                        case 17:
                            tyreStints += "M";
                            break;
                        case 18:
                            tyreStints += "H";
                            break;
                        case 7:
                            tyreStints += "I";
                            break;
                        case 8:
                            tyreStints += "W";
                            break;
                        default:
                            break;
                    }
                }
                #endregion

              
                //Console.WriteLine($"{item.FinishingPosition}\t{item.StartingGridPosition}\t{bestLapTime.PadRight(16)}" +
                //    $"{raceTime.PadRight(16)}{item.PenaltiesTimeSeconds}\t{item.PointsScored}");

                IRow row = sheet.GetRow(item.FinishingPosition);

                row.GetCell(3).SetCellValue(item.StartingGridPosition);

                int positionGained = item.StartingGridPosition - item.FinishingPosition;
                var cellGrid = row.GetCell(4);
                var cellPosGained = row.GetCell(5);
                if (positionGained > 0)
                {
                    cellGrid.SetCellValue("▲");
                    cellPosGained.SetCellValue(positionGained);
                }
                else if (positionGained < 0)
                {
                    cellGrid.SetCellValue("▼");                                      
                    cellPosGained.SetCellValue(-positionGained);
                }
                else
                {
                    cellPosGained.SetCellValue(0);
                }

                row.GetCell(6).SetCellValue(tyreStints);
                row.GetCell(7).SetCellValue(bestLapTime);
                //if (item.BestLapTimeSeconds == fastestLapOfTheRace)
                //{
                //    row.GetCell(7).CellStyle.SetFont(purpleFont);
                //}
                
                row.GetCell(8).SetCellValue(raceTime);
                if (item.PenaltiesTimeSeconds > 0)
                {
                    row.GetCell(9).SetCellValue($"+{ item.PenaltiesTimeSeconds.ToString()}sec");
                }
                row.GetCell(10).SetCellValue(item.PointsScored);
            }

            var outputStream = File.Create(filePath);
            workbook.Write(outputStream);
            outputStream.Close();       
        }
    }
}