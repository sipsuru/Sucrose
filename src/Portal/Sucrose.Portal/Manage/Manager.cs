﻿using System.Net;
using System.Windows.Media;
using Wpf.Ui.Controls;
using SEST = Skylark.Enum.ScreenType;
using SEWTT = Skylark.Enum.WindowsThemeType;
using SMC = Sucrose.Memory.Constant;
using SMMI = Sucrose.Manager.Manage.Internal;
using SMR = Sucrose.Memory.Readonly;
using SSDEPT = Sucrose.Shared.Dependency.Enum.PerformanceType;
using SSDEST = Sucrose.Shared.Dependency.Enum.StretchType;
using SWHWT = Skylark.Wing.Helper.WindowsTheme;

namespace Sucrose.Portal.Manage
{
    internal static class Manager
    {
        public static Stretch BackgroundStretch => SMMI.PortalSettingManager.GetSetting(SMC.BackgroundStretch, DefaultBackgroundStretch);

        public static WindowBackdropType BackdropType => SMMI.PortalSettingManager.GetSetting(SMC.BackdropType, DefaultBackdropType);

        public static SSDEPT NetworkPerformance => SMMI.BackgroundogSettingManager.GetSetting(SMC.NetworkPerformance, SSDEPT.Resume);

        public static SSDEPT MemoryPerformance => SMMI.BackgroundogSettingManager.GetSetting(SMC.MemoryPerformance, SSDEPT.Pause);

        public static SSDEPT CpuPerformance => SMMI.BackgroundogSettingManager.GetSetting(SMC.CpuPerformance, SSDEPT.Pause);

        public static IPAddress Host => SMMI.LauncherSettingManager.GetSettingAddress(SMC.Host, IPAddress.Loopback);

        public static SEST ScreenType => SMMI.EngineSettingManager.GetSetting(SMC.ScreenType, SEST.DisplayBound);

        public static SSDEST StretchType => SMMI.EngineSettingManager.GetSetting(SMC.StretchType, SSDEST.Fill);

        public static SEWTT Theme => SMMI.GeneralSettingManager.GetSetting(SMC.ThemeType, SWHWT.GetTheme());

        public static int Port => SMMI.LauncherSettingManager.GetSettingStable(SMC.Port, 0);

        public static WindowBackdropType DefaultBackdropType => WindowBackdropType.None;

        public static Stretch DefaultBackgroundStretch => Stretch.UniformToFill;

        public static Mutex Mutex => new(true, SMR.PortalMutex);
    }
}