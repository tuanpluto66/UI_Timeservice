using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_TIMESERVICE
{
    public abstract class LogHelper
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        //Framework internal method

        #region  Debug information
        /// <summary>
        ///  Debug information
        /// </summary>
        /// <param name="message">Description</param>
        /// <param name="exception">Exception information</param>
        public static void Debug(object message, Exception exception = null)
        {
            Logger.Debug(message, exception);
        }
        public static void DebugFormat(string format, params object[] args)
        {
            Logger.DebugFormat(format, args);
        }
        public static void DebugFormat(Exception exception, string format, params object[] args)
        {
            Logger.DebugFormat(format, args, exception);
        }
        public static void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.DebugFormat(formatProvider, format, args);
        }
        public static void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.DebugFormat(formatProvider, format, args, exception);
        }
        #endregion

        #region  General information 
        /// <summary>
        ///  General information 
        /// </summary>
        /// <param name="message">Description</param>
        /// <param name="exception">Exception information</param>
        public static void Info(object message, Exception exception = null)
        {
            Logger.Info(message, exception);
        }
        public static void InfoFormat(string format, params object[] args)
        {
            Logger.InfoFormat(format, args);
        }
        public static void InfoFormat(Exception exception, string format, params object[] args)
        {
            Logger.InfoFormat(format, args, exception);
        }
        public static void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.InfoFormat(formatProvider, format, args);
        }
        public static void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.InfoFormat(formatProvider, format, args, exception);
        }
        #endregion

        #region  caveat 
        /// <summary>
        ///  caveat 
        /// </summary>
        /// <param name="message">Description</param>
        /// <param name="exception">Exception information</param>
        public static void Warn(object message, Exception exception = null)
        {
            Logger.Warn(message, exception);
        }
        public static void WarnFormat(string format, params object[] args)
        {
            Logger.WarnFormat(format, args);
        }
        public static void WarnFormat(Exception exception, string format, params object[] args)
        {
            Logger.WarnFormat(format, args, exception);
        }
        public static void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.WarnFormat(formatProvider, format, args);
        }
        public static void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.WarnFormat(formatProvider, format, args, exception);
        }
        #endregion

        #region  General error
        /// <summary>
        ///  General error
        /// </summary>
        /// <param name="message">Description</param>
        /// <param name="exception">Exception information</param>
        public static void Error(object message, Exception exception = null)
        {
            Logger.Error(message, exception);
        }
        public static void ErrorFormat(string format, params object[] args)
        {
            Logger.ErrorFormat(format, args);
        }
        public static void ErrorFormat(Exception exception, string format, params object[] args)
        {
            Logger.ErrorFormat(format, args, exception);
        }
        public static void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.ErrorFormat(formatProvider, format, args);
        }
        public static void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.ErrorFormat(formatProvider, format, args, exception);
        }
        #endregion

        #region  Fatal error
        /// <summary>
        ///  Fatal error
        /// </summary>
        /// <param name="message">Description</param>
        /// <param name="exception">Exception information</param>
        public static void Fatal(object message, Exception exception = null)
        {
            Logger.Fatal(message, exception);
        }
        public static void FatalFormat(string format, params object[] args)
        {
            Logger.FatalFormat(format, args);
        }
        public static void FatalFormat(Exception exception, string format, params object[] args)
        {
            Logger.FatalFormat(format, args, exception);
        }
        public static void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.FatalFormat(formatProvider, format, args);
        }
        public static void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Logger.FatalFormat(formatProvider, format, args, exception);
        }
        #endregion 
    }
}
