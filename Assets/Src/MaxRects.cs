using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NewEditorNS;
using LitJson;
using System.IO;

public class MaxRects : MonoBehaviour
{
    static MaxRects instance;
    static GameObject mainGO;
    public static MaxRects Instance
    {
        get
        {
            if (mainGO == null)
            {
                mainGO = new GameObject();
                mainGO.name = "MaxRects";
                instance = mainGO.AddComponent<MaxRects>();
            }
            return instance;
        }
    }


    List<rectData> oriRect;

    Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();

    private KeyCode InputKey = 0;

    private int counter = 0;

    // 每个图集---源图片列表
    private Dictionary<string, List<rectData>> histDic = new Dictionary<string, List<rectData>>();
    private Dictionary<KeyCode, int> inputReactDic;

    int key = 0;

    // Use this for initialization
    void Start()
    {
        key = 0;
    }

    Texture2D ScaleTextureBilinear(Texture2D originalTexture, float scaleFactor)
    {
        Texture2D newTexture = new Texture2D(Mathf.CeilToInt(originalTexture.width * scaleFactor), Mathf.CeilToInt(originalTexture.height * scaleFactor));
        float scale = 1.0f / scaleFactor;
        int maxX = originalTexture.width - 1;
        int maxY = originalTexture.height - 1;
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                // Bilinear Interpolation
                float targetX = x * scale;
                float targetY = y * scale;
                int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                int x2 = Mathf.Min(maxX, x1 + 1);
                int y2 = Mathf.Min(maxY, y1 + 1);

                float u = targetX - x1;
                float v = targetY - y1;
                float w1 = (1 - u) * (1 - v);
                float w2 = u * (1 - v);
                float w3 = (1 - u) * v;
                float w4 = u * v;
                Color color1 = originalTexture.GetPixel(x1, y1);
                Color color2 = originalTexture.GetPixel(x2, y1);
                Color color3 = originalTexture.GetPixel(x1, y2);
                Color color4 = originalTexture.GetPixel(x2, y2);
                Color color = new Color(Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                    Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                    Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                    Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                newTexture.SetPixel(x, y, color);
            }
        }
        return newTexture;
    }


    public JsonData Combine(string atlasPath, Texture2D[] textureArr, string atlasName,List<string> picNameList)
    {
        InputKey = KeyCode.Alpha1;
        refreshRect();

        for (int texIndex = 0; texIndex < textureArr.Length; texIndex++)
        {
            Texture2D tex = textureArr[texIndex];
            rectData data = new rectData();
            data.key = Helper.GetFileName(picNameList[texIndex], true);
            data.texture = tex;
            data.rect = new Rect(0, 0, tex.width, tex.height);
            if(tex.width > tex.height && tex.width > 1024)
            {
                float sacal = 1f / ((float)tex.width / (float)1024);
                float w = sacal * tex.width;
                float h = tex.height * sacal;
                data.rect = new Rect(0, 0, w, h);
                data.texture = ScaleTextureBilinear(tex, sacal);
            }
            if(tex.width < tex.height && tex.height > 1024)
            {
                float sacal = 1f / ((float)tex.height / (float)1024);
                float w = sacal * tex.width;
                float h = tex.height * sacal;
                data.rect = new Rect(0, 0, w, h);
                data.texture = ScaleTextureBilinear(tex, sacal);
            }
            oriRect.Add(data);
        }
        List<rectData> rects = new List<rectData>();

        for (int j = 0, length = oriRect.Count; j < length; j++)
        {
            rects.Add(oriRect[j]);
        }
        key = 0;
        CopyOri(rects, atlasName);
        DrawCanvas(atlasPath, atlasName);

        return SaveAtlas(atlasPath, atlasName); ;
    }

    JsonData SaveAtlas(string path, string name)
    {
        JsonData _jd = new JsonData();
        string s = "";
        //List<rectData> item = histDic[key.ToString()];
        foreach (var item in histDic)
        {
            JsonData jsd = JsonMapper.ToObject(JsonMapper.ToJson(new List<JsonData>()));
            foreach (var mItem in item.Value)
            {
                JsonData js = new JsonData();
                js["tile"] = mItem.key;
                js["atlas"] = item.Key + ".png";// = JsonMapper.ToObject(JsonMapper.ToJson(new List<JsonData>()));
                js["rect"] = mItem.GetRectData();
                _jd.Add(js);
                jsd.Add(js);
            }
            s = path;
            Directory.CreateDirectory(s);
            s = s + "/"  + item.Key;
        }
        foreach (var mItem in textureList)
        {
            s = path;
            mItem.Value.Apply();
            s = s + "/" + mItem.Key;
            File.WriteAllBytes(s + ".png", mItem.Value.EncodeToPNG());
        }
        return _jd;
    }

    //图集按2n裁切空白
    void DrawCanvas(string atlasPath, string atlasName)
    {
        foreach (var item in histDic)
        {
            int hMax = 0;
            int wMax = 0;
            foreach (var mItem in item.Value)
            {
                if (mItem.rect.yMax > hMax)
                {
                    hMax = (int)mItem.rect.yMax;
                }
                if (mItem.rect.xMax > wMax)
                {
                    wMax = (int)mItem.rect.xMax;
                }
            }
            // 最接近hMax的2的幂值
            hMax = Mathf.NextPowerOfTwo(hMax);
            wMax = Mathf.NextPowerOfTwo(wMax);
            Texture2D texture = new Texture2D(wMax, hMax);

            //小图等比缩放
            foreach (var mItem in item.Value)
            {
                mItem.altasWidth = wMax;
                mItem.altasHeight = hMax;
            }

            foreach (var mItem in item.Value)
            {
                if (mItem.rect != new Rect())
                {
                    Color[] color = mItem.texture.GetPixels();
                    Debug.Log(mItem.rect);
                    texture.SetPixels((int)mItem.rect.x, (int)mItem.rect.y, (int)mItem.rect.width, (int)mItem.rect.height, color);
                    counter++;
                }
            }
            if(texture != null)
                textureList.Add(item.Key, texture);
        }
        //SaveAtlas(atlasPath, atlasName);
    }


    void CopyOri(List<rectData> _ori,string atlasName)
    {
        if (_ori == null || _ori.Count == 0)
        {
            //Debug.Log("完成");
            return;
        }
        List<Rect> rc = new List<Rect>();
        foreach (var item in _ori)
        {
            rc.Add(item.rect);
        }
        List<rectData> _surplus = new List<rectData>();
        List<rectData> mItemp = new List<rectData>();

        // 图集组合优化： 一个图集，固定size，源图片中选择最佳组合，最大化利用图集size
        MaxRectsBinPack temp = new MaxRectsBinPack(1024, 1024, false);
        temp.insert2(rc, new List<Rect>(), inputReactDic[InputKey]);

        for (int i = 0; i < _ori.Count; i++)
        {
            Rect rect = temp.usedRectangles[i];
            rectData data = new rectData();
            data = _ori[i];
            if (rect == new Rect())
            {
                _surplus.Add(data);
            }
            else
            {
                data.rect = rect;
                mItemp.Add(data);
            }
        }
        if (mItemp.Count != 0)
        {
            histDic.Add((atlasName + "_"+(++key)).ToString(), mItemp);
            CopyOri(_surplus, atlasName);
        }
        else
        {
            foreach (var item in _surplus)
            {
                Debug.LogError("图片大于1024相素 ： " + item.key);
                //NGUIDebug.Log("图片大于1024相素 ： " + item.key);
            }
        }
    }

    void refreshRect()
    {
        oriRect = new List<rectData>();
        oriRect.Clear();
        histDic.Clear();
        textureList.Clear();
    }

    void Awake()
    {

        inputReactDic = new Dictionary<KeyCode, int>();
        inputReactDic.Add(KeyCode.Alpha1, FreeRectangleChoiceHeuristic.BestAreaFit);
        inputReactDic.Add(KeyCode.Alpha2, FreeRectangleChoiceHeuristic.BestLongSideFit);
        inputReactDic.Add(KeyCode.Alpha3, FreeRectangleChoiceHeuristic.BestShortSideFit);
        inputReactDic.Add(KeyCode.Alpha4, FreeRectangleChoiceHeuristic.BottomLeftRule);
        inputReactDic.Add(KeyCode.Alpha5, FreeRectangleChoiceHeuristic.ContactPointRule);
    }
}

