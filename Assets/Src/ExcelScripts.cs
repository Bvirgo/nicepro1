using UnityEngine;
using System.Collections;
using System.IO;
using Excel;
using System.Data;

public class ExcelScripts : MonoBehaviour
{

    void Start()
    {
        FileStream m_Stream = File.Open(Application.dataPath +
            "\\Excel\\UserLevel.xlsx", FileMode.Open, FileAccess.Read);
        //使用OpenXml读取Excel文件
        IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(m_Stream);
        //将Excel数据转化为DataSet
        DataSet mResultSets = mExcelReader.AsDataSet();
        //读取行数
        int rowCount = mResultSets.Tables[0].Rows.Count;
        //逐行读取,从第一行读以跳过表头
        for (int i = 1; i < rowCount; i++)
        {
            //将读取的Excel数据转化成数据实体
            UserLevel mUser = new UserLevel();
            mUser.Name = mResultSets.Tables[0].Rows[i][0].ToString();
            mUser.Level = mResultSets.Tables[0].Rows[i][1].ToString();
            mUser.Description = mResultSets.Tables[0].Rows[i][2].ToString();
            mUser.Skill = mResultSets.Tables[0].Rows[i][3].ToString();
            //输出Debug信息
            Debug.Log(mUser.ToString());
            //ADD:更多逻辑
        }
    }

    //定义一个数据实体类UserLevel
    private class UserLevel
    {
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_Level;
        public string Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        private string m_Description;
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        private string m_Skill;
        public string Skill
        {
            get { return m_Skill; }
            set { m_Skill = value; }
        }

        public override string ToString()
        {
            return string.Format("Name={0}&Level={1}&Description={2}&Skill={3}",
                           m_Name, m_Level, m_Description, m_Skill);
        }
    }
}