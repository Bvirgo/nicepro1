using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MyFrameWork;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
public class TxtToImg : MonoBehaviour
{
    public RenderTexture tx;
    public InputField m_iptTxt;
    public InputField m_iptScale;
    public UnityEngine.UI.Image m_img;

    public Text m_txt;

    public Texture m_dyTx;

    private bool m_bGUI = false;

    private int fW = 260;
    private int fH = 325;
    public Rect rect;
	// Use this for initialization
	void Start () 
    {
	    rect = new Rect(250, 70, fW, fH);
	}

    public void CreateImg()
    {
        Debug.Log("Txt Size:"+ tx.width +"--H:"+tx.height);

        m_txt.text = m_iptTxt.text;

        m_bGUI = true;

        Texture2D myTexture2D = new Texture2D(tx.width, tx.height);
        RenderTexture.active = tx;
        myTexture2D.ReadPixels(new Rect(0, 0, tx.width, tx.height), 0, 0);
        myTexture2D.Apply();
        m_dyTx = myTexture2D;
        RenderTexture.active = null;

        //m_img.overrideSprite = Sprite.Create(myTexture2D, new Rect(0, 0, myTexture2D.width, myTexture2D.height), Vector2.zero);
        //m_img.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width, tx.height);

    }

    public void TxtToImg1()
    {
        m_txt.text = m_iptTxt.text;

        m_bGUI = true;

        Bitmap bm = Helper.TextToBitmapAllDefault(m_txt.text);

        m_dyTx = Helper.BitMapToTexture2D(bm);
    }


    void OnGUI()
    {
        if (m_bGUI)
        {
            rect = GUI.Window(0, rect, DoMyWindow, "图片");
        }
    }

    void DoMyWindow(int windowID)
    {
        GUI.Box(new Rect(5, 20, fW - 10, fH - 25), m_dyTx);

        if (GUI.Button(new Rect(fW - 85, fH - 55, 80, 50), "刷新"))
        {

        }

        if (GUI.Button(new Rect(fW - 170, fH - 55, 80, 50), "放大"))
        {
            Debug.Log("放大！" + "--size:" + rect.size);
            fW += 5;
            fH += 5;
            Texture2D myTexture2D = new Texture2D(m_dyTx.width + 5, m_dyTx.height + 5);
            RenderTexture.active = tx;
            
            myTexture2D.ReadPixels(new Rect(0, 0, tx.width, tx.height), 0, 0);
            myTexture2D.Apply();
            m_dyTx = myTexture2D;
            RenderTexture.active = null;

            m_img.overrideSprite = Sprite.Create(myTexture2D, new Rect(0, 0, myTexture2D.width, myTexture2D.height), Vector2.zero);
            m_img.GetComponent<RectTransform>().sizeDelta = new Vector2(myTexture2D.width, myTexture2D.height);
            m_dyTx = myTexture2D;
            rect.size = rect.size + new Vector2(5, 5);
        }

        GUI.DragWindow(new Rect(0, 0, fW, fH));
        //  
    }
}
