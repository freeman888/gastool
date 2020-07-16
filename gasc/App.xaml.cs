using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace gasc
{

    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string gas = "";
        [STAThread]
        public static void GMain(string s)
        {
            try
            {
                gas = s;
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        [STAThread]
        public static void Main(string[] ar)
        {

            gas = @"get Array;
get IO;
get Math;
fun Main(arg):
    IO.Tip(123456);
fun Test(t1,t2):
    IO.Write(""hello"");
";

            /*
             * 
             * */


            //App app = new App();
            //app.InitializeComponent();
            ////app.Run();
            //try
            //{
            //    Out.Gas2IL("f:\\1.gas", "f:\\code.xml");
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine(e.ToString());
            //}
            if (ar.Length == 2)
            {
                try
                {
                    Out.Gas2IL(ar[0], ar[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    Console.Read();
                }
            }
            else if (ar.Length == 1)
            {
                try
                {
                    Out.Gas2IL("f:\\1.gas", "f:\\code.xml");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Console.Read();
                }
            }
            else
            {
                Console.WriteLine("input error");
                Console.WriteLine("params: [gas path] [xml path]");
                Console.Read();
            }
        }


        public static string Getfunandvar(string sourcecode, XmlDocument xmlDocument, string outpath)
        {

            XmlElement xmlElement = xmlDocument.CreateElement("code");
            xmlElement.SetAttribute("minversion", "1902");

            {

                string[] codes = sourcecode.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                Hashtable functions = new Hashtable(), variables = new Hashtable();
                XmlElement x_lib = null, x_dflib = xmlDocument.CreateElement("lib") ;
                for (int index = 0; index < codes.Length;)
                {
                    
                    if (codes[index] == "" || codes[index].Length < 6)
                    { index++; continue; }
                    string sign, content, end, code;
                    code = codes[index];
                    sign = code.Substring(0, 4);
                    end = code.Substring(code.Length - 1, 1);
                    if (end != ":" && end != ";")
                    {
                        index++; continue;
                    }
                    content = code.Substring(4, code.Length - 5);

                    if (sign == "lib " && end == ":")
                    {
                        x_lib = xmlDocument.CreateElement("lib");
                        x_lib.SetAttribute("name", content);
                        index++;
                        

                        xmlElement.AppendChild(x_lib);
                    }
                    else
                    {
                        if (x_lib == null)
                            x_lib = x_dflib;
                        code = code.Substring(4);
                        sign = code.Substring(0, 4);
                        end = code.Substring(code.Length - 1, 1);

                        //2、判断为get 方法
                        if (sign == "get " && end == ";")
                        {
                            content = content.Replace(" ", "");
                            foreach (string item in content.Split(','))
                            {
                                XmlElement getter = xmlDocument.CreateElement("get");
                                getter.SetAttribute("value", item);
                                x_lib.AppendChild(getter);
                            }
                            index++; continue;
                        }
                        else if (sign == "var " && end == ";")
                        { //variables.Add(content, new Variable(0)); index++ ;
                            content = content.Replace(" ", "");
                            foreach (string item in content.Split(','))
                            {
                                XmlElement varer = xmlDocument.CreateElement("var");
                                varer.SetAttribute("value", item);
                                x_lib.AppendChild(varer);
                            }
                            index++; continue;
                        }
                        else if (sign == "fun " && end == ":")
                        {
                            bool isref = false;

                            string adj, functionname, xcname;
                            adj = content.Substring(0, content.IndexOf("("));
                            List<string> list = new List<string>(Regex.Split(adj, " +"));
                            functionname = list[list.Count - 1];
                            if (list.IndexOf("ref") != -1)
                                isref = true;

                            xcname = content.Substring(content.IndexOf("(") + 1, content.LastIndexOf(")") - content.IndexOf("(") - 1);

                            index++;
                            ArrayList array_str_sentences = new ArrayList();
                            for (; index < codes.Length && codes[index].Length >= 4 && codes[index].Substring(0, 4) == "    "; index++)
                            {
                                array_str_sentences.Add(codes[index].Substring(4, codes[index].Length - 4));
                            }
                            Function.New_User_Function new_User_Function = new Function.New_User_Function(functionname, xcname);
                            new_User_Function.isreffunction = isref;
                            new_User_Function.sentences = Sentence.GetSentencesfromArrayList(array_str_sentences);
                            functions.Add(functionname, new_User_Function);

                            
                            new_User_Function.ToXml(xmlDocument, x_lib);

                            //432432432fafa


                        }

                        else if (sign == "cls ")
                        {
                            content = content.Replace(" ", "");
                            content = content.Substring(3);
                            index++;
                            ArrayList cls_content = new ArrayList();
                            for (; index < codes.Length && codes[index].Length >= 8 && codes[index].Substring(0, 8) == "        "; index++)
                            {
                                cls_content.Add(codes[index].Substring(8));
                            }

                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    
                    
                    

                }
                xmlElement.AppendChild(x_dflib);
                xmlDocument.AppendChild(xmlElement);
                if (outpath == null)
                {
                    return xmlElement.InnerXml;
                }
                StreamWriter streamWriter = new StreamWriter(outpath);
                xmlDocument.Save(streamWriter);
                streamWriter.Close();
                return "ok";
            }
        }



        public static void AyalyseCode(string sourcecode, XmlDocument xmlDocument, string outpath)
        {
            XmlElement xmlElement = xmlDocument.CreateElement("code");
            xmlElement.SetAttribute("minversion", "2007");
            string[] codes = sourcecode.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for(var index = 0;index < codes.Length;)
            {
                var code = codes[index];
                if(code.Length<6 || string.IsNullOrWhiteSpace(code))
                { index++;continue; }
                var start = code.Substring(0, 4);
                var end = code.Substring(code.Length - 1);
                if(end != ";" && end != ":")
                { index++;continue; }
                if (start == "lib ")
                {
                    index++;
                    var libname = code.Substring(4, code.Length - 5);
                    List<string> libcontents = new List<string>();
                    for (; index < codes.Length && codes[index].Length >= 4 && codes[index].Substring(0, 4) == "    "; index++)
                        libcontents.Add(codes[index].Substring(4));
                    LibCreat(libname, libcontents, xmlDocument, xmlElement);
                    xmlDocument.AppendChild(xmlElement);

                }
                else 
                { Console.WriteLine("What Fuck"); return; }

                
            }

            xmlDocument.AppendChild(xmlElement);
            StreamWriter streamWriter = new StreamWriter(outpath);
            xmlDocument.Save(streamWriter);
            streamWriter.Close();

        }

        private static void LibCreat(string libname, List<string> libcontents, XmlDocument xmlDocument, XmlElement xmlElement)
        {
            XmlElement x_lib = xmlDocument.CreateElement("lib");
            x_lib.SetAttribute("name", libname);
            for(var index = 0;index < libcontents.Count;)
            {
                var code = libcontents[index];
                if (code.Length < 6 || string.IsNullOrWhiteSpace(code))
                { index++; continue; }
                var start = code.Substring(0, 4);
                var end = code.Substring(code.Length - 1);
                if (end != ";" && end != ":")
                { index++; continue; }
                var content = code.Substring(4, code.Length - 5);
                if(start == "get " && end == ";" )
                {
                    content = content.Replace(" ", "");
                    foreach (string item in content.Split(','))
                    {
                        XmlElement getter = xmlDocument.CreateElement("get");
                        getter.SetAttribute("value", item);
                        x_lib.AppendChild(getter);
                    }
                    index++; continue;
                }
                else if (start == "var " && end == ";")
                { //variables.Add(content, new Variable(0)); index++ ;
                    content = content.Replace(" ", "");
                    foreach (string item in content.Split(','))
                    {
                        XmlElement varer = xmlDocument.CreateElement("var");
                        varer.SetAttribute("value", item);
                        x_lib.AppendChild(varer);
                    }
                    index++; continue;
                }
                else if (start == "fun " && end == ":")
                {
                    bool isref = false;

                    string adj, functionname, xcname;
                    adj = content.Substring(0, content.IndexOf("("));
                    List<string> list = new List<string>(Regex.Split(adj, " +"));
                    functionname = list[list.Count - 1];
                    if (list.IndexOf("ref") != -1)
                        isref = true;

                    xcname = content.Substring(content.IndexOf("(") + 1, content.LastIndexOf(")") - content.IndexOf("(") - 1);

                    index++;
                    ArrayList array_str_sentences = new ArrayList();
                    for (; index < libcontents.Count && libcontents[index].Length >= 4 && libcontents[index].Substring(0, 4) == "    "; index++)
                    {
                        array_str_sentences.Add(libcontents[index].Substring(4, libcontents[index].Length - 4));
                    }
                    Function.New_User_Function new_User_Function = new Function.New_User_Function(functionname, xcname);
                    new_User_Function.isreffunction = isref;
                    new_User_Function.sentences = Sentence.GetSentencesfromArrayList(array_str_sentences);
                    //functions.Add(functionname, new_User_Function);


                    new_User_Function.ToXml(xmlDocument, x_lib);

                    //432432432fafa
                }
                else if(start == "cls " && end == ":")
                {
                    index++;
                    List<string> clscontents = new List<string>();
                    for(;index < libcontents.Count && libcontents[index].Length >= 4 && libcontents[index].Substring(0,4) == "    ";index++)
                    {
                        clscontents.Add(libcontents[index].Substring(4));
                    }
                    ClsCreat(content, clscontents, xmlDocument, x_lib);
                    
                }
                else { Console.WriteLine("Fuckkkkk");return; }
                xmlElement.AppendChild(x_lib);
            }
        }

        private static void ClsCreat(string clsname, List<string> clscontents, XmlDocument xmlDocument, XmlElement x_lib)
        {
            XmlElement x_cls = xmlDocument.CreateElement("cls");
            x_cls.SetAttribute("name", clsname);
            for(var index = 0;index<clscontents.Count;)
            {
                var code = clscontents[index];
                if (code.Length < 6 || string.IsNullOrWhiteSpace(code))
                { index++; continue; }
                var start = code.Substring(0, 4);
                var end = code.Substring(code.Length - 1);
                if (end != ";" && end != ":")
                { index++; continue; }
                var content = code.Substring(4, code.Length - 5);
                if (start == "var " && end == ";")
                { //variables.Add(content, new Variable(0)); index++ ;
                    content = content.Replace(" ", "");
                    foreach (string item in content.Split(','))
                    {
                        XmlElement varer = xmlDocument.CreateElement("member");
                        varer.SetAttribute("value", item);
                        x_cls.AppendChild(varer);
                    }
                    index++; continue;
                }
                else if (start == "fun " && end == ":")
                {
                    bool isref = false;
                    bool _static = false;
                    string adj, functionname, xcname;
                    adj = content.Substring(0, content.IndexOf("("));
                    List<string> list = new List<string>(Regex.Split(adj, " +"));
                    functionname = list[list.Count - 1];
                    if (list.IndexOf("ref") != -1)
                        isref = true;
                    if (list.IndexOf("static") != -1)
                        _static = true;
                    xcname = content.Substring(content.IndexOf("(") + 1, content.LastIndexOf(")") - content.IndexOf("(") - 1);

                    index++;
                    ArrayList array_str_sentences = new ArrayList();
                    for (; index < clscontents.Count && clscontents[index].Length >= 4 && clscontents[index].Substring(0, 4) == "    "; index++)
                    {
                        array_str_sentences.Add(clscontents[index].Substring(4, clscontents[index].Length - 4));
                    }
                    Function.New_Member_Function new_User_Function = new Function.New_Member_Function(functionname, xcname,_static);
                    new_User_Function.isreffunction = isref;
                    new_User_Function.sentences = Sentence.GetSentencesfromArrayList(array_str_sentences);
                    //functions.Add(functionname, new_User_Function);


                    new_User_Function.ToXml(xmlDocument, x_cls);
                    x_lib.AppendChild(x_cls);
                    //432432432fafa
                }
                else { Console.WriteLine("Wrong!");return; }

            }
        }
    }
}
