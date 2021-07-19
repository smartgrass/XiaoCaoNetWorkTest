using System;
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

    private static AllPosMsg allPosMsg = new AllPosMsg();
    private static DateTime setUpTime;

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
        setUpTime = DateTime.Now;
        Console.WriteLine("SERVER : 等待数据 ---");

        SendPlayerList();

        while (true)
        {
            Thread.Sleep(100);
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
                    Console.WriteLine("客户端已连接 : " + client.Client.RemoteEndPoint);
                    byte[] recvData;

                    while (client.Connected)
                    {
                        recvData = new byte[1024];
                        int myRequestLength = 0;
                        do
                        {
                            myRequestLength = stream.Read(recvData, 0, recvData.Length);
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
            await Task.Delay(200);
            long time = DateTime.Now.Ticks;
            int _len = allPosMsg.PosPlayerMsgList.Count;
            for (int i = 0; i < _len; i++)
            {
                var item = allPosMsg.PosPlayerMsgList[i].SendTime = time;
            }
            CallAllClient(MakeBaseMsg(allPosMsg,MsgTypeEnum.Allplayer));
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
                PosPlayerMsg msg = PosPlayerMsg.Parser.ParseFrom(baseMsg.ContextBytes);
                msg.SendTime = GetTime();
                UpdateAllPlayerPos(msg);
            }
            else if (baseMsg.MsgTypeEnum == (int)MsgTypeEnum.Login)
            {
                CSLoginInfo loginInfo = CSLoginInfo.Parser.ParseFrom(baseMsg.ContextBytes);
                AddConectClient(new PlayerClient(client, loginInfo.UserName, loginInfo.PlayerID));
                Console.WriteLine($"yns {loginInfo.PlayerID} conected....");
                //allPosMsg.PosPlayerMsgList.Add
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

    private static void UpdateAllPlayerPos(PosPlayerMsg msg)
    {
        var list = allPosMsg.PosPlayerMsgList;
        int len = list.Count;
        for (int i = 0; i < len; i++)
        {
            if (list[i].PlayerId == msg.PlayerId)
            {
                list[i].Pos = msg.Pos;
                return;
            }
        }
        Console.WriteLine("add pos " +  msg.PlayerId);
        list.Add(msg);
    }
    private static BaseMsg MakeBaseMsg(IMessage message,MsgTypeEnum msgType)
    {
        BaseMsg msg = new BaseMsg();
        msg.MsgTypeEnum = (int)msgType;
        msg.ContextBytes = message.ToByteString();

        return msg;
    }
    private static long GetTime()
    {
        TimeSpan time = DateTime.Now - setUpTime;
        return time.Ticks;
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
        var stream = client.GetStream();
        if (stream.CanWrite) stream.Write(bytes);
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
    public static Vector3 ToVec3(this PosPlayerMsg pos)
    {
        return new Vector3(pos.Pos.X, pos.Pos.Y, pos.Pos.Z);
    }

}
