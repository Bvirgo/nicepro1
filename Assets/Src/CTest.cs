using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class CTest : MonoBehaviour {

    public Texture tex;
    private Rect rect;
    // Use this for initialization  
    void Start()
    {
        //rect = new Rect(0, 0, 400, 400);
        rect = new Rect(250, 70, fW, fH);
    }

    // Update is called once per frame  
    void Update()
    {

    }

    //void OnGUI()
    //{
    //    //GUI.DrawTexture(new Rect(0, 0, 405,397), test);  
    //    rect = GUI.Window(0, rect, DoMyWindow, "Test");
    //}
    //void DoMyWindow(int windowID)
    //{
    //    GUI.Box(new Rect(0, 0, 250, 300), tex);
    //    //GUI.DrawTexture(new Rect(0, 0, 405, 397), tex);
    //    GUI.DragWindow(new Rect(0, 0, 400, 400));
    //    //  
    //}  

    private float fW = 260;
    private float fH = 325;
    void OnGUI()
    {

       rect = GUI.Window(0, rect, DoMyWindow, "街景");

    }

    string ABurl = "http://uploads.pek3a.qingstor.com/v1/1470969096712.zip";
    string imgUrl = "http://uploads.pek3a.qingstor.com/v1/zhoukou626";

    void DoMyWindow(int windowID)
    {
        GUI.Box(new Rect(5, 20, fW - 10, fH - 25), tex);

        if (GUI.Button(new Rect(fW - 85, fH - 55, 80, 50), "网络图片"))
        {
            //EventManager.instance.DispatchEvent(null, UICONST.REFRESH_STREET, null);

            MyWWWMgr.Instance.GetAb(ABurl, (w) => { });

           MyWWWMgr.Instance.GetFile(imgUrl, LocalCacheEntry.CacheType.Texture, (resp, entry) =>
            {
                if (resp.Error.Equals(HttpResp.ErrorType.None))
                {
                    //callback(File.ReadAllBytes(entry.FbxPath));
                    tex = entry.Texture;
                }
            });
        }

        if (GUI.Button(new Rect(fW - 170, fH - 55, 80, 50), "放大"))
        {
            Debug.Log("放大！"+"--size:"+rect.size);
            fW += 5;
            fH += 5;
            //rect = new Rect(250, 70, fW, fH);
            rect.size = rect.size + new Vector2(5,5);
        }

        //GUI.DrawTexture(new Rect(0, 0, 405, 397), tex);
        GUI.DragWindow(new Rect(0, 0, fW, fH));
        //  
        Event e = Event.current;
        Debug.Log("Event:"+e.type);
        switch(e.type)
        {
            case EventType.MouseDown:
                Debug.Log("MuseDown");
                break;
            case EventType.DragUpdated:
                Debug.Log("DragUpdated:"+ e.delta);
                break;
            case EventType.DragPerform:
                Debug.Log("DragPerform:" + e.delta);
                break;
        }
    }
}
