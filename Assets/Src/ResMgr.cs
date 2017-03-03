using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public  class ResMgr : MonoBehaviour
{
    public InputField m_iptABName;
    public InputField m_iptAseetName;
    public Image m_img;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void OnSAAssetBundle(string _strName)
    {
        StartCoroutine(LoadAB(_strName));
    }

    public void OnLoadPrefabs()
    {
        string strTime = DateTime.Now.ToShortDateString();
        Debug.Log("DateTime.Now.ToShortDateString：" + strTime);
        Debug.Log("DateTime.Now" + DateTime.Now.ToString("yyyy:MM:dd-hh:mm"));
        Debug.Log("DateTime.Now.ToLocalTime" + DateTime.Now.ToLocalTime().ToString("yyyy:mm:dd-hh:mm"));   
        StartCoroutine(LoadAB<GameObject>(p => 
        {
            GameObject obj = GameObject.Instantiate(p);
            obj.transform.position = Vector3.zero;
        }));
    }

    public void OnAutoBuild()
    {
 
    }

    IEnumerator LoadAB<T>(Action<T> _mc) where T : UnityEngine.Object
    {
        AssetBundle AssetBundleCsv = new AssetBundle();
        // 包名

        string strBundleName = m_iptABName.text;

        string str1 = "file://" + Application.dataPath + "/ABs/" + strBundleName;
        WWW www = new WWW(str1);
        yield return www;
        AssetBundleCsv = www.assetBundle;

        T res = AssetBundleCsv.LoadAsset<T>(m_iptAseetName.text);

        _mc(res);

        AssetBundleCsv.Unload(false);
    }
    IEnumerator LoadAB(string _strName)
    {
        AssetBundle AssetBundleCsv = new AssetBundle();
        // 包名
        string strBundleName = "t.assetbundle";
        strBundleName = "zp028007003";
        strBundleName = m_iptABName.text;

        string str1 = "file://" + Application .dataPath+"/ABs/"+strBundleName;
        WWW www = new WWW(str1);
        yield return www;
        AssetBundleCsv = www.assetBundle;

        Texture[] tex = AssetBundleCsv.LoadAllAssets<Texture>();
        Debug.Log("Texture个数："+tex.Length);

        Texture2D _tx = AssetBundleCsv.LoadAsset<Texture2D>(m_iptAseetName.text);

        Sprite spr = Sprite.Create(_tx, new Rect(0, 0, _tx.width, _tx.height), Vector2.zero);
        m_img.overrideSprite = spr;
        m_img.GetComponent<RectTransform>().sizeDelta = new Vector2(_tx.width, _tx.height);
        AssetBundleCsv.Unload(false);
    }

    public void OnNextSc()
    {

        SceneManager.LoadScene("GUIWindow");
     }

}
