﻿using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace tamagotchi_pet
{
    /// <summary>
    /// Interaction logic for TamagotchiWindowControl.
    /// </summary>
    public partial class TamagotchiWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TamagotchiWindowControl"/> class.
        /// </summary>
        public TamagotchiWindowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Button 1 Invoked '{0}'", this.ToString()),
                "TamagotchiWindow");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Button 2 Invoked '{0}'", this.ToString()),
                "TamagotchiWindow");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Button 3 Invoked '{0}'", this.ToString()),
                "TamagotchiWindow");
        }
    }
}