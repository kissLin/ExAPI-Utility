using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire {

    #region HttpErrorCodeEnum
    /// <summary>
    /// 判斷 http code Status
    /// </summary>
    [Flags]
    internal enum HttpErrorCodeEnum {
        /// <summary>Enum初始值</summary> 
        [rCode("#"), rCodeDesc("not Implemented")]
        none,

        /// <summary>
        /// 200,。
        /// </summary>
        [rCode("200"), rCodeDesc("請求已成功，請求所希望的回應頭或資料體將隨此回應返回")]
        OK = 200,

        /// <summary>
        /// 400，由於包含語法錯誤，當前請求無法被伺服器理解。除非進行修改，否則用戶端不應該重複提交這個請求。
        /// </summary>
        [rCode("400"), rCodeDesc("由於包含語法錯誤，當前請求無法被伺服器理解。除非進行修改，否則用戶端不應該重複提交這個請求")]
        Bad_Request = 400,

        /// <summary>
        /// 401,當前請求需要用戶驗證。
        /// </summary>
        [rCode("401"), rCodeDesc("當前請求需要用戶驗證")]
        Unauthorized = 401,

        /// <summary>
        /// 402,該狀態碼是為了將來可能的需求而預留的。
        /// </summary>
        [rCode("402"), rCodeDesc("該狀態碼是為了將來可能的需求而預留的")]
        Payment_Required = 402,

        /// <summary>
        /// 403,伺服器已經理解請求，但是拒絕執行它。
        /// </summary>
        [rCode("403"), rCodeDesc("伺服器已經理解請求，但是拒絕執行它")]
        Forbidden = 403,

        /// <summary>
        /// 404,請求失敗，請求所希望得到的資源未被在伺服器上發現。
        /// </summary>
        [rCode("404"), rCodeDesc("請求失敗，請求所希望得到的資源未被在伺服器上發現")]
        Not_Found = 404,

        /// <summary>
        /// 405,請求行中指定的請求方法不能被用於請求相應的資源。
        /// </summary>
        [rCode("405"), rCodeDesc("請求行中指定的請求方法不能被用於請求相應的資源")]
        Method_Not_Allowed = 405,

        /// <summary>
        /// 408,請求超時。用戶端沒有在伺服器預備等待的時間內完成一個請求的傳送。
        /// </summary>
        [rCode("408"), rCodeDesc("請求超時。用戶端沒有在伺服器預備等待的時間內完成一個請求的傳送")]
        Request_Timeout = 408,

        /// <summary>
        /// 415,對於當前請求的方法和所請求的資源，請求中提交的實體並不是伺服器中所支援的格式，因此請求被拒絕。
        /// </summary>
        [rCode("415"), rCodeDesc("對於當前請求的方法和所請求的資源，請求中提交的實體並不是伺服器中所支援的格式，因此請求被拒絕")]
        Unsupported_Media_Type = 415,

        /// <summary>
        /// 500,伺服器遇到了一個未曾預料的狀況，導致了它無法完成對請求的處理。一般來說，這個問題都會在伺服器的程式碼出錯時出現。
        /// </summary>
        [rCode("500"), rCodeDesc("伺服器遇到了一個未曾預料的狀況，導致了它無法完成對請求的處理。一般來說，這個問題都會在伺服器的程式碼出錯時出現")]
        Internal_Server_Error = 500,

        /// <summary>
        /// 502,作為閘道器或者代理工作的伺服器嘗試執行請求時，從上游伺服器接收到無效的回應。
        /// </summary>
        [rCode("502"), rCodeDesc("作為閘道器或者代理工作的伺服器嘗試執行請求時，從上游伺服器接收到無效的回應")]
        Bad_Gateway = 502,

        /// <summary>
        /// 503,由於臨時的伺服器維護或者過載，伺服器當前無法處理請求。
        /// </summary>
        [rCode("503"), rCodeDesc("由於臨時的伺服器維護或者過載，伺服器當前無法處理請求")]
        Service_Unavailable = 503,
    }
    #endregion HttpErrorCodeEnum

    #region EnumExtension
    /// <summary>
    /// 
    /// </summary>
    internal static class EnumExtension
    {


#if net451
        /// <summary>
        /// <summary>取得列舉 Description 的值</summary>    
        /// </summary>
        /// <param name="eu"></param>
        /// <returns></returns>
        public static string GetDesc(this Enum eu) {
            FieldInfo fi = eu.GetType().GetField(eu.ToString());
            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes.Any() ? attributes[0].Description : fi.Name;
        }
#endif
        /// <summary>
        /// 取得列舉 Rcode 的值
        /// </summary>
        /// <param name="eu"></param>
        /// <returns></returns>
        public static string Get_rCode(this Enum eu)
        {
            FieldInfo fi = eu.GetType().GetField(eu.ToString());
            var attributes = fi.GetCustomAttributes(typeof(rCode), false) as rCode[];
            return attributes.Any() ? attributes[0].value : fi.Name;
        }

        /// <summary>
        /// <summary>取得列舉 Description 的值</summary>   
        /// </summary>
        /// <param name="eu"></param>
        /// <returns></returns>
        public static string Get_rCodeDesc(this Enum eu)
        {
            FieldInfo fi = eu.GetType().GetField(eu.ToString());
            var attributes = fi.GetCustomAttributes(typeof(rCodeDesc), false) as rCodeDesc[];
            return attributes.Any() ? attributes[0].value : fi.Name;
        }
    }

    #region Attribute

    /// <summary>Enum自定參數：rCode</summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class rCode : System.Attribute
    {
        private string _rCode;
        /// <summary>
        /// 
        /// </summary>
        public string value { get { return this._rCode; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public rCode(string value) : base()
        {
            this._rCode = value;
        }
    }

    /// <summary>Enum自定參數：rCodeDesc</summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class rCodeDesc : System.Attribute
    {
        private string _rCodeDescription;
        /// <summary>
        /// 
        /// </summary>
        public string value { get { return this._rCodeDescription; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public rCodeDesc(string value) : base()
        {
            this._rCodeDescription = value;
        }
    }

    #endregion

    #endregion EnumExtension

    #region rCodeStatusEnumLog
    internal enum rCodeStatusEnumLog
    {
        /// <summary>Enum初始值</summary> 
        [rCode("#"), rCodeDesc("not Implemented")]
        none,

        /// <summary>0000:成功</summary> 
        [rCode("0000"), rCodeDesc("Success!")]
        rCode_0000,

        /// <summary>0005:系統發生錯誤</summary> 
        [rCode("0005"), rCodeDesc("System Service Exception")]
        rCode_0005
    }
    #endregion rCodeStatusEnumLog

}
