using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace vkinviter
{
    public static class Logger
    {
        private static StreamWriter _streamWriter;
        private static Object thisLock = new Object();
        private static string logfilePath;


        public static string OpenLogFile(string logFileDir = "")
        {
            logfilePath = Path.Combine(logFileDir, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log");
            lock (thisLock)
            {
                _streamWriter = new StreamWriter(logfilePath, true, Encoding.UTF8);
            }
            return logfilePath;
        }

        public static void CloseLogFile()
        {
            if (_streamWriter == null)
                return;

            lock (thisLock)
            {
                _streamWriter.Close();
            }
        }

        public static void LogMethod(params object[] methodArgs)
        {
            if (_streamWriter == null)
                return;

            StackTrace callStack = new System.Diagnostics.StackTrace();
            string methodName = callStack.GetFrame(1).GetMethod().Name;
            ParameterInfo[] methodParams = callStack.GetFrame(1).GetMethod().GetParameters();
            ;

            if (methodArgs.Length == 0)
                AddText("::{0}", methodName);
            else
            {
                if (methodArgs.Length != methodParams.Length)
                    throw new ArgumentException("Difference between count of logged args and actual arguments of method " + methodName);

                int i = 0;
                string paramInfoStr = string.Empty;
                foreach (ParameterInfo param in methodParams)
                {
                    object value = methodArgs[i];
                    Type valueType = param.ParameterType;
                    MethodInfo toStringMethod = valueType.GetMethod("ToString", Type.EmptyTypes);
                    paramInfoStr += param.Name + " = " + toStringMethod.Invoke(value, null).ToString();

                    paramInfoStr += "; ";
                    i++;
                }
                AddText("::{0}, Args: {1}", methodName, paramInfoStr);
            }
        }

        public static string AddText(string format, params object[] args)
        {
            if (_streamWriter == null)
                return null;

            return AddText(string.Format(format, args));
        }

        public static string AddText(string str)
        {
            if (_streamWriter == null)
                return null;

            string strWithTime = string.Format("{0} {1}",
                DateTime.Now.ToString("[HH:mm:ss]"), str);

            lock (thisLock)
            {
                _streamWriter.WriteLine(strWithTime);
                _streamWriter.Flush();
            }
            return strWithTime;
        }
    }
}
