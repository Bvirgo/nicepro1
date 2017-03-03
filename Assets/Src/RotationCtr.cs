using UnityEngine;
using System.Collections;

public class RotationCtr : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        GizmosRotate.Instance.SetRotate(transform.eulerAngles);
        GizmosRotate.Instance.SetPosition(transform.position);

	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 vEA = GizmosRotate.Instance.GetRotate();
        transform.eulerAngles = vEA;
	}
}