public class FreeRectangleChoiceHeuristic
{
    public const int BestShortSideFit = 0; ///< -BSSF: Positions the Rectangle against the short side of a free Rectangle into which it fits the best.
    public const int BestLongSideFit = 1; ///< -BLSF: Positions the Rectangle against the long side of a free Rectangle into which it fits the best.
    public const int BestAreaFit = 2; ///< -BAF: Positions the Rectangle into the smallest free Rectangle into which it fits.
    public const int BottomLeftRule = 3; ///< -BL: Does the Tetris placement.
    public const int ContactPointRule = 4; ///< -CP: Choosest the placement where the Rectangle touches other Rectangles as much as possible.
}

public class MaxRectsBinPack
{
    public int binWidth = 0;
    public int binHeight = 0;
    public bool allowRotations = false;

    public List<Rect> usedRectangles = new List<Rect>();
    public List<Rect> freeRectangles = new List<Rect>();

    private int score1 = 0; // Unused in this function. We don't need to know the score after finding the position.
    private int score2 = 0;
    private int bestShortSideFit;
    private int bestLongSideFit;

    public MaxRectsBinPack(int width, int height, bool rotations = true)
    {
        init(width, height, rotations);
    }


    public void init(int width, int height, bool rotations = true)
    {
        if (count(width) % 1 != 0 || count(height) % 1 != 0)
        {
            Debug.Log("Must be 2,4,8,16,32,...512,1024,...");
        }
        binWidth = width;
        binHeight = height;
        allowRotations = rotations;

        Rect n = new Rect();
        n.x = 0;
        n.y = 0;
        n.width = width;
        n.height = height;

        usedRectangles = new List<Rect>();

        freeRectangles = new List<Rect>();
        freeRectangles.Add(n);
    }

