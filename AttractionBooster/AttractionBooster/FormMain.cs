using System.Threading;
using System.Windows.Forms;

namespace AttractionBooster
{
    public partial class FormMain : Form
    {
        private Thread _renderThread;

        private CancellationTokenSource _renderCancelTokenSource;

        public FormMain()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint 
                | ControlStyles.AllPaintingInWmPaint 
                | ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void FormMain_Load(object sender, System.EventArgs e)
        {
            _renderCancelTokenSource = new CancellationTokenSource();

            _renderThread = new Thread(new Renderer(PictureBoxMain.CreateGraphics(), ClientSize.Width, ClientSize.Height).Run);
            _renderThread.Start(_renderCancelTokenSource.Token);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _renderCancelTokenSource.Cancel();
        }
    }
}
