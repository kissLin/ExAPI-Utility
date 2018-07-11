using System;
#if net451
using System.ComponentModel;
#endif
using System.Linq;
using System.Reflection;

namespace Lion.ExAPI.Utility.Extensions {
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtension {

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
        public static string Get_rCode(this Enum eu) {
            FieldInfo fi = eu.GetType().GetField(eu.ToString());
            var attributes = fi.GetCustomAttributes(typeof(rCode), false) as rCode[];
            return attributes.Any() ? attributes[0].value : fi.Name;
        }

        /// <summary>
        /// <summary>取得列舉 Description 的值</summary>   
        /// </summary>
        /// <param name="eu"></param>
        /// <returns></returns>
        public static string Get_rCodeDesc(this Enum eu) {
            FieldInfo fi = eu.GetType().GetField(eu.ToString());
            var attributes = fi.GetCustomAttributes(typeof(rCodeDesc), false) as rCodeDesc[];
            return attributes.Any() ? attributes[0].value : fi.Name;
        }
    }

    #region Attribute

    /// <summary>Enum自定參數：rCode</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class rCode : System.Attribute {
        private string _rCode;
        /// <summary>
        /// 
        /// </summary>
        public string value { get { return this._rCode; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public rCode(string value) : base() {
            this._rCode = value;
        }
    }

    /// <summary>Enum自定參數：rCodeDesc</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class rCodeDesc : System.Attribute {
        private string _rCodeDescription;
        /// <summary>
        /// 
        /// </summary>
        public string value { get { return this._rCodeDescription; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public rCodeDesc(string value) : base() {
            this._rCodeDescription = value;
        }
    }

    #endregion
}
