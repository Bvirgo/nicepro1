using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorTool : MonoBehaviour
{
    /// <summary>
    /// 需要手动指定AssetName
    /// </summary>
    [MenuItem("Tools/Simple Build")]
    static void BuildABs()
    {
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("Assets/ABs", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/BuildVersion")]
    public static void CreateVersion()
    {
        var vc = ScriptableObject.CreateInstance<VersionConfig>();
        string strOutPath = "Assets/Resources/version.asset";
        AssetDatabase.CreateAsset(vc, "Assets/Resources/version.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 清除Ab
        ClearAssetBundlesName();

        // 打包
        AssetImporter assetImporter = AssetImporter.GetAtPath(strOutPath);
        assetImporter.assetBundleName = "version.assetbundle";


        //根据BuildSetting里面所激活的平台进行打包  
        BuildPipeline.BuildAssetBundles("Assets/ABs", 0, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();

        Debug.Log("打包完成");
    }

    /// <summary>  
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包  
    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下  
    /// </summary>  
    static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
    }
}
