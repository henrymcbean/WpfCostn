using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCostn.Model
{
    public class StyleDetailsClass
    {
        #region Instance Properties
        public Int32 CostdbID { get; set; }
        public String CTStyle { get; set; }
        public String CTVarn { get; set; }
        public String CTStyleDescr { get; set; }
        public String CTMainFabric { get; set; }
        public Int16? CTGarType { get; set; }
        public DateTime? CTDesignDate { get; set; }
        public double? CTSelPrice1 { get; set; }
        public Int16? CTSpareShort1 { get; set; }
        public String TypeDesc { get; set; }
        public String Category { get; set; }
        public Int32 Template { get; set; }
        public Int32 CompletionFlag { get; set; }
    }
}
