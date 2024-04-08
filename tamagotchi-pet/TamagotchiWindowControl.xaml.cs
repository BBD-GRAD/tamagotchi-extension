using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using tamagotchi_pet.Dialogs;
using tamagotchi_pet.Services;
using tamagotchi_pet.Utils;
using TamagotchiAPI.Models;

namespace tamagotchi_pet
{
    /// <summary>
    /// Interaction logic for TamagotchiWindowControl.
    /// </summary>
    public partial class TamagotchiWindowControl : UserControl
    {
        // client configuration

        /// <summary>
        /// Initializes a new instance of the <see cref="TamagotchiWindowControl"/> class.
        /// </summary>
        private DispatcherTimer gameTimer;

        private double targetX, targetY;
        private Pet _pet;

        public TamagotchiWindowControl()
        {
            InitializeComponent();

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(2000);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            targetX = Canvas.GetLeft(petImage);
            targetY = Canvas.GetTop(petImage);

            Loaded += OnLoaded; // Handle loaded event to start async operations
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await InitializePetAsync();
        }

        //TODO rename and init user settings also
        private async Task InitializePetAsync()
        {
            Dictionary<string, string> retrievedTokens = SecureTokenStorage.RetrieveTokens();
            string idToken = retrievedTokens["id_token"];

            if (!string.IsNullOrEmpty(idToken))
            {
                var (hasPet, petMessage, pet) = await ApiService.GetPetAsync(idToken);
                if (hasPet)
                {
                    MessageBox.Show("Pet already exists: " + pet.PetName); // debug
                    _pet = pet;
                    petNameLabel.Text = _pet.PetName;
                }
                else
                {
                    MessageBox.Show(petMessage); // Show why the pet was not retrieved or created
                }
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (_pet == null)
            {
                petImage.Visibility = Visibility.Hidden;
                return;
            }
            else
            {
                petImage.Visibility = Visibility.Visible;
            }

            GenerateNewTargetPosition();
            AnimatePetToPosition(targetX, targetY);
        }

        private void GenerateNewTargetPosition() //TODO move all this to own class
        {
            double minX = movementArea.Margin.Left;
            double minY = movementArea.Margin.Top;
            double maxX = gameCanvas.ActualWidth - movementArea.Margin.Right - petImage.Width;
            double maxY = gameCanvas.ActualHeight - movementArea.Margin.Bottom - petImage.Height;

            Random rand = new Random();
            targetX = rand.Next((int)minX, (int)maxX);
            targetY = rand.Next((int)minY, (int)maxY);
        }

        private void AnimatePetToPosition(double newX, double newY)
        {
            double currentX = Canvas.GetLeft(petImage);
            double currentY = Canvas.GetTop(petImage);

            double distance = Math.Sqrt(Math.Pow(newX - currentX, 2) + Math.Pow(newY - currentY, 2));

            double speed = 10;

            double timeInSeconds = distance / speed;
            Duration animationDuration = new Duration(TimeSpan.FromSeconds(timeInSeconds));

            var xAnimation = new DoubleAnimation()
            {
                From = currentX,
                To = newX,
                Duration = animationDuration,
                EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut }
            };
            var yAnimation = new DoubleAnimation()
            {
                From = currentY,
                To = newY,
                Duration = animationDuration,
                EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut }
            };

            petImage.BeginAnimation(Canvas.LeftProperty, xAnimation);
            petImage.BeginAnimation(Canvas.TopProperty, yAnimation);
        }

        private void UpdateTamagotchiState()
        {
            // Update game states like hunger, happiness etc.
        }

        private void CheckGameConditions()
        {
            // Check conditions and update game state accordingly
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

        private async void BtnAccount_Click(object sender, RoutedEventArgs e)
        {
            await AuthFlow.StartAuth();

            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Activate();

            Dictionary<string, string> retrievedTokens = SecureTokenStorage.RetrieveTokens();
            string idToken = retrievedTokens["id_token"];

            var (hasPet, petMessage, pet) = await ApiService.GetPetAsync(idToken);
            if (!hasPet)
            {
                MessageBox.Show(petMessage);
                InputDialog inputDialog = new InputDialog();
                if (inputDialog.ShowDialog() == true)
                {
                    string petName = inputDialog.ResponseText;
                    var (createSuccess, createMessage, createdPet) = await ApiService.CreatePetAsync(idToken, petName);
                    MessageBox.Show(createMessage); //check for empty text TODO
                    _pet = createdPet;
                }
            }
            else
            {
                MessageBox.Show("Pet already exists: " + pet.PetName); // debug
                _pet = pet;
            }
            petNameLabel.Text = _pet.PetName;
        }

        private void BtnSettings(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Settings Invoked", this.ToString()),
                "TamagotchiWindow");
        }
    }
}