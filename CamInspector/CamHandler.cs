using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;

namespace CamInspector
{
    class CamHandler
    {
        DataTable dt = new DataTable();

        public void IPCamInit(System.Windows.Controls.DataGrid dataGrid)
        {
            // 建立 DataTable
            DataTable dt = new DataTable();
            dt.Columns.Add("選取", typeof(bool));
            dt.Columns.Add("名稱", typeof(string));
            dt.Columns.Add("使用者名稱", typeof(string));
            dt.Columns.Add("密碼", typeof(string));
            dt.Columns.Add("IP", typeof(string));
            dt.Columns.Add("Port", typeof(int));
            dt.Columns.Add("路徑", typeof(string));

            // 設定列高度
            dataGrid.RowHeight = 30;  // 固定每列高度為 30px

            // DataGrid 不自動產生欄位
            dataGrid.AutoGenerateColumns = false;
            dataGrid.Columns.Clear();

            // 顯示文字置中樣式
            var textCenterStyle = new System.Windows.Style(typeof(System.Windows.Controls.TextBlock));
            textCenterStyle.Setters.Add(new Setter(System.Windows.Controls.TextBlock.TextAlignmentProperty, System.Windows.TextAlignment.Center));
            textCenterStyle.Setters.Add(new Setter(System.Windows.Controls.TextBlock.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Center));

            // 編輯文字置中樣式
            var textEditStyle = new System.Windows.Style(typeof(System.Windows.Controls.TextBox));
            textEditStyle.Setters.Add(new Setter(System.Windows.Controls.TextBox.TextAlignmentProperty, System.Windows.TextAlignment.Center));
            textEditStyle.Setters.Add(new Setter(System.Windows.Controls.TextBlock.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Center));

            // CheckBox 置中樣式
            var checkCenterStyle = new System.Windows.Style(typeof(System.Windows.Controls.CheckBox));
            checkCenterStyle.Setters.Add(new Setter(System.Windows.Controls.CheckBox.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center));
            checkCenterStyle.Setters.Add(new Setter(System.Windows.Controls.CheckBox.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Center));

            foreach (DataColumn col in dt.Columns)
            {
                switch (Type.GetTypeCode(col.DataType))
                {
                    case TypeCode.Boolean:
                        DataGridCheckBoxColumn checkColumn = new DataGridCheckBoxColumn
                        {
                            Header = col.ColumnName,
                            Binding = new System.Windows.Data.Binding(col.ColumnName),
                            Width = 150,
                            ElementStyle = checkCenterStyle
                        };
                        dataGrid.Columns.Add(checkColumn);
                        break;

                    case TypeCode.String:
                    case TypeCode.Int32:
                    default:
                        DataGridTextColumn defaultColumn = new DataGridTextColumn
                        {
                            Header = col.ColumnName,
                            Binding = new System.Windows.Data.Binding(col.ColumnName),
                            Width = 150,
                            ElementStyle = textCenterStyle,        // 顯示時置中
                            EditingElementStyle = textEditStyle    // 編輯時置中
                        };
                        dataGrid.Columns.Add(defaultColumn);
                        break;
                }
            }

            // 設定 DataGrid 資料來源
            dataGrid.ItemsSource = dt.DefaultView;
        }







    }
}
