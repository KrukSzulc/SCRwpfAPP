
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCRwpfApp
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        static int id = 0;
        static Firebase myDAO = new Firebase();
        //IFirebaseConfig config;
        public MainWindow()
        {
            InitializeComponent();
            text.Content = myDAO.READ();
       
        }

        private void btn_post_Click(object sender, RoutedEventArgs e)
        {
            TaskSCR task = new TaskSCR();
            task.Name = "zadanie" + id;
            task.id = id.ToString();
            task.uuid = id.ToString() + "asdasd";
            id++;
            myDAO.POST(task);
        }
    }
}
