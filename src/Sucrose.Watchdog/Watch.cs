﻿using SCMI = Sucrose.Common.Manage.Internal;
using SELLT = Skylark.Enum.LevelLogType;

namespace Sucrose.Watchdog
{
    internal static class Watch
    {
        public static void Watch_ThreadException(Exception Exception)
        {
            WatchLog(Exception, "THREAD");
        }

        public static void Watch_FirstChanceException(Exception Exception)
        {
            WatchLog(Exception, "FIRST CHANCE");
        }

        public static void Watch_UnobservedTaskException(Exception Exception)
        {
            WatchLog(Exception, "UNOBSERVED TASK");
        }

        public static void Watch_DispatcherUnhandledException(Exception Exception)
        {
            WatchLog(Exception, "DISPATCHER UNHANDLED");
        }

        public static void Watch_GlobalUnhandledExceptionHandler(Exception Exception)
        {
            WatchLog(Exception, "GLOBAL UNHANDLED");
        }

        private static void WatchLog(Exception Exception, string Type)
        {
            WriteLog($"{Type} EXCEPTION START");
            WriteLog($"Application crashed: {Exception.Message}.");
            WriteLog($"Inner exception: {Exception.InnerException}.");
            WriteLog($"Stack trace: {Exception.StackTrace}.");
            WriteLog($"{Type} EXCEPTION FINISH");
        }

        private static void WriteLog(string Text)
        {
#if CEFSHARP
            SCMI.CefSharpLogManager.Log(SELLT.Error, Text);
#elif TRAY_ICON
            SCMI.TrayIconLogManager.Log(SELLT.Error, Text);
#elif MEDIA_ELEMENT
            SCMI.MediaElementLogManager.Log(SELLT.Error, Text);
#elif USER_INTERFACE
            SCMI.UserInterfaceLogManager.Log(SELLT.Error, Text);
#endif
        }
    }
}