using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Models {
    /// <summary>
    /// 
    /// </summary>
    public class TokenApplyPlace {
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 員工編號
        /// </summary>
        public string CreateID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateID { get; set; }
        /// <summary>
        /// 員工姓名
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// API金鑰
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApiSecret1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApiSecret2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AllowIPs { get; set; }

        /// <summary>
        /// Token 的值
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Token Sehma
        /// </summary>
        public string TokenType { get; set; }
        /// <summary>
        /// 開發人員ID
        /// </summary>
        public int ApplicationID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime RefreshDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpireDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
    }
}
