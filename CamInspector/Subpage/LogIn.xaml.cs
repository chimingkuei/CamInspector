using CamInspector.Icon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static CamInspector.Subpage.AuthManager;

namespace CamInspector.Subpage
{
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();
        }

        #region Function
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
        }

        #region PasswordEye
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
            => authMgr.UpdateWatermark(Password, Password_Watermark);
        private void Password_GotFocus(object sender, RoutedEventArgs e)
            => authMgr.HideWatermark(Password_Watermark);
        private void Password_LostFocus(object sender, RoutedEventArgs e)
            => authMgr.ShowWatermarkIfEmpty(Password, Password_Watermark);
        #endregion
        #endregion

        #region Parameter and Init
        // 宣告委派
        public delegate void VerifyEventHandller(AuthStatus status);
        // 宣告事件
        public event VerifyEventHandller VerifyAction;
        AuthManager authMgr = new AuthManager("Account.ini");
        #endregion

        #region Main
        private void Main_Btn_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as System.Windows.Controls.Button).Name)
            {
                case nameof(PasswordEye):
                    {
                        authMgr.BasePasswordEye(Password, PasswordText);
                        break;
                    }
                case nameof(Log_In):
                    {
                        if (!string.IsNullOrEmpty(Account.Text) && !string.IsNullOrEmpty(Password.Password))
                        {
                            // 驗證格式-->user:Admin;password:Admin
                            if (authMgr.CheckPassword(Account.Text, Password.Password) != AuthStatus.Success)
                            {
                                MessageBox.Show("帳戶或密碼不正確!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            // 驗證成功
                            VerifyAction.Invoke(AuthStatus.Success);
                            this.Closing -= WindowClosing;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("帳戶或密碼未設定!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                    }
            }
        }
        #endregion

    }
}
