using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        IFirebaseClient client;

        public MainWindow()
        {
            InitializeComponent();
            textview.Content = "asdasd";
        }



        void initializeFireBase()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "BIVXBcQRPDF0z0PrtKF5umRUn5R4K31KdrkGvni6MZtWGsCxczFbr8crhqwIFyoRUAQ5jtnOPmi1s-bNjB9HEOM",
                BasePath = "https://yourfirebase.firebaseio.com/"
            };
            client = new FirebaseClient(config);


        }
    }
}
