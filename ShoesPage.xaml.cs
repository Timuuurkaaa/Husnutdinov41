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
        List<Product> selectedProducts = new List<Product>();
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        int CountRecords;
        private User _currentUser;
        public ShoesPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            if (user == null)
            {
                FIOTB.Text = "вы зашли как гость!";
            }
            else
            {
                FIOTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
                switch (user.UserRole)
                {
                    case 1:
                        RoleTB.Text = "Клиент"; break;
                    case 2:
                        RoleTB.Text = "Менеджер"; break;
                    case 3:
                        RoleTB.Text = "Администратор"; break;
                }
            }
            OrderBtn.Visibility = Visibility.Hidden;
            var currentShoes = Husnutdinov41Entities.GetContext().Product.ToList();
            ShoesListView.ItemsSource = currentShoes;
            ComboType.SelectedIndex = 0;
            UpdateShoes();
            CountRecords = Husnutdinov41Entities.GetContext().Product.ToList().Count;
            TBAll.Text = " из " + CountRecords.ToString();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ShoesListView.SelectedIndex >= 0)
            {
                var prod = ShoesListView.SelectedItem as Product;
                selectedProducts.Add(prod);

                // Проверяем, существует ли уже OrderProduct для этого товара
                var existingOP = selectedOrderProducts.FirstOrDefault(op => op.ProductArticleNumber == prod.ProductArticleNumber);

                if (existingOP == null)
                {
                    // Создаем новый OrderProduct, если его нет
                    var newOrderProd = new OrderProduct
                    {
                        ProductArticleNumber = prod.ProductArticleNumber,
                        ProductCount = 1
                    };
                    selectedOrderProducts.Add(newOrderProd);
                    ////newOrderProd.OrderID = newOrderID;
                }
                else
                {
                    // Увеличиваем количество, если товар уже есть в заказе
                    existingOP.ProductCount++;
                }

                OrderBtn.Visibility = Visibility.Visible;
                ShoesListView.SelectedIndex = -1;

            }
        }

        //private void OrderBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    selectedProducts = selectedProducts.Distinct().ToList();
        //    OrderWindow orderWindow = new OrderWindow(selectedOrderProducts, selectedProducts, _currentUser);
        //    orderWindow.ShowDialog();
        //}

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts = selectedProducts.Distinct().ToList();

            // Добавьте этот код для инициализации Quantity в Product
            foreach (var product in selectedProducts)
            {
                // Находим соответствующий OrderProduct для текущего Product
                var orderProduct = selectedOrderProducts.FirstOrDefault(op =>
                    op.ProductArticleNumber == product.ProductArticleNumber);

                if (orderProduct != null)
                {
                    // Устанавливаем Quantity в Product на основе ProductCount из OrderProduct
                    product.Quantity = orderProduct.ProductCount;
                }
                else
                {
                    // Если OrderProduct не найден (хотя это маловероятно), устанавливаем значение по умолчанию
                    product.Quantity = 1;
                }
            }

            OrderWindow orderWindow = new OrderWindow(selectedOrderProducts, selectedProducts, _currentUser);
            bool? result = orderWindow.ShowDialog();

            // Если заказ успешно сохранен (DialogResult = true)
            if (result == true)
            {
                selectedProducts.Clear();
                selectedOrderProducts.Clear();
                ShoesListView.Items.Refresh(); // Обновить отображение списка
            }

            // Обновить видимость кнопки
            OrderBtn.Visibility = selectedProducts.Any() ? Visibility.Visible : Visibility.Hidden;

            //orderWindow.ShowDialog();

            //// После закрытия окна:
            //if (selectedProducts.Count == 0)
            //{
            //    OrderBtn.Visibility = Visibility.Hidden;
            //}
            //else
            //{
            //    OrderBtn.Visibility = Visibility.Visible;
            //}
        }
    }
}
