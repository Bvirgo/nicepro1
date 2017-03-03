using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class CallBackTest : MonoBehaviour 
{
    public static CallBackTest Instance;
    public GameObject[] m_pObj = new GameObject[3];
    private List<CTestItem> m_pInt = new List<CTestItem>();
    private CLambda m_lbd;

    private Dictionary<CTestItem, int> m_dic = new Dictionary<CTestItem, int>();

    private Dictionary<int, GameObject> m_dicObj = new Dictionary<int, GameObject>();
	// Use this for initialization

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
	void Start () 
    {
        for (int i = 0; i < 10;++i )
        {
            CTestItem item = new CTestItem(i);
            m_pInt.Add(item);
        }

        m_lbd = new CLambda();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnGo()
    {
        for (int i = 0; i < m_pObj.Length;++i )
        {
            OnTest2(m_pObj[i]);
            //OnTest(m_pObj,i);
        }

        //OnTest2(m_pObj[1]);

        //OnTest3();

        GetHanderAds(m_lbd);

        GetHanderAds(m_pObj);

        GetHanderAds(new CLambda());
    }

    private string GetHanderAds(object _go)
    {
        string res = "";
        GCHandle hander = GCHandle.Alloc(_go);
        var pin = GCHandle.ToIntPtr(hander);
        res = _go.ToString()+ "--内存地址：" + pin;
        Debug.LogWarning(res);
        return res;
    }

    GameObject tempObj;

    private void OnTest( GameObject[] _pObj ,int _nIndex)
    {
        // 结论：lambda表达式，可以访问表达式外的对象，并且会保留这个对象的存储特性，是临时变量，那么出了函数体就会被析构！ 
        // 并且，lambda表达式不支持ref,out 修饰的变量，也就是说，只要是作为形参传过来的，或者函数内部临时构造的对象，出了该函数就会被析构，再用它肯定是空！
        int nMax = m_pInt.Count;
        for (int i = 0; i < m_pInt.Count;++i )
        {
            CTestItem item = m_pInt[i];
            m_lbd.OnGo(i, (res) =>
                {
                    Debug.LogWarning("添加：" + item.m_n + "--返回：" + res);
                    m_dic.Add(item, res);
                    //Debug.LogWarning("添加：" + _pObj[_nIndex].name + "--返回：" + res);
                    m_dicObj.Add(res, _pObj[_nIndex]);
                    --nMax;
                    if (nMax < 1)
                    {
                        foreach (var key in m_dic.Keys)
                        {
                            Debug.LogWarning("Key:" + key + "--Value:" + m_dic[key]);
                        }

                        foreach (var key in m_dicObj.Keys)
                        {
                            Debug.LogWarning("Key:" + key + "--Value:" + m_dicObj[key].name);
                        }
                    }
                });
        }
    }

    private void OnTest2(GameObject _obj)
    {
        // 结论：lambda表达式，可以访问表达式外的对象，并且会保留这个对象的存储特性，是临时变量，那么出了函数体就会被析构！ 
        // 并且，lambda表达式不支持ref,out 修饰的变量，也就是说，只要是作为形参传过来的，或者函数内部临时构造的对象，出了该函数就会被析构，再用它肯定是空！
        int nMax = m_pInt.Count;
        for (int i = 0; i < m_pInt.Count; ++i)
        {
            GameObject tempObj1 = _obj;
            CTestItem item = m_pInt[i];
            m_lbd.OnGoObj(i,tempObj1, (res,obj) =>
            {
                //Debug.Log("添加：" + item.m_n + "--返回：" + res);
                //m_dic.Add(item, res);
                Debug.Log("添加：" + obj.name + "--返回：" + res);
                m_dicObj.Add(res, obj);
                --nMax;
                if (nMax < 1)
                {
                    //foreach(var key in m_dic.Keys)
                    //{
                    //    Debug.LogWarning("Key:"+key + "--Value:"+m_dic[key]);
                    //}

                    foreach (var key in m_dicObj.Keys)
                    {
                        Debug.LogWarning("Key:" + key + "--Value:" + m_dicObj[key].name);
                    }
                }
            });
        }
    }

    private void OnTest3()
    {
        // 结论：lambda表达式，可以访问表达式外的对象，并且会保留这个对象的存储特性，是临时变量，那么出了函数体就会被析构！ 
        // 并且，lambda表达式不支持ref,out 修饰的变量，也就是说，只要是作为形参传过来的，或者函数内部临时构造的对象，出了该函数就会被析构，再用它肯定是空！
        int nMax = m_pObj.Length;
        for (int i = 0; i < m_pObj.Length; ++i)
        {
            // 必须要转存一次，在lambda回调中直接用m_pObj[i]，i都会变为3肯定越界
            // 猜测：在lambda之前被修改的值，在lambda中会被记录，i是在循环执行完一次之后再被修改
            GameObject obj = m_pObj[i];
            m_lbd.OnGo(i, (res) =>
            {
                Debug.Log("当前I："+i);
                m_dicObj.Add(res, obj);
                --nMax;
                if (nMax < 1)
                {

                    foreach (var key in m_dicObj.Keys)
                    {
                        Debug.LogWarning("Key:" + key + "--Value:" + m_dicObj[key].name);
                    }
                }
            });
        }
    }
}

public class CLambda
{
    public CLambda()
    { }

    public void OnGo(int _i,Action<int> _cb)
    {
        CallBackTest.Instance.StartCoroutine(Analysis(_i, _cb));
    }

    IEnumerator Analysis(int _i,Action<int> _cb)
    {
        yield return new WaitForSeconds(1);
        _cb(_i);
    }

    public void OnGoObj(int _i,GameObject _obj, Action<int,GameObject> _cb)
    {
        CallBackTest.Instance.StartCoroutine(Analysis1(_i, _obj,_cb));
    }

    IEnumerator Analysis1(int _i, GameObject _obj, Action<int, GameObject> _cb)
    {
        yield return new WaitForSeconds(1);
        _cb(_i,_obj);
    }
}

public class CTestItem
{
    public int m_n;
    public CTestItem(int _i)
    {
        m_n = _i; 
    }
}
