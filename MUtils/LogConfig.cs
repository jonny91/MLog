/*************************************************************************************
 *
 * 文 件 名:   LogConfig.cs
 * 描    述: 日志配置
 * 
 * 创 建 者：  洪金敏 
 * 创建时间：  2021-05-30 16:24:24
*************************************************************************************/

using System;

namespace MUtils
{
    public enum LogColorEnum
    {
        NONE,
        RED,
        GREEN,
        BLUE,
        CYAN,
        MAGENTA,
        YELLOW,
    }

    public enum LoggerType
    {
        UNITY,
        CONSOLE,
    }

    public class LogConfig
    {
        public bool EnableLog = true;
        public string LogPrefix = "#";
        public bool EnableTime = true;
        public string LogSeparate = ">>";
        public bool EnableThreadID = true;
        public bool EnableTrace = true;
        public bool EnableSave = true;

        public bool EnableCover = true;
        public string SavePath = $"{AppDomain.CurrentDomain.BaseDirectory}Logs\\";
        public string SaveName = "ConsoleLog.txt";
        public LoggerType LoggerType = LoggerType.CONSOLE;
    }

    interface ILogger
    {
        void Log(string msg, LogColorEnum logColor = LogColorEnum.NONE);
        void Warn(string msg);
        void Error(string msg);
    }
}