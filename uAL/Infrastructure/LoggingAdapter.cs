using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace uAL.Infrastructure
{
    public class LoggingAdapter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static LoggingAdapter()
        {

        }

        public static void Debug(string message)
        {
            logger.Debug(message);
        }

        public static void Info(string message)
        {
            logger.Info(message);
        }

        public static void Error(string message,[Optional] Exception ex)
        {
            logger.Error(message,ex);
        }
    }
}
