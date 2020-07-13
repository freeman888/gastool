using System;
using System.Collections;
using System.Xml;

namespace gasc
{
    public class Sentence
    {
        public static Sentence[] GetSentencesfromArrayList(ArrayList array_sentences)
        {
            ArrayList true_sentences = new ArrayList();
            for (int index = 0; index < array_sentences.Count;)
            {
                string code = array_sentences[index].ToString(), end = "";
                try { end = code.Substring(code.Length - 1, 1); }
                catch { }
                //这是注释或者空
                if (code == "" || code.Length < 2 || code.Substring(0, 2) == "//" ||(end!=":" && end!=";"))
                {
                    index++; continue;
                }
                //这是指导
                
                //这是if
                else if (code.Length >= 6 && code.Substring(0, 2) == "if" && end == ":")
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction( Variable.Clearspace(code));
                    string boolname = code.Replace("if(", "").Replace("):", "");
                    index++;
                    ArrayList string_allsentences = new ArrayList();
                    for (; index < array_sentences.Count && array_sentences[index].ToString().Length >= 4 && array_sentences[index].ToString().Substring(0, 4) == "    "; index++)
                    {
                        string_allsentences.Add(array_sentences[index].ToString().Substring(4, array_sentences[index].ToString().Length - 4));
                    }
                    New_Sentence_if sif = new New_Sentence_if(boolname, string_allsentences);
                    sif.mycode = truecode;
                    true_sentences.Add(sif);
                }
                //这是else
                else if (code.Length >=5 && code.Substring(0, 4) == "else" && end == ":")
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction(Variable.Clearspace(code));
                    //string boolname = code.Replace("else(", "").Replace("):", "");
                    index++;
                    ArrayList string_allsentences = new ArrayList();
                    for (; index < array_sentences.Count && array_sentences[index].ToString().Length >= 4 && array_sentences[index].ToString().Substring(0, 4) == "    "; index++)
                    {
                        string_allsentences.Add(array_sentences[index].ToString().Substring(4, array_sentences[index].ToString().Length - 4));
                    }
                    New_Sentence_if sif = true_sentences[true_sentences.Count - 1] as New_Sentence_if ;
                    
                    sif.elsesentences = GetSentencesfromArrayList(string_allsentences);
                    
                }
                //这是elif
                else if (code.Length >= 8 && code.Substring(0, 4) == "elif" && end == ":")
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction(Variable.Clearspace(code));
                    string boolname = code.Replace("elif(", "").Replace("):", "");
                    index++;
                    ArrayList string_allsentences = new ArrayList();
                    for (; index < array_sentences.Count && array_sentences[index].ToString().Length >= 4 && array_sentences[index].ToString().Substring(0, 4) == "    "; index++)
                    {
                        string_allsentences.Add(array_sentences[index].ToString().Substring(4, array_sentences[index].ToString().Length - 4));
                    }
                    New_Sentence_if sif = true_sentences[true_sentences.Count - 1] as New_Sentence_if;
                    New_Sentence_if new_Sentence_If = new New_Sentence_if(boolname, string_allsentences);
                    sif.elseifsentences.Add(new_Sentence_If);

                }
                //这是while
                else if (code.Length >= 8 && code.Substring(0, 5) == "while" && end == ":")
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction( Variable.Clearspace(code));
                    string boolname = code.Replace("while(", "").Replace("):", "");
                    index++;
                    ArrayList string_allsentences = new ArrayList();
                    for (; index < array_sentences.Count && array_sentences[index].ToString().Length >= 4 && array_sentences[index].ToString().Substring(0, 4) == "    "; index++)
                    {
                        string_allsentences.Add(array_sentences[index].ToString().Substring(4, array_sentences[index].ToString().Length - 4));
                    }
                     New_Sentence_while sif = new New_Sentence_while(boolname, string_allsentences);
                    sif.mycode = truecode;
                    true_sentences.Add(sif);
                }
                //这是try
                else if (code.Length >= 4 && code.Substring(0, 3) == "try" && end == ":")
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction(Variable.Clearspace(code));
                    index++;
                    ArrayList string_allsentences = new ArrayList();
                    for (; index < array_sentences.Count && array_sentences[index].ToString().Length >= 4 && array_sentences[index].ToString().Substring(0, 4) == "    "; index++)
                    {
                        string_allsentences.Add(array_sentences[index].ToString().Substring(4, array_sentences[index].ToString().Length - 4));
                    }
                    New_Sentence_try sif = new New_Sentence_try( string_allsentences);
                    sif.mycode = truecode;
                    true_sentences.Add(sif);
                }
                //这是catch
                else if (code.Length >= 9 && code.Substring(0, 5) == "catch" && end == ":")
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction(Variable.Clearspace(code));
                    //string boolname = code.Replace("else(", "").Replace("):", "");
                    index++;

                    
                    string exname = code.Substring(6, code.IndexOf(')') - 6);
                    bool var_new = false;
                    if (exname.Length > 4 && exname.Substring(0, 4) == "var ")
                    {
                        exname = exname.Substring(4);
                        var_new = true;
                    }
                    ArrayList string_allsentences = new ArrayList();
                    for (; index < array_sentences.Count && array_sentences[index].ToString().Length >= 4 && array_sentences[index].ToString().Substring(0, 4) == "    "; index++)
                    {
                        string_allsentences.Add(array_sentences[index].ToString().Substring(4, array_sentences[index].ToString().Length - 4));
                    }
                    New_Sentence_try sif = true_sentences[true_sentences.Count - 1] as New_Sentence_try;
                    sif.exname = exname;
                    sif.var_new = var_new;
                    sif.catchsentences = GetSentencesfromArrayList(string_allsentences);

                }
                //这是foreach
                else if (code.Length >= 14 && code.Substring(0, 7) == "foreach" && end == ":")
                {
                    //foreach(s in ss):
                    string truecode = code;
                    code = Opearate.SetMathFunction(Variable.Clearspace(code));
                    string content = code.Substring(8, code.LastIndexOf(')') - 8);
                    int jiantoupos = content.IndexOf(",");
                    string dfz, you;
                    dfz = content.Substring(0, jiantoupos);
                    you = content.Substring(jiantoupos + 1);
                    index++;
                    ArrayList string_allsentences = new ArrayList();
                    for (; index < array_sentences.Count && array_sentences[index].ToString().Length >= 4 && array_sentences[index].ToString().Substring(0, 4) == "    "; index++)
                    {
                        string_allsentences.Add(array_sentences[index].ToString().Substring(4, array_sentences[index].ToString().Length - 4));
                    }
                    New_Sentence_foreach sif = new New_Sentence_foreach(dfz,you, string_allsentences);
                    sif.mycode = truecode;
                    true_sentences.Add(sif);
                }
                //这是ruturn(aa);
                else if (code.Length >= 10 && code.Substring(0, 6) == "return" && end == ";")
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction(Variable.Clearspace(code));
                    string contnet = code.Substring(7, code.Length - 9);
                    true_sentences.Add(new New_Sentence_Return(contnet, index + 1) { mycode = truecode});
                    index++;
                    continue;
                }
                //这是while for 等等

                //这是var
                else if (code.Length >= 6 && code.Substring(0, 4) == "var " && end == ";")
                {
                    //var s,a;
                    string truecode = code;
                    string content = Variable.Resulter.SetOutDH(code.Substring(4,code.Length-5));
                    content =  Variable.Clearspace(content);
                    string[] contents = content.Split(new string[] { "\a|truedh|\a" }, StringSplitOptions.None);
                    foreach(string str in contents)
                    {
                        string cstr = str ;
                        int dhpos = Variable.IsEqualExist(cstr);
                        if (dhpos == -1)
                        { true_sentences.Add(new New_Sentence_Newref(cstr, index ){ mycode = truecode }); }
                        else
                        {
                            //a=123
                            string refname = cstr.Substring(0, dhpos), rightname = cstr.Substring(dhpos + 1, cstr.Length - dhpos - 1);
                            true_sentences.Add(new New_Sentence_Newref(refname, index + 1) { mycode = truecode });
                            string xrs = Opearate.SetMathFunction(rightname+";");
                            xrs = xrs.Substring(0, xrs.Length - 1);
                            true_sentences.Add(new New_Sentence_GiveResult(refname, xrs) { mycode = truecode });
                            
                        }
                    }
                    index++;
                }
                else if (Variable.IsEqualExist(code) != -1)
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction(code);
                    int dhpos = Variable.IsEqualExist(code);
                    string refname = Variable.Clearspace(code.Substring(0, dhpos)), rightname = Variable.Clearspace(code.Substring(dhpos + 1, code.Length - dhpos - 2));
                    true_sentences.Add(new New_Sentence_GiveResult(refname, rightname) { mycode = truecode});
                    index++; continue;
                }
                else
                {
                    string truecode = code;
                    code = Opearate.SetMathFunction(code);
                    true_sentences.Add(new New_Sentence_Usefunction(Variable.Clearspace(code.Substring(0, code.Length - 1))) { mycode = truecode});
                   
                    index++;
                    continue;
                }
            }
            return (Sentence[])true_sentences.ToArray(typeof(Sentence));
        }
        
        public int number;
        public string mycode = "";

        public virtual void ToXml(XmlDocument xmlDocument,XmlElement xmlElement) { }

        /// <summary>
        /// 新建引用语句
        /// </summary>
        public class New_Sentence_Newref : Sentence
        {
            private string refname;
            public New_Sentence_Newref(string refname, int n)
            {
                number = n;
                this.refname = refname;
            }
            public override void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("var_s");
                element.SetAttribute("varname", refname);
                element.SetAttribute("str", mycode);
                xmlElement.AppendChild(element);
            }
        }
        public class New_Sentence_Return : Sentence
        {
            Variable.Resulter resulter;
            public New_Sentence_Return(string xret_name, int n)
            {
                number = n;
                resulter = new Variable.Resulter(xret_name);
            }
            public override void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("return_s");
                element.SetAttribute("str", mycode);
                resulter.ToXml(xmlDocument, element);
                xmlElement.AppendChild(element);

            }
        }
        public class New_Sentence_if : Sentence
        {
            public Variable.Resulter resulter;
            public Sentence[] thensentences, elsesentences;
            public ArrayList elseifsentences = new ArrayList();
            public bool realif;
            public New_Sentence_if(string xbn, ArrayList str_allsens)
            {
                resulter = new Variable.Resulter(xbn);
                thensentences = GetSentencesfromArrayList(str_allsens);
            }
            

            public override void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("if_s"),
                    then = xmlDocument.CreateElement("then");
                element.SetAttribute("str", mycode);
                {
                    XmlElement express = xmlDocument.CreateElement("express"),
                        run = xmlDocument.CreateElement("run");
                    resulter.ToXml(xmlDocument, express);
                    foreach (var i in thensentences)
                        i.ToXml(xmlDocument, run);
                    then.AppendChild(express);
                    then.AppendChild(run);
                }
                element.AppendChild(then);
                foreach(New_Sentence_if i in elseifsentences)
                {
                    XmlElement elif = xmlDocument.CreateElement("elif");
                    {
                        XmlElement express = xmlDocument.CreateElement("express"),
                            run = xmlDocument.CreateElement("run");
                        i.resulter.ToXml(xmlDocument, express);
                        foreach (var ii in i.thensentences)
                            ii.ToXml(xmlDocument, run);
                        elif.AppendChild(express);
                        elif.AppendChild(run);
                    }
                    element.AppendChild(elif);
                }
                if (elsesentences != null)
                {
                    XmlElement _else = xmlDocument.CreateElement("else");
                    {
                        XmlElement run = xmlDocument.CreateElement("run");
                        foreach (var i in elsesentences)
                            i.ToXml(xmlDocument, run);
                        _else.AppendChild(run);
                    }
                    element.AppendChild(_else);
                }
                xmlElement.AppendChild(element);
                
            }
        }
        public class New_Sentence_GiveResult : Sentence
        {
            Variable.Resulter resulter ,togive;
            /// <summary>
            /// 构造Giveresult
            /// </summary>
            /// <param name="xvn">赋给引用名</param>
            /// <param name="xrs">右侧表达式</param>
            public New_Sentence_GiveResult(string xvn, string xrs)
            {
                togive = new Variable.Resulter(xvn);
                resulter = new Variable.Resulter(xrs);
            }
            public override void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("getres_s");
                element.SetAttribute("str", mycode);
                togive.ToXml(xmlDocument, element);
                resulter.ToXml(xmlDocument, element);
                xmlElement.AppendChild(element);
            }

        }
        public class New_Sentence_Usefunction : Sentence
        {
            Variable.Resulter resulter;
            public New_Sentence_Usefunction(string xrs)
            {
                resulter = new Variable.Resulter(xrs);
            }
            public override void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("usefun_s");
                element.SetAttribute("str", mycode);
                resulter.ToXml(xmlDocument, element);
                xmlElement.AppendChild(element);

            }
        }
        public class New_Sentence_while : Sentence
        {
            public Variable.Resulter resulter;
            public Sentence[] childsentences;
            public New_Sentence_while(string xbn, ArrayList str_allsens)
            {
                resulter = new Variable.Resulter(xbn);
                childsentences = GetSentencesfromArrayList(str_allsens);
            }
            
            public override void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("while_s");
                element.SetAttribute("str", mycode);
                {
                    XmlElement express = xmlDocument.CreateElement("express");
                    resulter.ToXml(xmlDocument, express);
                    XmlElement run = xmlDocument.CreateElement("run");
                    foreach (var i in childsentences)
                        i.ToXml(xmlDocument, run);
                    element.AppendChild(express);
                    element.AppendChild(run);
                }
                xmlElement.AppendChild(element);
            }
        }
        public class New_Sentence_try : Sentence
        {
            public string exname;
            public Sentence[] trysentences, catchsentences;
            internal bool var_new;

            public New_Sentence_try( ArrayList str_allsens)
            {
                
                
                trysentences = GetSentencesfromArrayList(str_allsens);
            }
           

            public override void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("try_s");
                element.SetAttribute("str", mycode);
                XmlElement trys = xmlDocument.CreateElement("then");
                foreach (var i in trysentences)
                    i.ToXml(xmlDocument, trys);
                XmlElement catchs = xmlDocument.CreateElement("catch");
                catchs.SetAttribute("except", exname);
                catchs.SetAttribute("var_new", var_new.ToString());
                
                foreach (var i in catchsentences)
                    i.ToXml(xmlDocument, catchs);
                element.AppendChild(trys);
                element.AppendChild(catchs);
                xmlElement.AppendChild(element);
            }
        }
        public class New_Sentence_foreach : Sentence
        {
            Variable.Resulter resulter;
            string fzvar;//待赋值变量
            public Sentence[] childsentences;
            bool var_new = false;
            /// <summary>
            /// 构造foreach
            /// </summary>
            /// <param name="fzv">待赋值变量</param>
            /// <param name="xbn">右侧表达式</param>
            /// <param name="str_allsens">旗下语句</param>
            public New_Sentence_foreach(string fzv, string xbn, ArrayList str_allsens)
            {
                if (fzv.Length >4 && fzv.Substring(0, 4) == "var ")
                {
                    var_new = true;
                    fzvar = fzv.Substring(4);
                }
                else
                {
                    fzvar = fzv;
                }
                resulter = new Variable.Resulter(xbn);
                childsentences = GetSentencesfromArrayList(str_allsens);
            }
            public override void ToXml(XmlDocument xmlDocument, XmlElement xmlElement)
            {
                XmlElement element = xmlDocument.CreateElement("foreach_s");
                element.SetAttribute("var_togive", fzvar);
                element.SetAttribute("var_new", var_new.ToString() );
                element.SetAttribute("str", mycode);
                XmlElement from = xmlDocument.CreateElement("from");
                resulter.ToXml(xmlDocument, from);
                XmlElement torun = xmlDocument.CreateElement("run");
                foreach (var i in childsentences)
                    i.ToXml(xmlDocument, torun);
                element.AppendChild(from);
                element.AppendChild(torun);
                xmlElement.AppendChild(element);
            }
        }
    }
}