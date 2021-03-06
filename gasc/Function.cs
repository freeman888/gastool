﻿

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;
namespace gasc
{


    public interface IFunction
    {
        object IRun(Hashtable xc);
        string Istr_xcname { get; set; }
        bool Iisreffunction { get; set; }

    }

    public partial class Function:IFunction
    {
        //实现IFunction
        public object IRun(Hashtable xc)
        {
            return Run(xc);
        }
        string IFunction. Istr_xcname
        {
            get
            {
                return str_xcname;
            }
            set
            {
                str_xcname = value;
            }
        }
        bool IFunction. Iisreffunction
        {
            get
            {
                return isreffunction;
            }
            set
            {
                isreffunction = value;
            }
        }

       
        /// <summary>
        /// 父类
        /// </summary>
        public class Head
        {
            public virtual void AddFunctions(Hashtable h) { }
        }
        public virtual object Run(Hashtable xc)//父方法
        {
            
            return new object();
        }
        public string str_xcname;
        public bool isreffunction = false;

        //以下为用户自定义函数
        public class New_User_Function : Function
        {
            public Sentence[] sentences;
            public string name;
            public New_User_Function(string fname, string fxc)
            {
                str_xcname = fxc; name = fname;
            }
            public void ToXml(XmlDocument xmlDocument,XmlElement xmlElement)
            {
                XmlElement myfun =  xmlDocument.CreateElement("deffun");
                myfun.SetAttribute("funname", name);
                myfun.SetAttribute("params", str_xcname);
                myfun.SetAttribute("isref", isreffunction.ToString());
                foreach(var i in sentences)
                {
                    i.ToXml(xmlDocument, myfun);
                }
                xmlElement.AppendChild(myfun);
            }
        }
        
        public class New_Member_Function:Function
        {
            public Sentence[] sentences;
            public string name;
            public bool isstatic = false;
            public New_Member_Function(string fname, string fxc,bool _static)
            {
                str_xcname = fxc; name = fname;this.isstatic = _static;
            }
            public void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement myfun = xmlDocument.CreateElement("memfun");
                myfun.SetAttribute("funname", name);
                myfun.SetAttribute("params", str_xcname);
                myfun.SetAttribute("isref", isreffunction.ToString());
                myfun.SetAttribute("isstatic", isstatic.ToString());
                foreach (var i in sentences)
                {
                    i.ToXml(xmlDocument, myfun);
                }
                xmlElement.AppendChild(myfun);
            }
        }

    }
}