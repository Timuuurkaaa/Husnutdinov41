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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Husnutdinov41
{
    /// <summary>
    /// Логика взаимодействия для ShoesPage.xaml
    /// </summary>
    public partial class ShoesPage : Page
    {
        public ShoesPage()
        {
            InitializeComponent();
            var currentShoes = Husnutdinov41Entities.GetContext().Product.ToList();
            ShoesListView.ItemsSource = currentShoes;
        }

        private void ShoesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
