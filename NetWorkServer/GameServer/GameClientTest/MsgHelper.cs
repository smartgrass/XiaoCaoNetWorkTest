using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
static class MsgHelper
{
    public static BaseMsg GetSendBaseMsg(byte[] context,MsgTypeEnum msgType)
    {
        BaseMsg baseMsg = new BaseMsg();

        baseMsg.MsgType = (int)msgType;
        baseMsg.Bytes = pb.ByteString.CopyFrom(context);
        //baseMsg.Bytes =;
        System.Console.WriteLine("Msg bulid " + baseMsg.Bytes.ToStringUtf8());

        return baseMsg;
    }
    public static BaseMsg GetSendBaseMsg(pb.ByteString context, MsgTypeEnum msgType)
    {
        BaseMsg baseMsg = new BaseMsg();

        baseMsg.MsgType = (int)msgType;
        baseMsg.Bytes =context;
        //baseMsg.Bytes =;

        return baseMsg;
    }


}

