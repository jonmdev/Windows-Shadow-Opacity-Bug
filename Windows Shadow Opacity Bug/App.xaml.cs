using System.Diagnostics;

namespace Windows_Shadow_Opacity_Bug {
    public partial class App : Application {
        private double screenWidth;
        private double screenHeight;
        public event Action screenSizeChanged = null;
        double deltaTime = 0;
        int showHide = 1;

        public App() {
            InitializeComponent();

            ContentPage mainPage = new();
            MainPage = mainPage;

            AbsoluteLayout abs = new();
            mainPage.Content = abs;

            VerticalStackLayout vert = new();
            abs.Children.Add(vert);

            //create a group of labels on screen
            List<View> labelList = new();
            for (int i=0; i < 5; i++) {
                Label label = new();
                label.Text = "HELLO MAUI";
                label.FontAttributes = FontAttributes.Bold;
                label.FontSize = 60;
                label.Shadow = new() { Offset =  new Point(0, 5), Radius = 7 };
                vert.Children.Add(label);
                labelList.Add(label);
            }

            
            //animation timer and function
            var timer = Application.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds( 1 / 180);
            timer.Start();
            DateTime dateTime = DateTime.Now;
            timer.Tick += delegate {
                deltaTime = (DateTime.Now - dateTime).TotalSeconds;
                dateTime = DateTime.Now;

                double animationTime = 1;
                double targetOpacity = (showHide + 1) / 2;

                for (int i = 0; i < labelList.Count; i++) {
                    if (targetOpacity != labelList[i].Opacity) {

                        //tweening the label opacity between 1 and 0 will cause the shadow to be lost:
                        labelList[i].Opacity = Math.Clamp(labelList[i].Opacity + showHide * deltaTime/ animationTime, 0, 1);
                        
                        //alternatively if you immediately change the opacity between zero and one in 1 step back and forth each time click rather than tween, shadow is NOT lost:
                        //labelList[i].Opacity = Math.Clamp(showHide, 0, 1); 
                    }
                }

            };

            //tap function
            TapGestureRecognizer tap = new();
            tap.Tapped += delegate {
                showHide *= -1;
            };

            //border to take clicks
            Border clickBorder = new();
            abs.Children.Add(clickBorder);
            clickBorder.BackgroundColor = Colors.Blue;
            clickBorder.Opacity = 0;
            clickBorder.GestureRecognizers.Add(tap);

            //screen resize function
            mainPage.SizeChanged += delegate {
                if (mainPage.Width > 0 && mainPage.Height > 0) {
                    screenWidth = mainPage.Width;
                    screenHeight = mainPage.Height;
                    vert.WidthRequest = screenWidth;
                    vert.HeightRequest = screenHeight;
                    clickBorder.WidthRequest = screenWidth;
                    clickBorder.HeightRequest = screenHeight;
                    abs.HeightRequest = screenHeight;
                    abs.WidthRequest = screenWidth;

                }

            };


        }
    }
}