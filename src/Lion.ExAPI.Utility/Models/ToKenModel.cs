using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Models {
    /// <summary>
    /// 
    /// </summary>
    public class ToKenModel {
        /// <summary>
        /// Token值
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public string CreateDateTime { get; set; }
        /// <summary>
        /// 刷新日期
        /// </summary>
        public string RefreshDateTime { get; set; }
        /// <summary>
        /// 到期日期
        /// </summary>
        public string ExpireDateTime { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 驗證碼
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Checksum { get; set; }
    }
}
