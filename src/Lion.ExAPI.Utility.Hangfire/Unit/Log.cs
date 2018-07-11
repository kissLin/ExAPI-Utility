using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
#if net451
using System.Web;
#endif
namespace Lion.ExAPI.Utility.Hangfire.Unit {
    #region Log
    internal class Log {
        /// <summary>
        /// lock object
        /// </summary>
        private static object _thisLock = new object();

        #region ObjectErrorLog
        /// <summary>
        /// 設定錯誤資訊取得錯誤檔案
        /// </summary>
        /// <param name="webRootPath">錯誤的頁面名稱</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        public void ObjectErrorLog(string sErrMsg, string webRootPath) {

            if (!System.IO.Directory.Exists(webRootPath + "\\wwwroot\\Log\\Temp\\ObjectException")) {
                System.IO.Directory.CreateDirectory(webRootPath + "\\wwwroot\\Log\\Temp\\ObjectException\\");
            }

            string logFilename = DateTime.Now.ToString("yyyyMMdd");
            string TXTF = webRootPath + "\\wwwroot\\Log" + "\\Temp\\ObjectException\\" + logFilename + ".log";
            if (System.IO.File.Exists(TXTF) == false) {
                System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                stmWriter.Dispose();
                fs.Dispose();
            } else {
                lock (_thisLock) {
                    //檢查檔案存在
                    System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                    stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                    stmWriter.Dispose();
                    fs.Dispose();
                }

            }

        }
        #endregion ObjectErrorLog

