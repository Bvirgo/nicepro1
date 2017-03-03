using UnityEngine;
using System.Collections;
using ProtoBuf;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using MyBook;

public class ProNet : MonoBehaviour
{
    private  string PATH = "c://data.bin";
    //private string strPath = Application.streamingAssetsPath + "/localProtodata.bin";

    #region UI
    public Text m_txtGet;
    public InputField m_iptSender;
    #endregion

    #region 本地存储
    void Start()
    {
        //生成数据  
        List<Test> testData = new List<Test>();

        for (int i = 0; i < 100; i++)
        {
            testData.Add(new Test() { Id = i, data = new List<string>(new string[] { "1", "2", "3" }) });
        }


        //将数据序列化后存入本地文件  
        using (Stream file = File.Create(PATH))
        {
            Serializer.Serialize<List<Test>>(file, testData);
            file.Close();
        }

        //将数据从文件中读取出来，反序列化  
        List<Test> fileData;
        using (Stream file = File.OpenRead(PATH))
        {
            fileData = Serializer.Deserialize<List<Test>>(file);
        }
        //打印数据  
        foreach (Test data in fileData)
        {
            Debug.Log(data);
        }

  
    }
   #endregion

    #region 特性序列化protobuf
    //将二进制流解码成Protobuf对象，显示出来
    void OnRecMsg(byte[] msg)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Write(msg, 0, msg.Length);
            ms.Position = 0;
            ChatMsg chatMsg = Serializer.Deserialize<ChatMsg>(ms);
            //textList.Add(chatMsg.sender + ":" + chatMsg.msg);
            m_txtGet.text = chatMsg.sender + ":" + chatMsg.msg;
        }
    }

    //将消息体编码成二进制流
    private byte[] serial(string sender, string msg)
    {
        ChatMsg chatMsg = new ChatMsg();
        chatMsg.sender = sender;
        chatMsg.msg = msg;

        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<ChatMsg>(ms, chatMsg);
            byte[] data = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(data, 0, data.Length);
            return data;
        }
    }

    public void OnSendMsg()
    {
        byte[] pData = serial("毛主席说", m_iptSender.text);

        OnRecMsg(pData);
    }
    #endregion

    #region .proto文件转.cs 
    public void OnTestProtoFile()
    {
        ReadProtoCS(SerialProtoCS());
    }

    /// <summary>
    /// proto文件转为cs文件的使用
    /// </summary>
    /// <returns></returns>
    private byte[] SerialProtoCS()
    {
        MyClass mc = new MyClass();
        mc.grade = "一年级1班";
        for (int i = 0; i < 5; ++i)
        {
            Person p = new Person() { name = (i + 1).ToString(), sex = true, age = 23 };
            mc.PersonList.Add(p);
        }

        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<MyClass>(ms, mc);
            byte[] data = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(data, 0, data.Length);
            return data;
        }
    }

    void ReadProtoCS(byte[] msg)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Write(msg, 0, msg.Length);
            ms.Position = 0;
            MyClass mc = Serializer.Deserialize<MyClass>(ms);
            m_txtGet.text = mc.grade;
            for (int i = 0; i < mc.PersonList.Count; ++i)
            {
                m_txtGet.text += "\nName:" + mc.PersonList[i].name;

            }
        }
    }
    #endregion
  
}

[ProtoContract]
public class Test
{

    [ProtoMember(1)]
    public int Id
    {
        get;
        set;
    }


    [ProtoMember(2)]
    public List<string> data
    {
        get;
        set;
    }


    public override string ToString()
    {
        string str = Id + ":";
        foreach (string d in data)
        {
            str += d + ",";
        }
        return str;
    }
}

/// <summary>
/// 消息对象
/// </summary>
[ProtoContract]
public class ChatMsg
{

    [ProtoMember(1)]
    public string sender;//发送者
    [ProtoMember(2)]
    public string msg;//消息

}
