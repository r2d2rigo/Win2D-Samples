using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace IrisBlurWin2D
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Radial gradient brush that will be used to create the transparency mask.
        /// </summary>
        private CanvasRadialGradientBrush radialBrush;

        /// <summary>
        /// Image to be drawn.
        /// </summary>
        private CanvasBitmap image;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BlurAmount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Canvas.Invalidate();
        }

        private void BlurRadius_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Canvas.Invalidate();
        }

        private void Canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            // Create instance of radial gradient brush; the center will be transparent and the extremes opaque black.
            radialBrush = new CanvasRadialGradientBrush(sender, Colors.Transparent, Colors.Black);

            // Load image to draw.
            args.TrackAsyncAction(Task.Run(async () =>
            {
                image = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///SpotlightImage.png"));
            }).AsAsyncAction());
        }

        private void Canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            // Start drawing session and clear background to white.
            var session = args.DrawingSession;
            args.DrawingSession.Clear(Colors.White);

            // Set the center of the radial gradient as the center of the image.
            radialBrush.Center = new System.Numerics.Vector2((float)(image.Size.Width / 2.0f), (float)(image.Size.Height / 2.0f));
            // Assing gradient radius from slider control.
            radialBrush.RadiusX = radialBrush.RadiusY = (float)BlurRadius.Value;

            // Draw unaltered image first.
            session.DrawImage(image, image.Bounds);

            // Create a layer, this way all elements drawn will be affected by a transparent mask
            // which in our case is the radial gradient.
            using (session.CreateLayer(radialBrush))
            {
                // Create gaussian blur effect.
                using (var blurEffect = new GaussianBlurEffect())
                {
                    // Set image to blur.
                    blurEffect.Source = image;
                    // Set blur amount from slider control.
                    blurEffect.BlurAmount = (float)BlurAmount.Value;
                    // Explicitly set optimization mode to highest quality, since we are using big blur amount values.
                    blurEffect.Optimization = EffectOptimization.Quality;
                    // This prevents the blur effect from wrapping around.
                    blurEffect.BorderMode = EffectBorderMode.Hard;
                    // Draw blurred image on top of the unaltered one. It will be masked by the radial gradient
                    // thus showing a transparent hole in the middle, and properly overlaying the alpha values.
                    session.DrawImage(blurEffect, 0, 0);
                }
            }
        }
    }
}
