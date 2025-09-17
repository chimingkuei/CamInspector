using CamInspector.Subpage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using System.Windows.Forms;
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
        Homepage, ParameterSettings, DetectionDisplay, ProductHistory
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

    #region ParameterConfig Class
    public class ParameterSerialNumber
    {
        [JsonProperty("Devicename_val")]
        public string Devicename_val { get; set; }
        [JsonProperty("Username_val")]
        public string Username_val { get; set; }
        [JsonProperty("Password_val")]
        public string Password_val { get; set; }
        [JsonProperty("IP_val")]
        public string IP_val { get; set; }
        [JsonProperty("Port_val")]
        public int Port_val { get; set; }
        [JsonProperty("Path_val")]
        public string Path_val { get; set; }
    }

    public class ParameterModel
    {
        [JsonProperty("SerialNumbers")]
        public ParameterSerialNumber SerialNumbers { get; set; }
    }

    public class ParameterRootObject
    {
        [JsonProperty("Models")]
        public List<ParameterModel> Models { get; set; }
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

        private void SaveConfig(int model, int serialnumber, bool backup = true, bool encryption = false)
            => Config.Save(model, serialnumber, SerialNumberClass(), backup, encryption);
        #endregion

        #region ParameterConfig
        private void ParameterLoadConfig(int model, int serialnumber, bool encryption = false)
        {
            List<ParameterRootObject> Parameter_info = ParameterConfig.Load(encryption);
            if (Parameter_info != null)
            {
                cam.ipCamList.Add(new IPCam
                {
                    IsSelected = false,
                    Devicename = Parameter_info[model].Models[serialnumber].SerialNumbers.Devicename_val,
                    Username = Parameter_info[model].Models[serialnumber].SerialNumbers.Username_val,
                    Password = Parameter_info[model].Models[serialnumber].SerialNumbers.Password_val,
                    IP = Parameter_info[model].Models[serialnumber].SerialNumbers.IP_val,
                    Port = Parameter_info[model].Models[serialnumber].SerialNumbers.Port_val,
                    Path = Parameter_info[model].Models[serialnumber].SerialNumbers.Path_val
                });
            }
        }

        private int CountString(JToken token, string targetName)
        {
            int count = 0;
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    if (property.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                        count++;
                    count += CountString(property.Value, targetName);
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    count += CountString(item, targetName);
                }
            }
            return count;
        }

        private void MainParameterLoadConfig()
        {
            string jsonPath = ParameterConfig.configPath;
            if (File.Exists(jsonPath))
            {
                string jsonContent = File.ReadAllText(jsonPath);
                JToken json = JToken.Parse(jsonContent);
                int num = CountString(json, "SerialNumbers") / CountString(json, "Models");
                for (int i = 0; i < num; i++)
                {
                    ParameterLoadConfig(0, i);
                }
            }
            else
            {
                // 結構:2個Models、Models下在各2個SerialNumbers
                ParameterSerialNumber serialnumber_ = new ParameterSerialNumber();
                List<ParameterModel> models = new List<ParameterModel>
                {
                    new ParameterModel { SerialNumbers = serialnumber_ },
                    new ParameterModel { SerialNumbers = serialnumber_ }
                };
                List<ParameterRootObject> rootObjects = new List<ParameterRootObject>
                {
                    new ParameterRootObject { Models = models },
                    new ParameterRootObject { Models = models }
                };
                ParameterConfig.SaveInit(rootObjects);
            }
        }

        private ParameterSerialNumber ParameterSerialNumberClass()
        {
            ParameterSerialNumber serialnumber_ = new ParameterSerialNumber
            {
                Devicename_val = cam.ReadRow(parameterIndex).DeviceName,
                Username_val = cam.ReadRow(parameterIndex).Username,
                Password_val = cam.ReadRow(parameterIndex).Password,
                IP_val = cam.ReadRow(parameterIndex).IP,
                Port_val = cam.ReadRow(parameterIndex).Port,
                Path_val = cam.ReadRow(parameterIndex).Path
            };
            return serialnumber_;
        }

        private void ParameterSaveConfig(int model, int serialnumber, bool backup = true, bool encryption = false)
            => ParameterConfig.Save(model, serialnumber, ParameterSerialNumberClass(), backup, encryption);

        private void MainParameterSaveConfig()
        {
            ParameterConfig.ConfigBackup();
            int datagridRowCount = Parameter.Items.Count - 1;
            int rootCount = 2;   // 需要幾個 RootObject
            //int modelCount = 3;  // 每個 RootObject 裡需要幾個 Model
            List<ParameterRootObject> rootObjects = new List<ParameterRootObject>();
            for (int i = 0; i < rootCount; i++)
            {
                List<ParameterModel> models = new List<ParameterModel>();

                for (int j = 0; j < datagridRowCount; j++)
                {
                    ParameterSerialNumber serialnumber_ = new ParameterSerialNumber();
                    models.Add(new ParameterModel { SerialNumbers = serialnumber_ });
                }

                rootObjects.Add(new ParameterRootObject { Models = models });
            }
            ParameterConfig.SaveInit(rootObjects);
            for (int i = 0; i < datagridRowCount; i++)
            {
                parameterIndex = i;
                ParameterSaveConfig(0, parameterIndex, backup:false);
            }
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

        #region Box Event
        private void Box_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.None)
            {
                startPoint = e.GetPosition(Box_Canvas);

                // 建立新的矩形
                currentRect = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 4
                };

                // 矩形也要能監聽點擊事件
                currentRect.MouseDown += TargetBox_MouseDown;

                Canvas.SetLeft(currentRect, startPoint.X);
                Canvas.SetTop(currentRect, startPoint.Y);
                Box_Canvas.Children.Add(currentRect);

                isDrawing = true;
            }
        }

        private void Box_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed && currentRect != null)
            {
                Point pos = e.GetPosition(Box_Canvas);

                double x = Math.Min(pos.X, startPoint.X);
                double y = Math.Min(pos.Y, startPoint.Y);
                double w = Math.Abs(pos.X - startPoint.X);
                double h = Math.Abs(pos.Y - startPoint.Y);

                Canvas.SetLeft(currentRect, x);
                Canvas.SetTop(currentRect, y);
                currentRect.Width = w;
                currentRect.Height = h;
            }
            else if (isDragging && selectedRect != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // 拖曳矩形
                Point pos = e.GetPosition(Box_Canvas);
                double dx = pos.X - dragStartPoint.X;
                double dy = pos.Y - dragStartPoint.Y;

                Canvas.SetLeft(selectedRect, Canvas.GetLeft(selectedRect) + dx);
                Canvas.SetTop(selectedRect, Canvas.GetTop(selectedRect) + dy);

                dragStartPoint = pos; // 更新拖曳起點
            }
        }

        private void Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                currentRect = null; // 框選完成，準備下一次
            }
            else if (isDragging)
            {
                isDragging = false;
            }
        }

        // 點擊矩形 → 選取
        private void TargetBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                // 清除舊的選取
                if (selectedRect != null)
                    selectedRect.Stroke = Brushes.Red;

                // 設定新的選取
                selectedRect = rect;
                selectedRect.Stroke = Brushes.Lime; // 綠色表示被選取

                // 準備拖曳
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    isDragging = true;
                    dragStartPoint = e.GetPosition(Box_Canvas);
                }

                e.Handled = true; // 防止冒泡到 Image
            }
        }

        // Delete 鍵刪除
        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete && selectedRect != null)
            {
                Box_Canvas.Children.Remove(selectedRect);
                selectedRect = null;
            }
        }
        #endregion
        #endregion

        #region Parameter and Init
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTime();
            cam.dataGrid = Parameter;
            cam.IPCamInit();
            MainParameterLoadConfig();
            LoadConfig(0, 0);
            Version();
            this.KeyDown += MainWindow_KeyDown; // 監聽鍵盤事件
        }
        BaseConfig<RootObject> Config = new BaseConfig<RootObject>(@"Config/Config.json");
        BaseConfig<ParameterRootObject> ParameterConfig = new BaseConfig<ParameterRootObject>(@"Config/ParameterConfig.json");
        BaseLogRecord Logger = new BaseLogRecord();
        CamHandler cam = new CamHandler();
        LightHandler light = new LightHandler();
        private Point startPoint;
        private Rectangle currentRect;
        private bool isDrawing = false;
        private Rectangle selectedRect = null;
        private bool isDragging = false;
        private Point dragStartPoint;
        private int parameterIndex = 0;
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
                case nameof(Detection_Display):
                    {
                        SelectPage(Page.DetectionDisplay, "檢測顯示");
                        break;
                    }
                case nameof(Product_History):
                    {
                        SelectPage(Page.ProductHistory, "產品履歷");
                        break;
                    }
                case nameof(Window_Closing):
                    {
                        if (System.Windows.MessageBox.Show("確認要關閉程式嗎？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                        //light.TwoLights(LightPanel, "CCTV1", LightColor.Off);
                        //light.TwoLights(LightPanel, "CCTV2", LightColor.Off);
                        MainParameterSaveConfig();
                        break;
                    }
                case nameof(Demo1):
                    {
                        // 控制第一組燈變成綠燈亮
                        //var frame1 = LightPanel.Children[0] as Border;
                        //light.SetLight(frame1, LightColor.Green);

                        // 控制第二組燈熄滅
                        //var frame2 = LightPanel.Children[1] as Border;
                        //light.SetLight(frame2, LightColor.Red);
                        //cam.ReadDataGrid();
                        break;
                    }
            }
        }
        #endregion

    }
}
