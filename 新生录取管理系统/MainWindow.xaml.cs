using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility;

namespace 新生录取管理系统
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    class StudentItem
    {
        public string ExamineeNumber { get;set;}
        public string Name { get;set;}
        public string IDNumber { get;set;}
        public bool Recruit {  get; set; }

        public string C1 {  get; set; }
        public string C2 {  get; set; }
        public string C3 {  get; set; }
        public string C4 {  get; set; }
        public string C5 {  get; set; }
        public string C6 {  get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            name_textblock.Text = Login.Account?.Name;
            name_tooltip.Text = Login.Account?.Name;

            this.admit_inquery_table.CanUserAddRows = false;

            ObservableCollection<StudentItem> students = new ObservableCollection<StudentItem>();
            students.Add(new StudentItem
            {
                ExamineeNumber = "1",
                IDNumber = "2",
                Name = "lyl",
                C1 = "1",
                C2 = "2",
                C3 = "3",
                C4 = "4",
                C5 = "5",
                C6 = "6",
                Recruit = true,
            });
            this.admit_inquery_table.DataContext = students;
        }

        private void admit_inquery_table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }
    }
}