    private float count(float n)
    {
        if (n >= 2)
            return count(n / 2);
        return n;
    }

    /**
        * Insert a new Rectangle 
        * @param width
        * @param height
        * @param method
        * @return 
        * 
        */
 
    public void insert2(List<Rect> Rectangles, List<Rect> dst, int method)
    {
        if (dst == null)
            dst = new List<Rect>();
        else
            dst.Clear();

        while (Rectangles.Count > 0)
        {
            int bestScore1 = int.MaxValue;
            int bestScore2 = int.MaxValue;
            int bestRectangleIndex = -1;
            Rect bestNode = new Rect();

            for (int i = 0; i < Rectangles.Count; ++i)
            {
                int score1 = 0;
                int score2 = 0;
                Rect newNode = scoreRectangle((int)Rectangles[i].width, (int)Rectangles[i].height, (int)method, score1, score2);

                if (score1 < bestScore1 || (score1 == bestScore1 && score2 < bestScore2))
                {
                    bestScore1 = score1;
                    bestScore2 = score2;
                    bestNode = newNode;
                    bestRectangleIndex = i;
                }
            }

            if (bestRectangleIndex == -1)
                return;

            placeRectangle(bestNode);
            Rectangles.RemoveAt(bestRectangleIndex);//Rectangles.splice(bestRectangleIndex,1);
        }
    }

    private void placeRectangle(Rect node)
    {
        int numRectanglesToProcess = freeRectangles.Count;
        for (int i = 0; i < numRectanglesToProcess; i++)
        {
            if (splitFreeNode(freeRectangles[i], node))
            {
                freeRectangles.RemoveAt(i);//freeRectangles.splice(i,1);
                --i;
                --numRectanglesToProcess;
            }
        }

        pruneFreeList();

        usedRectangles.Add(node);
    }

