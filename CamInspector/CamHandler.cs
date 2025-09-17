using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CamInspector
{
    public class IPCam
    {
        [DisplayName("選取")]
        public bool IsSelected { get; set; }

        [DisplayName("Device名稱")]
        public string Devicename { get; set; }

        [DisplayName("使用者名稱")]
        public string Username { get; set; }

        [DisplayName("密碼")]
        public string Password { get; set; }

        [DisplayName("IP")]
        public string IP { get; set; }

        [DisplayName("Port")]
        public int Port { get; set; }

        [DisplayName("路徑")]
        public string Path { get; set; }
    }

    class CamHandler
    {
        public DataGrid dataGrid = new DataGrid();
        public ObservableCollection<IPCam> ipCamList = new ObservableCollection<IPCam>();

        private void SetHeaderStyle()
        {
            // DataGrid Header 置中
            var headerStyle = new Style(typeof(DataGridColumnHeader));
            headerStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            headerStyle.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            headerStyle.Setters.Add(new Setter(FrameworkElement.HeightProperty, 40.0));
            headerStyle.Setters.Add(new Setter(DataGridColumnHeader.FontSizeProperty, 28.0));  // 設文字大小
            dataGrid.ColumnHeaderStyle = headerStyle;
        }

        private (Style textCenterStyle, Style textEditStyle) SetTextCellStyle()
        {
            // 顯示文字置中樣式
            var textCenterStyle = new Style(typeof(TextBlock));
            textCenterStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            textCenterStyle.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            textCenterStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 28.0));  // 設文字大小

            // 編輯文字置中樣式
            var textEditStyle = new Style(typeof(TextBox));
            textEditStyle.Setters.Add(new Setter(TextBox.TextAlignmentProperty, TextAlignment.Center));
            textEditStyle.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            textEditStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 28.0));  // 設文字大小
            return (textCenterStyle, textEditStyle);
        }

        private Style SetCheckBoxCellStyle()
        {
            // CheckBox 置中樣式
            var checkCenterStyle = new Style(typeof(CheckBox));
            checkCenterStyle.Setters.Add(new Setter(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            checkCenterStyle.Setters.Add(new Setter(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center));
            checkCenterStyle.Setters.Add(new Setter(CheckBox.LayoutTransformProperty, new ScaleTransform(2, 2))); // 設框框大小
            return checkCenterStyle;
        }

        public void IPCamInit()
        {
            // DataGrid 不自動產生欄位
            dataGrid.AutoGenerateColumns = false;
            dataGrid.Columns.Clear();
            SetHeaderStyle();
            // 設定列高度
            dataGrid.RowHeight = 40;
            // 動態建立欄位 (根據 IPCam 屬性)
            var properties = typeof(IPCam).GetProperties();
            foreach (var prop in properties)
            {
                var displayNameAttr = prop.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                           .FirstOrDefault() as DisplayNameAttribute;

                string headerText = displayNameAttr != null ? displayNameAttr.DisplayName : prop.Name;
                switch (Type.GetTypeCode(prop.PropertyType))
                {
                    case TypeCode.Boolean:
                        dataGrid.Columns.Add(new DataGridCheckBoxColumn
                        {
                            Header = headerText,
                            Binding = new System.Windows.Data.Binding(prop.Name),
                            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                            ElementStyle = SetCheckBoxCellStyle()
                        });
                        break;

                    default:
                        dataGrid.Columns.Add(new DataGridTextColumn
                        {
                            Header = headerText,
                            Binding = new System.Windows.Data.Binding(prop.Name),
                            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                            ElementStyle = SetTextCellStyle().textCenterStyle,
                            EditingElementStyle = SetTextCellStyle().textEditStyle
                        });
                        break;
                }
            }
            // 指定 DataGrid 資料來源
            dataGrid.ItemsSource = ipCamList;
            // 測試增加一筆資料
            //ipCamList.Add(new IPCam
            //{
            //    IsSelected = false,
            //    Name = "Camera1",
            //    Username = "admin",
            //    Password = "1234",
            //    IP = "192.168.1.100",
            //    Port = 8080,
            //    Path = "/video"
            //});
        }

        public (string DeviceName, string Username, string Password, string IP, int Port, string Path) ReadRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGrid.Items.Count)
            {
                if (dataGrid.Items[rowIndex] is IPCam cam)
                {
                    return (
                        cam.Devicename ?? string.Empty,
                        cam.Username ?? string.Empty,
                        cam.Password ?? string.Empty,
                        cam.IP ?? string.Empty,
                        cam.Port, // 假設 Port 是 int，不用處理 null
                        cam.Path ?? string.Empty
                    );
                }
            }
            return (string.Empty, string.Empty, string.Empty, string.Empty, 0, string.Empty);
        }



    }
}
