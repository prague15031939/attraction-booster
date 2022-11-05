using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAttractionBooster.ViewModel;

namespace WpfAttractionBooster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static WriteableBitmap BitmapMain { get; set; }

        private static CancellationTokenSource _cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapMain = new WriteableBitmap((int)BorderImage.ActualWidth, (int)BorderImage.ActualHeight, 96, 96, PixelFormats.Bgra32, null);
            ImageMain.Source = BitmapMain;

            _cancellationTokenSource = new CancellationTokenSource();

            DataContext = new MainViewModel(this, BitmapMain, _cancellationTokenSource.Token);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
