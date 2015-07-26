using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Media3D;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleParallax
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Test> Tests = new ObservableCollection<Test>();
        public MainPage()
        {
            Tests.Add(new Test { Foo = "Item1" });
            Tests.Add(new Test { Foo = "Item2" });
            Tests.Add(new Test { Foo = "Item3" });
            Tests.Add(new Test { Foo = "Item4" });
            Tests.Add(new Test { Foo = "Item5" });
            Tests.Add(new Test { Foo = "Item6" });
            Tests.Add(new Test { Foo = "Item7" });
            Tests.Add(new Test { Foo = "Item8" });
            Tests.Add(new Test { Foo = "Item9" });
            Tests.Add(new Test { Foo = "Item10" });

            this.InitializeComponent();
            gview.ItemsSource = Tests;

        }
        private void NormalizeParallax(UIElement target)
        {
            var transform = target.Transform3D as CompositeTransform3D;

            if (transform != null)
            {
                var transformToVisual = ParallaxRoot.TransformToVisual(target);
                var center = new Point(ParallaxRoot.ActualWidth / 2.0, RootGrid.ActualHeight / 2.0);

                var transformedCenter = transformToVisual.TransformPoint(center);

                transform.CenterX = transformedCenter.X;
                transform.CenterY = transformedCenter.Y - ParallaxRoot.VerticalOffset;

                transform.ScaleX = transform.ScaleY =
                    -transform.TranslateZ / PerspectiveTransform.Depth + 1.0;
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NormalizeParallax(RainierBackgroundImage);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await Task.Delay(120);
            NormalizeParallax(RainierBackgroundImage);
        }
    }
}
