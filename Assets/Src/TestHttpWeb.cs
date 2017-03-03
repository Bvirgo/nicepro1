using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System;
using System.Threading;
using System.Text;

public class RequestState
{
    const int m_buffetSize = 1024;
    public StringBuilder m_requestData;
    public byte[] m_bufferRead;
    public HttpWebRequest m_request;
    public HttpWebResponse m_response;
    public Stream m_streamResponse;

    public RequestState()
    {
        m_bufferRead = new byte[m_buffetSize];
        m_requestData = new StringBuilder("");
        m_request = null;
        m_streamResponse = null;
    }
}

public class TestHttpWeb : MonoBehaviour
{

    FileStream fileStream = null;

    // 周口地块切图
    string strPath = "http://uploads.pek3a.qingstor.com/v1/zhoukou626";

    // Use this for initialization  
    void Start()
    {
        fileStream = new FileStream("disunity_v0.3.4.zip", FileMode.Create);


        DownloadMusicAsyn();
    }

    void DownloadMusicAsyn()
    {
        Debug.Log("DownloadMusicAsyn Thread Start");

        try
        {

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(strPath);

            RequestState myRequestState = new RequestState();
            myRequestState.m_request = myHttpWebRequest;

            Debug.Log("BeginGetResponse Start");
            //异步获取;  
            IAsyncResult result = (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

            Debug.Log("BeginGetResponse End");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }


    }

    void RespCallback(IAsyncResult result)
    {
        Debug.Log("RespCallback 0");

        try
        {
            RequestState myRequestState = (RequestState)result.AsyncState;
            HttpWebRequest myHttpWebRequest = myRequestState.m_request;

            Debug.Log("RespCallback EndGetResponse");
            myRequestState.m_response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(result);

            Stream responseStream = myRequestState.m_response.GetResponseStream();
            myRequestState.m_streamResponse = responseStream;

            //开始读取数据;  
            IAsyncResult asyncreadresult = responseStream.BeginRead(myRequestState.m_bufferRead, 0, 1024, new AsyncCallback(ReadCallBack), myRequestState);

            return;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }




    void ReadCallBack(IAsyncResult result)
    {
        Debug.Log("ReadCallBack");
        try
        {
            RequestState myRequestState = (RequestState)result.AsyncState;
            Stream responseStream = myRequestState.m_streamResponse;
            int read = responseStream.EndRead(result);

            Debug.Log("read size =" + read);

            if (read > 0)
            {
                //将接收的数据写入;  
                fileStream.Write(myRequestState.m_bufferRead, 0, 1024);
                fileStream.Flush();
                //fileStream.Close();  

                //继续读取数据;  
                myRequestState.m_bufferRead = new byte[1024];
                IAsyncResult asyncreadresult = responseStream.BeginRead(myRequestState.m_bufferRead, 0, 1024, new AsyncCallback(ReadCallBack), myRequestState);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }


    void TimeoutCallback(object state, bool timeout)
    {
        if (timeout)
        {
            HttpWebRequest request = state as HttpWebRequest;
            if (request != null)
            {
                request.Abort();
            }

        }
    }


    // Update is called once per frame  
    void Update()
    {

    }
}