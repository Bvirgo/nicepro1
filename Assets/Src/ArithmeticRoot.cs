using UnityEngine;
using System.Collections;

public class ArithmeticRoot : MonoBehaviour 
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void OnBtn1()
    {
        int[] a = new int[] { 23,15,1,58,2,79,56,16,13,23,87};
        string str1 = "";
        for (int i = 0; i < a.Length;++i )
        {
            str1 += a[i].ToString() + "--";            
        }

        Debug.Log("初始值："+ str1);

        BFPTR.MyBFPTR(a, 0, a.Length - 1, 4);

        str1 = "";
        for (int i = 0; i < a.Length; ++i)
        {
            str1 += a[i].ToString() + "--";
        }

        Debug.Log("排序过后：" + str1);
    }

    public void OnBtn2()
    {

    }

    public void OnBtn3()
    {

    }
}
