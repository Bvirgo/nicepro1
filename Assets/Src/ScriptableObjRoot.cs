using UnityEngine;
using System.Collections;

public class ScriptableObjRoot : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadSobj());
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator LoadSobj()
    {
        WWW www = new WWW("file://" + Application.dataPath + "/ABs/version.assetbundle");
        yield return www;

        //转换资源为VersionConfig，这个sd对象将拥有原来在编辑器中设置的数据。                
        VersionConfig sd = www.assetBundle.mainAsset as VersionConfig;
        VersionConfig vc = www.assetBundle.LoadAsset<VersionConfig>("version");
        if (sd != null)
        {
            print(sd.version);
        }
        if (vc != null)
        {
            print(vc.version);
        }
    }
}


public class VersionConfig : ScriptableObject
{
    public string version;
}