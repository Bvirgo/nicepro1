using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuildMeshTest : MonoBehaviour
{

    private List<Vector3> m_pMesh = new List<Vector3>();

    private Camera m_mianCamera;

    private bool m_bStartPoint = false;

    private GameObject m_objUp;

    private GameObject m_objDown;

    public Text m_txt;
	// Use this for initialization
	void Start () 
    {
        m_mianCamera = Camera.main;

        m_txt.gameObject.SetActive(m_bStartPoint);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (m_bStartPoint)
        {
            m_txt.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 vNewPos = GetWorldPos();
                Debug.Log("当前屏幕坐标：" + Input.mousePosition + "--世界坐标Hit：" + vNewPos);
                if (!m_pMesh.Contains(vNewPos))
                {
                    m_pMesh.Add(vNewPos);
                }

                if (m_pMesh.Count > 2)
                {
                    CreateMesh();
                }
            }       
        }

        if (Input.GetMouseButtonDown(1))
        {
            m_bStartPoint = false;
            m_txt.gameObject.SetActive(m_bStartPoint);
            m_pMesh.Clear();
        }
	}

    /// <summary>
    /// 射线检测获取世界坐标
    /// </summary>
    /// <returns></returns>
    Vector3 GetWorldPos()
    {
        Ray ray = m_mianCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] pHit = Physics.RaycastAll(ray);
        for (int i = 0; i < pHit.Length;++i )
        {
            RaycastHit hit = pHit[i];
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return hit.point;
            }
        }
        return Vector3.zero;
    }

    public void OnBtn1()
    {
        m_bStartPoint = !m_bStartPoint;

        m_txt.gameObject.SetActive(m_bStartPoint);

        if (!m_bStartPoint)
        {
            m_pMesh.Clear();
        }
    }

    private void CreateMesh()
    {
        if (m_objDown != null)
        {
            Destroy(m_objDown);
        }

        if (m_objUp != null)
        {
            Destroy(m_objUp);
        }

        Texture2D tex = Texture2D.whiteTexture;
        float tile = 5.0f;
        m_objUp = BuildMesh.Create(m_pMesh, tex, tile);
        m_objUp.name = "greenUp";
        m_objUp.AddComponent<MeshCollider>();

        m_pMesh.Reverse();
        m_objDown = BuildMesh.Create(m_pMesh, tex, tile);
        m_objDown.name = "greenDown";
        m_objDown.AddComponent<MeshCollider>();
    }

    public void OnBtn2()
    {

    }

    public void OnBtn3()
    {

    }
}
