using System;
using System.Net.Sockets;

namespace GameClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TcpClientTool client = new TcpClientTool();
            client.Start();

        }
    }
}

