using UnityEngine;
using System.Collections;

public class NewCutScreen : MonoBehaviour
{

    public DragFocus m_objFocus;

    //定义图片保存路径  
    private string mPath1;
    private string mPath2;
    private string mPath3;

    Rect rect = new Rect(0, 0, 1024, 768);

    //相机  
    //public Transform CameraTrans;

    public Camera m_cam;

    void Start()
    {
        //初始化路径  
        mPath1 = Application.dataPath + "\\ScreenShot\\ScreenShot_New_1.png";
        mPath2 = Application.dataPath + "\\ScreenShot\\ScreenShot_New_2.png";
        mPath3 = Application.dataPath + "\\ScreenShot\\ScreenShot_New_3.png";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("Pos:" + Input.mousePosition);
        }
    }

    #region 三种截屏方案

    //主方法，使用UGUI实现  
    void OnGUI()
    {
        // 全屏截图
        if (GUILayout.Button("全屏截图", GUILayout.Height(30)))
        {
            Helper.CaptureByUnity(mPath1);
        }

        // 可以选择区域
        if (GUILayout.Button("区域截图", GUILayout.Height(30)))
        {

            //StartCoroutine(Helper.CaptureByRect(new Rect(0, 0, 1024, 768), mPath2));
            
            // 指定截屏区域： 这个是左下角半屏幕
            //rect = new Rect(Screen.width *0f, Screen.height *0f,Screen.width*0.5f ,Screen.height*0.5f);
            StartCoroutine(Helper.CaptureByRect(rect, mPath2));
        }

        if (GUILayout.Button("选择截图区域", GUILayout.Height(30)))
        {
            m_objFocus.ChooseRect((r) =>
                {
                    rect = r;
                    Debug.Log("选定区域：" + rect);
                });
        }

        if (GUILayout.Button("拉线", GUILayout.Height(30)))
        {
            m_objFocus.DrawLine();
        }
    }
    #endregion
}
