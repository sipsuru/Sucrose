﻿using System.Windows;
using SGHMBL = Sucrose.Globalization.Helper.MessageBoxLocalization;

namespace Sucrose.MessageBox
{
    /// <summary>
    /// Interaction logic for LightErrorMessageBox.xaml
    /// </summary>
    public partial class LightErrorMessageBox : Window
    {
        public LightErrorMessageBox(string Culture, string ErrorMessage)
        {
            InitializeComponent();
            Title = SGHMBL.GetValue("WindowTitle", Culture);
            Error_Title.Text = SGHMBL.GetValue("ErrorTitle", Culture);
            Close_Button.Content = SGHMBL.GetValue("CloseButton", Culture);
            Error_Message.Text = SGHMBL.GetValue("ErrorMessage", Culture) + Environment.NewLine + ErrorMessage;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}