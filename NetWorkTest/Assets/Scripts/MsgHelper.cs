
using Google.Protobuf;

static class MsgHelper
{
    //有时不一定要使用这样写法
    //可以考虑使用扩展方法

    public static BaseMsg GetSendBaseMsg(IMessage context, MsgTypeEnum msgType,int playerID)
    {
        BaseMsg baseMsg = new BaseMsg();
        baseMsg.MsgTypeEnum = (int)msgType;
        baseMsg.PlayerId = playerID;
        baseMsg.MsgWayEnum = 0;
        baseMsg.ContextBytes = context.ToByteString();
        return baseMsg;
    }

}

