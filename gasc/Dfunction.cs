using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace gasc
{
    partial class Function
    {
        public class DFunction : IFunction
        {
            public string str_xcname = "";
            public bool isreffunction = false;
            string IFunction.Istr_xcname { get => str_xcname; set => str_xcname = value; }
            bool IFunction.Iisreffunction { get => isreffunction; set => isreffunction = value; }
            public delegate object DRun(Hashtable xc);
            public DRun dRun;
            object IFunction.IRun(Hashtable xc)
            {
                return (dRun(xc));
            }
        }
    }
}
