using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Threading;

namespace gasc
{
    class Opearate
    {


        /*

          :
  1       !
  3       * / %
  3       + - &
  7       > >=(~) < <=(@) ==(#) !=($)  &= (Α)
  2       &&(^) ||(Β)
         
         
  3       , ; =
         */
        public static string SetMathFunction(string input)
        {



            string str_codes = input;
            Opearate p = SetString(str_codes);
            str_codes = p.donecode
                .Replace(">=", "~")
                .Replace("<=", "@")
                .Replace("==", "#")
                .Replace("!=", "$")
                .Replace("&=", "Α")
                .Replace("&&", "^")
                .Replace("||", "Β")
                ;
            string[] tstr = p.stringes;
            ArrayList codes = new ArrayList();
            for (int i = 0; i < str_codes.Length; i++)
            {
                codes.Add(str_codes.Substring(i, 1));
            }
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString() == "Β" || codes[i].ToString() == "^")
                {
                    i = AndOrChange(codes, i, codes[i].ToString()) - 1;
                }
                else if (
                    codes[i].ToString() == ">" ||
                    codes[i].ToString() == "<" ||
                    codes[i].ToString() == "~" ||
                    codes[i].ToString() == "@" ||
                    codes[i].ToString() == "#" ||
                    codes[i].ToString() == "$" ||
                    codes[i].ToString() == "Α"
                     )
                {

                    i = CompairChange(codes, i, codes[i].ToString()) - 1;

                }
                else if (codes[i].ToString() == "!")
                {


                    i = NotChange(codes, i) - 1;

                }
                else if (codes[i].ToString() == "+" || codes[i].ToString() == "-" || codes[i].ToString() == "&")
                {
                    i = SumChange(codes, i, codes[i].ToString()) - 1;
                }

                else if (codes[i].ToString() == "*" || codes[i].ToString() == "/" || codes[i].ToString() == "%")
                {
                    i = MulDivChange(codes, i, codes[i].ToString()) - 1;
                }
                //else if (codes[i].ToString() == ":" && i < codes.Count-1)
                //{
                //    i = GetM(codes, i)-1;
                //}

            }

