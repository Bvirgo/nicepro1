using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Drawing;
public class CanvasPic : MonoBehaviour
{
    public Transform m_focus;
	
	// Update is called once per frame
	void Update () 
    {
        m_focus.position = Input.mousePosition;
	}

    public void OnDrawArrow()
    {
        m_focus.gameObject.SetActive(true);
    }
}
