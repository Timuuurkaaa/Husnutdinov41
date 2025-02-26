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
        List<Product> Tablelist;
        public ShoesPage()
        {
            InitializeComponent();
            var currentShoes = Husnutdinov41Entities.GetContext().Product.ToList();
            Tablelist = Husnutdinov41Entities.GetContext().Product.ToList();
            ShoesListView.ItemsSource = currentShoes;
            ComboType.SelectedIndex = 0;
            UpdateShoes();
        }

        private void UpdateShoes()
        {
            var currentShoes = Husnutdinov41Entities.GetContext().Product.ToList();

            string SearchText = Search.Text;
            {
                if (SearchText != null)
                {
                    currentShoes = currentShoes.Where(p => (p.ProductName.ToLower().Contains(SearchText.ToLower()))).ToList();
                }
            }

            if (ComboType.SelectedIndex == 0)
            {
                currentShoes = currentShoes.Where(p => ((p.ProductDiscountAmount) >= 0 && (p.ProductDiscountAmount) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentShoes = currentShoes.Where(p => ((p.ProductDiscountAmount) >= 0 && (p.ProductDiscountAmount) < 10)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentShoes = currentShoes.Where(p => ((p.ProductDiscountAmount) >= 10 && (p.ProductDiscountAmount) < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentShoes = currentShoes.Where(p => ((p.ProductDiscountAmount) >= 15 && (p.ProductDiscountAmount) <= 100)).ToList();
            }

            if (RBUp.IsChecked.Value)
            {
                currentShoes = currentShoes.OrderBy(p => p.ProductCost).ToList();
            }
            if (RBDown.IsChecked.Value)
            {
                currentShoes = currentShoes.OrderByDescending(p => p.ProductCost).ToList();
            }

            ShoesListView.ItemsSource = currentShoes;

            TBVivedennieDannie.Text = "кол-во " + currentShoes.Count.ToString();
            TBAll.Text = " из " + Tablelist.Count.ToString();
        }

        private void ShoesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateShoes();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateShoes();
        }

        private void RBUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateShoes();
        }

        private void RBDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateShoes();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateShoes();
        }
    }
}
