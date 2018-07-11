using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Models {
    /// <summary>
    /// 建立token Model
    /// </summary>
    public class Token {
        /// <summary>
        /// Token值
        /// </summary>
        public string TokenValue { get; set; }
        /// <summary>
        /// Token型態
        /// </summary>
        public string TokenType { get; set; }
        /// <summary>
        /// 系統編號
        /// </summary>
        public int ApplicationID { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 刷新日期
        /// </summary>
        public DateTime RefreshDateTime { get; set; }
        /// <summary>
        /// 到期日期
        /// </summary>
        public DateTime ExpireDateTime { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// TOKEN版本
        /// </summary>
        public string VersionToken { get; set; }
    }
}
