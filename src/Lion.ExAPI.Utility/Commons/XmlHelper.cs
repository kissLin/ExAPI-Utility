using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lion.ExAPI.Utility.Commons {

    /// <summary>
    ///  xml Helper Only for Net45~Net46
    /// </summary>
    public static class XmlHelper {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string Serialize(object o) {
            XmlSerializer ser = new XmlSerializer(o.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            ser.Serialize(writer, o);
            return sb.ToString();
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml) {
            T result;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (TextReader tr = new StringReader(xml)) {
                result = (T)ser.Deserialize(tr);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T FromXml<T>(string xml) {
            T returnedXmlClass = default(T);

            try {
                using (TextReader reader = new StringReader(xml)) {
                    try {
                        returnedXmlClass =
                            (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                    }
                    catch (InvalidOperationException) {
                        // String passed is not XML, simply return defaultXmlClass
                    }
                }
            }
            catch (Exception ex) {
                ex.Message.ToString();
            }

            return returnedXmlClass;
        }
    }
}
