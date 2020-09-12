using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using Codemasters.F1_2020;
using DataTransform;

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
        // var names = new ArrayList(); // -----

        try
        {
            Console.WriteLine("Waiting for broadcast");
            while (true)
            {
                byte[] bytes = listener.Receive(ref groupEP);
                p.LoadBytes(bytes);
                // bool namesInitialized = false;
                if (p.PacketType == PacketType.FinalClassification) //todo: 未完赛/套圈处理/把完赛时间修改为delta
                {
                    fcp.LoadBytes(bytes);
                    Console.WriteLine($"Number of Cars : {fcp.NumberOfCars}");
                    Console.WriteLine("Pos.\tGrid\tBest\t\tTime\t\tPenalty\tPoints");

                    var data = fcp.FieldClassificationData;
                    Array.Sort(data, new FinalClassificationDataComparer());
                    foreach (var item in data)
                    {
                        if (item.FinishingPosition == 0) { continue; }
                        Console.WriteLine($"{item.FinishingPosition}\t{item.StartingGridPosition}\t{StringConverter.FloatToStringTime(item.BestLapTimeSeconds)}\t" +
                            $"{StringConverter.DoubleToStringTime(item.TotalRaceTimeSeconds)}\t{item.PenaltiesTimeSeconds}\t{item.PointsScored}");
                    }
                }
                //else if (p.PacketType == PacketType.Participants && !namesInitialized)
                //{
                //    namesInitialized = true;
                //    pp.LoadBytes(bytes);
                //    var data = pp.FieldParticipantData;
                //    var l = data.Length;
                //    for (int i = 0; i < l; i++)
                //    {
                //        names.Add(data[i].Name);
                //        Console.WriteLine($"The driver #{i} is {data[i].Name}"); //--------------
                //    }                   
                //} 
                
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