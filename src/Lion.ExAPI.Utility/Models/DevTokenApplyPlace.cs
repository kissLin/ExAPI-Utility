using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Core.Models
{
    public class DevTokenApplyPlace
    {
        /// <summary>
        /// token id
        /// </summary>
        public int TokenId { get; set; }
        /// <summary>
        /// 正式機 / 測試機
        /// </summary>
        public string TokenStatus { get; set; }
        /// <summary>
        /// token key
        /// </summary>
        public string keys { get; set; }
        /// <summary>
        /// token secrets
        /// </summary>
        public string secrets { get; set; }
        /// <summary>
        /// token 啟用/停用
        /// </summary>
        public bool sts { get; set; }
        /// <summary>
        /// token value
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime expire { get; set; }
        //public DateTime RefreshDateTime { get; set; }
        /// <summary>
        /// 申請代碼
        /// </summary>
        public string apltApplyno { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string apltSitenames { get; set; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string apltSitename { get; set; }
        /// <summary>
        /// 使用部門
        /// </summary>
        public string apltComdep { get; set; }
        /// <summary>
        /// 使用者資訊
        /// </summary>
        public string apltIdnaex { get; set; }
        /// <summary>
        /// 接口名稱
        /// </summary>
        public string apiPort { get; set; }
        /// <summary>
        /// api 專案名稱
        /// </summary>
        public string canme { get; set; }
        /// <summary>
        /// api 使用的站台
        /// </summary>
        public string iisName { get; set; }
        /// <summary>
        /// api domain url
        /// </summary>
        public string iisPort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserIP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HostIP { get; set; }

        public List<String> iisPortandapiPort { get; set; }
    }
}
