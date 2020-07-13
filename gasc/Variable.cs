
using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace gasc
{




    public class Variable
    {





        public class Resulter
        {
            /// <summary>
            /// 1 数字,字符 2 变量 3 方法 4取成员
            /// </summary>
            int condition;
            Variable ret_veriable;
            Resulter[] childresulters;
            Resulter functionresulter;
            string functionname, ret_variablename,csname;


            Resulter fromr;string mem;
            public Resulter(string form)
            {
                if (form.Length == 0)
                {
                    condition = 1;
                    ret_veriable = new Variable(0);
                    return;
                }

                string startform, endform;
                startform = form.Substring(0, 1);
                endform = form.Substring(form.Length - 1, 1);
                while(startform == "(" && endform == ")")
                {
                    //    ( q w e r ) length = 6
                    //    0 1 2 3 4 5
                    form = form.Substring(1, form.Length - 2);
                    startform = form.Substring(0, 1);
                    endform = form.Substring(form.Length - 1, 1);
                }
                //1、判断为数字
                if (startform == "1" ||
                   startform == "2" ||
                   startform == "3" ||
                   startform == "4" ||
                   startform == "5" ||
                   startform == "6" ||
                   startform == "7" ||
                   startform == "8" ||
                   startform == "9" ||
                   startform == "0"
                   )
                {
                    ret_veriable = new Variable(Convert.ToDouble(form));
                    condition = 1;
                }
                //2、判断为字符
                else if (startform == "\"" && endform == "\"")
                {
                    form = ZY(form);
                    ret_veriable = new Variable(form.Substring(1, form.Length - 2));
                    condition = 1;
                }
                //3、判断为方法
                else if (endform == ")")
                {
                    ArrayList al = new ArrayList();
                    for(int i = 0;i<form.Length;i++)
                    {
                        al.Add(form.Substring(i, 1));
                    }

                    condition = 3;
                    //int khpos = form.IndexOf("(");
                    int khpos = Opearate.FindLeft(al,al.Count-1,new string[0])-1;
                    functionname = form.Substring(0, khpos);
                    //O.M(1,2)
                    functionresulter = new Resulter(functionname);
                    csname = form.Substring(khpos + 1, form.Length - khpos - 2);
                    if (csname == "")
                    {
                        childresulters = new Resulter[0];
                    }
                    else
                    {
                        childresulters = GetResultersformcs(csname);
                    }
                }
                else if(System.Text.RegularExpressions.Regex.IsMatch(form,":[A-Za-z0-9_.]+$"))
                {
                    mem = System.Text.RegularExpressions.Regex.Match(form, ":[A-Za-z0-9_.]+$").Value.Substring(1);
                    string left = form.Substring(0, form.Length - mem.Length-1);
                    fromr = new Resulter(left);
                    condition = 4;
                }
                //4、判断为变量
                else
                {
                    ret_variablename = form;
                    condition = 2;
                }
            }
            public Variable Run(Hashtable basehashtable)
            {
                return null;
            }

            public void ToXml(XmlDocument xmlDocument,XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("arg");
                string con;
                switch(condition)
                {
                    case 1:
                        if (ret_veriable.value is double)
                            con = "num";
                        else
                            con = "str";
                        element.SetAttribute("value", ret_veriable.value.ToString());
                    break;
                    case 2:
                        con = "var";
                        element.SetAttribute("value", ret_variablename);
                    break;
                    case 3:
                        con = "fun";
                        XmlElement infun = xmlDocument.CreateElement("fun");
                        XmlElement inparams = xmlDocument.CreateElement("params");
                        functionresulter.ToXml(xmlDocument, infun);
                        foreach (var i in childresulters)
                            i.ToXml(xmlDocument, inparams);
                    element.AppendChild(infun);
                    element.AppendChild(inparams);
                        
                    break;
                    case 4:
                        con = "mem";
                        element.SetAttribute("value", mem);
                        fromr.ToXml(xmlDocument, element);
                    break;
                    default:
                    throw new Exception("Wrong");
                }
                element.SetAttribute("con", con);
                xmlElement.AppendChild(element);

            }

            /// <summary>
            /// 设置逗号
            /// </summary>
            /// <param name="csname"></param>
            /// <returns></returns>
            public static string SetOutDH(string csname)
            {
                int cc = 0;
                bool isinstring = false;
                ArrayList arr_string = new ArrayList();
                for (int i = 0; i < csname.Length; i++)
                {
                    arr_string.Add(csname.Substring(i, 1));
                }
                for (int i = 0; i < arr_string.Count; i++)
                {
                    if (arr_string[i].ToString() == "\"")
                    { isinstring = !isinstring; }
                    else if (!isinstring)
                    {
                        if (arr_string[i].ToString() == "(")
                        { cc++; }
                        if (arr_string[i].ToString() == ")")
                        { cc--; }
                        if (arr_string[i].ToString() == "," && cc == 0)
                        { arr_string[i] = "\a|truedh|\a"; }
                    }
                }
                StringBuilder sb = new StringBuilder();
                foreach (string s in arr_string)
                {
                    sb.Append(s);
                }
                return sb.ToString();
            }

            public static Resulter[] GetResultersformcs(string csname)
            {
                csname = SetOutDH(csname);

                if (csname.Substring(0,1) == "(" && csname.Substring(csname.Length-1) == ")" && !csname .Contains("\a|truedh|\a"))
                {
                    csname = csname.Substring(1, csname.Length - 2);
                }
                string csname_new = SetOutDH(csname);

                string[] csnames = csname_new.Split(new string[] { "\a|truedh|\a" }, StringSplitOptions.None);
                Resulter[] resulters = new Resulter[csnames.Length];
                for (int i = 0; i < resulters.Length; i++)
                {
                    resulters[i] = new Resulter(csnames[i]);
                }
                return resulters;

            }
            
            
        }

        /// <summary>
        /// 清理空格的地方
        /// </summary>
        /// <param name="basstring"></param>
        /// <returns></returns>
        public static string Clearspace(string basstring)
        {
            ArrayList array_string = new ArrayList();
            for (int i = 0; i < basstring.Length; i++)
            {
                array_string.Add(basstring.Substring(i, 1));
            }
            //开始清理 
            bool isinstring = false;
            for (int i = 0; i < array_string.Count;)
            {
                if (array_string[i].ToString() == "\"")
                {
                    isinstring = !isinstring;
                    i++;
                }
                else
                {
                    if (array_string[i].ToString() == " " && !isinstring)
                    {
                        try
                        {
                            if (array_string[i - 3].ToString() == "v" && array_string[i - 2].ToString() == "a" && array_string[i - 1].ToString() == "r")
                            {
                                i++; continue;
                            }

                        }
                         catch
                        {

                        }
                        array_string.RemoveAt(i); continue;
                    }
                    i++;
                }
            }
            //开始连接
            StringBuilder sb = new StringBuilder();
            foreach (string s in array_string)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取不干扰原变量表的新变量表
        /// </summary>
        /// <param name="basehashtable">原变量表</param>
        /// <returns>新变量表</returns>
        public static Hashtable GetOwnVariables(Hashtable basehashtable)
        {
            Hashtable hashtable = new Hashtable();
            foreach (DictionaryEntry inbase in basehashtable)
            {
                hashtable.Add(inbase.Key, inbase.Value);
            }
            return hashtable;
        }
        
        internal static int IsEqualExist(string code)
        {
            bool isinsting = false;
            int deep = 0;
            for (int i = 0; i < code.Length; i++)
            {
                string s = code.Substring(i, 1);
                
                try
                {
                    if ((
                        s == "=" || 
                        s == ">" || 
                        s == "<" || 
                        s == "!" || 
                        s == "&")
                        && code.Substring(i+1,1) == "=" && !isinsting)
                    {
                        i ++;
                        continue;
                    }
                }
                catch
                {
                    return -1;
                }
                if (s == "\"")
                {
                    isinsting = !isinsting;
                }
                else if (s == "(")
                    deep++;
                else if (s == ")")
                    deep--;
                else if (s == "=" && !isinsting && deep == 0)
                {
                    return i;
                }
            }
            return -1;
        }




        /*  "       \"      
            ,       \,     
            (       \(     
            )       \)     
            换行    \n     
            Tab     \t     
            Enter   \r     
            \       \\     
            =       \=     
		 */
   

        public static string ZY(string oldstring)
        {
            return oldstring
             .Replace("\\\"", "\"")
             .Replace("\\,", ",")
             .Replace("\\(", "(")
             .Replace("\\)", ")")
             .Replace("\\n", "\n")
             .Replace("\\t", "\t")
             .Replace("\\r", "\r")
             .Replace("\\\\", "\\");
        }

        public static T GetTrueVariable<T>(Hashtable oldhs, string variablename)
        {
            T t = (T)((Variable)oldhs[variablename]).value;
            return t;
        }

        public Variable(object o)
        {
            value = o;
        }

        public bool isconst = false;
        private object m_value;

        public object value {
            get
            {
                return m_value;
            }
            set
            {
                if(isconst)
                {
                    throw new Exception("常量不允许赋值");
                }
                else
                {
                    m_value = value;
                }
            }
        }

    }
}