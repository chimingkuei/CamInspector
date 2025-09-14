using CamInspector.Subpage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static CamInspector.BaseLogRecord;

namespace CamInspector
{
    enum Page
    {
        Homepage, ParameterSettings, DeviceSettings, ProductHistory
    }

    #region Config Class
    public class SerialNumber
    {
        [JsonProperty("Parameter1_val")]
        public string Parameter1_val { get; set; }
        [JsonProperty("Parameter2_val")]
        public string Parameter2_val { get; set; }
    }

    public class Model
    {
        [JsonProperty("SerialNumbers")]
        public SerialNumber SerialNumbers { get; set; }
    }

    public class RootObject
    {
        [JsonProperty("Models")]
        public List<Model> Models { get; set; }
    }
    #endregion

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Function
        #region Config
        private SerialNumber SerialNumberClass()
        {
            SerialNumber serialnumber_ = new SerialNumber
            {
                Parameter1_val = Parameter1.Text,
                Parameter2_val = Parameter2.Text
            };
            return serialnumber_;
        }

        private void LoadConfig(int model, int serialnumber, bool encryption = false)
        {
            List<RootObject> Parameter_info = Config.Load(encryption);
            if (Parameter_info != null)
            {
                Parameter1.Text = Parameter_info[model].Models[serialnumber].SerialNumbers.Parameter1_val;
                Parameter2.Text = Parameter_info[model].Models[serialnumber].SerialNumbers.Parameter2_val;
            }
            else
            {
                // 結構:2個Models、Models下在各2個SerialNumbers
                SerialNumber serialnumber_ = SerialNumberClass();
                List<Model> models = new List<Model>
                {
                    new Model { SerialNumbers = serialnumber_ },
                    new Model { SerialNumbers = serialnumber_ }
                };
                List<RootObject> rootObjects = new List<RootObject>
                {
                    new RootObject { Models = models },
                    new RootObject { Models = models }
                };
                Config.SaveInit(rootObjects, encryption);
            }
        }

        private void SaveConfig(int model, int serialnumber, bool encryption = false)
        {
            Config.Save(model, serialnumber, SerialNumberClass(), encryption);
        }
        #endregion

        #region Dispatcher Invoke 
        public string DispatcherGetValue(System.Windows.Controls.TextBox control)
        {
            string content = "";
            this.Dispatcher.Invoke(() =>
            {
                content = control.Text;
            });
            return content;
        }

        public void DispatcherSetValue(string content, System.Windows.Controls.TextBox control)
        {
            this.Dispatcher.Invoke(() =>
            {
                control.Text = content;
            });
        }

        #region IntegerUpDown Invoke
        //public int? DispatcherIntegerUpDownGetValue(Xceed.Wpf.Toolkit.IntegerUpDown control)
        //{
        //    int? content = null;
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        if (int.TryParse(control.Text, out int result))
        //        {
        //            content = result;
        //        }
        //        else
        //        {
        //            content = null;
        //        }
        //    });
        //    return content;
        //}
        #endregion
        #endregion

        #region Version
        private void WriteVersionFile()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;  // 執行檔目錄
            string assemblyInfoPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, @"..\..\..\Properties\AssemblyInfo.cs"));
            if (File.Exists(assemblyInfoPath))
            {
                // 讀取檔案內容
                string content = File.ReadAllText(assemblyInfoPath);
                // 使用正則表示式擷取 AssemblyFileVersion
                Regex regex = new Regex(@"\[assembly:\s*AssemblyFileVersion\s*\(\s*""(?<version>[\d\.]+)""\s*\)\s*\]");
                Match match = regex.Match(content);
                if (match.Success)
                {
                    string version = match.Groups["version"].Value;
                    // 將版本寫入 TXT 檔案
                    string outputPath = "AssemblyVersion.txt";
                    File.WriteAllText(outputPath, version);
                }
            }
        }

        private void ShowVersion()
        {
            string filePath = "AssemblyVersion.txt";
            if (File.Exists(filePath))
            {
                string version = File.ReadAllText(filePath).Trim();
                Version_Number.Text = $"Version︰{version}";
            }
        }

        private void Version()
        {
            WriteVersionFile();
            ShowVersion();
        }
        #endregion

        private void SelectPage(Page page, string description)
        {
            Main.SelectedIndex = (int)page;
            Logger.WriteLog("切換"+ description + "!", LogLevel.General, richTextBoxGeneral);
        }

        private void SystemTime()
        {
            // 建立計時器
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            // 設定 Tick 事件
            timer.Tick += (sender, e) =>
            {
                System_Time.Text = DateTime.Now.ToString();
            };

            // 啟動計時器
            timer.Start();
        }

        public void AuthVerifyAction(AuthStatus status)
        {
            if (status == AuthStatus.Success)
                Account_LogIn.Text = "Log Out";
        }

        private void OpenFolder(string description, System.Windows.Controls.TextBox textbox)
        {
            System.Windows.Forms.FolderBrowserDialog path = new System.Windows.Forms.FolderBrowserDialog();
            path.Description = description;
            path.ShowDialog();
            textbox.Text = path.SelectedPath;
        }
        #endregion

        #region Parameter and Init
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTime();
            LoadConfig(0, 0);
            Version();
        }
        BaseConfig<RootObject> Config = new BaseConfig<RootObject>();
        BaseLogRecord Logger = new BaseLogRecord();
        #endregion

        #region Menu Block
        private void Menu_Rbn(object sender, RoutedEventArgs e)
        {
            switch ((sender as System.Windows.Controls.RadioButton).Name)
            {
                case nameof(Home_Page):
                    {
                        SelectPage(Page.Homepage, "主頁");
                        break;
                    }
                case nameof(Parameter_Setting):
                    {
                        SelectPage(Page.ParameterSettings, "參數設定");
                        break;
                    }
                case nameof(Device_Setting):
                    {
                        SelectPage(Page.DeviceSettings, "設備設定");
                        break;
                    }
                case nameof(Product_History):
                    {
                        SelectPage(Page.ProductHistory, "產品履歷");
                        break;
                    }
                case nameof(Window_Closing):
                    {
                        if (MessageBox.Show("確認要關閉程式嗎？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            this.Close();
                        Window_Closing.IsChecked = false;
                        break;
                    }
            }
        }
        #endregion

        #region StatusBar Block
        private void LogIn_Switch(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleButton;
            switch (toggle.IsChecked)
            {
                case true:
                    LogIn logIn = new LogIn();
                    logIn.VerifyAction += AuthVerifyAction;
                    logIn.ShowDialog();
                    break;
                case false:
                    Account_LogIn.Text = "Log In";
                    break;
            }
        }
        #endregion

        #region Main
        private void Main_Btn_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as System.Windows.Controls.Button).Name)
            {
                case nameof(Demo):
                    {
                        
                        break;
                    }
            }
        }
        #endregion

    }
}
