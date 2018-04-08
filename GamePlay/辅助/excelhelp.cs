using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Windows.Forms;

namespace STK_demonstration
{
    /// <summary>
    /// 为了保存起始时间而设计的继承类
    /// </summary>
    public class chain_pathex : chain_path
    {
        public chain_pathex(MFrm it) : base(it){}
        //代表这条链路的起始时间
        public DateTime tStart;
        public DateTime tEnd;
    }
    public class Msg
    {
        public string StartOrEnd;
        public string id;
        public List<string> PathNodes;
        public double time;
    }
    public class excelhelp
    {
        #region 变量区
        MFrm view;

        Microsoft.Office.Interop.Excel.Application xapp;
        Worksheet xsheet;
        bool isBuilt = false;//标识excel应用阿是打开

        string filepath;
        //在工作表上的下一时刻预告的索引
        public DateTime LTime;
        int indexS = 3;
        int indexE = 3;
        chain_pathex Echain;
        chain_pathex Schain;
        #endregion
        /// <summary>
        /// 构造函数，在mainfrm load的时候调用
        /// </summary>
        public excelhelp(MFrm it)
        {           
            view = it;
            filepath = Directory.GetCurrentDirectory() + "\\path.csv";
        }
        /// <summary>
        /// 打开xls的函数，在新建地面站后调用
        /// </summary>
        public bool begin()
        {
            //重置两个下一条链路的索引
            indexS = 3;
            indexE = 3;

            xapp = new ApplicationClass();
            xapp.Visible = false;
            isBuilt = true;
            Workbook xbook;           
            try
            {
                xbook=xapp.Workbooks.Open(filepath);
                xsheet = (Worksheet)xbook.Worksheets[1];
                return true;
            }
            catch
            {
                MessageBox.Show("打开excel表格出错");
                return false;
            }           
        }
        /// <summary>
        /// 退出excel，在程序退出的时候调用，同时手动回收垃圾
        /// </summary>
        public void end()
        {
            if (isBuilt)
            {
                try
                {
                    xapp.Quit();
                    xapp = null;
                    xsheet = null;
                    GC.Collect();//手动回收垃圾
                }
                catch { }
                isBuilt = false;
            }
        }      
        /// <summary>
        /// 将数据从excel表中读出并存入scenemana之中
        /// </summary>
        public void write()
        {
            Echain = ReadFromIndex(indexE);
            Schain = ReadFromIndex(indexS);
            while (Echain!=null||Schain!=null)
            {
                if(Echain!=null&&Schain!=null)
                {
                    var temp = Echain.tEnd - Schain.tStart;
                    double seconds = temp.TotalSeconds;
                    if (seconds <= 0)
                    {
                        view.Scenemana.msglist.Add(new Msg { time = (Echain.tEnd - view.dStartTime).TotalSeconds, PathNodes = Echain.path, StartOrEnd = "e",id=Echain.sensor_name});
                        indexE++;
                        Echain = ReadFromIndex(indexE);

                    }
                    else
                    {
                        view.Scenemana.msglist.Add(new Msg { time = (Schain.tStart - view.dStartTime).TotalSeconds, PathNodes = Schain.path, StartOrEnd = "s",id=Schain.sensor_name });
                        indexS++;
                        Schain = ReadFromIndex(indexS);

                    }
                }
                else if(Echain==null)
                {
                    view.Scenemana.msglist.Add(new Msg { time = (Schain.tStart - view.dStartTime).TotalSeconds, PathNodes = Schain.path, StartOrEnd = "s",id=Schain.sensor_name });
                    indexS++;
                    Schain = ReadFromIndex(indexE);

                }
                else if(Schain==null)
                {
                    view.Scenemana.msglist.Add(new Msg { time = (Echain.tEnd - view.dStartTime).TotalSeconds, PathNodes = Echain.path, StartOrEnd = "e",id=Echain.sensor_name });
                    indexE++;
                    Echain = ReadFromIndex(indexS);

                }
            }
        }
        /// <summary>
        /// 由索引值获得一个链路对象that在此处new()，目前未建设完成
        /// </summary>
        /// <param name="inx">目标链路在excel中的索引号</param>
        /// <returns>chain_pathex</returns>
        private chain_pathex ReadFromIndex(int inx)
        {
            chain_pathex Breturn = new chain_pathex(view);
            Range temp=xsheet.get_Range(string.Format("A{0}", inx));
            //进行判断该表是否为空，从而判断是否读到了表尾
            if (temp.Value2 == null) return null;
            //读入nodes节点信息
            string Nodes = temp.Value2.ToString();
            //读入通断时间信息
            Breturn.tStart = DateTime.ParseExact(temp.Next.Value2.ToString(), "yyyy:MM:dd:HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);            
            Breturn.tEnd = DateTime.ParseExact(temp.Next.Next.Value2.ToString(), "yyyy:MM:dd:HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            string[] nodelist = Nodes.Split(new string[]{ "->" }, StringSplitOptions.None);
            foreach(var str in nodelist)
            {
                Breturn.path.Add(str);
                //将系统物件表中的物体添加到链路的小表中
                Breturn.path_Objs.Add(view.Scenemana.stkObjs.Find(c => c.ObjName == str));                
            }
            //传感器名称等于链路的id号
            Breturn.sensor_name = Breturn.path[0] + "-" + Breturn.path[Breturn.path.Count-1];
            return Breturn;
        }
    }
}
