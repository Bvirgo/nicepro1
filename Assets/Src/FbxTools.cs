using UnityEngine;
using System.Collections;
using Jhqc.UnityFbxLoader;
using System.Windows.Forms;
using System.IO;

public class FbxTools : MonoBehaviour
{
    GameObject createModelByByteArr(byte[] byteArr, string fileName)
    {
        GameObject go;
        if (fileName.EndsWith("FBX") || fileName.EndsWith("fbx"))
        {
            var error = Jhqc.UnityFbxLoader.FbxLoader.LoadFbx(byteArr, out go);

            if ((error & ErrorCode.ChildNodeNotReset) != 0)
            {
                return go;
            }
        }   
        else
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);

        return go;
    }

    //这个方法不能用，OpenFileDialog编译不过，不知道为什么
    public void ImportOneFBX()
    {
        //OpenFileDialog ofd = new OpenFileDialog();
        //ofd.Filter = "Fbx(*.fbx),xml(*.xml)|*.fbx;*.xml;*.assetbundle";
        //ofd.Multiselect = false;
        //ofd.Title = "请选择需要导入的fbx文件";
        //var ret = ofd.ShowDialog();
        //if (ret == DialogResult.OK)
        //{
        //    string[] fileList = ofd.FileNames;

        //    for (int i = 0; i < fileList.Length; ++i)
        //    {
        //        string fileUrl = fileList[i];

        //        ResData d = LoadResource(fileUrl);

        //        createModelByByteArr(d.m_pBins, fileUrl);

        //    }
        //}
    }

    public ResData LoadResource(string _strPath)
    {
        ResData data = new ResData();
        if (File.Exists(_strPath))
        {

            using (var file = File.OpenRead(_strPath))
            {
                long filesize = file.Length;
                byte[] fbin = new byte[filesize];
                file.BeginRead(fbin, 0, (int)filesize, ar =>
                {
                    int bytesRead = file.EndRead(ar);
                    if (bytesRead == (int)filesize)
                    {
                        data.m_pBins = fbin;
                        data.m_strCrc = Crc32.CountCrc(fbin).ToString();
                    }
                    else
                    {
                        Debug.LogError("Read File Fail!");
                    }
                },
                   null);
            }
        }
        else
        {
            Debug.LogError(" File Dont Found!");

        }

        return data;
    }
}

public class ResData
{
    public ResData()
    {
        m_strCrc = "Error_Crc";
        m_pBins = null;
    }

    public string m_strCrc;

    public byte[] m_pBins;
  
}
