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
using Utility;

namespace 新生录取管理系统
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private Brush? greenBrush;
        private Brush? redBrush;
        private int rightInputCount = 0;
        public RegisterWindow()
        {
            InitializeComponent();
            greenBrush = this.FindResource("green_brush") as Brush;
            redBrush = this.FindResource("red_brush") as Brush;
        }

        private async void register_button_Click(object sender, RoutedEventArgs e)
        {
            string name = string.Empty;
            string id = string.Empty;
            string password = string.Empty;
            string repeatpassword = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
                name = name_input.Text;
                id = account_input.Text;
                password = password_input.Password;
                repeatpassword = repeatpassword_input.Password;
            });
            //name = name_input.Text;
            //id = account_input.Text;
            //password = password_input.Password;
            //repeatpassword = repeatpassword_input.Password;

            //输入内容不完全合法
            if (rightInputCount!= 0b1111)
            {
                Console.WriteLine("输入不合法");
                return;
            }
            else
            {
                Console.WriteLine("合法");
            }

            //注册
            Login.ErrorCode err = Login.ErrorCode.None;
            //await Task.Run(() => { Login.Register(name, id, password,out err); });
        
            
        }

        private void name_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = name_input.Text;
            flag1.Visibility = Visibility.Visible;
            if(AccountFormat.CheckName(name)) 
            {
                flag1.Foreground = greenBrush;
                flag1.Text = "√";
                rightInputCount|= 0b0001;
            }
            else
            {
                flag1.Foreground = redBrush;
                flag1.Text = "×";
                rightInputCount&= 0b1110;
            }
        }

        private void account_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            string account = account_input.Text;
            flag2.Visibility = Visibility.Visible;
            if(AccountFormat.CheckAccount(account))
            {
                flag2.Foreground = greenBrush;
                flag2.Text = "√";
                rightInputCount|= 0b0010;
            }
            else
            {
                flag2.Foreground= redBrush;
                flag2.Text = "×";
                rightInputCount&= 0b1101;
            }
        }

        private void password_input_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = password_input.Password;
            flag3.Visibility = Visibility.Visible;
            if (AccountFormat.CheckPassword(password))
            {
                flag3.Foreground = greenBrush;
                flag3.Text = "√";
                rightInputCount|= 0b0100;
            }
            else
            {
                flag3.Foreground = redBrush;
                flag3.Text = "×";
                rightInputCount&= 0b1011;
            }
        }

        private void repeatpassword_input_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = password_input.Password;
            string repeatpassword = repeatpassword_input.Password;
            flag4.Visibility = Visibility.Visible;
            if (password.Equals(repeatpassword) && AccountFormat.CheckPassword(repeatpassword))
            {
                flag4.Foreground = greenBrush;
                flag4.Text = "√";
                rightInputCount|=0b1000;
            }
            else
            {
                flag4.Foreground = redBrush;
                flag4.Text = "×";
                rightInputCount&= 0b0111;
            }
        }
    }
}
