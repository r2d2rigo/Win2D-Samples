using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace TiledBackground
{
    public sealed partial class MainPage : Page
    {
        private CanvasBitmap backgroundImage;
        private CanvasImageBrush backgroundBrush;
        private bool resourcesLoaded;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BackgroundCanvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(Task.Run(async () =>
            {
                // Load the background image and create an image brush from it
                this.backgroundImage = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Background.jpg"));
                this.backgroundBrush = new CanvasImageBrush(sender, this.backgroundImage);

                // Set the brush's edge behaviour to wrap, so the image repeats if the drawn region is too big
                this.backgroundBrush.ExtendX = this.backgroundBrush.ExtendY = CanvasEdgeBehavior.Wrap;

                this.resourcesLoaded = true;
            }).AsAsyncAction());

        }

        private void BackgroundCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            // Just fill a rectangle with our tiling image brush, covering the entire bounds of the canvas control
            var session = args.DrawingSession;
            session.FillRectangle(new Rect(new Point(), sender.RenderSize), this.backgroundBrush);
        }

        private void ScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // Don't modify the brush properties if it hasn't been initialized yet
            if (!this.resourcesLoaded)
            {
                return;
            }

            // Apply a scale matrix transform to the brush; this way we can control how big the image will be drawn
            this.backgroundBrush.Transform = System.Numerics.Matrix3x2.CreateScale((float)(e.NewValue / 100.0));
            this.BackgroundCanvas.Invalidate();
        }

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // Don't modify the brush properties if it hasn't been initialized yet
            if (!this.resourcesLoaded)
            {
                return;
            }

            // Change the opacity of the brush
            this.backgroundBrush.Opacity = (float)(e.NewValue / 100.0);
            this.BackgroundCanvas.Invalidate();
        }
    }
}
