using System;
using System.Net;
using System.Net.Sockets;
using Codemasters.F1_2020;
using F1_2020_ResultOutput;

public class Program
{
    private const int listenPort = 20777;
    private const string ipString = "127.0.0.1";

    public static void Main()
    {
        StartListener();
    }

    private static void StartListener()
    {
        UdpClient listener = new UdpClient(listenPort);
        IPAddress ip = IPAddress.Parse(ipString);
        IPEndPoint groupEP = new IPEndPoint(ip, listenPort);

        var p = new Packet();
        var fcp = new FinalClassificationPacket();
        // var pp = new ParticipantPacket();
        var sp = new SessionPacket();

        //string[] names = new string[22];
        //for (int i = 0; i < 22; i++)
        //{
        //    names[i] = "_unnamed_";
        //}
        double[] qualiTimes = new double[22];

        // int pTick = -1;
        int sTick = -1;
        bool qualiStated = false;
        bool raceStated = false;               
        bool isRacing = true;

        try
        {
            Console.WriteLine("Waiting for broadcast");
            
            while (true)
            {
                byte[] bytes = listener.Receive(ref groupEP);
                p.LoadBytes(bytes);
                if (p.PacketType == PacketType.FinalClassification)// once Match/Quali is finished
                {
                    if (isRacing && !raceStated)
                    {
                        raceStated = true;
                        FinalClassificationOutput.OutputResult(bytes, qualiTimes);
                        Console.WriteLine("Excel file exported");
                    }
                    else if (!qualiStated )
                    {
                        qualiStated = true;
                        fcp.LoadBytes(bytes);
                        for (int i = 0; i < fcp.FieldClassificationData.Length; i++)
                        {
                            qualiTimes[i] = fcp.FieldClassificationData[i].BestLapTimeSeconds;
                        }
                        Console.WriteLine("Qualifying Time stated");
                    }
                }
                //else if (p.PacketType == PacketType.Participants) // Every 5 seconds
                //{
                //    pTick = (pTick + 1) % 6;
                //    if (pTick != 0) { continue; } // check once per 30s
                //    pp.LoadBytes(bytes);
                //    for (int i = 0; i < pp.FieldParticipantData.Length; i++)
                //    {
                //        names[i] = pp.FieldParticipantData[i];
                //        // Console.WriteLine($"The driver #{i} is {data[i].Name}");
                //    }
                //    Console.WriteLine("Name refreshed");
                //}
                // does NOT deal with Q1/Q2/Q3
                else if(p.PacketType == PacketType.Session) // 2 per second
                {
                    sTick = (sTick + 1) % 60;
                    if (sTick != 0) { continue; } // check once per 30s
                    sp.LoadBytes(bytes);
                    if (sp.SessionTypeMode == SessionPacket.SessionType.ShortQualifying || sp.SessionTypeMode == SessionPacket.SessionType.OneShotQualifying)
                    {
                        isRacing = false;
                        Console.WriteLine("Current session: qualifying");
                    }
                    else if (sp.SessionTypeMode == SessionPacket.SessionType.Race || sp.SessionTypeMode == SessionPacket.SessionType.Race2)
                    {
                        isRacing = true;
                        Console.WriteLine("Current session: race");
                    }
                    
                }

                // Console.WriteLine($"Received broadcast from {groupEP} :");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            listener.Close();
        }
    }
}