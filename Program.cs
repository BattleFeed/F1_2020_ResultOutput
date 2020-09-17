using System;
using System.Collections;
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
        // var names = new ArrayList(); // -----

        try
        {
            Console.WriteLine("Waiting for broadcast");
            while (true)
            {
                byte[] bytes = listener.Receive(ref groupEP);
                p.LoadBytes(bytes);      
                
                // bool resultReceived = false;
                // bool namesInitialized = false;
                if (p.PacketType == PacketType.FinalClassification) // Match is finished
                {
                    // resultReceived = true;
                    FinalClassificationOutput.OutputResult(bytes);
                }
                //else if (p.PacketType == PacketType.Participants && !namesInitialized)
                //{
                //    namesInitialized = true;
                //    var pp = new ParticipantPacket();
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