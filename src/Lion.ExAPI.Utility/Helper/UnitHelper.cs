using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Helper {
    internal class UnitHelper {

        public static bool checkApiFormat(string jsonReturn) {
            try {
                if (!string.IsNullOrEmpty(jsonReturn)) {
                    var responseBody = JsonConvert.DeserializeObject<Lion.ExAPI.Utility.Models.V2.ResponseBase<Object>>(jsonReturn);
                    return true;
                }else {
                    return false;
                }
            }
            catch {
                return false;
            }
        }

        public static bool checkApiFormat1(string jsonReturn) {
            try {
                if (!string.IsNullOrEmpty(jsonReturn)) {
                    var responseBody = JsonConvert.DeserializeObject<Lion.ExAPI.Utility.Models.V2.ResponseBase>(jsonReturn);
                    return true;
                } else {
                    return false;
                }
            }
            catch {
                return false;
            }
        }
    }
}
