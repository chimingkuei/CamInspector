using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CamInspector.Subpage
{
    public enum AuthStatus
    {
        Success, InvalidUserOrPassword, FileMissing
    }

    class AuthManager
    {
        public string accountPath { get; set; }

        public AuthManager(string _accountPath)
        {
            accountPath = _accountPath;
        }

        private AuthStatus JudgeUser(string[] lines, string newUser)
        {
            foreach (string line in lines)
            {
                var parts = line.Split(new[] { "user:", ";password:" }, StringSplitOptions.None);
                if (parts.Length >= 2 && parts[1] == newUser)
                {
                    return AuthStatus.Success;
                }
            }
            return AuthStatus.InvalidUserOrPassword;
        }

        public void Register(string newUser, string password)
        {
            string newEntry = $"user:{newUser};password:{password}";
            if (!File.Exists(accountPath))
            {
                File.WriteAllText(accountPath, newEntry + Environment.NewLine);
                MessageBox.Show("新建檔案並添加user!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string[] lines = File.ReadAllLines(accountPath);
            if (JudgeUser(lines, newUser) == AuthStatus.Success)
            {
                MessageBox.Show("相同user內容已存在，未作變更!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                File.AppendAllText(accountPath, newEntry + Environment.NewLine);
                MessageBox.Show("新user設定已添加!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void Revoke(string user)
        {
            if (!File.Exists(accountPath))
            {
                MessageBox.Show("檔案不存在，無法刪除使用者!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string[] lines = File.ReadAllLines(accountPath);
            var updatedLines = lines
                .Where(line =>
                {
                    var parts = line.Split(new[] { "user:", ";password:" }, StringSplitOptions.None);
                    return parts.Length < 2 || parts[1] != user;
                })
                .ToArray();
            if (updatedLines.Length == lines.Length)
            {
                MessageBox.Show($"未找到使用者 {user}，未做變更!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                File.WriteAllLines(accountPath, updatedLines);
                MessageBox.Show($"使用者 {user} 的資料已刪除!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public AuthStatus CheckPassword(string logInUser, string logInPassword)
        {
            if (!File.Exists(accountPath))
            {
                MessageBox.Show("記錄檔案不存在!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return AuthStatus.FileMissing;
            }
            // 讀取所有符合格式的使用者資料
            var userDict = File.ReadLines(accountPath)
                .Where(line => line.StartsWith("user:"))
                .Select(line => line.Split(';'))
                .Where(parts => parts.Length == 2)
                .ToDictionary(
                    parts => parts[0].Split(':')[1].Trim(),   // user
                    parts => parts[1].Split(':')[1].Trim()    // password
                );
            // 驗證帳號密碼
            return userDict.TryGetValue(logInUser, out var storedPassword) && storedPassword == logInPassword
                ? AuthStatus.Success
                : AuthStatus.InvalidUserOrPassword;
        }

        #region PasswordEye ControlItem
        // 通用方法：更新浮水印顯示
        public void UpdateWatermark(PasswordBox passwordBox, UIElement watermark)
        {
            watermark.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }
        // 通用方法：獲得焦點時隱藏浮水印
        public void HideWatermark(UIElement watermark)
        {
            watermark.Visibility = Visibility.Collapsed;
        }
        // 通用方法：失去焦點時，如果內容為空，顯示浮水印
        public void ShowWatermarkIfEmpty(PasswordBox passwordBox, UIElement watermark)
        {
            if (string.IsNullOrEmpty(passwordBox.Password))
                watermark.Visibility = Visibility.Visible;
        }

        public void BasePasswordEye(PasswordBox password, TextBox text)
        {
            if (password.Password != null)
            {
                bool isCollapsed = text.Visibility == Visibility.Collapsed;
                if (isCollapsed)
                    text.Text = password.Password;
                else
                    password.Password = text.Text;
                text.Visibility = isCollapsed ? Visibility.Visible : Visibility.Collapsed;
                password.Visibility = isCollapsed ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

    }



}
