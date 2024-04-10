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

        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPicker.SelectedItem is ComboBoxItem selectedColorItem)
            {
                SelectedTheme = MapColorToEnum(selectedColorItem.Tag.ToString());
            }
            SimulationSpeed = SpeedSlider.Value;
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