using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace tamagotchi_pet.Services
{
    internal class GameService
    {
        private readonly Image petImage;

        private double minX = 45;
        private double maxX = 165;
        private bool movingToMax = true;

        public GameService(ref Image petImage)
        {
            this.petImage = petImage;
        }

        public void AnimatePet()
        {
            double currentX = Canvas.GetLeft(petImage);
            double targetX = movingToMax ? maxX : minX;

            double timeInSeconds = 10;
            Duration animationDuration = new Duration(TimeSpan.FromSeconds(timeInSeconds));

            var xAnimation = new DoubleAnimation()
            {
                From = currentX,
                To = targetX,
                Duration = animationDuration,
                EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut }
            };

            xAnimation.Completed += (sender, e) =>
            {
                movingToMax = !movingToMax;
                AnimatePet();
            };

            petImage.BeginAnimation(Canvas.LeftProperty, xAnimation);
        }

        public double UpdateResource(double resource, double depletionTime, double refillTime, bool isActive, Button resourceButton, Color activeColor, Color inactiveColor, double delta, double resourceMax = 100)
        {
            if (!isActive)
            {
                resourceButton.Background = new SolidColorBrush(inactiveColor);
                return Math.Max(0, resource - resourceMax / (depletionTime / delta));
            }
            else
            {
                resourceButton.Background = new SolidColorBrush(activeColor);
                return Math.Min(resourceMax, resource + resourceMax / (refillTime / delta));
            }
        }

        public void UpdatePetState(double resource, ref double timeWithoutResource, double gracePeriod, ref bool isDyingFromResourceDepletion, ref bool isActive, double delta, double resourceMax = 100)
        {
            if (resource == 0)
            {
                if (!isDyingFromResourceDepletion)
                {
                    timeWithoutResource += delta;
                }
                if (timeWithoutResource >= gracePeriod)
                {
                    isDyingFromResourceDepletion = true;
                    timeWithoutResource = 0;
                }
            }
            else if (resource == resourceMax)
            {
                isActive = false;
            }
            else
            {
                isDyingFromResourceDepletion = false;
                timeWithoutResource = 0;
            }
        }
    }
}