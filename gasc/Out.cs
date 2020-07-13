using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace gasc
{
    public static class Out
    {
        /// <summary>
        /// 从 gas 文件到 IL 文件
        /// </summary>
        /// <param name="gaspath">gas文件地址</param>
        /// <param name="outpath">IL文件地址</param>
        /// <returns>成功与否</returns>
        public static bool Gas2IL(string gaspath,string outpath)
        {
            try
            {
                string gas;
                using (StreamReader streamReader = new StreamReader(gaspath))
                {
                    gas = streamReader.ReadToEnd();
                };
                App.AyalyseCode(gas, new System.Xml.XmlDocument(), outpath);
                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
        }
        public enum Mode { Function ,Sentence}
        public static string IModeGas2IL(string gas,Mode mode)
        {
            if(mode == Mode.Sentence)
            {
                var sentences = gas.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries);
                var dones = new List<string>();
                dones.Add("fun IMode():");
                foreach(var item in sentences)
                {
                    dones.Add("    " + item);
                }
                string done = "";
                foreach(var item in dones)
                {
                    done += item + "\r\n";
                }
                return App.Getfunandvar(done, new System.Xml.XmlDocument(), null);
            }
            return null;
        }
    }
}
