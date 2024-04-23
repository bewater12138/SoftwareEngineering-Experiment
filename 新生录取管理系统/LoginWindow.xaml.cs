﻿using System;
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
using Utility;

namespace 新生录取管理系统
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void login_button_Click(object sender, RoutedEventArgs e)
        {
            string  id          = string.Empty,
                    password    = string.Empty;
            input_account.Dispatcher.Invoke(() => { id = input_account.Text; });
            input_password.Dispatcher.Invoke(() => { password = input_password.Password; });
            Account? account = null;

            Login.ErrorCode err = Login.ErrorCode.None;
            StartLogin();
            await Task.Run(() => { account = Login.LoginRemote(id, password, out err); });
            if (err == Login.ErrorCode.None)
            {
                //登录成功，切换至主窗口
                App.Current.MainWindow = new MainWindow();
                App.Current.MainWindow.Show();
                this.Close();
            }
            else
            {
                string err_msg = "";
                switch(err)
                {
                    case Login.ErrorCode.Timeover:err_msg = "登录超时";break;
                    case Login.ErrorCode.LoginFail:err_msg = "账号或密码错误";break;
                    case Login.ErrorCode.NetworkError:err_msg = "网络问题";break;
                }

                MessageBox.Show(err_msg);
                LoginOver();
            }
        }

        private void register_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StartLogin()
        {
            login_button.IsEnabled = false;
            register_button.IsEnabled = false;
            login_button_bolder.Visibility = Visibility.Collapsed;
            login_button.Visibility = Visibility.Collapsed;
            register_button.Visibility = Visibility.Collapsed;

            login_effect.IsEnabled = true;
            login_effect.Visibility = Visibility.Visible;
        }

        private void LoginOver()
        {
            login_button.IsEnabled = true;
            register_button.IsEnabled = true;
            login_button_bolder.Visibility = Visibility.Visible;
            login_button.Visibility = Visibility.Visible;
            register_button.Visibility = Visibility.Visible;

            login_effect.IsEnabled = false;
            login_effect.Visibility = Visibility.Collapsed;
        }
    }
}
