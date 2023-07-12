﻿using CefSharp;
using CefSharp.Wpf;
using System.Globalization;
using System.IO;
using System.Windows;
using Application = System.Windows.Application;
using SDEWT = Sucrose.Dependency.Enum.WallpaperType;
using SECSVV = Sucrose.Engine.CS.View.Video;
using SESHR = Sucrose.Engine.Shared.Helper.Run;
using SEWTT = Skylark.Enum.WindowsThemeType;
using SGMR = Sucrose.Globalization.Manage.Resources;
using SMC = Sucrose.Memory.Constant;
using SMMI = Sucrose.Manager.Manage.Internal;
using SMR = Sucrose.Memory.Readonly;
using STSHI = Sucrose.Theme.Shared.Helper.Info;
using SWDEMB = Sucrose.Watchdog.DarkErrorMessageBox;
using SWHWT = Skylark.Wing.Helper.WindowsTheme;
using SWLEMB = Sucrose.Watchdog.LightErrorMessageBox;
using SWW = Sucrose.Watchdog.Watch;

namespace Sucrose.Live.CS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string Directory => SMMI.EngineSettingManager.GetSetting(SMC.Directory, Path.Combine(SMR.DocumentsPath, SMR.AppName));

        private static string Culture => SMMI.GeneralSettingManager.GetSetting(SMC.CultureName, SGMR.CultureInfo.Name);

        private static SEWTT Theme => SMMI.GeneralSettingManager.GetSetting(SMC.ThemeType, SWHWT.GetTheme());

        private static string Folder => SMMI.EngineSettingManager.GetSetting(SMC.Folder, string.Empty);

        private static Mutex Mutex => new(true, SMR.LiveMutex);

        private static bool HasError { get; set; } = true;

        public App()
        {
            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);

            System.Windows.Forms.Application.ThreadException += (s, e) =>
            {
                Exception Exception = e.Exception;

                SWW.Watch_ThreadException(Exception);

                //Close();
                Message(Exception.Message);
            };

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Exception Exception = e.Exception;

                SWW.Watch_FirstChanceException(Exception);

                //Close();
                //Message(Exception.Message);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception Exception = (Exception)e.ExceptionObject;

                SWW.Watch_GlobalUnhandledExceptionHandler(Exception);

                //Close();
                Message(Exception.Message);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Exception Exception = e.Exception;

                SWW.Watch_UnobservedTaskException(Exception);

                e.SetObserved();

                //Close();
                Message(Exception.Message);
            };

            Current.DispatcherUnhandledException += (s, e) =>
            {
                Exception Exception = e.Exception;

                SWW.Watch_DispatcherUnhandledException(Exception);

                e.Handled = true;

                //Close();
                Message(Exception.Message);
            };

            SGMR.CultureInfo = new CultureInfo(Culture, true);
        }

        protected void Close()
        {
            Environment.Exit(0);
            Current.Shutdown();
            Shutdown();
        }

        protected void Message(string Message)
        {
            if (HasError)
            {
                HasError = false;

                string Path = SMMI.CefSharpLiveLogManager.LogFile();

                switch (Theme)
                {
                    case SEWTT.Dark:
                        SWDEMB DarkMessageBox = new(Message, Path);
                        DarkMessageBox.ShowDialog();
                        break;
                    default:
                        SWLEMB LightMessageBox = new(Message, Path);
                        LightMessageBox.ShowDialog();
                        break;
                }

                Close();
            }
            else
            {
                Close();
            }
        }

        protected void Configure()
        {
            if (SMMI.EngineSettingManager.CheckFile() && !string.IsNullOrEmpty(Folder))
            {
                string InfoPath = Path.Combine(Directory, Folder, SMR.SucroseInfo);

                if (File.Exists(InfoPath))
                {
#if NET48_OR_GREATER && DEBUG
                    CefRuntime.SubscribeAnyCpuAssemblyResolver();
#endif

                    CefSettings Settings = new()
                    {
                        CachePath = Path.Combine(SMR.AppDataPath, SMR.AppName, SMR.CacheFolder, SMR.CefSharp)
                    };

                    Settings.CefCommandLineArgs.Add("enable-gpu", "1"); // GPU kullanımını etkinleştirir
                    Settings.CefCommandLineArgs.Add("enable-gpu-vsync", "1"); // GPU dikey senkronizasyonunu etkinleştirir
                    Settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1"); // GPU bileşimini devre dışı bırakır
                    Settings.CefCommandLineArgs.Add("disable-direct-write", "1"); // Doğrudan yazmayı devre dışı bırakır
                                                                                  //Settings.CefCommandLineArgs.Add("disable-frame-rate-limit", "1"); // Kare hızı limitini devre dışı bırakır
                    Settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1"); // Başlangıç çerçevesi zamanlamasını etkinleştirir
                    Settings.CefCommandLineArgs.Add("disable-breakpad", "1"); // Crash dump raporlamasını devre dışı bırakır
                    Settings.CefCommandLineArgs.Add("disable-extensions", "1"); // Uzantıları devre dışı bırakır

                    Settings.CefCommandLineArgs.Add("multi-threaded-message-loop", "1"); // Çoklu iş parçacıklı mesaj döngüsünü etkinleştirir
                    Settings.CefCommandLineArgs.Add("no-sandbox", "1"); // Sandbox'u devre dışı bırakır
                    Settings.CefCommandLineArgs.Add("off-screen-rendering-enabled", "1"); // Ekran dışı işlemeyi etkinleştirir

                    Settings.CefCommandLineArgs.Add("disable-back-forward-cache", "1"); // Geri önbelleği devre dışı bırakır

                    Settings.CefCommandLineArgs.Add("disable-web-security", "1"); // Web güvenliğini devre dışı bırakır
                    Settings.CefCommandLineArgs.Add("disable-geolocation", "1"); // Konum hizmetlerini devre dışı bırakır

                    Settings.CefCommandLineArgs.Add("disable-surfaces", "1"); // Yüzeyleri devre dışı bırakır

                    Settings.CefCommandLineArgs.Add("autoplay-policy", "no-user-gesture-required"); // Otomatik oynatma politikasını ayarlar

                    Settings.CefCommandLineArgs.Add("enable-media-stream", "1"); // Ortam akışını etkinleştirir
                    Settings.CefCommandLineArgs.Add("enable-accelerated-video-decode", "1"); // Hızlandırılmış video çözümlemeyi etkinleştirir

                    Settings.CefCommandLineArgs.Add("allow-running-insecure-content", "1"); // Güvenli olmayan içeriğin çalışmasına izin verir
                    Settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream", "1"); // Ortam akışı için sahte UI kullanır
                    Settings.CefCommandLineArgs.Add("enable-usermedia-screen-capture", "1"); // Kullanıcı ortam akışı ekran yakalama özelliğini etkinleştirir
                    Settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing", "1"); // Kullanıcı ortam akışı ekran yakalama özelliğini etkinleştirir
                    Settings.CefCommandLineArgs.Add("debug-plugin-loading", "1"); // Eklenti yüklemeyi hata ayıklar
                    Settings.CefCommandLineArgs.Add("allow-outdated-plugins", "1"); // Eski eklentilerin çalışmasına izin verir
                    Settings.CefCommandLineArgs.Add("always-authorize-plugins", "1"); // Her zaman eklentileri yetkilendirir
                    Settings.CefCommandLineArgs.Add("enable-npapi", "1"); // NPAPI eklentilerini etkinleştirir

                    //Example of checking if a call to Cef.Initialize has already been made, we require this for
                    //our .Net 5.0 Single File Publish example, you don't typically need to perform this check
                    //if you call Cef.Initialze within your WPF App constructor.
                    if (!Cef.IsInitialized)
                    {
                        //Perform dependency check to make sure all relevant resources are in our output directory.
                        Cef.Initialize(Settings, performDependencyCheck: true, browserProcessHandler: null);
                    }

                    STSHI Info = STSHI.ReadJson(InfoPath);

                    string FilePath = Path.Combine(Directory, Folder, Info.Source);

                    if (File.Exists(FilePath))
                    {
                        switch (Info.Type)
                        {
                            case SDEWT.Video:

                                SECSVV Engine = new(FilePath);
                                Engine.Show();
                                break;
                            default:
                                Close();
                                break;
                        }
                    }
                    else
                    {
                        Close();
                    }
                }
                else
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Cef.Shutdown();

            Close();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Mutex.WaitOne(TimeSpan.Zero, true) && SESHR.Check())
            {
                Mutex.ReleaseMutex();

                Configure();
            }
            else
            {
                Close();
            }
        }
    }
}