using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.Brushes;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TiledBackground
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
                this.backgroundImage = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Background.jpg"));
                this.backgroundBrush = new CanvasImageBrush(sender, this.backgroundImage);
                this.backgroundBrush.ExtendX = this.backgroundBrush.ExtendY = CanvasEdgeBehavior.Wrap;

                this.resourcesLoaded = true;
            }).AsAsyncAction());

        }

        private void BackgroundCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var session = args.DrawingSession;
            session.FillRectangle(new Rect(new Point(), sender.RenderSize), this.backgroundBrush);
        }

        private void ScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!this.resourcesLoaded)
            {
                return;
            }

            this.backgroundBrush.Transform = System.Numerics.Matrix3x2.CreateScale((float)(e.NewValue / 100.0));
            this.BackgroundCanvas.Invalidate();
        }

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!this.resourcesLoaded)
            {
                return;
            }

            this.backgroundBrush.Opacity = (float)(e.NewValue / 100.0);
            this.BackgroundCanvas.Invalidate();
        }
    }
}
