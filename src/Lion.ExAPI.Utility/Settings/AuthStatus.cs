using Lion.ExAPI.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Settings {
    /// <summary>
    /// 
    /// </summary>
    public enum AuthStatus {
        /// <summary>[rCode:0406]未設定/異常</summary>
        [rCode("0406"), rCodeDesc("Auth發生異常錯誤")]
        None,
        /// <summary>[rCode:0000]驗證通過</summary>
        [rCode("0000"), rCodeDesc("")]
        Authorized,
        /// <summary>[rCode:0400]Token不存在</summary>
        [rCode("0400"), rCodeDesc("Token異常，請重新取得")]
        Revoked_TokenNotExists,
        /// <summary>[rCode:0404]header[Authorization]不存在</summary>
        [rCode("0404"), rCodeDesc("請於header中加上Authorization驗證")]
        Revoked_AuthorizationNotExists,
        /// <summary>[rCode:0405]header[Authorization]格式錯誤</summary>
        [rCode("0405"), rCodeDesc("Authorization格式錯誤")]
        Revoked_AuthorizationFormatIncorrect,
        /// <summary>[rCode:0402]無使用權限（應用程式不存在）</summary>
        [rCode("0402"), rCodeDesc("Token異常，請重新申請ApiKey")]
        Revoked_AppNotExists,
        /// <summary>[rCode:0403]無使用權限（非允許IP）</summary>
        [rCode("0403"), rCodeDesc("此IP無權限存取")]
        Revoked_NotAllowIP,
        /// <summary>[rCode:0401]Token已過期</summary>
        [rCode("0401"), rCodeDesc("Token已超過時效，請重新取得")]
        Revoked_Expired,
        /// <summary>[rCode:0401]V2回傳的格式不對</summary>
        [rCode("0408"), rCodeDesc("V2回傳的格式不對")]
        Rvoked_ErrorReponseBase,
        /// <summary>[rCode:0401]Controller版本不為V2</summary>
        [rCode("0407"), rCodeDesc("Controller版本不為V2")]
        Revoked_ErrorUrlPath,
        /// <summary>[rCode:0401]Token，停用</summary>
        [rCode("0409"), rCodeDesc("Token，停用")]
        Revoked_TokenDisabled,
        /// <summary>[rCode:0401]無法使用這支API，請申請API</summary>
        [rCode("0410"), rCodeDesc("無法使用這支API，請申請API")]
        Revoked_NotAllowUrl
    }
}