    private Rect scoreRectangle(int width, int height, int method,
                                        int score1, int score2)
    {
        Rect newNode = new Rect();
        score1 = int.MaxValue;
        score2 = int.MaxValue;
        switch (method)
        {
            case FreeRectangleChoiceHeuristic.BestShortSideFit:
                newNode = findPositionForNewNodeBestShortSideFit(width, height);
                break;
            case FreeRectangleChoiceHeuristic.BottomLeftRule:
                newNode = findPositionForNewNodeBottomLeft(width, height, score1, score2);
                break;
            case FreeRectangleChoiceHeuristic.ContactPointRule:
                newNode = findPositionForNewNodeContactPoint(width, height, score1);
                // todo: reverse
                score1 = -score1; // Reverse since we are minimizing, but for contact point score bigger is better.
                break;
            case FreeRectangleChoiceHeuristic.BestLongSideFit:
                newNode = findPositionForNewNodeBestLongSideFit(width, height, score2, score1);
                break;
            case FreeRectangleChoiceHeuristic.BestAreaFit:
                newNode = findPositionForNewNodeBestAreaFit(width, height, score1, score2);
                break;
        }

        // Cannot fit the current Rectangle.
        if (newNode.height == 0)
        {
            score1 = int.MaxValue;
            score2 = int.MaxValue;
        }

        return newNode;
    }

