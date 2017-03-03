using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public class CalcLngLat : MonoBehaviour {

    public InputField ipt_lngDis;
    public InputField ipt_lngOffset;


    public InputField ipt_latDis;
    public InputField ipt_latOffset;

    public Text txt_Res;

    public void OnCalc()
    {

        float lngDistance = float.Parse(ipt_lngDis.text);
        float lngOffset = float.Parse(ipt_lngOffset.text);
        float LngRatio = Convert.ToSingle(lngOffset / lngDistance);

        float latDistance = float.Parse(ipt_latDis.text);
        float latOffset = float.Parse(ipt_latOffset.text);
        float LatRatio = Convert.ToSingle(latOffset / latDistance);

        string strRes = "LngRat:" + LngRatio.ToString("F15") + "\n" + "LatRat:" + LatRatio.ToString("F15");

        txt_Res.text = strRes;

        Debug.Log(strRes);
    }
}
