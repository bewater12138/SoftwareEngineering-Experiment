using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace 新生录取管理系统
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 


    public partial class App : Application
    {
        [DllImport("kernel32.dll",CharSet = CharSet.Unicode)]
        static extern private bool AllocConsole();

        public App()
        {
            AllocConsole();
            Network.Initialize();
        }
    }

}
