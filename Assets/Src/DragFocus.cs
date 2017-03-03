using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Vectrosity;
using System.Collections.Generic;
using System;
public class DragFocus : MonoBehaviour,IDragHandler,IDropHandler
{
    private Vector2[] points;

    // 区域左上角
    private Vector2 m_vStart;

    // 区域右上角
    private Vector2 m_vEnd;

    public Material m_matLine;
    public Material m_matCirc;
    VectorLine squareLine;

    VectorLine m_curline;

    Action<Rect> m_actCutPic;

    List<VectorLine> m_pLine = new List<VectorLine>();

    Rect m_rect;

    public enum drawType 
    {
        Line,
        Rect
    }

    private drawType m_curType = drawType.Rect;
	// Use this for initialization
	void Start ()
    {

        points = new Vector2[] { Vector2.zero, Vector2.zero};

	}

    /// <summary>
    /// 默认先画框
    /// </summary>
    /// <param name="_actRect"></param>
    public void ChooseRect(Action<Rect> _actRect)
    {
        // 清除之前
        CleanLine();

        squareLine = new VectorLine("Square", new Vector2[8], m_matLine, 3.0f, LineType.Discrete, Joins.Weld);
        m_pLine.Add(squareLine);
        gameObject.SetActive(true);
        m_actCutPic = _actRect;
        m_curType = drawType.Rect;
    }

    /// <summary>
    /// 拉线
    /// </summary>
    public void DrawLine()
    {
        gameObject.SetActive(true);
        m_curType = drawType.Line;
        m_curline = new VectorLine("Line", points, m_matLine, 3.0f, LineType.Discrete, Joins.Weld);
        m_pLine.Add(m_curline);
    }

	// Update is called once per frame
	void Update () 
    {
	    if (Input.GetMouseButtonDown(0))
        {
            transform.position = Input.mousePosition;
            m_vStart = Input.mousePosition;
        }
	}

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        // 拉框，选择截屏区域
        if (m_curType == drawType.Rect)
        {
            float fW = Mathf.Abs(Input.mousePosition.x - m_vStart.x);
            float fH = Mathf.Abs(Input.mousePosition.y - m_vStart.y);
            //m_rect = new Rect(m_vStart.x, m_vStart.y, fW, fH);
            // Rect 是以左下角为起点
            m_rect = new Rect(m_vStart.x, m_vStart.y - fH, fW, fH);
            CreateRect(m_vStart.x, m_vStart.y, fW, fH, Color.red);
        }
        //else if (IsInRect(Input.mousePosition))
        else if (m_rect.Contains(Input.mousePosition))
        {          
            DrawLine(m_vStart, Input.mousePosition);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (m_curType == drawType.Line)
        {
            DrawCirc(Input.mousePosition);
        }
        else
        {
            m_actCutPic(m_rect);
            m_vEnd = Input.mousePosition;
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 绘制批注线
    /// </summary>
    /// <param name="_vStart"></param>
    /// <param name="_vEnd"></param>
    private void DrawLine(Vector2 _vStart, Vector2 _vEnd)
    {
        points[0] = _vStart;
        points[1] = _vEnd;
        m_curline.Resize(points);
        m_curline.Draw();
    }

    /// <summary>
    /// 绘制矩形：Rect左上角为起点
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    private void CreateRect(float posX, float posY, float width, float height, Color color)
    {
        squareLine.MakeRect(new Rect(posX, posY, width, height));
        squareLine.SetColor(color);
        squareLine.Draw();
    }

    /// <summary>
    /// 画圆
    /// </summary>
    /// <param name="_vCenter"></param>
    void DrawCirc(Vector2 _vCenter)
    {
        VectorLine circleLine = new VectorLine("Circle", new Vector2[100], m_matCirc, 10.0f, LineType.Discrete, Joins.Weld);
        circleLine.MakeCircle(_vCenter, 5);
        circleLine.SetColor(Color.green);
        circleLine.Draw();
        m_pLine.Add(circleLine);
    }

    /// <summary>
    /// 清除描线
    /// </summary>
    public void CleanLine()
    {
        VectorLine.Destroy(m_pLine);
    }
}
