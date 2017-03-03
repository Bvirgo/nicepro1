using UnityEngine;
using System.Collections;

public static class BFPTR 
{
    //插入排序  
    static void InsertSort(int[] a, int l, int r)
    {
        for (int i = l + 1; i <= r; i++)
        {
            if (a[i - 1] > a[i])
            {
                int t = a[i];
                int j = i;
                while (j > l && a[j - 1] > t)
                {
                    a[j] = a[j - 1];
                    j--;
                }
                a[j] = t;
            }
        }
    }

    //寻找中位数的中位数  
    static int FindMid(int[] a, int l, int r)
    {
        if (l == r) return a[l];
        int i = 0;
        int n = 0;
        for (i = l; i < r - 5; i += 5)
        {
            InsertSort(a, i, i + 4);
            n = i - l;
            //swap(a[l + n / 5], a[i + 2]);
            Swap(a, l + n / 5, i + 2);
        }

        //处理剩余元素  
        int num = r - i + 1;
        if (num > 0)
        {
            InsertSort(a, i, i + num - 1);
            n = i - l;
            //swap(a[l + n / 5], a[i + num / 2]);
            Swap(a, 1+ n/5, i + num/2);
        }
        n /= 5;
        if (n == l) return a[l];
        return FindMid(a, l, l + n);
    }

    private static void Swap(int[] a, int _nSource , int _nDes)
    {
        if (a != null && _nSource > -1 && _nSource < a.Length && _nDes > -1 && _nDes < a.Length)
        {
            int nTemp = a[_nSource];
            a[_nSource ]= a[_nDes];
            a[_nDes] = nTemp;
        }
    }

    //寻找中位数的所在位置  
    static int FindId(int[] a, int l, int r, int num)
    {
        for (int i = l; i <= r; i++)
            if (a[i] == num)
                return i;
        return -1;
    }

    //进行划分过程  
    static int Partion(int[] a, int l, int r, int p)
    {
        //swap(a[p], a[l]);
        Swap(a, p, l);
        int i = l;
        int j = r;
        int pivot = a[l];
        while (i < j)
        {
            while (a[j] >= pivot && i < j)
                j--;
            a[i] = a[j];
            while (a[i] <= pivot && i < j)
                i++;
            a[j] = a[i];
        }
        a[i] = pivot;
        return i;
    }

    /// <summary>
    /// 数组排序o（n）
    /// </summary>
    /// <param name="a">数组</param>
    /// <param name="l">起始位置</param>
    /// <param name="r">结束位置</param>
    /// <param name="k">TopK</param>
    /// <returns></returns>
   public static int MyBFPTR(int[] a, int l, int r, int k)
    {
        int num = FindMid(a, l, r);    //寻找中位数的中位数  
        int p = FindId(a, l, r, num); //找到中位数的中位数对应的id  
        int i = Partion(a, l, r, p);

        int m = i - l + 1;
        if (m == k) return a[i];
        if (m > k) return MyBFPTR(a, l, i - 1, k);
        return MyBFPTR(a, i + 1, r, k - m);
    }

}
