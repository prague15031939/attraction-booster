using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAttractionBooster.Core;

namespace WpfAttractionBooster.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Thread _renderThread;

        public MainViewModel(MainWindow window, WriteableBitmap bitmap, CancellationToken cancellationToken)
        {
            _renderThread = new Thread(new Renderer(bitmap, window.ImageMain, (int)window.ActualWidth, (int)window.ActualHeight).Run);
            _renderThread.Start(cancellationToken);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}