/*************************************************************************************
 *
 * 文 件 名:   MLog.cs
 * 描    述:  日志工具
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2021-05-30 16:23:25
*************************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

public static class ExtensionMethods
{
    public static void Log(this object self, string msg, params object[] args)
    {
        MUtils.MLog.Log(msg, args);
    }


    public static void Log(this object self, object obj)
    {
        MUtils.MLog.Log(obj);
    }

    public static void Trace(this object self, object obj)
    {
        MUtils.MLog.Trace(obj);
    }


    public static void Trace(this object self, string msg, params object[] args)
    {
        MUtils.MLog.Trace(msg, args);
    }

    public static void Warn(this object self, string msg, params object[] args)
    {
        MUtils.MLog.Warn(msg, args);
    }


    public static void Warn(this object self, object obj)
    {
        MUtils.MLog.Warn(obj);
    }

    public static void Error(this object self, string msg, params object[] args)
    {
        MUtils.MLog.Error(msg, args);
    }

    public static void Error(this object self, object obj)
    {
        MUtils.MLog.Error(obj);
    }
}

namespace MUtils
{
    public static class MLog
    {
        class UnityLogger : ILogger
        {
            private Type _type = Type.GetType("UnityEngine.Debug,UnityEngine");

            public void Log(string msg, LogColorEnum logColor = LogColorEnum.NONE)
            {
                if (logColor != LogColorEnum.NONE)
                {
                    msg = ColorUnityLog(msg, logColor);
                }

                _type.GetMethod("Log", new Type[] {typeof(object)}).Invoke(null, new[] {msg});
            }

            public void Warn(string msg)
            {
                _type.GetMethod("LogWarning", new Type[] {typeof(object)}).Invoke(null, new[] {msg});
            }

            public void Error(string msg)
            {
                _type.GetMethod("LogError", new Type[] {typeof(object)}).Invoke(null, new[] {msg});
            }

            private string ColorUnityLog(string msg, LogColorEnum color)
            {
                switch (color)
                {
                    case LogColorEnum.RED:
                        msg = $"<color=#ff0000>{msg}</color>";
                        break;
                    case LogColorEnum.GREEN:
                        msg = $"<color=#00ff00>{msg}</color>";
                        break;
                    case LogColorEnum.BLUE:
                        msg = $"<color=#0000ff>{msg}</color>";
                        break;
                    case LogColorEnum.CYAN:
                        msg = $"<color=#00ffff>{msg}</color>";
                        break;
                    case LogColorEnum.MAGENTA:
                        msg = $"<color=#ff00ff>{msg}</color>";
                        break;
                    case LogColorEnum.YELLOW:
                        msg = $"<color=#ffff00>{msg}</color>";
                        break;
                    case LogColorEnum.NONE:
                    default:
                        break;
                }

                return msg;
            }
        }

        class ConsoleLogger : ILogger
        {
            public void Log(string msg, LogColorEnum logColor = LogColorEnum.NONE)
            {
                WriteConsoleLog(msg, logColor);
            }

            public void Warn(string msg)
            {
                WriteConsoleLog(msg, LogColorEnum.YELLOW);
            }

            public void Error(string msg)
            {
                WriteConsoleLog(msg, LogColorEnum.RED);
            }

            private void WriteConsoleLog(string msg, LogColorEnum color)
            {
                switch (color)
                {
                    case LogColorEnum.RED:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogColorEnum.GREEN:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogColorEnum.BLUE:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogColorEnum.CYAN:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogColorEnum.MAGENTA:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogColorEnum.YELLOW:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogColorEnum.NONE:
                    default:
                        Console.WriteLine(msg);
                        break;
                }
            }
        }

        public static LogConfig Cfg;
        private static ILogger _logger;
        private static StreamWriter _logFileWriter = null;

        public static void InitSettings(LogConfig cfg = null)
        {
            if (cfg == null)
            {
                Cfg = new LogConfig();
            }
            else
            {
                Cfg = cfg;
            }

            if (Cfg.LoggerType == LoggerType.CONSOLE)
            {
                _logger = new ConsoleLogger();
            }
            else
            {
                _logger = new UnityLogger();
            }

            if (!Cfg.EnableSave)
            {
                return;
            }

            if (Cfg.EnableCover)
            {
                var path = Cfg.SavePath + Cfg.SaveName;
                try
                {
                    if (Directory.Exists(Cfg.SavePath))
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(Cfg.SavePath);
                    }

                    _logFileWriter = File.AppendText(path);
                    _logFileWriter.AutoFlush = true;
                }
                catch
                {
                    _logFileWriter = null;
                }
            }
            else
            {
                var prefix = DateTime.Now.ToString("yyyyMMdd@HH-mm-ss");
                var path = Cfg.SavePath + prefix + Cfg.SaveName;
                try
                {
                    if (!Directory.Exists(Cfg.SavePath))
                    {
                        Directory.CreateDirectory(Cfg.SavePath);
                    }

                    _logFileWriter = File.AppendText(path);
                    _logFileWriter.AutoFlush = true;
                }
                catch
                {
                    _logFileWriter = null;
                }
            }
        }

        public static void Log(string msg, params object[] args)
        {
            if (Cfg.EnableLog == false)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg, args));
            _logger.Log(msg);
            WriteToFile($"[L]{msg}");
        }


        public static void Log(object obj)
        {
            if (Cfg.EnableLog == false)
            {
                return;
            }

            var msg = DecorateLog(obj.ToString());
            _logger.Log(msg);
            WriteToFile($"[L]{msg}");
        }

        public static void Trace(object obj)
        {
            if (Cfg.EnableLog == false)
            {
                return;
            }

            var msg = DecorateLog(obj.ToString(), true);
            _logger.Log(msg, LogColorEnum.MAGENTA);
            WriteToFile($"[T]{msg}");
        }


        public static void Trace(string msg, params object[] args)
        {
            if (Cfg.EnableLog == false)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg, args));
            _logger.Log(msg, LogColorEnum.MAGENTA);
            WriteToFile($"[T]{msg}");
        }

        public static void Warn(string msg, params object[] args)
        {
            if (Cfg.EnableLog == false)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg, args));
            _logger.Warn(msg);
            WriteToFile($"[W]{msg}");
        }


        public static void Warn(object obj)
        {
            if (Cfg.EnableLog == false)
            {
                return;
            }

            var msg = DecorateLog(obj.ToString());
            _logger.Warn(msg);
            WriteToFile($"[W]{msg}");
        }

        public static void Error(string msg, params object[] args)
        {
            if (Cfg.EnableLog == false)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg, args));
            _logger.Error(msg);
            WriteToFile($"[E]{msg}");
        }

        public static void Error(object obj)
        {
            if (Cfg.EnableLog == false)
            {
                return;
            }

            var msg = DecorateLog(obj.ToString());
            _logger.Error(msg);
            WriteToFile($"[E]{msg}");
        }


        /// <summary>
        /// 修饰输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isTrace">打印堆栈信息</param>
        private static string DecorateLog(string msg, bool isTrace = false)
        {
            var stringBuilder = new StringBuilder(Cfg.LogPrefix, 100);
            if (Cfg.EnableTime)
            {
                stringBuilder.AppendFormat(" {0:hh:mm:ss--fff}", DateTime.Now);
            }

            if (Cfg.EnableThreadID)
            {
                stringBuilder.AppendFormat(" {0}", GetThreadID());
            }

            stringBuilder.AppendFormat(" {0} {1}", Cfg.LogSeparate, msg);
            if (isTrace)
            {
                stringBuilder.AppendFormat(" \nStackTrace:{0}", GetLogTrace());
            }

            return stringBuilder.ToString();
        }

        private static string GetThreadID()
        {
            return $" ThreadID:{Thread.CurrentThread.ManagedThreadId}";
        }

        private static string GetLogTrace()
        {
            StackTrace st = new StackTrace(3, true);
            string traceInfo = "";
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                traceInfo += $"\n  {sf.GetFileName()}::{sf.GetMethod()} line:{sf.GetFileLineNumber()}";
            }

            return traceInfo;
        }

        private static void WriteToFile(string msg)
        {
            if (Cfg.EnableSave && _logFileWriter != null)
            {
                try
                {
                    _logFileWriter.WriteLine(msg);
                }
                catch
                {
                    _logFileWriter = null;
                }
            }
        }
    }
}