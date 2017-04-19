using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.UI;

public class JsonReTest : MonoBehaviour
{
    string[] m_pStr;
    string m_strPath;
	// Use this for initialization
	void Start () {
        m_strPath = Application.streamingAssetsPath + "//ZHJson.txt";
        m_pStr = new string[] { "我是1", "我是2", "我是3" };
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 嵌套Json读取
    /// </summary>
    public void OnReadJson()
    {
        string strJD = File.ReadAllText(m_strPath);
        JsonData JD = JsonMapper.ToObject(strJD);
        
        for (int i = 0; i < JD.Count; i++)
        {
            int nId = JD[i].ReadInt("id");
            string strInfo = JD[i].ReadString("info","Error");
            if (!strInfo.Equals("Error"))
            {
                JsonData infoJD = JsonMapper.ToObject(strInfo);
                string strData = infoJD.ReadString("data","dataError");
                if (!strData.Equals("dataError"))
                {
                    try
                    {
                        JsonData dataJD = JsonMapper.ToObject(strData);
                        for (int j = 0; j < dataJD.Count; j++)
                        {
                            Debug.LogWarning("解析出：" + dataJD[j].ReadString("fileName"));
                        }

                    }
                    catch (Exception)
                    {

                        Debug.LogError("解析出错："+nId) ;
                    }
                }
            }
        }
    }

    public void OnSaveJson()
    {
        JsonData JDArray = JsonUtils.EmptyJsonArray;
        for (int i = 0; i < m_pStr.Length; i++)
        {
            JsonData JD = JsonUtils.EmptyJsonObject;
            JD["name"] = m_pStr[i];
            JD["id"] = i + 1;
            JDArray.Add(JD);
        }
        // UTF-8编码
        string strUTF8 = Helper.UnicodeToGB(JDArray.ToJson());

        Helper.SaveInfo(strUTF8, "JsonDataToJson");


        // Unicode编码
        JsonData JD1 = new JsonData();
        for (int i = 0; i < m_pStr.Length; i++)
        {
            JsonData JD = JsonUtils.EmptyJsonObject;
            JD["name"] = m_pStr[i];
            JD["id"] = i + 1;
            JD1.Add(JD);
        }

        Helper.SaveInfo(JDArray.ToJson(), "JsonDataToJson1");


        string strToJson = JsonMapper.ToJson(JD1);
        Helper.SaveInfo(strToJson, "JsonMapperToJson");

        // 总结：JsonMapper.ToJson(JD)  等于  JD.ToJson(); 讲JsonData转为字符串


        // JsonMapper.ToObject()：字符串解析为Json结构
        string strJsonData = "[{\"name\":\"我是1\",\"id\":1},{\"name\":\"我是2\",\"id\":2},{\"name\":\"我是3\",\"id\":3}]";
        JsonData JD2 = JsonMapper.ToObject(strJsonData);
        for (int i = 0; i < JD2.Count; i++)
        {
            foreach (var item in JD2[i].Keys)
            {
                string strTips = string.Format("Key:{0}     Value:{1}", item, JD2[i][item]);
                Debug.LogWarning(strTips);
            }
        }
    }
}
