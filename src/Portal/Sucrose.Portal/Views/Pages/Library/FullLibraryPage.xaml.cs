﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using SMMM = Sucrose.Manager.Manage.Manager;
using SMR = Sucrose.Memory.Readonly;
using SPMI = Sucrose.Portal.Manage.Internal;
using SPVCLC = Sucrose.Portal.Views.Controls.LibraryCard;
using SSTHI = Sucrose.Shared.Theme.Helper.Info;

namespace Sucrose.Portal.Views.Pages.Library
{
    /// <summary>
    /// FullLibraryPage.xaml etkileşim mantığı
    /// </summary>
    public partial class FullLibraryPage : Page, IDisposable
    {
        private readonly List<string> Themes = new();

        public FullLibraryPage(List<string> Themes)
        {
            this.Themes = Themes;
            DataContext = this;

            InitializeComponent();

            Pagination();
        }

        private void Pagination()
        {
            ThemePagination.SelectPageChanged += ThemePagination_SelectPageChanged;
        }

        private async Task AddThemes(string Search, int Page)
        {
            Dispose();

            int Count = 0;

            PageScroll.ScrollToVerticalOffset(0);

            ThemePagination.Visibility = Visibility.Collapsed;

            foreach (string Theme in Themes)
            {
                if (string.IsNullOrEmpty(Search))
                {
                    if (SMMM.LibraryPagination * Page > Count && SMMM.LibraryPagination * Page <= Count + SMMM.LibraryPagination)
                    {
                        SPVCLC LibraryCard = new(Path.Combine(SMMM.LibraryLocation, Theme), SSTHI.ReadJson(Path.Combine(SMMM.LibraryLocation, Theme, SMR.SucroseInfo)));

                        LibraryCard.IsVisibleChanged += ThemeCard_IsVisibleChanged;

                        ThemeLibrary.Children.Add(LibraryCard);

                        Empty.Visibility = Visibility.Collapsed;

                        await Task.Delay(50);
                    }

                    Count++;
                }
                else
                {
                    SSTHI Info = SSTHI.ReadJson(Path.Combine(SMMM.LibraryLocation, Theme, SMR.SucroseInfo));
                    string Description = Info.Description.ToLowerInvariant();
                    string Title = Info.Title.ToLowerInvariant();

                    if (Title.Contains(Search) || Description.Contains(Search))
                    {
                        if (SMMM.LibraryPagination * Page > Count && SMMM.LibraryPagination * Page <= Count + SMMM.LibraryPagination)
                        {
                            SPVCLC LibraryCard = new(Path.Combine(SMMM.LibraryLocation, Theme), Info);

                            LibraryCard.IsVisibleChanged += ThemeCard_IsVisibleChanged;

                            ThemeLibrary.Children.Add(LibraryCard);

                            Empty.Visibility = Visibility.Collapsed;

                            await Task.Delay(50);
                        }

                        Count++;
                    }
                }
            }

            if (ThemeLibrary.Children.Count <= 0)
            {
                Empty.Visibility = Visibility.Visible;
            }

            ThemePagination.MaxPage = (int)Math.Ceiling((double)Count / SMMM.LibraryPagination);
        }

        private async void FullLibraryPage_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeLibrary.ItemMargin = new Thickness(SMMM.AdaptiveMargin);
            ThemeLibrary.MaxItemsPerRow = SMMM.AdaptiveLayout;

            await AddThemes(SPMI.SearchService.SearchText, ThemePagination.SelectPage);
        }

        private async void ThemePagination_SelectPageChanged(object sender, EventArgs e)
        {
            Dispose();

            await AddThemes(SPMI.SearchService.SearchText, ThemePagination.SelectPage);
        }

        private async void ThemeCard_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                await Task.Delay(250);

                if (ThemeLibrary.Children.Count <= 0)
                {
                    if (ThemePagination.MaxPage > ThemePagination.SelectPage)
                    {
                        await AddThemes(SPMI.SearchService.SearchText, ThemePagination.SelectPage);
                    }
                    else if (ThemePagination.SelectPage > 0)
                    {
                        ThemePagination.SelectPage--;
                    }
                    else
                    {
                        Empty.Visibility = Visibility.Visible;
                        ThemePagination.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public void Dispose()
        {
            ThemeLibrary.Children.Clear();

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}