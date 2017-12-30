using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vueAdminAPI.Common
{
    public class LoggerHelper
    {
        static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");
        static readonly log4net.ILog logmonitor = log4net.LogManager.GetLogger("logmonitor");

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ErrorMsg"></param>
        /// <param name="ex"></param>
        public static void Error(string ErrorMsg, Exception ex = null)
        {
            if (ex != null)
            {
                logerror.Error(ErrorMsg, ex);
            }
            else
            {
                logerror.Error(ErrorMsg);
            }
        }

        /// <summary>
        /// 记录消息日志
        /// </summary>
        /// <param name="Msg"></param>
        public static void Info(string Msg)
        {
            loginfo.Info(Msg);
        }

        /// <summary>
        /// 记录监控日志
        /// </summary>
        /// <param name="Msg"></param>
        public static void Monitor(string Msg)
        {
            logmonitor.Info(Msg);
        }
    }
}