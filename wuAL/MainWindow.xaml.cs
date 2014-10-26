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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO;
using uAL;
using System.ComponentModel;

namespace wuAL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        ObservableCollection<TorrentActionData> _TorrentActions = new ObservableCollection<TorrentActionData>();
        bool _connected = false;
        bool _localhost = true;
        bool _connecting = false;
        string _hostAddress = "N/A";
        Regex _hostnameRegExp = new Regex("[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\:[0-9]{1,5}");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Public bindings
        public ObservableCollection<TorrentActionData> TorrentActions { get { return _TorrentActions; } }
        public bool Connected { get { return _connected && !_connecting; } }
        public bool NotConnected { get { return !_connected && !_connecting; } }
        public bool Connecting { get { Console.WriteLine("AMAGAD!"); return _connecting; } }
        public bool IsLocalhost
        {
            get { return _localhost; }
            set
            {
                _localhost = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsLocalhost"));
            }
        }
        public string HostAddress { get { return _hostAddress; } }
        public bool IsValidSettings { get { return Properties.Settings.Default.validConnectionSettings; }
            set
            {
                Properties.Settings.Default.validConnectionSettings = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsValidSettings"));
            }
        }
        #endregion

        private void Connect(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void ValidateSettings()
        {
            if (!IsValidSettings)
            {
                IsLocalhost = (HostnameBox.Text.Contains("127.0.0.1") || HostnameBox.Text.ToLower().Contains("localhost"));
                if (!IsLocalhost)
                {
                    if (_hostnameRegExp.IsMatch(HostnameBox.Text))
                        if (Directory.Exists(TorrentDirBox.Text))
                            Properties.Settings.Default.validConnectionSettings = true;

                }
                else if (IsLocalhost)
                    IsValidSettings = true;
            }
            Console.WriteLine(HostnameBox.Text + " is " + IsLocalhost);
        }

        private void Connect()
        {
            if (IsValidSettings)
            {
                try
                {
                    _connecting = true;
                    TorrentAPI ta = new TorrentAPI(Properties.Settings.Default.hostname, Properties.Settings.Default.username, Properties.Settings.Default.password);
                    Properties.Settings.Default.torrentDir = ta.GetDownloadDir();
                    _connecting = false;
                    _connected = true;
                }
                catch (Exception ex)
                {
                    _connecting = false;
                    _connected = false;
                }
            }
        }

        private void SettingsTextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateSettings();
        }

        private void SettingsTextChanged(object sender, RoutedEventArgs e)
        {
            ValidateSettings();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _connecting = !_connecting;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Visuals : INotifyPropertyChanged
    {
        private bool connected;

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        public Visuals() { }

        public bool Connected
        {
            get { return connected; }
            set
            {
                connected = value;
                OnPropertyChanged("Connected");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class TorrentActionData
    {
        public string TorrentName { get; set; }
        public string Action { get; set; }
    }
}
