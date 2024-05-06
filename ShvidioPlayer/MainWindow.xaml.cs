using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShvidioPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private Random random = new Random();
        DispatcherTimer tax_timer = new DispatcherTimer();
        private bool full_window = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            tax_timer.Interval = TimeSpan.FromSeconds(0.1);
            tax_timer.Tick += change_taxometr;
        }

        private void change_taxometr(object sender, EventArgs e)
        {
            SpeedValue.Value = random.NextDouble();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private MediaState _mediaState = MediaState.Stopped;
        internal MediaState MediaState 
        {
            get { return _mediaState; } 
            set
            {
                _mediaState = value;
                FirePropertyChanged("PlayButtonName");
            }
        }


        public string PlayButtonName
        {
            get
            {
                return (MediaState == MediaState.Playing) ? "Pause" : "Play";
            }
        }

        private void PlayerSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Pause();
            media.Position = TimeSpan.FromSeconds(PlayerSlider.Value);
            Play();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            var dick = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".avi",
                Filter = " Video files (*.avi)| *.avi|All files(*.*)|*.*",
                CheckFileExists = true,

            };
            if (dick.ShowDialog(this) == true)
            {
                var filename = dick.FileName;
                if (MediaState == MediaState.Playing || MediaState == MediaState.Paused)
                {
                    Stop();
                }
                media.Source = new Uri(filename);
                Play();
                tax_timer.Start();
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (media.Source != null)
            {
                if (MediaState == MediaState.Playing && media.CanPause)
                {
                    Pause();
                }
                else if (MediaState == MediaState.Stopped || MediaState == MediaState.Paused)
                {
                    Play();
                }
            }
            
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (MediaState == MediaState.Playing || MediaState == MediaState.Paused)
            {
                Stop();
            }
        }

        private void media_Loaded(object sender, RoutedEventArgs e)
        {
            media.MediaOpened += new RoutedEventHandler(media_MediaOpened);
            media.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(media_MediaFailed);
            media.MediaEnded += new RoutedEventHandler(media_MediaEnded);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(Timer_tick);

            timer.Start();
        }

        private void Timer_tick(object sender, EventArgs e)
        {
            if (MediaState == MediaState.Playing)
            {
                PlayerSlider.Value = media.Position.TotalSeconds;

                TimeSpan t = TimeSpan.FromSeconds(Math.Round(media.Position.TotalSeconds, 0));
                string answer = string.Format("{0:D2}:{1:D2}:{2:D2} - {3:D2}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                Math.Round(PlayerSlider.Maximum, 1).ToString()
                                );
                Time_video.Text = answer;
            }
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            Stop();
            Play();
        }

        private void media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Error loading file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            PlayerSlider.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
        }


        private void Stop()
        {
            media.Stop();
            MediaState = MediaState.Stopped;
            PlayerSlider.Value = 0;
            Play();
        }

        private void Play()
        {
            media.Play();
            MediaState = MediaState.Playing;
        }

        private void Pause()
        {
            media.Pause();
            MediaState = MediaState.Paused;
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Rol_up_button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Max_window_button_button_Click(object sender, RoutedEventArgs e)
        {
            if (full_window)
            {
                full_window = false;
                this.WindowState = WindowState.Normal;
            }
            else
            {
                full_window = true;
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Change_tax(object sender, RoutedEventArgs e)
        {
            if (tax_timer.IsEnabled == false)
            {
                tax_timer.Start();
            }
            else
            {
                tax_timer.Stop();
            }
        }
    }
}
