﻿using UnityEngine;
using System.Collections;
using MyFrameWork;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Text;

public static class Helper
{
    #region Txt To Image

    static System.Drawing.Font _fontDefault = new System.Drawing.Font("宋体", 20);// = new System.Drawing.Font("宋体", 20);
    public static Bitmap TextToBitmapAllDefault(string text)
    {
        return TextToBitmap(text, _fontDefault, Rectangle.Empty, System.Drawing.Color.Black, System.Drawing.Color.White);
    }

    /// <summary>
    /// 把文字转换才Bitmap
    /// new System.Drawing.Font("宋体", 12)
    /// Rectangle.Empty
    /// Brushes.Black
    /// System.Drawing.Color.White
    /// </summary>
    /// <param name="text"></param>
    /// <param name="font"></param>
    /// <param name="rect">用于输出的矩形，文字在这个矩形内显示，为空时自动计算</param>
    /// <param name="fontcolor">字体颜色</param>
    /// <param name="backColor">背景颜色</param>
    /// <returns></returns>
    public static Bitmap TextToBitmap(string text, System.Drawing.Font font, Rectangle rect, System.Drawing.Color fontColor,
        System.Drawing.Color backColor)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }
        System.Drawing.Graphics g;
        Bitmap bmp;
        StringFormat format = new StringFormat(StringFormatFlags.NoClip);
        //new PointF() 
        if (rect == Rectangle.Empty)
        {
            bmp = new Bitmap(1, 1);
            g = System.Drawing.Graphics.FromImage(bmp);
            var Sz = TextRenderer.MeasureText(g, text, font);
            bmp.Dispose();
            bmp = new Bitmap(Sz.Width, Sz.Height);
            rect = new Rectangle(0, 0, Sz.Width, Sz.Height);

            //bmp = new Bitmap(1, 1);
            //g = System.Drawing.Graphics.FromImage(bmp);
            ////计算绘制文字所需的区域大小（根据宽度计算长度），重新创建矩形区域绘图
            //SizeF sizef = g.MeasureString(text, font, PointF.Empty, format);

            //int width = (int)(sizef.Width + 1);
            //int height = (int)(sizef.Height + 1);
            //rect = new Rectangle(0, 0, width, height);
            //bmp.Dispose();
            //bmp = new Bitmap(width, height);
        }
        else
        {
            bmp = new Bitmap(rect.Width, rect.Height);
        }

        g = System.Drawing.Graphics.FromImage(bmp);

        //使用ClearType字体功能
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.FillRectangle(new SolidBrush(backColor), rect);
        //g.DrawString(text, font, Brushes.Black, rect, format);
        TextRenderer.DrawText(g, text, font, rect, fontColor);
        //bmp.Save("fontphoto");
        return bmp;
    }

    /// <summary>
    /// 扩展BipMap的大小
    /// </summary>
    /// <param name="isUsePercent"> if true, 0 < pamrams < 1 of new size</param>
    public static Bitmap ExtendBitMap(Bitmap bmp, float up, float down, float left, float right, bool isUsePercent = false)
    {
        Rect rect = new Rect(0, 0, bmp.Width, bmp.Height);
        rect = rect.Expand(up, down, left, right, isUsePercent);
        Bitmap newBmp = new Bitmap(FloatCeilingToInt(rect.width), FloatCeilingToInt(rect.height));
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBmp);
        g.FillRectangle(new SolidBrush(bmp.GetPixel(0, 0)), 0, 0, newBmp.Width, newBmp.Height);

        left = isUsePercent ? rect.width * left : left;
        up = isUsePercent ? rect.height * up : up;
        g.DrawImage(bmp, left, up, bmp.Width, bmp.Height);
        return newBmp;
    }

    public static Texture2D BitMapToTexture2D(Bitmap picBitMap)
    {
        if (null == picBitMap)
        {
            return null;
        }

        Texture2D ret = new Texture2D(picBitMap.Width, picBitMap.Height);
        if (ret.LoadImage(ImageToByteArrayDefault(picBitMap)))
        {
            return ret;
        }
        return null;
    }

    public static byte[] ImageToByteArrayDefault(System.Drawing.Image imageIn)
    {
        return ImageToByteArray(imageIn, System.Drawing.Imaging.ImageFormat.Png);
    }

    public static byte[] ImageToByteArray(System.Drawing.Image imageIn, System.Drawing.Imaging.ImageFormat saveFormat)
    {
        using (var ms = new MemoryStream())
        {
            imageIn.Save(ms, saveFormat);
            return ms.ToArray();
        }
    }

    public static System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
    {
        MemoryStream ms = new MemoryStream(byteArrayIn);
        System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
        return returnImage;
    }
    #endregion

    #region Int

    /// <summary>
    /// 之后可以和其他方法测试效率
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNumber(string str)
    {
        try
        {
            int.Parse(str);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    #endregion

    #region Vector3

    /// <summary>
    /// 等比缩放Vector3
    /// </summary>
    /// <param name="tar"></param>
    /// <param name="multi"></param>
    /// <returns></returns>
    public static Vector3 GetScaledVector(this Vector3 tar, float multi)
    {
        return new Vector3(tar.x * multi, tar.y * multi, tar.z * multi);
    }
    #endregion

    #region 导入文件统一入口
    private const string FILEFILTER = "Fbx(*.fbx),xml(*.xml)|*.fbx;*.xml;*.assetbundle|*.txt";
    //private const string FILEFILTER = "Fbx(*.fbx)|*.fbx";
    /// <summary>
    /// 导入文件，统一入口
    /// </summary>
    /// <param name="_strFilter">后缀名过滤</param>
    /// <returns></returns>
    public static string[] ImportFile(string _strFilter = FILEFILTER, string _strDefDir = null)
    {
        string[] fileList = new string[] { "" };
        //var ofd = new OpenFileDialog();
        //ofd.Filter = _strFilter;
        //ofd.Multiselect = true;

        //// 设置默认目录
        //if (!string.IsNullOrEmpty(_strDefDir))
        //{
        //    ofd.InitialDirectory = _strDefDir;
        //}

        //ofd.Title = "请选择需要导入的文件（可以多选）";
        //var ret = ofd.ShowDialog();

        //if (ret == DialogResult.OK)
        //{
        //    fileList = ofd.FileNames;
        //}

        return fileList;
    }
    #endregion

    #region 文件保存调试信息
    public static void SaveInfo(string _strMsg, string _strFileName = "CityEditorInfo", string _strType = ".txt", bool _bAppend = false)
    {
        string strPath = UnityEngine.Application.streamingAssetsPath + "\\" + _strFileName + _strType;
        byte[] myByte = System.Text.Encoding.UTF8.GetBytes(_strMsg);
        //using (FileStream fsWrite = new FileStream(@"D:\CityEditorInfo.txt", FileMode.Append))
        using (FileStream fsWrite = new FileStream(strPath, _bAppend ? FileMode.Append : FileMode.Create))
        {
            fsWrite.Write(myByte, 0, myByte.Length);
        };
    }
    #endregion

    #region Math
    public static int FloatCeilingToInt(float f)
    {
        return (int)Math.Ceiling((double)f);
    }
    #endregion Math

    #region 三种截屏方案

    /// 使用Application类下的CaptureScreenshot()方法实现截图  
    /// 优点：简单，可以快速地截取某一帧的画面、全屏截图  
    /// 缺点：不能针对摄像机截图，无法进行局部截图  
    /// </summary>  
    /// <param name="mFileName">M file name.</param>  
    public static void CaptureByUnity(string mFileName)
    {
        ScreenCapture.CaptureScreenshot(mFileName, 0);
    }

    /// <summary>  
    /// 根据一个Rect类型来截取指定范围的屏幕  
    /// 左下角为(0,0)  
    /// </summary>  
    /// <param name="mRect">M rect.</param>  
    /// <param name="mFileName">M file name.</param>  
    public static IEnumerator CaptureByRect(Rect mRect, string mFileName)
    {
        //等待渲染线程结束  
        yield return new WaitForEndOfFrame();
        //初始化Texture2D  
        Texture2D mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);
        //读取屏幕像素信息并存储为纹理数据  
        mTexture.ReadPixels(mRect, 0, 0);
        //应用  
        mTexture.Apply();


        //将图片信息编码为字节信息  
        byte[] bytes = mTexture.EncodeToPNG();
        //保存  
        System.IO.File.WriteAllBytes(mFileName, bytes);

        //如果需要可以返回截图  
        //return mTexture;  
    }

    public static IEnumerator CaptureByCamera(Camera mCamera, Rect mRect, string mFileName)
    {
        //等待渲染线程结束  
        yield return new WaitForEndOfFrame();

        //初始化RenderTexture  
        RenderTexture mRender = new RenderTexture((int)mRect.width, (int)mRect.height, 0);
        //设置相机的渲染目标  
        mCamera.targetTexture = mRender;
        //开始渲染  
        mCamera.Render();

        //激活渲染贴图读取信息  
        RenderTexture.active = mRender;

        Texture2D mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);
        //读取屏幕像素信息并存储为纹理数据  
        mTexture.ReadPixels(mRect, 0, 0);
        //应用  
        mTexture.Apply();

        //释放相机，销毁渲染贴图  
        mCamera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(mRender);

        //将图片信息编码为字节信息  
        byte[] bytes = mTexture.EncodeToPNG();
        //保存  
        System.IO.File.WriteAllBytes(mFileName, bytes);

        //如果需要可以返回截图  
        //return mTexture;  
    }
    #endregion

    #region Unicode转中文
    public static string UnicodeToGB(string text)
    {
        System.Text.RegularExpressions.MatchCollection mc = System.Text.RegularExpressions.Regex.Matches(text, "\\\\u([\\w]{4})");
        if (mc != null && mc.Count > 0)
        {
            foreach (System.Text.RegularExpressions.Match m2 in mc)
            {
                string v = m2.Value;
                string word = v.Substring(2);
                byte[] codes = new byte[2];
                int code = Convert.ToInt32(word.Substring(0, 2), 16);
                int code2 = Convert.ToInt32(word.Substring(2), 16);
                codes[0] = (byte)code2;
                codes[1] = (byte)code;
                text = text.Replace(v, Encoding.Unicode.GetString(codes));
            }
        }
        else
        {
            text = "Unicode Error";
        }
        return text;
    }
    #endregion

    #region File
    public static string GetStandardPath(string path)
    {
        int loopNum = 20;
        path = path.Replace(@"\", @"/");
        while (path.IndexOf(@"//") != -1)
        {
            path = path.Replace(@"//", @"/");
            loopNum--;
            if (loopNum < 0)
            {
                return path;
            }
        }
        return path;
    }

    /// <summary>获取文件名后缀</summary>
    public static string GetFilePostfix(string fileName)
    {
        if (fileName == null)
            return null;
        string res;
        if (fileName.IndexOf(".") == -1)
            res = "";
        else
        {
            string[] ss = fileName.Split(new char[1] { '.' });
            res = ss[ss.Length - 1];
        }
        return res;
    }

    public static string GetFolderPath(string path, bool fullPath = true)
    {
        path = GetStandardPath(path);
        if (fullPath)//获取全路径
        {
            if (path.LastIndexOf(@"/") == path.Length - 1)
                return GetFolderPath(path.Substring(0, path.Length - 1));
            else
                return path.Substring(0, path.LastIndexOf(@"/") + 1);
        }
        else//获取父级文件夹名
        {
            string[] strArr = path.Split('/');

            if (path.LastIndexOf(@"/") == path.Length - 1)
                return strArr[strArr.Length - 2];
            else
                return strArr[strArr.Length - 1];
        }
    }

    public static string GetParentFolderPath(string path, bool fullPath = true)
    {
        path = GetStandardPath(path);
        if (fullPath)//获取全路径
        {
            if (path.LastIndexOf(@"/") == path.Length - 1)
                return GetFolderPath(path.Substring(0, path.Length - 1));
            else
                return path.Substring(0, path.LastIndexOf(@"/") + 1);
        }
        else//获取父级文件夹名
        {
            string[] strArr = path.Split('/');
            return strArr[strArr.Length - 2];
        }
    }

    public static string GetFileName(string path, bool needPostfix = false)
    {
        path = GetStandardPath(path);
        string fileFolderPath = path.Substring(0, path.LastIndexOf(@"/") + 1);

        string fileName = path.Substring(path.LastIndexOf("/") + 1, path.Length - fileFolderPath.Length);
        if (needPostfix)
            return fileName;
        else
            return fileName.Substring(0, fileName.LastIndexOf("."));
    }

    public static bool IsPic(string fileName)
    {
        string postFix = Helper.GetFilePostfix(fileName);
        return postFix == "png"
            || postFix == "PNG"
            || postFix == "jpg"
            || postFix == "JPG"
            || postFix == "jpeg"
            || postFix == "JPEG";
    }
    #endregion

    #region Debug
    public static void Debug(string _strTips, byte _bLevel = 1)
    {
        switch (_bLevel)
        {
            case 1:
                UnityEngine.Debug.Log(_strTips);
                break;

            case 2:
                UnityEngine.Debug.LogWarning(_strTips);
                break;

            case 3:
                UnityEngine.Debug.LogError(_strTips);
                break;

            default:
                break;
        }
    }
    #endregion

    #region Vector4

    public static Vector4  GetV4(string _str)
    {
        Vector4 vRes = Vector4.zero;
        string[] pStr = _str.Split(',');
        if (pStr.Length != 4)
        {
            return vRes;
        }

        try
        {
            float x = float.Parse(pStr[0]);
            float y = float.Parse(pStr[1]);
            float z = float.Parse(pStr[2]);
            float w = float.Parse(pStr[3]);
            vRes = new Vector4(x, y, z, w);
        }
        catch (Exception)
        {
            Helper.Debug("解析float字符串失败！", 2);
            return vRes;
        }

        return vRes;
    }

    #endregion
}

public static class UnityEngineExtention
{
    /// <summary>
    /// Expand rect size
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="up"></param>
    /// <param name="down"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="isUsePercent"> if true, 0 < pamrams < 1 of new size</param>
    /// <returns></returns>
    public static Rect Expand(this Rect rect, float up, float down, float left, float right, bool isUsePercent = false)
    {
        if (isUsePercent)
        {
            // perUp + perDown + rect.height/newHeight = 1
            float newHeight = rect.height / (1 - up - down);
            float newWidth = rect.width / (1 - left - right);

            rect.x -= (newWidth - rect.width) / 2;
            rect.y -= (newHeight - rect.height) / 2;
            rect.width = newWidth;
            rect.height = newHeight;
        }
        else
        {
            rect.x -= left;
            rect.y -= up;
            rect.width += left + right;
            rect.height += up + down;
        }

        return rect;
    }
}