        #region HangfireErrorLog
        /// <summary>
        /// 設定錯誤資訊取得錯誤檔案
        /// </summary>
        /// <param name="sErrMsg">錯誤的專案名稱</param>
        /// <param name="webRootPath">錯誤的頁面名稱</param>
        /// <param name="ex">錯誤訊息</param>
        public void HangfireErrorLog(string sErrMsg, string webRootPath, string ex = null) {

#if net451
            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = (DateTime.Now.Month.ToString().Length == 1) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            string sDay = (DateTime.Now.Day.ToString().Length == 1) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string sErrorTime = sYear + sMonth + sDay;

            dynamic strPath = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "Log\\Temp\\HangfireException\\";
            if (!System.IO.Directory.Exists(strPath)) {
                System.IO.Directory.CreateDirectory(strPath);
            }
            StreamWriter sw = new StreamWriter(strPath + sErrorTime + ".txt", true);

            try {
                sw.WriteLine(sLogFormat + Environment.NewLine + sErrMsg + Environment.NewLine + ex.ToString());
            }
            catch {
                throw;
            }
            finally {
                sw.Flush();
                sw.Close();
            }

#else
            if (!System.IO.Directory.Exists(webRootPath + "\\wwwroot\\Log\\Temp\\HangfireException")) {
                System.IO.Directory.CreateDirectory(webRootPath + "\\wwwroot\\Log\\Temp\\HangfireException\\");
            }

            string logFilename = DateTime.Now.ToString("yyyyMMdd");
            string TXTF = webRootPath + "\\wwwroot\\Log" + "\\Temp\\HangfireException\\" + logFilename + ".log";
            if (System.IO.File.Exists(TXTF) == false) {
                System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg + " Exception: " + ex);
                stmWriter.Dispose();
                fs.Dispose();
            } else {
                lock (_thisLock) {
                    //檢查檔案存在
                    System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                    stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                    stmWriter.Dispose();
                    fs.Dispose();
                }

            }
#endif


        }
        #endregion HangfireErrorLog

        #region SizeErrorLog
        /// <summary>
        /// 設定錯誤資訊取得錯誤檔案
        /// </summary>
        /// <param name="sErrMsg">錯誤的專案名稱</param>
        /// <param name="webRootPath">錯誤的頁面名稱</param>
        public void SizeErrorLog(string sErrMsg, string webRootPath) {
            if (!System.IO.Directory.Exists(webRootPath + "\\wwwroot\\Log\\Temp\\SizeException")) {
                System.IO.Directory.CreateDirectory(webRootPath + "\\wwwroot\\Log\\Temp\\SizeException\\");
            }

            string logFilename = DateTime.Now.ToString("yyyyMMdd");
            string TXTF = webRootPath + "\\wwwroot\\Log" + "\\Temp\\SizeException\\" + logFilename + ".log";
            if (System.IO.File.Exists(TXTF) == false) {
                System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));
                stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                stmWriter.Dispose();
                fs.Dispose();
            } else {
                lock (_thisLock) {
                    //檢查檔案存在
                    System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                    stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                    stmWriter.Dispose();
                    fs.Dispose();
                }

            }


        }
        #endregion SizeErrorLog

        #region JsonErrorLog
        /// <summary>
        /// 設定錯誤資訊取得錯誤檔案
        /// </summary>
        /// <param name="webRootPath">錯誤的頁面名稱</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        public void JsonErrorLog(string sErrMsg, string webRootPath) {
            if (!System.IO.Directory.Exists(webRootPath + "\\wwwroot\\Log\\Temp\\JsonException")) {
                System.IO.Directory.CreateDirectory(webRootPath + "\\wwwroot\\Log\\Temp\\JsonException\\");
            }

            string logFilename = DateTime.Now.ToString("yyyyMMdd");
            string TXTF = webRootPath + "\\wwwroot\\Log" + "\\Temp\\JsonException\\" + logFilename + ".log";
            if (System.IO.File.Exists(TXTF) == false) {
                System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));
                stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                stmWriter.Dispose();
                fs.Dispose();
            } else {
                lock (_thisLock) {
                    //檢查檔案存在
                    System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                    stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                    stmWriter.Dispose();
                    fs.Dispose();
                }

            }


        }
        #endregion JsonErrorLog

        #region ReponseSizeLog


        /// <summary>
        /// 設定錯誤資訊取得錯誤檔案
        /// </summary>
        /// <param name="webRootPath">錯誤的頁面名稱</param>
        /// <param name="sErrMsg">錯誤訊息</param>
        ///<param name="guid">錯誤訊息</param>
        public void ReponseSizeLog(string sErrMsg, string guid, string webRootPath) {
            string logFilename = DateTime.Now.ToString("yyyyMMdd");


#if net451
            
            if (!System.IO.Directory.Exists(webRootPath + "\\ReponseSizeLog\\" + logFilename + "\\" + guid.Substring(0, 2) + "\\")) {
                System.IO.Directory.CreateDirectory(webRootPath + "\\ReponseSizeLog\\" + logFilename + "\\" + guid.Substring(0, 2) + "\\");
            }


            string TXTF = webRootPath + "\\ReponseSizeLog\\" + logFilename + "\\" + guid.Substring(0, 2) + "\\" + guid + ".json";
            if (System.IO.File.Exists(TXTF) == false) {
                //System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                //System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));
                //stmWriter.WriteLine(sErrMsg);
                //stmWriter.Dispose();
                //fs.Dispose();
                CompressStringToFile(TXTF, sErrMsg);
            } else {
                lock (_thisLock) {
                    //檢查檔案存在
                    //System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    //System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                    //stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                    //stmWriter.Dispose();
                    //fs.Dispose();
                    CompressStringToFile(TXTF, sErrMsg);
                }
            }
#else

            if (!System.IO.Directory.Exists(webRootPath + "\\wwwroot\\Log\\Temp\\ReponseSizeLog\\" + logFilename + "\\" + guid.Substring(0, 2) + "\\")) {
                System.IO.Directory.CreateDirectory(webRootPath + "\\wwwroot\\Log\\Temp\\ReponseSizeLog\\" + logFilename + "\\" + guid.Substring(0, 2) + "\\");
            }


            string TXTF = webRootPath + "\\wwwroot\\Log\\Temp\\ReponseSizeLog\\" + logFilename + "\\" + guid.Substring(0, 2) + "\\" + guid + ".json";
            if (System.IO.File.Exists(TXTF) == false) {
                //System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                //System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));
                //stmWriter.WriteLine(sErrMsg);
                //stmWriter.Dispose();
                //fs.Dispose();
                CompressStringToFile(TXTF, sErrMsg);
            } else {
                lock (_thisLock) {
                    //檢查檔案存在
                    //System.IO.FileStream fs = new System.IO.FileStream(TXTF, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    //System.IO.StreamWriter stmWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));

                    //stmWriter.WriteLine(Convert.ToString(DateTime.Now) + " " + sErrMsg);
                    //stmWriter.Dispose();
                    //fs.Dispose();
                    CompressStringToFile(TXTF, sErrMsg);
                }
            }

#endif

        }
        #endregion ReponseSizeLog

        #region 壓縮檔案

        //壓縮檔案
        private static void CompressStringToFile(string strCompressFileName, string strFileContent)
        {
            try
            {
                //資料暫存的路徑
                string tempFilePath = Path.GetTempFileName();

                //將建立新檔案，將指定的資料寫入檔案中
                File.WriteAllText(tempFilePath, strFileContent);

                byte[] b;

                //開啟在新建立的檔案，並且將資料資料全部讀取
                using (FileStream f_ReadTempFileContent = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
                {
                    b = new byte[f_ReadTempFileContent.Length];
                    f_ReadTempFileContent.Read(b, 0, (int)f_ReadTempFileContent.Length);
                }

                //建立一個 *.gz 的檔案
                using (FileStream f_CompressFileName = new FileStream(strCompressFileName, FileMode.Create))

                //開始進行壓縮
                using (GZipStream gz = new GZipStream(f_CompressFileName, CompressionMode.Compress, false))
                {
                    gz.Write(b, 0, b.Length);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ObjectErrorLog(ex.Message.ToString(), Directory.GetCurrentDirectory());
            }
        }
        #endregion 壓縮檔案

        #region 解壓縮檔案
        //解壓縮檔案
        private static void DecompressStringToFile(string strCompressFileName, string strDecompressFileName)
        {
            int count = 0;
            int bufferSize = 4096;
            byte[] buffer = new byte[4096];

            try
            {
                //讀取壓縮的 *.gz 的檔案
                using (FileStream fs_CompressFile = new FileStream(strCompressFileName, FileMode.Open, FileAccess.Read, FileShare.Read))

                //建立一個解壓縮後的檔案路徑與名稱
                using (FileStream fs_DecompressFile = new FileStream(strDecompressFileName, FileMode.Create, FileAccess.Write, FileShare.None))

                //解壓縮 *.gz 的檔案
                using (GZipStream gz = new GZipStream(fs_CompressFile, CompressionMode.Decompress, true))

                    //讀取 *.gz 中的壓縮檔內容，並且寫入到新建立的檔案當中
                    //直到內容結束為止
                    while (true)
                    {
                        count = gz.Read(buffer, 0, bufferSize);

                        if (count != 0)
                            fs_DecompressFile.Write(buffer, 0, count);

                        if (count != bufferSize)
                            break;
                    }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ObjectErrorLog(ex.Message.ToString(), Directory.GetCurrentDirectory());
            }

        }
        #endregion 解壓縮檔案


    }
    #endregion Log
}
