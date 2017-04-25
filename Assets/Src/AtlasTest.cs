using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using LitJson;
using UnityEngine.UI;

public class AtlasTest : MonoBehaviour
{
    public Image m_img;
    public string m_strPath;
    private string m_strAtlasPath;
    private string m_strAtlasName;
    private string m_strAtlasJDPath;
    Queue m_qImgFile;
    List<Texture2D> m_pTexture;
    List<string> m_pFileName;
    // Use this for initialization
    void Start ()
    {
        m_strPath = Application.streamingAssetsPath + "/Pic";
        m_strAtlasPath = Application.streamingAssetsPath+"/Atlas";
        m_strAtlasJDPath = Application.streamingAssetsPath + "/AtlasJD.txt";
        m_strAtlasName = "myAtlas";
        m_pTexture = new List<Texture2D>();
    }

    #region 打包图集
    void OnLoadTexture(Action _sCb)
    {
        if (m_qImgFile.Count > 0)
        {
            string strPath = (string)m_qImgFile.Dequeue();
            MyWWWMgr.Instance.GetResByWWW("file:///" + strPath, (w) =>
            {
                m_pTexture.Add(w.texture);
                OnLoadTexture(_sCb);
            }, (resp) =>
            {
                Debug.LogWarning("加载资源错误：" + strPath);
                OnLoadTexture(_sCb);
            });
        }
        else
        {
            _sCb();
        }
    }

    /// <summary>
    /// 打包图集
    /// </summary>
    public void OnCreateAtlas()
    {
        OnLoadLocalPics();
    }

    /// <summary>
    /// 加载原图片
    /// </summary>
    private void OnLoadLocalPics()
    {
        m_qImgFile = new Queue();
        m_pFileName = new List<string>();
        DirectoryInfo dinfo = new DirectoryInfo(m_strPath);
        FileInfo[] pFiles = dinfo.GetFiles();
        for (int i = 0; i < pFiles.Length; i++)
        {
            if (!pFiles[i].FullName.Contains("meta"))
            {
                m_qImgFile.Enqueue(pFiles[i].FullName);
                m_pFileName.Add(pFiles[i].FullName);
            }
        }

        Debug.LogWarning("获取基础图片个数：" + m_qImgFile.Count);

        OnLoadTexture(CreateAtlas);
    }

    /// <summary>
    /// 打图集
    /// </summary>
    private void CreateAtlas()
    {
        // 返回的是：每个原始图片，所在图集，已经区域
        /*
         * [{
	            "tile": "zp039403011072.jpg",
	            "atlas": "myAtlas_1.png",
	            "rect": "0,0,0.5,0.125"
            },
            {
	            "tile": "zp039403011073.jpg",
	            "atlas": "myAtlas_1.png",
	            "rect": "0,0.125,1,0.25"
            }]
         */
        JsonData jd = MaxRects.Instance.Combine(m_strAtlasPath, m_pTexture.ToArray(), m_strAtlasName, m_pFileName);

        Helper.SaveInfo(jd.ToJson(), "AtlasJD");

        AnalysismAtlas();
    }
    #endregion

    #region 解析单个图片
    List<AtlasItem> m_pAtlas = new List<AtlasItem>();
    int m_nIndex = 0;
    struct AtlasItem
    {
        public string name;
        public string atlas;
        public Rect rect;
    };

    private void AnalysismAtlas()
    {
        m_pAtlas.Clear();
        // 解析Json
        string strJs = File.ReadAllText(m_strAtlasJDPath);
        JsonData atlasJd = JsonMapper.ToObject(strJs);
        for (int i = 0; i < atlasJd.Count; i++)
        {
            string strName = atlasJd[i].ReadString("tile");
            string strAtlas = atlasJd[i].ReadString("atlas");
            string strRect = atlasJd[i].ReadString("rect");
            AtlasItem item;
            item.name = strName;
            item.atlas = strAtlas;
            Vector4 v = Helper.GetV4(strRect);
            // 图集大小就是1024 * 1024，保存的时候，这个区域值是除了1024的
            Rect rect = new Rect(v.x * 1024, v.y * 1024, v.z * 1024, v.w * 1024);
            item.rect = rect;
            m_pAtlas.Add(item);
        }
    }

    public void OnLoadImgByAtlas()
    {
        m_nIndex = m_nIndex > m_pAtlas.Count - 1 ? 0 : m_nIndex;
        //m_nIndex = 0;
        AtlasItem item = m_pAtlas[m_nIndex++];

        string strAtlasPath = m_strAtlasPath + "/" + item.atlas;

        MyWWWMgr.Instance.GetResByWWW("file:///" + strAtlasPath, (w) =>
        {

            CreateImg(item, w.texture);

        }, (resp) =>
        {
            Helper.Debug("加载图集：" + strAtlasPath + "失败！");
        });
    }

    private void CreateImg(AtlasItem _item,Texture2D _tx)
    {
        Texture2D txAtlas = _tx;
        int hMax = (int)_item.rect.height;
        int wMax = (int)_item.rect.width;
        // 最接近hMax的2的幂值
        hMax = Mathf.NextPowerOfTwo(hMax);
        wMax = Mathf.NextPowerOfTwo(wMax);

        Helper.Debug(string.Format("创建图片：{0}，H：{1}，W：{2}", _item.name, hMax, wMax));

        // 获取制定Rect区域的像素值
        Color[] pColor = txAtlas.GetPixels((int)_item.rect.x, (int)_item.rect.y, (int)_item.rect.width, (int)_item.rect.height);

        Texture2D txImg = new Texture2D(wMax, hMax);
        txImg.SetPixels(0, 0, wMax, hMax, pColor);

        // 我日，这个是必须的,否则这个Texture是坏损的！！！！！！
        // 为什么没有Apply(),但是保存出来还是对的？  因为执行EncodeToJPG()的时候，默认执行了Apply()
        txImg.Apply();

        string strPath = Application.streamingAssetsPath + "/" + _item.name;
        File.WriteAllBytes(strPath,txImg.EncodeToJPG());

        Sprite spr = Sprite.Create(txImg, new Rect(0, 0, wMax, hMax), Vector2.one,96);
        m_img.sprite = spr;
    }
    #endregion


}
