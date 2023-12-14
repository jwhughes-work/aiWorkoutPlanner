namespace aiWorkoutPlanner
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
#if WINDOWS || MACCATALYST || __MACOS__
            window.Width = 900;
#endif
            return window;
        }
    }
}
