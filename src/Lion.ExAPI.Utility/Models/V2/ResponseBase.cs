using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Models.V2 {
    /// <summary>
    /// [過期,請改用 ResponseBase]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseBase<T> {
        /// <summary>
        /// 
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rCode { get; set; }
        /// <summary>
        /// Token 到期時間
        /// </summary>
        [System.ComponentModel.DefaultValue(typeof(DateTime), "")]
        public DateTime TokenExpires { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ResponseBase : ResponseBase<dynamic> {

    }
}
