using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Hangfire.Models {
    #region ResponseBase
    internal class ResponseBase<T> {

        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }
    }

    internal class ResponseBase {

        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

        public dynamic Data { get; set; }
    }
    #endregion ResponseBase
}
