using UnityEngine;
using System.Collections;

public class BoxSelector : MonoBehaviour {
private Color  NormalColor   ;
private Color  HilightColor   ;
 bool OverBox    = false;
 GizmoController GC    = null;


void Start(){
	if(!GetComponent<Renderer>())
		return;
		
	NormalColor = GetComponent<Renderer>().material.color;
	HilightColor = new Color(NormalColor.r * 1.2f, NormalColor.g * 1.2f, NormalColor.b * 1.2f, 1f);
	GC = (GizmoController)GameObject.Find("GizmoAdvanced").GetComponent("GizmoController");
	GC.Hide();
}//Start

void OnMouseEnter(){
	GetComponent<Renderer>().material.color = HilightColor;
	OverBox = true;
}//OnMouseEnter

void OnMouseExit(){
	GetComponent<Renderer>().material.color = NormalColor;
	OverBox = false;
}//OnMouseEnter

void OnMouseDown(){	
	if(GC == null)
		return;
		
	if(GC.IsOverAxis())
		return;
	
	GC.SetSelectedObject(transform);
	
	if(GC.IsHidden())
		GC.Show(GIZMO_MODE.TRANSLATE);
}//OnMouseDown
}
