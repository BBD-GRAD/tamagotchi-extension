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
using tamagotchi_pet.Models;
using Serilog;
using Newtonsoft.Json.Linq;
using System.Windows.Media;

namespace tamagotchi_pet
{
    /// <summary>
    /// Interaction logic for TamagotchiWindowControl.
    /// </summary>
    public partial class TamagotchiWindowControl : UserControl
    {
        private DispatcherTimer gameTimer;
        private Pet _pet = null;
        private double targetX, targetY;
        private Dictionary<string, string> _tokens;
        private double refreshRate = 1_000; // ms
        private double simulationSpeed = 60;

        private bool needWater = false;
        private double _waterElapsedTime = 0;
        private double _waterTimeOver = 0;
        private double _gracePeriod = 600_000;
        private double timeToDie = 800_000;

        private double waterRefillTime = 300_000;

        private double _waterNeedTime = 3_600_000;

        private bool IsEating = false;
        private bool dyingFromThirst = false;

        private bool IsDrinking = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TamagotchiWindowControl"/> class.
        /// </summary>
        ///
        static TamagotchiWindowControl()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/tamagotchi.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public TamagotchiWindowControl()
        {
            InitializeComponent();
            petImage.Visibility = Visibility.Hidden;
            waterImage.Visibility = Visibility.Hidden;
            Loaded += OnLoaded; //user settigns TODO
        }

        private void StartGame()
        {
            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(refreshRate)
            };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            targetX = Canvas.GetLeft(petImage);
            targetY = Canvas.GetTop(petImage);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> retrievedTokens = TokenStorage.RetrieveTokens();
            if (retrievedTokens.Count == 0)
            {
                Logging.Logger.Debug("OnLoaded: No token file found");
                MessageBox.Show("No previous session tokens found please login.");
            }
            else
            {
                Logging.Logger.Debug("OnLoaded: Tokens retrieved successfully.");
                _tokens = retrievedTokens;
                _pet = await ApiService.GetPetAsync(_tokens["id_token"]);
                petNameLabel.Text = _pet?.PetName;
                //get settings
            }
            StartGame();
        }

        private async void GameLoop(object sender, EventArgs e)
        {
            double delta = refreshRate * simulationSpeed;
            if (_pet == null)
            {
                petImage.Visibility = Visibility.Hidden;

                Logging.Logger.Debug("GameLoop: No pet found.");
                CreatePetDialog inputDialog = new CreatePetDialog();
                if (inputDialog.ShowDialog() == true)
                {
                    _pet = await ApiService.CreatePetAsync(_tokens["id_token"], inputDialog.ResponseText);
                    petNameLabel.Text = _pet?.PetName;
                    Logging.Logger.Debug("GameLoop: Pet created: " + _pet?.PetName);
                }
            }
            else
            {
                Logging.Logger.Debug($"GameLoop: PET {_pet?.PetName} Water (dyingfrom:{dyingFromThirst}): {_pet.Water:F2} Health: {_pet.Health:F2}");

                petImage.Visibility = Visibility.Visible;
                GenerateNewTargetPosition();
                AnimatePetToPosition(targetX, targetY);

                _pet.XP += (delta) / 1000; //bigint?

                if (!IsDrinking)
                {
                    _pet.Water = Math.Max(0, _pet.Water - 100 / (_waterNeedTime / (delta)));
                    BtnWater.Background = new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    _pet.Water = Math.Min(100, _pet.Water + 100 / (waterRefillTime / (delta)));
                    BtnWater.Background = new SolidColorBrush(Colors.Blue);
                }

                if (_pet.Water == 0)
                {
                    if (!dyingFromThirst)
                    {
                        _waterTimeOver += delta;
                    }
                    if (_waterTimeOver >= _gracePeriod)
                    {
                        dyingFromThirst = true;
                        _waterTimeOver = 0;
                    }
                    waterImage.Visibility = Visibility.Visible;
                }
                else if (_pet.Water == 100)
                {
                    IsDrinking = false;
                }
                else
                {
                    waterImage.Visibility = Visibility.Hidden;
                    dyingFromThirst = false;
                }

                if (dyingFromThirst)
                {
                    _pet.Health = Math.Max(0, _pet.Health - 100 / (timeToDie / (refreshRate * simulationSpeed)));
                }
            }
        }

        //private bool needWater = false;
        //private double _waterElapsedTime = 0;
        //private double _waterTimeOver = 0;
        //private double _gracePeriod = 300_000;
        //private double _waterNeedTime = 3_600_000;

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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Button 1 Invoked '{0}'", this.ToString()),
                "TamagotchiWindow");
        }

        private void BtnWater_Click(object sender, RoutedEventArgs e)
        {
            if (IsDrinking)
            {
                IsDrinking = false;
                BtnWater.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                IsDrinking = true;
                BtnWater.Background = new SolidColorBrush(Colors.Blue);
            }
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

            Dictionary<string, string> retrievedTokens = TokenStorage.RetrieveTokens();
            _tokens = retrievedTokens;
            _pet = await ApiService.GetPetAsync(_tokens["id_token"]);
            petNameLabel.Text = _pet?.PetName;
        }

        private void BtnSettings(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Settings Invoked", this.ToString()),
                "TamagotchiWindow");
        }
    }
}