    private Rect findPositionForNewNodeBottomLeft(int width, int height, int bestY, int bestX)
    {
        Rect bestNode = new Rect();
        //memset(bestNode, 0, sizeof(Rectangle));

        bestY = int.MaxValue;
        Rect rect;
        int topSideY;
        for (int i = 0; i < freeRectangles.Count; i++)
        {
            rect = freeRectangles[i];
            // Try to place the Rectangle in upright (non-flipped) orientation.
            if (rect.width >= width && rect.height >= height)
            {
                topSideY = (int)(rect.y + height);
                if (topSideY < bestY || (topSideY == bestY && rect.x < bestX))
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = width;
                    bestNode.height = height;
                    bestY = topSideY;
                    bestX = (int)rect.x;
                }
            }
            if (allowRotations && rect.width >= height && rect.height >= width)
            {
                topSideY = (int)(rect.y + width);
                if (topSideY < bestY || (topSideY == bestY && rect.x < bestX))
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = height;
                    bestNode.height = width;
                    bestY = topSideY;
                    bestX = (int)rect.x;
                }
            }
        }
        return bestNode;
    }

    private Rect findPositionForNewNodeBestShortSideFit(int width, int height)
    {
        Rect bestNode = new Rect();
        //memset(&bestNode, 0, sizeof(Rectangle));

        bestShortSideFit = int.MaxValue;
        bestLongSideFit = score2;
        Rect rect;
        int leftoverHoriz;
        int leftoverVert;
        int shortSideFit;
        int longSideFit;

        for (int i = 0; i < freeRectangles.Count; i++)
        {
            rect = freeRectangles[i];
            // Try to place the Rectangle in upright (non-flipped) orientation.
            if (rect.width >= width && rect.height >= height)
            {
                leftoverHoriz = (int)Mathf.Abs(rect.width - width);
                leftoverVert = (int)Mathf.Abs(rect.height - height);
                shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
                longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);

                if (shortSideFit < bestShortSideFit || (shortSideFit == bestShortSideFit && longSideFit < bestLongSideFit))
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = width;
                    bestNode.height = height;
                    bestShortSideFit = shortSideFit;
                    bestLongSideFit = longSideFit;
                }
            }
            float flippedLeftoverHoriz;
            float flippedLeftoverVert;
            float flippedShortSideFit;
            float flippedLongSideFit;
            if (allowRotations && rect.width >= height && rect.height >= width)
            {
                flippedLeftoverHoriz = Mathf.Abs(rect.width - height);
                flippedLeftoverVert = Mathf.Abs(rect.height - width);
                flippedShortSideFit = Mathf.Min(flippedLeftoverHoriz, flippedLeftoverVert);
                flippedLongSideFit = Mathf.Max(flippedLeftoverHoriz, flippedLeftoverVert);

                if (flippedShortSideFit < bestShortSideFit || (flippedShortSideFit == bestShortSideFit && flippedLongSideFit < bestLongSideFit))
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = height;
                    bestNode.height = width;
                    bestShortSideFit = (int)flippedShortSideFit;
                    bestLongSideFit = (int)flippedLongSideFit;
                }
            }
        }

        return bestNode;
    }

    private Rect findPositionForNewNodeBestLongSideFit(int width, int height, int bestShortSideFit, int bestLongSideFit)
    {
        Rect bestNode = new Rect();
        //memset(&bestNode, 0, sizeof(Rectangle));
        bestLongSideFit = int.MaxValue;
        Rect rect;

        int leftoverHoriz;
        int leftoverVert;
        int shortSideFit;
        int longSideFit;
        for (int i = 0; i < freeRectangles.Count; i++)
        {
            rect = freeRectangles[i];
            // Try to place the Rectangle in upright (non-flipped) orientation.
            if (rect.width >= width && rect.height >= height)
            {
                leftoverHoriz = (int)Mathf.Abs(rect.width - width);
                leftoverVert = (int)Mathf.Abs(rect.height - height);
                shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
                longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);

                if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit))
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = width;
                    bestNode.height = height;
                    bestShortSideFit = shortSideFit;
                    bestLongSideFit = longSideFit;
                }
            }

            if (allowRotations && rect.width >= height && rect.height >= width)
            {
                leftoverHoriz = (int)Mathf.Abs(rect.width - height);
                leftoverVert = (int)Mathf.Abs(rect.height - width);
                shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
                longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);

                if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit))
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = height;
                    bestNode.height = width;
                    bestShortSideFit = shortSideFit;
                    bestLongSideFit = longSideFit;
                }
            }
        }
        //Debug.Log(bestNode);
        return bestNode;
    }

    private Rect findPositionForNewNodeBestAreaFit(int width, int height, int bestAreaFit, int bestShortSideFit)
    {
        Rect bestNode = new Rect();
        //memset(&bestNode, 0, sizeof(Rectangle));

        bestAreaFit = int.MaxValue;

        Rect rect;

        int leftoverHoriz;
        int leftoverVert;
        int shortSideFit;
        int areaFit;

        for (int i = 0; i < freeRectangles.Count; i++)
        {
            rect = freeRectangles[i];
            areaFit = (int)(rect.width * rect.height - width * height);

            // Try to place the Rectangle in upright (non-flipped) orientation.
            if (rect.width >= width && rect.height >= height)
            {
                leftoverHoriz = (int)Mathf.Abs(rect.width - width);
                leftoverVert = (int)Mathf.Abs(rect.height - height);
                shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);

                if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit))
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = width;
                    bestNode.height = height;
                    bestShortSideFit = shortSideFit;
                    bestAreaFit = areaFit;
                }
            }

            if (allowRotations && rect.width >= height && rect.height >= width)
            {
                leftoverHoriz = (int)Mathf.Abs(rect.width - height);
                leftoverVert = (int)Mathf.Abs(rect.height - width);
                shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);

                if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit))
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = height;
                    bestNode.height = width;
                    bestShortSideFit = shortSideFit;
                    bestAreaFit = areaFit;
                }
            }
        }
        return bestNode;
    }

    /// Returns 0 if the two intervals i1 and i2 are disjoint, or the length of their overlap otherwise.
    private float commonIntervalLength(float i1start, float i1end, float i2start, float i2end)
    {
        if (i1end < i2start || i2end < i1start)
            return 0;
        return Mathf.Min(i1end, i2end) - Mathf.Max(i1start, i2start);
    }

    private float contactPointScoreNode(float x, float y, float width, float height)
    {
        float score = 0;

        if (x == 0 || x + width == binWidth)
            score += height;
        if (y == 0 || y + height == binHeight)
            score += width;
        Rect rect;
        for (int i = 0; i < usedRectangles.Count; i++)
        {
            rect = usedRectangles[i];
            if (rect.x == x + width || rect.x + rect.width == x)
                score += (int)commonIntervalLength(rect.y, rect.y + rect.height, y, y + height);
            if (rect.y == y + height || rect.y + rect.height == y)
                score += (int)commonIntervalLength(rect.x, rect.x + rect.width, x, x + width);
        }
        return score;
    }

    private Rect findPositionForNewNodeContactPoint(int width, int height, int bestContactScore)
    {
        Rect bestNode = new Rect();
        //memset(&bestNode, 0, sizeof(Rectangle));

        bestContactScore = -1;

        Rect rect;
        float score;
        for (int i = 0; i < freeRectangles.Count; i++)
        {
            rect = freeRectangles[i];
            // Try to place the Rectangle in upright (non-flipped) orientation.
            if (rect.width >= width && rect.height >= height)
            {
                score = contactPointScoreNode(rect.x, rect.y, width, height);
                if (score > bestContactScore)
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = width;
                    bestNode.height = height;
                    bestContactScore = (int)score;
                }
            }
            if (allowRotations && rect.width >= height && rect.height >= width)
            {
                score = contactPointScoreNode(rect.x, rect.y, height, width);
                if (score > bestContactScore)
                {
                    bestNode.x = rect.x;
                    bestNode.y = rect.y;
                    bestNode.width = height;
                    bestNode.height = width;
                    bestContactScore = (int)score;
                }
            }
        }
        return bestNode;
    }

    private bool splitFreeNode(Rect freeNode, Rect usedNode)
    {
        // Test with SAT if the Rectangles even intersect.
        if (usedNode.x >= freeNode.x + freeNode.width || usedNode.x + usedNode.width <= freeNode.x ||
            usedNode.y >= freeNode.y + freeNode.height || usedNode.y + usedNode.height <= freeNode.y)
            return false;
        Rect newNode;
        if (usedNode.x < freeNode.x + freeNode.width && usedNode.x + usedNode.width > freeNode.x)
        {
            // New node at the top side of the used node.
            if (usedNode.y > freeNode.y && usedNode.y < freeNode.y + freeNode.height)
            {
                newNode = new Rect(freeNode);
                newNode.height = usedNode.y - newNode.y;
                freeRectangles.Add(newNode);
            }

            // New node at the bottom side of the used node.
            if (usedNode.y + usedNode.height < freeNode.y + freeNode.height)
            {
                newNode = new Rect(freeNode);
                newNode.y = usedNode.y + usedNode.height;
                newNode.height = freeNode.y + freeNode.height - (usedNode.y + usedNode.height);
                freeRectangles.Add(newNode);
            }
        }

        if (usedNode.y < freeNode.y + freeNode.height && usedNode.y + usedNode.height > freeNode.y)
        {
            // New node at the left side of the used node.
            if (usedNode.x > freeNode.x && usedNode.x < freeNode.x + freeNode.width)
            {
                newNode = new Rect(freeNode);
                newNode.width = usedNode.x - newNode.x;
                freeRectangles.Add(newNode);
            }

            // New node at the right side of the used node.
            if (usedNode.x + usedNode.width < freeNode.x + freeNode.width)
            {
                newNode = new Rect(freeNode);
                newNode.x = usedNode.x + usedNode.width;
                newNode.width = freeNode.x + freeNode.width - (usedNode.x + usedNode.width);
                freeRectangles.Add(newNode);
            }
        }

        return true;
    }

    private void pruneFreeList()
    {
        for (int i = 0; i < freeRectangles.Count; i++)
            for (int j = i + 1; j < freeRectangles.Count; j++)
            {
                if (isContainedIn(freeRectangles[i], freeRectangles[j]))
                {

                    freeRectangles.RemoveAt(i);//freeRectangles.splice(i,1);
                    break;
                }
                if (isContainedIn(freeRectangles[j], freeRectangles[i]))
                {
                    freeRectangles.RemoveAt(j);//freeRectangles.splice(j,1);
                }
            }
    }

    private bool isContainedIn(Rect a, Rect b)
    {
        return a.x >= b.x && a.y >= b.y
            && a.x + a.width <= b.x + b.width
            && a.y + a.height <= b.y + b.height;
    }
}

public class rectData
{
    public string key;
    public Rect rect = new Rect();
    public Texture2D texture;

    public int altasWidth = 1024;
    public int altasHeight = 1024;

    public JsonData ToJsonData()
    {
        JsonData _jd = new JsonData();
        _jd["key"] = key;
        _jd["rect"] = GetJsonData();
        return _jd;
    }

    public string GetRectData()
    {
        return rect.x / altasWidth + "," + rect.y / altasHeight + "," + rect.width / altasWidth + "," + rect.height / altasHeight;
    }

    public JsonData GetJsonData()
    {
        JsonData _jd = new JsonData();
        _jd["x"] = rect.x / 1024f;
        _jd["y"] = rect.y / 1024f;
        _jd["w"] = rect.width / 1024f;
        _jd["h"] = rect.height / 1024f;
        return _jd;
    }
}