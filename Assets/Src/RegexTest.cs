using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
public class RegexTest : MonoBehaviour 
{

    public InputField txt_Regex;
    public InputField txt_Source;

    public Text txt_Res;

    public void OnMath(string _strRegex)
    {
        Regex gx = new Regex(txt_Regex.text);
        if (gx.IsMatch(txt_Source.text))
        {
            txt_Res.text = "符合要求!";
            txt_Res.color = Color.green;
        }
        else
        {
            txt_Res.text = "滚蛋！!";
            txt_Res.color = Color.red;
        }

       Regex gx1 = new  Regex(@"^\d+$");
       if (gx1.IsMatch(txt_Source.text))
       {
           Debug.Log("满足");
       }
       else
       {
           Debug.Log("滚蛋");
       }
    }
}
