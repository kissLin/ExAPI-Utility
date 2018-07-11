using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Core.Models
{
    public class DevToken
    {
        public int token_idno { get; set; }
        public string token_type { get; set; }
        public string token_fkey { get; set; }
        public string token_fsecret { get; set; }
        public string token_fcode { get; set; }
        public DateTime token_fexpire { get; set; }
        public string token_fcause { get; set; }
        public string token_fsts { get; set; }
        public string token_tkey { get; set; }
        public string token_tsecret { get; set; }
        public string token_tcode { get; set; }
        public DateTime token_texpire { get; set; }
        public string token_tcause { get; set; }
        public string token_tsts { get; set; }
        public string token_notice { get; set; }
        public string idno { get; set; }
        public string token_istfno { get; set; }
        public string token_istfn { get; set; }
        public DateTime token_idate { get; set; }
        public string token_mstfno { get; set; }
        public string token_mstfn { get; set; }
        public DateTime token_mdate { get; set; }
    }
}
