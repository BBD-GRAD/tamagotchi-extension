using System;
using System.Windows;
using System.Collections.Generic;
using tamagotchi_pet.Models;
using System.Windows.Controls;

namespace tamagotchi_pet.Dialogs
{
    public partial class SettingsDialog : Window
    {
        public Themes SelectedTheme { get; private set; }
        public double SimulationSpeed { get; private set; }

        public static double LastSliderValue { get; set; } = 1;
        public static Themes LastSelectedTheme { get; set; } = Themes.Red;

        public SettingsDialog()
        {
            InitializeComponent();

            SpeedSlider.Value = LastSliderValue;

            foreach (ComboBoxItem item in ColorPicker.Items)
            {
                if (item.Tag.ToString() == LastSelectedTheme.ToString())
                {
                    ColorPicker.SelectedItem = item;
                    break;
                }
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPicker.SelectedItem is ComboBoxItem selectedColorItem)
            {
                SelectedTheme = MapColorToEnum(selectedColorItem.Tag.ToString());
                LastSelectedTheme = SelectedTheme;
            }
            SimulationSpeed = SpeedSlider.Value;
            LastSliderValue = SpeedSlider.Value;
            this.DialogResult = true;
            this.Close();
        }

        private Themes MapColorToEnum(string color)
        {
            var colorMapping = new Dictionary<string, Themes>
            {
                { "Red", Themes.Red },
                { "Green", Themes.Green },
                { "Blue", Themes.Blue },
                { "Black", Themes.Black }
            };

            return colorMapping[color];
        }
    }
}