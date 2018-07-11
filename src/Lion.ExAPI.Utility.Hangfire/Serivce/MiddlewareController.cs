using System;
using Lion.ExAPI.Utility.Hangfire.Models;
using Lion.ExAPI.Utility.Hangfire.Unit;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#if NETSTANDARD1_6
using Microsoft.AspNetCore.Http;
using Lion.ExAPI.Utility.Hangfire.Core.Models;
using Lion.ExAPI.Utility.Hangfire.Core.Unit;
#endif
namespace Lion.ExAPI.Utility.Hangfire.Core.Serivce {
    public class MiddlewareController {
        private RequestReponseModels _requestreponseModels;
        private Log mylog;

        public MiddlewareController() {
            mylog = new Log();
        }


#if NETSTANDARD1_6
        #region EnterInsert to Hangfire
        internal async Task EnterInsertAsync(HttpContext context, Exception exception = null) {
            string result = string.Empty;
            if (context.Request.Path.HasValue) {
                if (context.Request.Path.Value.IndexOf("/V2/ErrLogRecord", StringComparison.OrdinalIgnoreCase) >= 0 || 
                    context.Request.Path.Value.IndexOf("/V2/LogRecord", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    context.Request.Path.Value.IndexOf("/V2/LogTrace", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    context.Request.Path.Value.IndexOf("/V2/ExtraLog", StringComparison.OrdinalIgnoreCase) >= 0 ) {
                    if (JsonSplit.IsJson(context.Items[SettingHelper.Re_quest].ToString())) {
                        ClientLog(context.Items[SettingHelper.Re_quest].ToString(), context.Items[SettingHelper.App_lication].ToString());
                    } else {
                        if (exception != null) {
                          await NormalAsync(context, exception);
                        } else {
                            await NormalAsync(context);
                        }
                    }
                } else {
                    if (exception != null) {
                        await NormalAsync(context, exception);
                    } else {
                        await NormalAsync(context);
                    }
                }
            }
        }
        #endregion EnterInsert to Hangfire
#endif
#if NETSTANDARD1_6
        #region Normal to Hangfire
        private async Task NormalAsync(HttpContext context, Exception exception = null) {

            if (exception == null) {
                _requestreponseModels = new RequestReponseModels() {
                    connectModel = new HandlingLogContext(context).GetData(),
                    errModel = null
                };

                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(JsonConvert.SerializeObject(_requestreponseModels.connectModel)) > SettingHelper.Over_ReponseSize) {
                    mylog.SizeErrorLog(JsonConvert.SerializeObject(_requestreponseModels.connectModel), $"{Directory.GetCurrentDirectory()}");
                } else {
                    var client = new HttpClient()
                    {
                        BaseAddress = new Uri(ToolsContext.config.GetSection(SettingHelper.LogDomain).Value + "api/Log/LogConnectErrorAsync"),
                        Timeout = TimeSpan.FromMilliseconds(300)
                    };
                    try
                    {
                        var response = await client.PostAsync(ToolsContext.config.GetSection(SettingHelper.LogDomain).Value + "api/Log/LogConnectErrorAsync", new StringContent(JsonConvert.SerializeObject(_requestreponseModels).ToString(), Encoding.UTF8, "application/json"));
                    }
                    catch (Exception e)
                    {
                        mylog.HangfireErrorLog(e.Message, $"{Directory.GetCurrentDirectory()}");
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
            } else {

                _requestreponseModels = new RequestReponseModels() {
                    connectModel = new HandlingLogContext(context).GetData(),
                    errModel = new ErrLogContext(context, exception).GetErrData()
                };

                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(JsonConvert.SerializeObject(_requestreponseModels.connectModel)) > SettingHelper.Over_ReponseSize ||
                    System.Text.ASCIIEncoding.ASCII.GetByteCount(JsonConvert.SerializeObject(_requestreponseModels.errModel)) > SettingHelper.Over_ReponseSize) {
                    mylog.SizeErrorLog(JsonConvert.SerializeObject(_requestreponseModels.connectModel), $"{Directory.GetCurrentDirectory()}");
                    mylog.SizeErrorLog(JsonConvert.SerializeObject(_requestreponseModels.errModel), $"{Directory.GetCurrentDirectory()}");
                }  else {

                    var client = new HttpClient()
                    {
                        BaseAddress = new Uri(ToolsContext.config.GetSection(SettingHelper.LogDomain).Value + "api/Log/LogConnectErrorAsync"),
                        Timeout = TimeSpan.FromMilliseconds(300)
                    };
                    try
                    {
                        var response = await client.PostAsync(ToolsContext.config.GetSection(SettingHelper.LogDomain).Value + "api/Log/LogConnectErrorAsync", new StringContent(JsonConvert.SerializeObject(_requestreponseModels).ToString(), Encoding.UTF8, "application/json"));
                    }
                    catch (Exception e)
                    {
                        mylog.HangfireErrorLog(e.Message, $"{Directory.GetCurrentDirectory()}");
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
            }

        }
        #endregion Normal to Hangfire
#endif

        #region Clinet to Hangfire
        public void ClientLog(string jsonResult,string ApplicationName) {
            //BackgroundJob.Enqueue<AddLogRepository>(x => AddLogRepository.ClientLog(jsonResult, ApplicationName));
        }
        #endregion Clinet to Hangfire
    }

}
