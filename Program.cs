using System;
using System.Net;
using System.Net.Sockets;
using Codemasters.F1_2020;
using F1_2020_ResultOutput;
using F1_2020_ResultOutput.Models;

public class Program
{
    private const int listenPort = 20777;
    private const string ipString = "127.0.0.1";

    public static MatchResultViewModel ResultVM { get; set; }

    public static void Main()
    {
        StartListener();
    }

    private static void StartListener()
    {
        UdpClient listener = new UdpClient(listenPort);
        IPAddress ip = IPAddress.Parse(ipString);
        IPEndPoint groupEP = new IPEndPoint(ip, listenPort);

        ResultVM = new MatchResultViewModel();

        var p = new Packet();              
        var sp = new SessionPacket();
              
        int sTick = -1;
        int pTick = -1;        
        bool isQualiStated = false;
        bool isRaceStated = false;
        bool isPlayerStated = false;
        bool isRacing = true;
        
        try
        {
            Console.WriteLine($"{DateTime.Now:t} Waiting for broadcast on {ipString}:{listenPort}");
            
            while (true)
            {
                byte[] bytes = listener.Receive(ref groupEP);
                p.LoadBytes(bytes);

                if (p.PacketType == PacketType.FinalClassification) // once when Qualifying/Race is finished
                {
                    if (!isRacing && !isQualiStated)
                    {
                        isQualiStated = true;

                        ResultVM.LoadQualiData(bytes);

                        Console.WriteLine($"{DateTime.Now:t} Qualifying time has been recorded.");
                    }
                    else if (isRacing && !isRaceStated)
                    {
                        isRaceStated = true;

                        ResultVM.LoadRaceData(bytes);
                        Console.WriteLine($"{DateTime.Now:t} Players in race : {ResultVM.NumberOfPlayers}");

                        DataExport.ExportToExcel(ResultVM);
                        DataExport.ExportToConsole(ResultVM);

                        Console.WriteLine($"{DateTime.Now:t} Race result has been exported to Excel file.");
                    }                    
                }                
                else if(p.PacketType == PacketType.Session) 
                {
                    sTick = (sTick + 1) % 60; // 2 times per sec; check once per 30s
                    if (sTick != 0) { continue; }
                    sp.LoadBytes(bytes);

                    // SessionType.ShortQualifying actually = 7  ???
                    if ((int)sp.SessionTypeMode == 7 || sp.SessionTypeMode == SessionPacket.SessionType.OneShotQualifying)
                    {
                        if (isRacing) // First detection of quali session
                        {
                            Console.WriteLine($"{DateTime.Now:t} Current session : Qualifying");
                            isRacing = false;
                        }
                    }
                    else if (sp.SessionTypeMode == SessionPacket.SessionType.Race || sp.SessionTypeMode == SessionPacket.SessionType.Race2)
                    {
                        if (!isRacing) // First detection of race session
                        {
                            Console.WriteLine($"{DateTime.Now:t} Current session : Race");
                            isRacing = true;
                        }
                    }                   
                }
                #region refresh name (enabled)
                else if (p.PacketType == PacketType.Participants) 
                {
                    pTick = (pTick + 1) % 6; // once every 5s; check once per 30s
                    if (pTick != 0) { continue; }

                    ResultVM.LoadParticipantData(bytes);
                    if (!isPlayerStated)
                    {
                        Console.WriteLine($"{DateTime.Now:t} Player names have been recorded.");
                        isPlayerStated = true;
                    }
                }
                #endregion

                // Console.WriteLine($"Received broadcast from {groupEP} :");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"{DateTime.Now:T} {e}");
        }
        finally
        {
            listener.Close();
        }
    }
}