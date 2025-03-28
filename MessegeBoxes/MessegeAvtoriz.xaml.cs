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

namespace CommunityBookLovers.MessegeBoxes
{
    /// <summary>
    /// Логика взаимодействия для MessegeAvtoriz.xaml
    /// </summary>
    public partial class MessegeAvtoriz : Window
    {
        public MessegeAvtoriz()
        {
            InitializeComponent();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
