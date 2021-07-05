using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Google.ProtocolBuffers;
using msginfo;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace protobuf_csharp_sport
{
    class Program
    {
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            beginProtocbuf();
        }

        private static void beginProtocbuf()
        {
            //启动服务端
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
            server.Start();
            server.BeginAcceptTcpClient(clientConnected, server); 
            Console.WriteLine("SERVER : 等待数据 ---");

            //启动客户端
            ThreadPool.QueueUserWorkItem(runClient);
            allDone.WaitOne();

            Console.WriteLine("SERVER : 退出 ---");
            // server.Stop();
        }

        //服务端处理
        private static void clientConnected(IAsyncResult result)
        {
            try
            {
                TcpListener server = (TcpListener)result.AsyncState;
                using (TcpClient client = server.EndAcceptTcpClient(result))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        //获取
                        Console.WriteLine("SERVER : 客户端已连接，数据读取中 --- ");
                        byte[] myRequestBuffer = new byte[1024];

                        int myRequestLength = 0;
                        do
                        {
                            myRequestLength = stream.Read(myRequestBuffer, 0, myRequestBuffer.Length);
                        }
                        while (stream.DataAvailable);
                         
                        CMsg msg = CMsg.ParseFrom(myRequestBuffer.RemoveEmptyByte(myRequestLength));

                        CMsgHead head = CMsgHead.ParseFrom(Encoding.ASCII.GetBytes(msg.Msghead));
                        CMsgReg body = CMsgReg.ParseFrom(Encoding.ASCII.GetBytes(msg.Msgbody));

                        IDictionary<Google.ProtocolBuffers.Descriptors.FieldDescriptor, object> d = head.AllFields;
                        foreach (var item in d)
                        {
                            Console.WriteLine(item.Value.ToString());
                        }

                        d = body.AllFields;
                        Console.WriteLine("===========================================");
                        foreach (var item in d)
                        {
                            Console.WriteLine(item.Value.ToString());
                        }
                      
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

        //客户端请求
        private static void runClient(object state)
        {
            try
            {
                CMsgHead head = CMsgHead.CreateBuilder()
                    .SetMsglen(5)
                    .SetMsgtype(1)
                    .SetMsgseq(3)
                    .SetTermversion(4)
                    .SetMsgres(5)
                    .SetTermid("11111111")
                    .Build();

                CMsgReg body = CMsgReg.CreateBuilder().
                    SetArea(22)
                   .SetRegion(33)
                   .SetShop(44)
                   .Build();

                CMsg msg = CMsg.CreateBuilder()
                    .SetMsghead(head.ToByteString().ToStringUtf8())
                    .SetMsgbody(body.ToByteString().ToStringUtf8())
                    .Build();


                Console.WriteLine("CLIENT : 对象构造完毕 ...");

                using (TcpClient client = new TcpClient())
                {
                    // client.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.116"), 12345));
                    client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));
                    Console.WriteLine("CLIENT : socket 连接成功 ...");

                    using (NetworkStream stream = client.GetStream())
                    {
                        //发送
                        Console.WriteLine("CLIENT : 发送数据 ...");
                      
                        msg.WriteTo(stream);

                        //关闭
                        stream.Close();
                    }
                    client.Close();
                    Console.WriteLine("CLIENT : 关闭 ...");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("CLIENT ERROR : {0}", error.ToString());
            }
        }

    }//end class



    public static class ExtensionClass {
        public static byte[] RemoveEmptyByte(this byte[] by,int length) 
        {
            byte[] returnByte = new byte[length];

            for (int i = 0; i < length; i++)
            {
                returnByte[i] = by[i];
            }
            return returnByte;

        }
    }