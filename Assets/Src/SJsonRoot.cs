using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.UI;
public class SJsonRoot : MonoBehaviour
{

    #region Base Member

    public Text txt;

    List<string> pConfigPaths = new List<string>();

    public List<OldNormCmpData> m_pOldNormCmp = new List<OldNormCmpData>();

    public List<OldWallCmpData> m_pOldWallCmp = new List<OldWallCmpData>();

    public List<OldWallCmpData> m_pOldGreenMeshCmp = new List<OldWallCmpData>();
    #endregion

    #region 加载StreamAsset中的Json数据
    public void OnLoadJson()
    {
        string strPath = Application.streamingAssetsPath;
        GetCompConfigPaths(strPath);

        LoadConfigs();
    }

    /// <summary>
    /// 获取指定目录下的所有文本文件
    /// </summary>
    /// <param name="_strPath"></param>
    private void GetCompConfigPaths(string _strPath)
    {
        DirectoryInfo folder = new DirectoryInfo(_strPath);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            // 如果这个文件是目录,递归到每个文件
            if (files[i] is DirectoryInfo)
            {
                GetCompConfigPaths(files[i].FullName);
            }
            else
            {
                if (files[i].Name.EndsWith(".txt"))
                {
                    if (pConfigPaths.Contains(files[i].FullName))
                    {
                        pConfigPaths.Remove(files[i].FullName);
                    }
                    pConfigPaths.Add(files[i].FullName);
                }
            }
        }
    }

    /// <summary>
    /// 解析Json
    /// </summary>
    /// <param name="_strPath"></param>
    private void AnalysisConfig(string _strPath)
    {
        Debug.LogWarning("加载Json:" + _strPath);
        JsonData ConfigJD = JsonUtils.ReadJsonFile(_strPath);
        if (ConfigJD != null)
        {
            JsonData wallJD = ConfigJD.ReadJsonData("wall");
            JsonData normalJD = ConfigJD.ReadJsonData("component");

            if (normalJD != null)
            {
                for (int i = 0; i < normalJD.Count; ++i)
                {
                    JsonData curJd = normalJD[i];
                    OldNormCmpData oncd = new OldNormCmpData();
                    oncd.p = curJd.ReadString("p");
                    oncd.e = curJd.ReadString("e");
                    oncd.s = curJd.ReadString("s");
                    oncd.t = curJd.ReadString("t");
                    m_pOldNormCmp.Add(oncd);
                }
            }

            if (wallJD != null)
            {
                for (int j = 0; j < wallJD.Count; ++j)
                {
                    JsonData curJd = wallJD[j];
                    OldWallCmpData owcd = new OldWallCmpData();
                    owcd.wallLength = curJd.ReadString("wallLength");
                    string strType = curJd.ReadString("wallType");
                    owcd.wallType = curJd.ReadString("wallType");
                    owcd.firstPoint = curJd.ReadString("firstPoint");
                    owcd.pointList = curJd.ReadJsonData("pointList");
                    if (owcd.pointList != null && owcd.pointList.Count > 1)
                    {
                        if (strType.StartsWith("GreenMesh"))
                        {
                            m_pOldGreenMeshCmp.Add(owcd);
                        }
                        else
                        {
                            m_pOldWallCmp.Add(owcd);
                        }

                    }
                }
            }
        }
    }

    /// <summary>
    /// 创建Json
    /// </summary>
    private void LoadConfigs()
    {
        for (int i = 0; i < pConfigPaths.Count; ++i)
        {
            string strPath = pConfigPaths[i];
            AnalysisConfig(strPath);
        }

        // 
        JsonData CmpJD = new JsonData();

        for (int j = 0; j < m_pOldNormCmp.Count; ++j)
        {
            OldNormCmpData oncd = m_pOldNormCmp[j];
            JsonData jd = new JsonData();
            jd["CompType"] = "normal";
            jd["CompCode"] = oncd.t;
            jd["p"] = oncd.p;
            jd["e"] = oncd.e;
            jd["s"] = oncd.s;
            CmpJD.Add(jd);
        }

        for (int k = 0; k < m_pOldWallCmp.Count; ++k)
        {
            OldWallCmpData oncd = m_pOldWallCmp[k];
            JsonData jd = new JsonData();
            jd["CompType"] = "wall";
            jd["wallType"] = oncd.wallType;
            jd["wallLength"] = oncd.wallLength;
            jd["firstPoint"] = oncd.firstPoint;
            jd["pointList"] = oncd.pointList;
            jd["needScaleTail"] = "true";
            CmpJD.Add(jd);
        }

        for (int i = 0; i < m_pOldGreenMeshCmp.Count; ++i)
        {
            OldWallCmpData oncd = m_pOldGreenMeshCmp[i];
            JsonData jd = new JsonData();
            jd["CompType"] = "greenMesh";
            jd["wallType"] = "null";
            jd["wallLength"] = oncd.wallLength;
            jd["firstPoint"] = oncd.firstPoint;
            jd["pointList"] = oncd.pointList;
            string strIndex = oncd.wallType.Substring(10, 3);
            jd["greenMeshMat"] = null;
            CmpJD.Add(jd);
        }

        JsonData blockJD = new JsonData();
        blockJD["compList"] = CmpJD;

        //Json 数据保存到本地:JsonData转string：ToJson
        Helper.SaveInfo(blockJD.ToJson(), "解析出来的临时数据", Application.streamingAssetsPath);
    }
    #endregion

    #region 从网上下载Json数据
    public void OnLoadExtralDataFromNet()
    {
        LoginExtralData(res =>
            {
                txt.text = res;
            });
    }

    /// <summary>
    /// 解析配置：先从本地获取，如果没有，从网上下，写到本地StreamAsset下
    /// </summary>
    /// <param name="_cb"></param>
    public void LoginExtralData(Action<string> _cb)
    {
        string strPath = UnityEngine.Application.streamingAssetsPath + "//LoginExtralData.txt";

        // 优先从本地读取
        if (File.Exists(strPath))
        {
            string strExtralData = File.ReadAllText(strPath, System.Text.Encoding.UTF8);
            SCDDataLoaded(strExtralData, _cb, (cb) =>
            {
                // 本地解析失败，还是从服务器下载
                LoadSCDFromNetByTxt(cb);
            });
        }
        else // 从服务器下载配置
        {
            LoadSCDFromNetByTxt(_cb);
        }
    }

    /// <summary>
    /// 获取文本方式请求SCD数据:有限从本地加载，如果没有从服务器加载txt，如果还没有，那就请求原始Jsondata
    /// </summary>
    /// <param name="_cb"></param>
    public void LoadSCDFromNetByTxt(Action<string> _cb)
    {
        // 这是一个文本配置文件
        string url = "http://artistwork.pek3a.qingstor.com/filecrc/loginextraldata.txt_2514441822";

        MyWWWMgr.Instance.GetFile(url,LocalCacheEntry.CacheType.Raw,(hRsp,entry) =>
        {
            byte[] data = entry.Bytes;
            string strExtralData = Encoding.UTF8.GetString(data);

            // 保存在本地
            string strPath = UnityEngine.Application.streamingAssetsPath + "//LoginExtralData.txt";

            using (FileStream fsWrite = new FileStream(strPath, FileMode.Create))
            {
                fsWrite.Write(data, 0, data.Length);
            };

            SCDDataLoaded(strExtralData, _cb);
        });
    }

    /// <summary>
    /// 解析SCD 配置
    /// </summary>
    /// <param name="_strData"></param>
    /// <param name="_cb"></param>
    /// <param name="_failCb"></param>
    private void SCDDataLoaded(string _strData, Action<string> _cb, Action<Action<string>> _failCb = null)
    {

        // 字符串 转 JsonData,直接从Jsondata中取需要的字段，不用整个解析
        JsonData compJd = JsonMapper.ToObject(_strData);

        if (compJd != null)
        {
            try
            {
                JsonData jd = compJd.ReadJsonData("extra_info")[0];
                string strName = jd.ReadString("name");
                string strUrl = jd.ReadString("url");
                _cb(string.Format("Name:{0}\nURL:{1}",strName,strUrl));
            }
            catch (Exception)
            {
                Debug.LogWarning("解析Json异常："+_strData);
                _failCb(_cb);
            }
        }
    }
    #endregion
}

public struct OldNormCmpData
{
    public string p;
    public string e;
    public string s;
    public string t;
}

public struct OldWallCmpData
{
    public string wallType;
    public string wallLength;
    public string firstPoint;
    public JsonData pointList;
}
