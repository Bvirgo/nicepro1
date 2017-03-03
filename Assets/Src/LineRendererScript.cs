using UnityEngine;
using System.Collections;

public class LineRendererScript : MonoBehaviour
{
    [SerializeField]
    private LineRenderer BodyLineRenderer;
    [SerializeField]
    private LineRenderer HeadLineRenderer;

    private bool IsFinished = false;
    private bool IsReady = false;
    private Vector3 StartPos;
    private Vector3 EndPos;

    private float zDepth = 10f;
    private float headWidth = 0.4f;
    private float bodyWidth = 0.3f;

    void Awake()
    {
        if (BodyLineRenderer == null)
            BodyLineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (IsFinished)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            IsReady = true;
            StartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartPos.z = zDepth;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            IsReady = false;
            IsFinished = true;
        }

        if (IsReady)
        {
            EndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            EndPos.z = zDepth;

            BodyLineRenderer.SetPosition(0, StartPos);
            BodyLineRenderer.SetPosition(1, EndPos);


            // 设置头部  
            Vector3 HeadStart = EndPos;
            Vector3 HeadEnd = HeadStart;

            Vector3 direction = EndPos - StartPos;

            // 根据当前线的长短，适当的调整箭头的大小  
            if (direction.magnitude > 2)
            {
                headWidth = 0.4f;
                bodyWidth = 0.2f;
            }
            else if (direction.magnitude > 1)
            {
                headWidth = 0.3f;
                bodyWidth = 0.15f;
            }
            else
            {
                headWidth = 0.2f;
                bodyWidth = 0.1f;
            }
            HeadLineRenderer.SetWidth(headWidth, 0);
            BodyLineRenderer.SetWidth(0, bodyWidth);

            direction.Normalize();
            HeadEnd = HeadEnd + direction * headWidth;

            HeadLineRenderer.SetPosition(0, HeadStart);
            HeadLineRenderer.SetPosition(1, HeadEnd);
        }
    }

}