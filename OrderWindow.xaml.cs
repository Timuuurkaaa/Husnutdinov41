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

namespace Husnutdinov41
{
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct = new OrderProduct();
        private User _currentUser;

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, User user)
        {
            InitializeComponent();
            _currentUser = user;
            //DateDeliveryOrder.IsEnabled = false;
            //DateDeliveryOrder.IsEnabled = false;
            if (user != null)
                FIOTB_Order.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
            else
                FIOTB_Order.Text = "";
            var pickuppoints = Husnutdinov41Entities.GetContext().PickUpPoint.ToList().Select(p => p.PickUpPointAddress).ToList();
            PickPointComboBox.ItemsSource = pickuppoints;

            //OrderIDTB.Text = selectedOrderProducts.First().OrderID.ToString();
            int nextOrderID = GetNextOrderID();
            OrderIDTB.Text = nextOrderID.ToString();

            // Инициализируем Quantity для каждого Product
            foreach (Product product in selectedProducts)
            {
                var orderProduct = selectedOrderProducts.FirstOrDefault(op => op.ProductArticleNumber == product.ProductArticleNumber);
                if (orderProduct != null)
                {
                    product.Quantity = orderProduct.ProductCount;
                }
                else
                {
                    product.Quantity = 1; // Если не найден, устанавливаем 1 (опционально)
                }
            }

            ProductOrderListView.ItemsSource = selectedProducts;

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            DateFormOrder.Text = DateTime.Now.ToString();
            SetDeliveryDate();
            CalculateTotalAndDiscount();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (PickPointComboBox.SelectedIndex == -1)
                errors.AppendLine("Выберите пункт выдачи!");
            if (DateFormOrder.SelectedDate.Value == null)
                errors.AppendLine("Введите дату формирования заказа!");
            if (DateDeliveryOrder.SelectedDate.Value == null)
                errors.AppendLine("Введите дату доставки заказа!");
            if (selectedOrderProducts.Count == 0) // Новая проверка
                errors.AppendLine("Добавьте хотя бы один товар в заказ!");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentUser == null)
                currentOrder.OrderClient = null;
            else
                currentOrder.OrderClient = _currentUser.UserID;
            currentOrder.OrderPickupPoint = PickPointComboBox.SelectedIndex + 1;
            currentOrder.OrderDate = DateFormOrder.SelectedDate.Value;
            currentOrder.OrderDeliveryDate = DateDeliveryOrder.SelectedDate.Value;
            currentOrder.OrderStatus = "Новый";
            currentOrder.OrderCode = GenerateUniqueOrderCode();

            Husnutdinov41Entities.GetContext().Order.Add(currentOrder);
            Husnutdinov41Entities.GetContext().SaveChanges();
            OrderIDTB.Text = currentOrder.OrderID.ToString();

            foreach (var op in selectedOrderProducts)
            {
                op.OrderID = currentOrder.OrderID;
                Husnutdinov41Entities.GetContext().OrderProduct.Add(op);
            }
            Husnutdinov41Entities.GetContext().SaveChanges();
            MessageBox.Show($"Заказ №{currentOrder.OrderID} сохранен! Код: {currentOrder.OrderCode}");
            this.DialogResult = true;
            Close();
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

            if (selectedOP != null)
            {
                selectedOP.ProductCount++;
                prod.Quantity = selectedOP.ProductCount;
                SetDeliveryDate();
                CalculateTotalAndDiscount();
                ProductOrderListView.Items.Refresh();
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

            if (selectedOP != null)
            {
                if (selectedOP.ProductCount > 1)
                {
                    selectedOP.ProductCount--;
                    prod.Quantity = selectedOP.ProductCount; // Синхронизируем Quantity
                    SetDeliveryDate();
                    CalculateTotalAndDiscount();
                    ProductOrderListView.Items.Refresh();
                }
                else
                {
                    // Удаляем OrderProduct из списка
                    selectedOrderProducts.Remove(selectedOP);

                    // Находим Product в selectedProducts по артикулу (чтобы избежать проблем с ссылками)
                    var productToRemove = selectedProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
                    if (productToRemove != null)
                    {
                        selectedProducts.Remove(productToRemove);
                    }

                    // Обновляем интерфейс
                    ProductOrderListView.Items.Refresh();
                    // Перепривязываем данные, чтобы обновить интерфейс
                    ProductOrderListView.ItemsSource = null;
                    ProductOrderListView.ItemsSource = selectedProducts;
                    SetDeliveryDate();
                    CalculateTotalAndDiscount();
                    ProductOrderListView.Items.Refresh();
                }
            }
        }

        private void SetDeliveryDate()
        {
            // Если есть в магазине, то через 3 дня доставка, если нет в магазине, то 6 дней доставки
            // Проверяем, достаточно ли товара на складе: (количество в заказе + 3) <= количество на складе
            // Если на складе меньше, чем (количество в заказе + 3), то срок 6 дней.

            bool DeliveryStatus = false;
            foreach (var p in selectedProducts)
            {
                if (p.ProductQuantityInStock < (p.Quantity + 3))
                {
                    DeliveryStatus = true;
                }
            }

            DateTime deliveryDate = DateFormOrder.SelectedDate.Value;
            deliveryDate = DeliveryStatus
                ? deliveryDate.AddDays(6) // Если есть товары <3 шт. → +6 дней
                : deliveryDate.AddDays(3); // В противном случае → +3 дня
            DateDeliveryOrder.SelectedDate = deliveryDate;
        }


        private void DateFormOrder_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDeliveryDate();
        }

        private int GenerateUniqueOrderCode()
        {
            var context = Husnutdinov41Entities.GetContext();
            Random random = new Random();
            int code;
            do
            {
                code = random.Next(100, 1000);
            } while (context.Order.Any(o => o.OrderCode == code));

            return code;
        }
        private int GetNextOrderID()
        {
            var sqlCommand = "SELECT IDENT_CURRENT('Order')";
            var nextID = Husnutdinov41Entities.GetContext().Database.SqlQuery<decimal>(sqlCommand).FirstOrDefault() + 1;
            return (int)nextID;
        }

        private void CalculateTotalAndDiscount()
        {
            decimal total = 0;
            decimal discount = 0;

            foreach (var orderProduct in selectedOrderProducts)
            {
                var product = selectedProducts.FirstOrDefault(p => p.ProductArticleNumber == orderProduct.ProductArticleNumber);
                if (product == null) continue;

                decimal price = product.ProductCost;
                decimal discountPercent = product.ProductDiscountAmount ?? 0;

                // Сумма без скидки
                total += orderProduct.ProductCount * price;

                // Сумма скидки для текущего товара
                discount += orderProduct.ProductCount * price * (discountPercent / 100);
            }

            // Итоговая сумма с учётом скидки
            decimal discountedTotal = total - discount;

            // Обновление отображения
            TotalAmountTB.Text = total.ToString("N0") + " ₽ ";
            TotalDiscountAmountTB.Text = discount.ToString("N0") + " ₽ ";
            DiscountAmountTB.Text = discountedTotal.ToString("N0") + " ₽";
        }
    }
}
