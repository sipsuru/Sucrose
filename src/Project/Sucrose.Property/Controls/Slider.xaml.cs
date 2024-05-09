﻿using SPHP = Sucrose.Property.Helper.Properties;
using SSTMSM = Sucrose.Shared.Theme.Model.SliderModel;
using ToolTip = System.Windows.Controls.ToolTip;
using UserControl = System.Windows.Controls.UserControl;

namespace Sucrose.Property.Controls
{
    /// <summary>
    /// Slider.xaml etkileşim mantığı
    /// </summary>
    public partial class Slider : UserControl
    {
        public Slider(string Key, SSTMSM Data)
        {
            InitializeComponent();

            InitializeData(Key, Data);
        }

        private void InitializeData(string Key, SSTMSM Data)
        {
            Label.Text = Data.Text;
            Component.Maximum = Data.Max;
            Component.Minimum = Data.Min;
            Component.Value = Data.Value;
            Component.TickFrequency = Data.Step;

            Component.ValueChanged += (s, e) => Component_Changed(Key, Data, e.NewValue);

            if (!string.IsNullOrEmpty(Data.Help))
            {
                ToolTip HelpTip = new()
                {
                    Content = Data.Help
                };

                Component.ToolTip = HelpTip;
            }
        }

        private void Component_Changed(string Key, SSTMSM Data, double Value)
        {
            Data.Value = Value;

            SPHP.Change(Key, Data);
        }
    }
}