            StringBuilder sb = new StringBuilder();
            foreach (string s in codes)
            {
                sb.Append(s);

            }
            string done = sb.ToString();
            done = string.Format(done, tstr);
            return done;


        }

        
        // ",", ";", "=", ">", "~", "<", "@", "#", "$", "+", "-", "*", "/", "%", "!", "^", "Β", "Α","&" 
        private static int NotChange(ArrayList codes, int charindex)
        {
            string[] stops = new string[] { ",", ";", "=", ">", "~", "<", "@", "#", "$", "+", "-", "*", "/", "%", "!", "^", "Β", "Α","&" };

            int right = FindRight(codes, charindex, stops);
            ArrayList arr_right = new ArrayList();
            for (int i = charindex + 1; i <= right; i++)
            {
                arr_right.Add(codes[i]);
            }
            ArrayList ret = new ArrayList();
            codes.RemoveRange(charindex, right - charindex + 1);
            ret.Add("Math.Not");
            ret.Add("(");
            ret.InsertRange(ret.Count, arr_right);
            ret.Add(")");
            codes.InsertRange(charindex, ret);
            return charindex;


        }


        // ",", ";", "=", ">", "~", "<", "@", "#", "$", "+", "-", "*", "/", "%", "^" , "Β" , "Α" ,"&" 
        private static int MulDivChange(ArrayList codes, int charindex, string methord)
        {
            ArrayList a = new ArrayList(), arr_left = new ArrayList(), arr_right = new ArrayList();
            switch (methord)
            {
                case "*":
                a.Add("Multiply");
                a.Add("(");
                break;
                case "/":
                a.Add("Divide"); a.Add("(");
                break;
                case "%":
                a.Add("MOD"); a.Add("(");
                break;
            }

            string[] stops = new string[] { ",", ";", "=", ">", "~", "<", "@", "#", "$", "+", "-", "*", "/", "%", "^", "Β", "Α", "&" };

            int left = FindLeft(codes, charindex, stops);
            int right = FindRight(codes, charindex, stops);

            for (int i = left; i < charindex; i++)
            {
                arr_left.Add(codes[i]);
            }
            for (int i = charindex + 1; i <= right; i++)
            {
                arr_right.Add(codes[i]);
            }

            a.InsertRange(a.Count, arr_left);
            a.Add(",");
            a.InsertRange(a.Count, arr_right);
            a.Add(")");

            codes.RemoveRange(left, right - left + 1);
            codes.InsertRange(left, a);
            return left;
        }

        //",", ";", "=", ">", "~", "<", "@", "#", "$", "+", "-", "^"  ,"Β", "Α","&" 
        private static int SumChange(ArrayList codes, int charindex, string methord)
        {
            ArrayList a = new ArrayList();
            switch (methord)
            {
                case "+":
                a.Add("Sum");
                a.Add("(");
                break;
                case "-":
                a.Add("Subtract");
                a.Add("(");
                break;
                case "&":
                a.Add("StringAdd");
                a.Add("(");
                break;
            }

            string[] stops = new string[] { ",", ";", "=", ">", "~", "<", "@", "#", "$", "+", "-", "^", "Β", "Α", "&" };
            ArrayList arr_left = new ArrayList(), arr_right = new ArrayList();
            int left = FindLeft(codes, charindex, stops);
            int right = FindRight(codes, charindex, stops);

            for (int i = left; i < charindex; i++)
            {
                arr_left.Add(codes[i]);

            }
            for (int i = charindex + 1; i <= right; i++)
            {
                arr_right.Add(codes[i]);
            }

            a.InsertRange(a.Count, arr_left);
            a.Add(",");
            a.InsertRange(a.Count, arr_right);
            a.Add(")");
            codes.RemoveRange(left, right - left + 1);
            codes.InsertRange(left, a);
            return left;
        }

        //",", ";", "=", ">", "~", "<", "@", "#", "$","^", "Β", "Α"
        private static int CompairChange(ArrayList codes, int charindex, string methord)
        {


            ArrayList a = new ArrayList();
            switch (methord)
            {
                case ">":
                a.Add("Bigger");
                a.Add("(");
                break;
                case "~":
                a.Add("BiggerAndEqual");
                a.Add("(");
                break;

                case "<":
                a.Add("Smaller");
                a.Add("(");
                break;

                case "@":
                a.Add("SmallerAndEqual");
                a.Add("(");
                break;

                case "#":
                a.Add("Equal");
                a.Add("(");
                break;

                case "$":
                a.Add("UnEqual");
                a.Add("(");
                break;
                case "Α":
                a.Add("StringIsEqual");
                a.Add("(");
                break;
            }

            string[] stops = new string[] { ",", ";", "=", ">", "~", "<", "@", "#", "$", "^", "Β", "Α" };
            int left = FindLeft(codes, charindex, stops);
            int right = FindRight(codes, charindex, stops);
            ArrayList arr_left = new ArrayList(), arr_right = new ArrayList();
            for (int i = left; i < charindex; i++)
            {
                arr_left.Add(codes[i]);

            }
            for (int i = charindex + 1; i <= right; i++)
            {
                arr_right.Add(codes[i]);
            }


            a.InsertRange(a.Count, arr_left);
            a.Add(",");
            a.InsertRange(a.Count, arr_right);
            a.Add(")");
            codes.RemoveRange(left, right - left + 1);
            codes.InsertRange(left, a);
            return left;



        }

        //",", ";", "=", "^", "Β"
        private static int AndOrChange(ArrayList codes, int charindex, string methord)
        {
            codes.RemoveAt(charindex);
            ArrayList a = new ArrayList(), arr_left = new ArrayList(), arr_right = new ArrayList();
            if (methord == "^")
            {
                a.Add("CompareAnd");

                a.Add("(");
            }
            else
            {
                a.Add("CompareOr");

                a.Add("(");
            }
            string[] stops = new string[] { ",", ";", "=", "^", "Β" };
            int left = FindLeft(codes, charindex, stops);
            int right = FindRight(codes, charindex, stops);
            for (int i = left; i < charindex; i++)
            {
                arr_left.Add(codes[i]);

            }
            for (int i = charindex; i <= right; i++)
            {
                arr_right.Add(codes[i]);
            }

            a.InsertRange(a.Count, arr_left);
            a.Add(",");
            a.InsertRange(a.Count, arr_right);
            a.Add(")");


            codes.RemoveRange(left, right - left + 1);
            codes.InsertRange(left, a);
            return left;


        }


        public static int FindRight(ArrayList codes, int charindex, string[] stops)
        {
            int ret = charindex + 1, cc = 0;
            ArrayList arr_stops = new ArrayList(stops);
            arr_stops.Add(")");
            while (ret < codes.Count)
            {
                if (codes[ret].ToString() == "(")
                {
                    
                    cc++;
                    ret++;
                    continue;
                }
                if (codes[ret].ToString() == ")" && cc != 0)
                {
                    cc--;
                    ret++;
                    continue;
                }
                if (cc != 0) { ret++; continue; }
                if (ret == codes.Count - 1)
                {
                    return ret - 1;
                }
                else
                {
                    //ret++;
                    for (int i = 0; i < arr_stops.Count; i++)
                    {
                        if (codes[ret].ToString() == arr_stops[i].ToString() || codes[ret].ToString() + codes[ret + 1].ToString() == arr_stops[i].ToString())
                        {
                            return ret - 1;
                        }
                    }
                    ret++;
                }
            }
            return codes.Count - 1;
        }


        public static int FindLeft(ArrayList codes, int charindex, string[] stops)
        {
            int ret
             = charindex - 1;
            int cc = 0;
            ArrayList arr_stops = new ArrayList(stops);
            arr_stops.Add("(");
            while (ret >= 0)
            {
                if (codes[ret].ToString() == ")") { cc++; ret--; continue; }
                if (codes[ret].ToString() == "(" && cc != 0) { cc--; ret--; continue; }
                if (cc != 0) { ret--; continue; }
                if (ret == 0)
                {
                    return 0;
                }
                else
                {
                    //ret--;
                    for (int i = 0; i < arr_stops.Count; i++)
                    {
                        if (codes[ret].ToString() == arr_stops[i].ToString() || codes[ret - 1].ToString() + codes[ret].ToString() == arr_stops[i].ToString())
                        { return ret + 1; }
                    }
                    ret--;
                }
            }
            return 0;
        }


        string[] stringes;
        string donecode;

        public static Opearate SetString(string str_code)
        {
            ArrayList strings = new ArrayList(), arr_ret_codes = new ArrayList();

            StringBuilder ret_codes = new StringBuilder();
            int index = 0;
            ArrayList codes = new ArrayList();
            for (int i = 0; i < str_code.Length; i++)
            {
                codes.Add(str_code.Substring(i, 1));
            }

            bool isin = false;
            int start = 0;
            bool lastiszy = false;
            for (int i = 0; i < codes.Count; i++)
            {
                string code = codes[i].ToString();
                if (code == "\\")
                {
                    lastiszy = true;
                    //ret_codes.Append("\\");
                    continue;
                }
                if (lastiszy)
                {
                    lastiszy = false;
                    //ret_codes.Append(code);
                    continue;
                }

                if (code == "\"")
                {
                    if (isin)
                    {
                        //a"bcd"
                        strings.Add(str_code.Substring(start, i - start + 1));
                        //codes.RemoveRange(start, i - start + 1);
                        //codes.Insert(start, "{" + index + "}");
                        ret_codes.Append("{" + index + "}");
                        index++;
                        isin = false;
                        continue;
                    }
                    else if (!isin)
                    {
                        isin = true;
                        start = i;
                        continue;
                    }
                }

                if (!isin)
                {
                    ret_codes.Append(code);
                }


            }

            //Console.WriteLine(ret_codes.ToString());
            Opearate p = new Opearate
            {
                donecode = ret_codes.ToString()
            };
            string[] sss = new string[strings.Count];
            for (int i = 0; i < sss.Length; i++)
            {
                sss[i] = strings[i].ToString();
            }
            p.stringes = sss;
            return p;

        }

    }

}
