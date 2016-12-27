using System;
using System.Windows;
using System.Windows.Controls;
using Ozeki.Camera;
using Ozeki.Media;
using System.Windows.Media;
using System.Windows.Threading;


namespace Basic_CameraViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        private DrawingImageProvider _provider;

        private MediaConnector _connector;

        private MotionDetector _detector;

        private CameraURLBuilderWPF _myCameraURLBuilder;

        private OzekiCamera _camera;

        public MainWindow()
        {
            InitializeComponent();

            _connector = new MediaConnector();

            _provider = new DrawingImageProvider();

            _detector = new MotionDetector();

            videoViewer.SetImageProvider(_provider);


          

           // MyWin.Show();


        }

        void GuiThread(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        private void StartMotionDetection()
        {
            _detector.HighlightMotion = HighlightMotion.Highlight;
            _detector.MotionColor = MotionColor.Red;
            _detector.MotionDetection += detector_MotionDetection;
            _detector.Start();
        }

        void detector_MotionDetection(object sender, MotionDetectionEvent e)
        {

            
            GuiThread(() =>
            {
                if (e.Detection)
                {

                    //MessageBox.Show("Motion Detected");
                    Play();
                    //MotionEventLabel.Content = "Motion Detected";

                }

                else
                    //MessageBox.Show("Motion Stopped");
                    Pause();
                //MotionEventLabel.Content = "Motion Stopped";


                    //video MyWin = new video(MotionEventLabel.Content.ToString());

            });
        }

        public void Pause()
        {
            MediaDefu.Pause(); ;
        }
        public void Play()
        {
            MediaDefu.Play();
            DispatcherTimer  dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();

        }

        void timer_Tick(object sender, EventArgs e)
        {
            var GetTime = (int)MediaDefu.NaturalDuration.TimeSpan.TotalSeconds;
            var Fimalas = GetTime - 3;
            var GetElapsed = (int)MediaDefu.Position.TotalSeconds;
            
            if (GetElapsed >= Fimalas)
            {
                MediaDefu.Position = TimeSpan.FromSeconds(2);
                //int sdfsdf = MediaDefu.NaturalDuration;

                MediaDefu.Play();
            }
            else
            {
                return;
            }
        }

        private void StopMotionDetection()
        {
            _detector.Stop();
            _detector.MotionDetection -= detector_MotionDetection;
            _detector.Dispose();
        }

        private void MotionChecked(object sender, RoutedEventArgs e)
        {
            MotionEventLabel.Content = String.Empty;
            var check = sender as CheckBox;
            if (check != null)
            {
                if ((bool)check.IsChecked)
                {
                    StartMotionDetection();
                    BottomGroupBox.Visibility = Visibility.Hidden;
                    TopGroupBox.Visibility = Visibility.Hidden;
                    videoViewer.Visibility = Visibility.Hidden;
                    TitleComp.Visibility = Visibility.Hidden;
                    MediaDefu.Visibility = Visibility.Visible;
                  

                }

                else
                    StopMotionDetection();
            }
        }

        private void Compose_Click(object sender, RoutedEventArgs e)
        {
            _myCameraURLBuilder = new CameraURLBuilderWPF();
            var result = _myCameraURLBuilder.ShowDialog();

            if (result == true)
            {
                if (_camera != null)
                {
                    _camera.Disconnect();
                    videoViewer.Stop();
                }

                _camera = new OzekiCamera(_myCameraURLBuilder.CameraURL);
                _camera.CameraStateChanged += _camera_CameraStateChanged;

                InvokeGuiThread(() =>
                {
                    UrlTextBox.Text = _myCameraURLBuilder.CameraURL;
                });

                Disconnect();
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
                      
                Streaming();
                     
            _connector.Connect(_camera.VideoChannel, _detector);
            _connector.Connect(_detector, _provider);

            _camera.Start();
            videoViewer.Start();

        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            _connector.Disconnect(_camera.VideoChannel, _detector);
            _connector.Disconnect(_detector, _provider);

            _detector.Stop();
            _camera.Disconnect();
            videoViewer.Stop();
        }

        private void Disconnect()
        {
            btn_Connect.IsEnabled = true;
            btn_Disconnect.IsEnabled = false;
        }

        private void Streaming()
        {
            btn_Connect.IsEnabled = false;
            btn_Disconnect.IsEnabled = true;
        }

        void _camera_CameraStateChanged(object sender, CameraStateEventArgs e)
        {
            InvokeGuiThread(() =>
            {
                if (e.State == CameraState.Streaming)
                {
                    Streaming();
                }
                if (e.State == CameraState.Disconnected)
                {
                    Disconnect();
                }

            });
        }

        private void InvokeGuiThread(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        
    }
}