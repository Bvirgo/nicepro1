using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class MyWWWMgr : MonoBehaviour
{
    #region BaseMember
    private static MyWWWMgr instance;
    public static MyWWWMgr Instance
    {
        get 
        {
            if (instance == null)
            {
                var go = new GameObject("NetRoot");
                instance = go.AddComponent<MyWWWMgr>();
            }

                return instance;
        }
    }

    #endregion

    #region 网络下载 + 缓存
    string ABurl = "http://uploads.pek3a.qingstor.com/v1/1470969096712.zip";
    string imgUrl = "http://uploads.pek3a.qingstor.com/v1/zhoukou626";

    public delegate void OnHttpResponse(HttpResp resp);

    public delegate void OnHttpGetFileResponse(HttpResp resp, LocalCacheEntry entry);
    public delegate void OnHttpStatusChange(HttpStatus status);

    public void GetAb(string _strUrl, Action<WWW> callback)
    {
        WWW w = new WWW(_strUrl);
        StartCoroutine(GetAb(w,callback));
    }

    private IEnumerator GetAb(WWW www, Action<WWW> callback)
    {
        yield return www;

        if (www.error != null)
        {
            //callback(www);
        }
        else if (www.isDone)
        {
            //callback(www);
            var ab = www.assetBundle;
            string strPath = URLToLocalPath(ABurl);
            if (!Directory.Exists(CacheFolder))
            {
                Directory.CreateDirectory(CacheFolder);
            }
            File.WriteAllBytes(strPath, www.bytes);
        }
        else
        {
            throw new UnityException("Undone www: " + www.url);
        }

        www.Dispose();
    }

    /// <summary>
    /// www善后
    /// </summary>
    /// <param name="www"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DoSend(WWW www, Action<WWW> callback)
    {
        SetStatus(HttpStatus.Transmitting);

        // 检测是否下载完毕，也可以通过IsDone函数检测  
        yield return www;

        // 能执行到这里，说明www已经完成，根据www状态来处理接下来的逻辑
        if (www.error != null)
        {
            callback(www);
        }
        else if (www.isDone)
        {
            callback(www);
        }
        else
        {
            throw new UnityException("Undone www: " + www.url);
        }

        // 释放资源
        www.Dispose();

        SetStatus(HttpStatus.Finished);
    }

    /// <summary>
    /// 网络返回结构
    /// </summary>
    /// <param name="www"></param>
    /// <returns></returns>
    private HttpResp GenerateResp(WWW www)
    {
        var resp = new HttpResp();

        resp.WwwText = www.text;
        return resp;
    }

    private void SetStatus(HttpStatus status)
    {

    }

    /// <summary>
    /// www状态：进行中，完成
    /// </summary>
    public enum HttpStatus
    {
        Transmitting,
        Finished
    }

    /// <summary>
    /// 根据URL 定位本地缓存的资源文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private string URLToLocalPath(string url)
    {
        return CacheFolder + WWW.EscapeURL(url);
    }

    /// <summary>
    /// 本地缓存
    /// </summary>
    public static string CacheFolder
    {
        get
        {
            return Application.streamingAssetsPath + "/wwwcache/";
        }
    }

    /// <summary>
    /// 带本地缓存的http文件获取
    /// 本地版本实际上是同步，网络版本是异步
    /// </summary>
    /// ABurl = "http://uploads.pek3a.qingstor.com/v1/1470969096712.zip";
    /// imgUrl = "http://uploads.pek3a.qingstor.com/v1/zhoukou626";
    public void GetFile(string url, LocalCacheEntry.CacheType type, OnHttpGetFileResponse callback)
    {
        var local = URLToLocalPath(url);

        if (File.Exists(local))
        {

            var fakeResp = new HttpResp();
            fakeResp.Error = HttpResp.ErrorType.None;
            fakeResp.WwwText = "";

            switch (type)
            {
                case LocalCacheEntry.CacheType.Texture:
                    var tex = new Texture2D(2, 2);
                    var imgBytes = File.ReadAllBytes(local);
                    tex.LoadImage(imgBytes);

                    callback(fakeResp, new LocalCacheEntry()
                    {
                        Type = LocalCacheEntry.CacheType.Texture,
                        Texture = tex
                    });
                    break;

                case LocalCacheEntry.CacheType.Fbx:
                    callback(fakeResp, new LocalCacheEntry()
                    {
                        Type = LocalCacheEntry.CacheType.Fbx,
                        FbxPath = local
                    });
                    break;

                case LocalCacheEntry.CacheType.AssetBundle:
                    var ab = AssetBundle.LoadFromFile(local);
                    if (null == ab)
                    {
                        return;
                    }
                    callback(fakeResp, new LocalCacheEntry()
                    {
                        Type = LocalCacheEntry.CacheType.AssetBundle,
                        AB = ab
                    });

                    break;

                case LocalCacheEntry.CacheType.Raw:
                    var bytes = File.ReadAllBytes(local);

                    callback(fakeResp, new LocalCacheEntry()
                    {
                        Type = LocalCacheEntry.CacheType.Raw,
                        Bytes = bytes
                    });

                    break;
                default:
                    break;
            }
        }
        else
        {

            switch (type)
            {
                case LocalCacheEntry.CacheType.Texture:
                    //unity www的设计是：new出来的瞬间，请求就立即发起了
                    //由于排队的需求，只能推迟new的时机
                    // Dosend：只是检测www的执行状况
                    var www = new WWW(url);
                    StartCoroutine(DoSend(www, (WWW w) =>
                    {
                        var resp = GenerateGetfileResp(w);

                        if (resp.Error != HttpResp.ErrorType.None)
                        {
                            callback(resp, null);
                        }
                        else
                        {
                            File.WriteAllBytes(URLToLocalPath(url), w.bytes);

                            callback(resp, new LocalCacheEntry()
                            {
                                Type = LocalCacheEntry.CacheType.Texture,
                                Texture = w.texture
                            });
                        }
                    }));
                    break;

                case LocalCacheEntry.CacheType.Fbx:
                    var www2 = new WWW(url);
                    StartCoroutine(DoSend(www2, (WWW w) =>
                    {
                        var resp = GenerateGetfileResp(w);

                        if (resp.Error != HttpResp.ErrorType.None)
                        {
                            callback(resp, null);
                        }
                        else
                        {
                            var newLocal = URLToLocalPath(url);
                            File.WriteAllBytes(newLocal, w.bytes);

                            callback(resp, new LocalCacheEntry()
                            {
                                Type = LocalCacheEntry.CacheType.Fbx,
                                FbxPath = newLocal
                            });
                        }
                    }));
                    break;

                case LocalCacheEntry.CacheType.AssetBundle:
                    var www3 = new WWW(url);
                    StartCoroutine(DoSend(www3, (WWW w) =>
                    {
                        var resp = GenerateGetfileResp(w);

                        if (resp.Error != HttpResp.ErrorType.None)
                        {
                            callback(resp, null);
                        }
                        else
                        {
                            var ab = w.assetBundle;
                            File.WriteAllBytes(URLToLocalPath(url), w.bytes);

                            callback(resp, new LocalCacheEntry()
                            {
                                Type = LocalCacheEntry.CacheType.AssetBundle,
                                AB = ab
                            });
                        }
                    }));
                    break;

                case LocalCacheEntry.CacheType.Raw:
                    var www4 = new WWW(url);
                    StartCoroutine(DoSend(www4, (WWW w) =>
                    {
                        var resp = GenerateGetfileResp(w);

                        if (resp.Error != HttpResp.ErrorType.None)
                        {
                            callback(resp, null);
                        }
                        else
                        {
                            var data = w.bytes;
                            File.WriteAllBytes(URLToLocalPath(url), data);

                            callback(resp, new LocalCacheEntry()
                            {
                                Type = LocalCacheEntry.CacheType.Raw,
                                Bytes = data
                            });
                        }
                    }));
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 生成网络返回结构
    /// </summary>
    /// <param name="www"></param>
    /// <returns></returns>
    private HttpResp GenerateGetfileResp(WWW www)
    {
        var resp = new HttpResp();

        if (!string.IsNullOrEmpty(www.error))
        {
            resp.Error = HttpResp.ErrorType.NetworkError;
            resp.ErrorText = www.error;
        }
        else
        {
            resp.Error = HttpResp.ErrorType.None;
            resp.WwwText = www.text;
        }

        return resp;
    }

   
    #endregion
}

/// <summary>
/// 资源类型
/// </summary>
public class LocalCacheEntry
{
    public CacheType Type { get; set; }

    public Texture2D Texture { get; set; }
    public string FbxPath { get; set; }
    public AssetBundle AB { get; set; }
    public byte[] Bytes { get; set; }

    public enum CacheType
    {
        Texture,
        Fbx,
        AssetBundle,
        Raw
    }
}

/// <summary>
/// 网络请求返回数据结构
/// </summary>
public class HttpResp
{
    public string WwwText { get; set; }
    public ErrorType Error { get; set; }
    internal string ErrorText { get; set; }

    /// <summary>
    /// 在Error的时候，会tostring成error内容
    /// </summary>
    public override string ToString()
    {
        if (Error != ErrorType.None)
        {
            return ErrorText;
        }
        else
        {
            return WwwText;
        }
    }

    public enum ErrorType
    {
        None,
        NetworkError,
        AccessExpired,
        LogicError
    }
}
