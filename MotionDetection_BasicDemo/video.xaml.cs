using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.DirectX.AudioVideoPlayback;


namespace Basic_CameraViewer
{
    /// <summary>
    /// Interaction logic for video.xaml
    /// </summary>
    public partial class video : Window
    {
        public video()
        {


            InitializeComponent();



        }

        public video(string content)
        {
            InitializeComponent();
            string Motion = "Motion Detected";
            string MotionStopped= "Motion Stopped";
            if (content.ToString() == Motion)
            {
                Play_Click();

            }
            else if(content.ToString() == MotionStopped)
            {
                PAUSE_Click();

            }
        }

        public void Play_Click()
        {
            VideoControl.Play();
        }
        public void PAUSE_Click()
        {
            VideoControl.Pause();
        }





    }
}
