package net.testSocket;

import java.io.IOException;
import java.io.InputStream;
import java.net.Socket;
import java.net.UnknownHostException;

import socket.exception.SmsClientException;
import socket.exception.SmsObjException;

import msginfo.Msg.CMsg;
import msginfo.Msg.CMsgHead;
import msginfo.Msg.CMsgReg;
import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import com.google.protobuf.InvalidProtocolBufferException;

//客户端的实现
public class TestSocket extends Activity {
    private TextView text1;
    private Button but1; 
    Socket socket = null;

    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Thread desktopServerThread=new Thread(new AndroidServer());
        // desktopServerThread.start();

        setContentView(R.layout.main);

        text1 = (TextView) findViewById(R.id.text1);
        but1 = (Button) findViewById(R.id.but1); 

        but1.setOnClickListener(new Button.OnClickListener() {
            @Override
            public void onClick(View v) {

                // edit1.setText("");
                // Log.e("dddd", "sent id");
                // new Thread() {
                // public void run() {
                try {
                    // socket=new Socket("192.168.1.102",54321);
                    //socket = new Socket("192.168.1.110", 10527);
                     socket = new Socket("192.168.1.116", 12345);
                    //得到发送消息的对象 
                    //SmsObj smsobj = new SmsObj(socket);
                    
                    //设置消息头和消息体并存入消息里面
                    // head
                    CMsgHead head = CMsgHead.newBuilder().setMsglen(5)
                            .setMsgtype(1).setMsgseq(3).setTermversion(41)
                            .setMsgres(5).setTermid("11111111").build();

                    // body
                    CMsgReg body = CMsgReg.newBuilder().setArea(22)
                            .setRegion(33).setShop(44).build();

                    // Msg
                    CMsg msg = CMsg.newBuilder()
                            .setMsghead(head.toByteString().toStringUtf8())
                            .setMsgbody(body.toByteString().toStringUtf8())
                            .build();

                    // PrintWriter out = new PrintWriter(new BufferedWriter(
                    // new OutputStreamWriter(socket.getOutputStream())),
                    // true);
                    // out.println(m.toString());
                    // out.println(m.toByteString().toStringUtf8());

                    // 向服务器发送信息
                    msg.writeTo(socket.getOutputStream());
                    //byte[] b = msg.toByteArray();
                    //smsobj.sendMsg(b);

                    // System.out.println("====msg==="
                    // + m.toByteString().toStringUtf8());
                    
                    // byte[] backBytes = smsobj.recvMsg();
                    //
                    // 接受服务器的信息
                    InputStream input = socket.getInputStream();

                    // DataInputStream dataInput=new DataInputStream();
                    //byte[] by = smsobj.recvMsg(input);
                    byte[] by=recvMsg(input);
                    setText(CMsg.parseFrom(by));

                    // BufferedReader br = new BufferedReader(
                    // new InputStreamReader(socket.getInputStream()));
                    // String mstr = br.readLine();
                    // if (!str .equals("")) {
                    // text1.setText(str);
                    // } else {
                    // text1.setText("数据错误");
                    // }
                    // out.close();
                    // br.close();

                    input.close();
                    //smsobj.close();
                    socket.close();
                } catch (UnknownHostException e) {
                    e.printStackTrace();
                } catch (IOException e) {
                    e.printStackTrace();
                } catch (Exception e) {
                    System.out.println(e.toString());
                }
                // };
                // }.start();

            }
        });

    }
    
    /**
     * 接收server的信息
     * 
     * @return
     * @throws SmsClientException
     * @author fisher
     */
    public byte[] recvMsg(InputStream inpustream) throws SmsObjException {
        try {
 
            byte len[] = new byte[1024];
            int count = inpustream.read(len);  
        
            byte[] temp = new byte[count];
            for (int i = 0; i < count; i++) {   
                    temp[i] = len[i];                              
            } 
            return temp;
        } catch (Exception localException) {
            throw new SmsObjException("SmapObj.recvMsg() occur exception!"
                    + localException.toString());
        }
    }

    /**
     * 得到返回值添加到文本里面
     * 
     * @param g
     * @throws InvalidProtocolBufferException
     */
    public void setText(CMsg g) throws InvalidProtocolBufferException {
        CMsgHead h = CMsgHead.parseFrom(g.getMsghead().getBytes());
        StringBuffer sb = new StringBuffer();
        if (h.hasMsglen())
            sb.append("==len===" + h.getMsglen() + "\n");
        if (h.hasMsgres())
            sb.append("==res===" + h.getMsgres() + "\n");
        if (h.hasMsgseq())
            sb.append("==seq===" + h.getMsgseq() + "\n");
        if (h.hasMsgtype())
            sb.append("==type===" + h.getMsgtype() + "\n");
        if (h.hasTermid())
            sb.append("==Termid===" + h.getTermid() + "\n");
        if (h.hasTermversion())
            sb.append("==Termversion===" + h.getTermversion() + "\n");

        CMsgReg bo = CMsgReg.parseFrom(g.getMsgbody().getBytes());
        if (bo.hasArea())
            sb.append("==area==" + bo.getArea() + "\n");
        if (bo.hasRegion())
            sb.append("==Region==" + bo.getRegion() + "\n");
        if (bo.hasShop())
            sb.append("==shop==" + bo.getShop() + "\n");
        if (bo.hasRet())
            sb.append("==Ret==" + bo.getRet() + "\n");
        if (bo.hasTermid())
            sb.append("==Termid==" + bo.getTermid() + "\n");

        text1.setText(sb.toString());
    }