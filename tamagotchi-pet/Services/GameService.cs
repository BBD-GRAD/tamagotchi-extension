using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace tamagotchi_pet.Services
{
    internal class GameService
    {
        private readonly Image petImage;
        private readonly Canvas gameCanvas;
        private readonly Rectangle movementArea;

        public GameService(ref Image petImage, ref Canvas gameCanvas, ref Rectangle movementArea)
        {
            this.petImage = petImage;
            this.gameCanvas = gameCanvas;
            this.movementArea = movementArea;
        }

        private (double x, double y) GenerateNewTargetPosition()
        {
            double minX = movementArea.Margin.Left;
            double minY = movementArea.Margin.Top;
            double maxX = gameCanvas.ActualWidth - movementArea.Margin.Right - petImage.Width;
            double maxY = gameCanvas.ActualHeight - movementArea.Margin.Bottom - petImage.Height;

            Random rand = new Random();
            var targetX = rand.Next((int)minX, (int)maxX);
            var targetY = rand.Next((int)minY, (int)maxY);
            return (targetX, targetY);
        }

        public void AnimatePetToPosition()
        {
            (double newX, double newY) = GenerateNewTargetPosition();

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