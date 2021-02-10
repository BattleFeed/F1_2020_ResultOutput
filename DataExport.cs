using System;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using F1_2020_ResultOutput.Models;

namespace F1_2020_ResultOutput
{
    public static class DataExport
    {
        public static void ExportToExcel(MatchResultViewModel vm)
        {
            #region initialize workbook and set font colors
            string templatePath = @"template.xlsx";
            string filePath = $"{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
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

            for(int i = 0; i < 22; i++)
            {
                var player = vm.PlayerResults[i];
                if (player.FinishPosition == 0) { continue; } // driver does not exist

                #region write data to workbook

                IRow row = sheet.GetRow(player.FinishPosition);

                if (player.Name.Length > 0)
                {
                    row.GetCell(1).SetCellValue(player.Name);
                }

                row.GetCell(2).SetCellValue(player.QualiTimeString);
               
                row.GetCell(3).SetCellValue(player.GridPosition);

                int positionGained = player.GridPosition - player.FinishPosition;
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

                row.GetCell(6).SetCellValue(player.TyreStintsString);
                
                row.GetCell(7).SetCellValue(player.FastestLapString);
                //if (item.BestLapTimeSeconds == fastestLapOfTheRace)
                //{
                //    row.GetCell(7).CellStyle.SetFont(purpleFont);
                //}
                
                row.GetCell(8).SetCellValue(player.FinishTimeString);

                row.GetCell(9).SetCellValue(player.PenaltyString);

                row.GetCell(10).SetCellValue(player.Point);
                #endregion
            }

            #region export to Excel file           
            var outputStream = File.Create(filePath);
            workbook.Write(outputStream);
            outputStream.Close();
            #endregion
        }

        public static void ExportToConsole(MatchResultViewModel vm)
        {
            var sortedVM = from player in vm.PlayerResults
                           where player.FinishPosition != 0
                           orderby player.FinishPosition
                           select player;

            Console.WriteLine($"{"Pos.",-8}{"Name",-24}{"Grid",-8}{"Tyres",-10}" +
                    $"{"Best",-16}{"Time",-16}{"Penalty",-8}{"Points",8}");
            foreach (var player in sortedVM)
            {
               Console.WriteLine($"{player.FinishPosition,-8}{player.Name,-24}{player.GridPosition,-8}{player.TyreStintsString,-10}" +
                    $"{player.FastestLapString,-16}{player.FinishTimeString,-16}{player.PenaltyString,-8}{player.Point,8}");
            }
        }
    }
}