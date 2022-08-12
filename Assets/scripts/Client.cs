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
    private int connections = 0;
    static int env_batch_size = 10;

    public void SendData()
    {
        if (connections == 0)
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
        connections = 1;

        for (int i = 0; i < env_batch_size; i++)
        {
            string text = "data for env " + i;
            byte[] data = Encoding.ASCII.GetBytes(text);
            sck.Send(data);

            byte[] buffer = new byte[255];
            int receive = sck.Receive(buffer);
            print("Received action for" + Encoding.Default.GetString(buffer));
        }

        // taking actions
        // returning positive or negative response

    }
 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
