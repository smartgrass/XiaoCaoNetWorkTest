﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

class GameSerever
{
    private static TcpListener listener;
    private static ManualResetEvent allDone = new ManualResetEvent(false);

    private static Dictionary<int, PlayerClient> playerClientDic = new Dictionary<int, PlayerClient>();
    private static List<PlayerClient> playerClients = new List<PlayerClient>();

    static void Main(string[] args)
    {

        beginProtocbuf();
    }

    private static void beginProtocbuf()
    {
        //启动服务端
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7777);


        listener.Start();
        //回调
        BeginAccept();
        Console.WriteLine("SERVER : 等待数据 ---");

        while (true)
        {

            
        }

        Console.WriteLine("SERVER : 退出 ---");
        // server.Stop();
    }

    private static void BeginAccept()
    {
        listener.BeginAcceptTcpClient(clientConnected, listener);
    }

    //服务端处理
    private static void clientConnected(IAsyncResult result)
    {
        try
        {
            TcpListener server = (TcpListener)result.AsyncState;
            Console.WriteLine(" AcceptSocket");

            using (TcpClient client = server.EndAcceptTcpClient(result))
            {
                BeginAccept();
                using (NetworkStream stream = client.GetStream())
                {
                    //获取
                    Console.WriteLine("SERVER : 客户端已连接，数据读取中 --- ");
                    byte[] recvData;

                    while (client.Connected)
                    {
                        recvData = new byte[1024];
                        int myRequestLength = 0;
                        do
                        {
                            Console.WriteLine("yns  wait recvData....");
                            myRequestLength = stream.Read(recvData, 0, recvData.Length);
                            Console.WriteLine("read len " + myRequestLength);
                        }
                        while (stream.DataAvailable);
                        if (myRequestLength == 0) CloseClient(client);

                        recvData = ReciveBytes(client, recvData, myRequestLength);
                    }
                    Console.WriteLine("===========================================");

                    Console.WriteLine("SERVER : 响应成功 ---");

                    Console.WriteLine("SERVER: 关闭连接 ---");
                    stream.Close();
                }
                client.Close();
            }
        }
        finally
        {
            allDone.Set();
        }
    }

    private static void CloseClient(TcpClient client)
    {
        client.Close();
    }


    private static async void SendPlayerList()
    {
        while (true)
        {
            await Task.Delay(20);

            

            //CallAllClient();
        }
    }




    private static byte[] ReciveBytes(TcpClient client, byte[] recvData, int myRequestLength)
    {
        if (recvData != null)
        {
            recvData = recvData.RemoveEmptyByte(myRequestLength);
            BaseMsg baseMsg = BaseMsg.Parser.ParseFrom(recvData);

            if (baseMsg.MsgTypeEnum == (int)MsgTypeEnum.Pos)
            {
                //转发消息给其他客户端
                CallAllClient(baseMsg);

            }
            else if (baseMsg.MsgTypeEnum == (int)MsgTypeEnum.Login)
            {
                CSLoginInfo loginInfo = CSLoginInfo.Parser.ParseFrom(baseMsg.ContextBytes);
                AddConectClient(new PlayerClient(client, loginInfo.UserName, loginInfo.PlayerID));
                Console.WriteLine($"yns {loginInfo.PlayerID} conected....");
                //转发消息给其他客户端
                CallOtherClient(baseMsg);
            }
        }
        return recvData;
    }

    private static void AddConectClient(PlayerClient tcpClient)
    {
        if (playerClientDic.ContainsKey(tcpClient.playerID))
        {
            Console.WriteLine($"Error {tcpClient.playerID} exsist");
            return;
        }
        playerClientDic.Add(tcpClient.playerID, tcpClient);
        playerClients.Add(tcpClient);
    }

    private static void CallAllClient(BaseMsg message)
    {
        int length = playerClients.Count;
        for (int i = 0; i < length; i++)
        {
            playerClients[i].SendMsg(message);
        }
    }
    private static void CallOtherClient(BaseMsg message, int selfID)
    {
        int length = playerClients.Count;
        for (int i = 0; i < length; i++)
        {
            if (playerClients[i].playerID != selfID)
            {
                playerClients[i].SendMsg(message);
            }
        }
    }
    private static void ReplyClient(BaseMsg message, int selfID)
    {
        playerClientDic[selfID].SendMsg(message);
    }


}//end class


public class PlayerClient
{
    public PlayerClient() { }
    public PlayerClient(TcpClient _client, string _playerName, int _playerID)
    {
        client = _client;
        playerName = _playerName;
        playerID = _playerID;
    }
    public void SendMsg(BaseMsg baseMsg)
    {
        byte[] bytes = baseMsg.ToByteArray();
        client.GetStream().Write(bytes);
    }
    public string playerName = "";
    public TcpClient client;
    public int playerID;
    public Vector3 pos;
    public int ground;

}
public static class ExtensionClass
{
    public static byte[] RemoveEmptyByte(this byte[] by, int length)
    {
        byte[] returnByte = new byte[length];

        for (int i = 0; i < length; i++)
        {
            returnByte[i] = by[i];
        }
        return returnByte;

    }
}
