using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CamInspector.Subpage
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        #region Function
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("請問是否要關閉？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        #region PasswordEye
        // Password
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
            => authMgr.UpdateWatermark(Password, Password_Watermark);
        private void Password_GotFocus(object sender, RoutedEventArgs e)
            => authMgr.HideWatermark(Password_Watermark);
        private void Password_LostFocus(object sender, RoutedEventArgs e)
            => authMgr.ShowWatermarkIfEmpty(Password, Password_Watermark);
        // Captcha
        private void Captcha_PasswordChanged(object sender, RoutedEventArgs e)
            => authMgr.UpdateWatermark(Captcha, Captcha_Watermark);
        private void Captcha_GotFocus(object sender, RoutedEventArgs e)
            => authMgr.HideWatermark(Captcha_Watermark);
        private void Captcha_LostFocus(object sender, RoutedEventArgs e)
            => authMgr.ShowWatermarkIfEmpty(Captcha, Captcha_Watermark);
        #endregion
        #endregion

        #region Parameter and Init
        AuthManager authMgr = new AuthManager("Account.ini");
        #endregion

        #region Main
        private void Main_Btn_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case nameof(PasswordEye):
                    {
                        authMgr.BasePasswordEye(Password, PasswordText);
                        break;
                    }
                case nameof(CaptchaEye):
                    {
                        authMgr.BasePasswordEye(Captcha, CaptchaPasswordText);
                        break;
                    }
                case nameof(SignUp):
                    {
                        if (!string.IsNullOrEmpty(Account.Text) && !string.IsNullOrEmpty(Password.Password))
                        {
                            if (Captcha.Password == "Admin")
                            {
                                authMgr.Register(Account.Text, Password.Password);
                            }
                            else
                            {
                                MessageBox.Show("驗證碼不對!", "訊息", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("帳戶或密碼未設定!", "訊息", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                    }
                case nameof(Revoke):
                    {
                        if (!string.IsNullOrEmpty(Account.Text))
                        {
                            if (Captcha.Password == "Admin")
                            {
                                authMgr.Revoke(Account.Text);
                            }
                            else
                            {
                                MessageBox.Show("驗證碼不對!", "訊息", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("帳戶未設定!", "訊息", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                    }
            }
        }
        #endregion

    }
}
