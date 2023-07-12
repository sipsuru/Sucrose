﻿using SMLM = Sucrose.Manager.LogManager;
using SMSM = Sucrose.Manager.SettingManager;

namespace Sucrose.Manager.Manage
{
    public static class Internal
    {
        public static SMSM ServerManager = new("Server.json");

        public static SMSM WebsiteManager = new("Website.json");

        public static SMSM EngineSettingManager = new("Engine.json");

        public static SMSM DiscordSettingManager = new("Discord.json");

        public static SMSM GeneralSettingManager = new("General.json");

        public static SMLM TrayIconLogManager = new("TrayIcon-{0}.log");

        public static SMLM CefSharpLogManager = new("CefSharp-{0}.log");

        public static SMSM TrayIconSettingManager = new("TrayIcon.json");

        public static SMLM CommandogLogManager = new("Commandog-{0}.log");

        public static SMLM AuroraLiveLogManager = new("AuroraLive-{0}.log");

        public static SMLM NebulaLiveLogManager = new("NebulaLive-{0}.log");

        public static SMLM VexanaLiveLogManager = new("VexanaLive-{0}.log");

        public static SMLM WebViewLiveLogManager = new("WebViewLive-{0}.log");

        public static SMLM CefSharpLiveLogManager = new("CefSharpLive-{0}.log");

        public static SMLM UserInterfaceLogManager = new("UserInterface-{0}.log");
    }
}