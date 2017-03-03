using UnityEngine;
using System.Collections;
using System.IO;
using System.Drawing;
using UnityEngine.UI;

public class CommbinImg : MonoBehaviour {

    public UnityEngine.UI.Image img;
    enum ImageMergeOrientation
    {
        Horizontal,
        Vertical
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnCombinImg()
    {
        const string folderPath = "D:\\Pictures";
        var images = new DirectoryInfo(folderPath).GetFiles("*.png", SearchOption.TopDirectoryOnly);

        CombineImages(images, "C:/FinalImage_H.png",ImageMergeOrientation.Horizontal);
        CombineImages(images, "C:/FinalImage_V.png", ImageMergeOrientation.Vertical);  
    }

    private void CombineImages(FileInfo[] files, string toPath, ImageMergeOrientation mergeType = ImageMergeOrientation.Vertical)
    {
        //change the location to store the final image.
        // URL：http://www.bianceng.cn/Programming/csharp/201410/45751.htm
        var finalImage = toPath;

        // 获取原始图片的长度宽度
        var finalWidth = 1;
        var finalHeight = 1;

        foreach(FileInfo file in files)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(file.FullName);
            if (mergeType == ImageMergeOrientation.Vertical)
            {
                finalWidth = finalWidth > img.Width ? finalWidth : img.Width;
                finalHeight += img.Height;
            }
            else if (mergeType == ImageMergeOrientation.Horizontal)
            {
                finalHeight = finalHeight > img.Height ? finalHeight : img.Height;
                finalWidth += img.Width;
            }
        }

        var finalImg = new Bitmap(finalWidth, finalHeight);
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImg);
        g.Clear(SystemColors.AppWorkspace);

        var width = finalWidth;
        var height = finalHeight;
        var nIndex = 0;
        foreach (FileInfo file in files)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(file.FullName);
            if (nIndex == 0)
            {
                g.DrawImage(img, new Point(0, 0));
                nIndex++;
                width = img.Width;
                height = img.Height;
            }
            else
            {
                switch (mergeType)
                {
                    case ImageMergeOrientation.Horizontal:
                        g.DrawImage(img, new Point(width, 0));
                        width += img.Width;
                        break;
                    case ImageMergeOrientation.Vertical:
                        g.DrawImage(img, new Point(0, height));
                        height += img.Height;
                        break;
                }
            }
            img.Dispose();
        }
        g.Dispose();
        finalImg.Save(finalImage, System.Drawing.Imaging.ImageFormat.Png);
        finalImg.Dispose();
    }
}
