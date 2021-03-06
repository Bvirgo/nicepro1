﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Windows.Forms;
using UnityEngine.UI;

/// <summary>  
/// 把Resource下的资源打包成.unity3d 到StreamingAssets目录下  
/// 自动遍历设置AssetName
/// </summary>  
public class MyBuilder : MonoBehaviour
{
    public Text m_txt;
    //public static string sourcePath = Application.dataPath + "/Resources";

    //获取选择的目录
    //[HideInInspector]
    //public  string sourcePath = AssetDatabase.GetAssetPath(Selection.activeObject);
    private string sourcePath;
    const string AssetBundlesOutputPath = "Assets/StreamingAssets";

    public  void BuildAssetBundle()
    {

        //FolderBrowserDialog fd = new FolderBrowserDialog();
        //if (fd.ShowDialog() == DialogResult.OK)
        //{

        //    sourcePath = fd.SelectedPath;
        //    m_txt.text = sourcePath;
        //    if (string.IsNullOrEmpty(sourcePath))
        //    {
        //        m_txt.text = "当前选中目录错误,请重新选择";
        //        return;
        //    }
        //}


        m_txt.text = "Application.dataPath:" + UnityEngine.Application.dataPath +"\n";
        m_txt.text += "Application.persistentDataPath:" + UnityEngine.Application.persistentDataPath;

        //string _source = Replace(sourcePath);
        //Debug.Log("_source path:" + _source);
        //string _assetPath = "Assets" + _source.Substring(UnityEngine.Application.dataPath.Length);

        //Debug.Log("UnityEngine.Application.dataPath:" + UnityEngine.Application.dataPath);

        //ClearAssetBundlesName();

        //Pack(sourcePath);

        //// 根据当前平台来创建保存文件夹
        //string outputPath = Path.Combine(AssetBundlesOutputPath, Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));
        //if (!Directory.Exists(outputPath))
        //{
        //    Directory.CreateDirectory(outputPath);
        //}

        ////根据BuildSetting里面所激活的平台进行打包  
        //BuildPipeline.BuildAssetBundles(outputPath, 0, EditorUserBuildSettings.activeBuildTarget);

        //AssetDatabase.Refresh();

        //Debug.Log("打包完成");

    }

    /// <summary>  
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包  
    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下  
    /// </summary>  
     void ClearAssetBundlesName()
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

    /// <summary>
    /// 递归查找指定文件夹下的所有文件
    /// </summary>
    /// <param name="source"></param>
     void Pack(string source)
    {
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            // 如果这个文件是目录,递归到每个文件
            if (files[i] is DirectoryInfo)
            {
                Pack(files[i].FullName);
            }
            else
            {
                if (!files[i].Name.EndsWith(".meta") && !files[i].Name.EndsWith(".txt"))
                {
                    SetAssetName2(files[i].FullName);
                    //SetAssetName(files[i].FullName);
                }
            }
        }
    }

    // 每个文件都打成独立的包
     void SetAssetName(string source)
    {
        // Eg: _source: E:/BuildAB/Assets/Resources/ZP028007001/sign/ZP028007001001.jpg
        // _assetpath:Assets/Resources/ZP028007001/sign/ZP028007001001.jpg
        // _assetPath2:Resources/ZP028007001/sign/ZP028007001001.jpg
        // AssetBundleName:ZP028007001/sign/ZP028007001001.unity3d
        // folderName:ZP028007001

        string _source = Replace(source);
        //Debug.Log("_source path:"+ _source);
        string _assetPath = "Assets" + _source.Substring(UnityEngine.Application.dataPath.Length);
        string _assetPath2 = _source.Substring(UnityEngine.Application.dataPath.Length + 1);
        //Debug.Log("_assetpath:"+ _assetPath);
        //Debug.Log("_assetPath2:"+ _assetPath2);

        //在代码中给资源设置AssetBundleName  
        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
        string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);
        // 替换文件后缀
        assetName = assetName.Replace(Path.GetExtension(assetName), ".assetbundle");
        //Debug.Log("AssetBundleName:"+assetName);  
        assetImporter.assetBundleName = assetName;
    }

    // 打包指定文件夹中所有文件为一个整包，包名：文件夹名
     void SetAssetName2(string source)
    {
        // Eg: _source: E:/BuildAB/Assets/Resources/ZP028007001/sign/ZP028007001001.jpg
        // _assetpath:Assets/Resources/ZP028007001/sign/ZP028007001001.jpg
        // _assetPath2:Resources/ZP028007001/sign/ZP028007001001.jpg
        // AssetBundleName:ZP028007001/sign/ZP028007001001.unity3d
        // folderName:ZP028007001

        string _source = Replace(source);
        //Debug.Log("_source path:" + _source);
        string _assetPath = "Assets" + _source.Substring(UnityEngine.Application.dataPath.Length);
        string _assetPath2 = _source.Substring(UnityEngine.Application.dataPath.Length + 1);
        Debug.Log("_assetpath:" + _assetPath);
        Debug.Log("_assetPath2:" + _assetPath2);

        //在代码中给资源设置AssetBundleName  
        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
        string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);
        Debug.Log("assetName:" + assetName);
        //string folderName = assetName.Substring(0, assetName.IndexOf("/"));
        string folderName = _assetPath2.Substring(0, _assetPath2.IndexOf("/"));
        Debug.Log("folerName:" + folderName);
        assetImporter.assetBundleName = folderName;
    }

     // 打包指定文件夹中所有文件为一个整包，包名：文件夹名
     void SetAssetName3(string source)
     {
         // Eg: _source: E:/BuildAB/Assets/Resources/ZP028007001/sign/ZP028007001001.jpg
         // _assetpath:Assets/Resources/ZP028007001/sign/ZP028007001001.jpg
         // _assetPath2:Resources/ZP028007001/sign/ZP028007001001.jpg
         // AssetBundleName:ZP028007001/sign/ZP028007001001.unity3d
         // folderName:ZP028007001

         string _source = Replace(source);
         //Debug.Log("_source path:" + _source);
         string _assetPath = "Assets" + _source.Substring(UnityEngine.Application.dataPath.Length);
         string _assetPath2 = _source.Substring(UnityEngine.Application.dataPath.Length + 1);
         Debug.Log("_assetpath:" + _assetPath);
         Debug.Log("_assetPath2:" + _assetPath2);

         //在代码中给资源设置AssetBundleName  
         AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
         string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);
         Debug.Log("assetName:" + assetName);
         //string folderName = assetName.Substring(0, assetName.IndexOf("/"));
         string folderName = _assetPath2.Substring(0, _assetPath2.IndexOf("/"));
         Debug.Log("folerName:" + folderName);
         assetImporter.assetBundleName = folderName;
     }


    static string Replace(string s)
    {
        return s.Replace("\\", "/");
    }
}

public class Platform
{
    public static string GetPlatformFolder(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "IOS";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSX:
                return "OSX";
            default:
                return null;
        }
    }
}