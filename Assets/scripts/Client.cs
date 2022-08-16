using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class Client : MonoBehaviour
{

    static Socket sck;
   

    public String SendData(string data_str)
    {
       
        int lng = data_str.Length;
        lng += 5;
        string text =  "" + lng + "." + data_str;
        //print(text);
        byte[] data = Encoding.ASCII.GetBytes(text);
        sck.Send(data);
        byte[] buffer = new byte[1024];

        int receive = sck.Receive(buffer);

        return Encoding.Default.GetString(buffer);
    }
    public void SendStatus(int env_number, bool status)
    {
        string text;
        if (status)
        {
            text = "1";
        }
        else
        {
            text = "0";
        }
        byte[] data = Encoding.ASCII.GetBytes(text);
        //print("before send status data");
        sck.Send(data);
    }

    public void connect_to_server()
    {
        sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.100.11"), 9090);
        try
        {
            sck.Connect(localEndPoint);
        }
        catch
        {
            Console.Write("Unable to connect to remote end point!\r\n");
        }
    }
    private void Start()
    {
        
    }

}
