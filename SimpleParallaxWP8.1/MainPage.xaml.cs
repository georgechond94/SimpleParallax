using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace SimpleParallaxWP8._1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string Value { get; set; } = "value";
        // this is the height of the empty space above the content
        // that we created in the XAML
        private const double EmptySpace = 0;

        // this will represent the vertical position of the image
        // we're storing it in a variable because we'll need it later
        private double _startPosition;
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            StatusBar.GetForCurrentView().HideAsync();

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Here's the magic 
            // To get the image where we want, we will shift it up by its height
            // Then we'll shift it down by the height of the empty space
            // Last, we'll divide that by two so we'll be directly between the top 
            // of the page and the bottom of the empty space
            _startPosition = (-Image.ActualHeight + EmptySpace) / 2;

            // set the TranslateY of the CompositeTransform we created in the XAML
            // to our calculated start position
            ImageTransform.TranslateY = _startPosition;

            // extract the vertical scrollbar from our ScrollViewer named 'Scroller'
            var scrollbar =
              ((FrameworkElement)VisualTreeHelper.GetChild(Scroller, 0)).FindName("VerticalScrollBar") as ScrollBar;

            if (scrollbar != null)
            {
                // listen for scroll events
                scrollbar.ValueChanged += Scrollbar_ValueChanged;
            }

            // when the mouse (the user tapping his screen) moves 
            // within the ScrollViewer, there's a chance that 
            // the ScrollViewer's Content's RenderTransform will be set to 
            // a new CompositeTransform that we need to grab a hold of 
            Scroller.PointerMoved += ScrollerOnMouseMove;

        }

        private void Scrollbar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // if we're in positive scrolling territory
            // where positive means down
            if (e.NewValue > 0)
            {
                // we're going to scroll our Image along with the content
                // but only half as fast
                // try removing the /2.0 and see what happens
                ImageTransform.TranslateY = _startPosition - e.NewValue / 2.0;

                // instead of using the scrollbar's value, you can also use the VerticalOffset
                // of the ScrollViewer itself
                // try it, see which one works better for you performance-wise
                // ImageTransform.TranslateY = _startPosition - Scroller.VerticalOffset/2.0;
            }
        }

        // this will be our backing property for the binding
        private double _verticalOffset;
        public double VerticalOffset
        {
            get { return _verticalOffset; }
            set
            {
                _verticalOffset = value;

                onVerticalOffsetChanged();
            }
        }

        private void onVerticalOffsetChanged()
        {
            // now let's do the same thing we did with the scrollbar

            // we only want to handle the squishes here, as the 
            // scrollbar events handle the normal scrolling
            // so we'll only respond to the squishes, when the content
            // is being moved down
            if (VerticalOffset >= 0)
            {
                ImageTransform.TranslateY = _startPosition + VerticalOffset / 2.0;
            }
        }

        private void ScrollerOnMouseMove(object sender, PointerRoutedEventArgs e)
        {
            // grab the content element
            var uiElement = Scroller.Content as UIElement;
            if (uiElement != null)
            {
                // try to grab its transform as a CompositeTransform
                var transform = uiElement.RenderTransform as CompositeTransform;

                // if it's actually a CompositeTransform
                if (transform != null)
                {
                    // we're good, let's go to town!

                    // let's set up the binding in a standard manner
                    var binding = new Binding();
                    binding.Path = new PropertyPath("VerticalOffset");
                    // in a perfect world, we use a reverse OneWay binding, where
                    // the DP we're binding to can set the backing property
                    // as this doesn't exist in the world of Windows Phone
                    // we're going to cheat and use TwoWay
                    binding.Mode = BindingMode.TwoWay;
                    binding.Source = this;

                    BindingOperations.SetBinding(transform, CompositeTransform.TranslateYProperty, binding);

                    // we're going to release the event handler
                    // since we only need to bind once
                    Scroller.PointerMoved -= ScrollerOnMouseMove;
                }
            }
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

        }
    }
}
