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
using TrainCommon;
using TrainCommon.Model;

namespace _12306BySelfService.SubPages
{
    /// <summary>
    /// MyContract.xaml 的交互逻辑
    /// </summary>
    public partial class MyContract : Page
    {
        public MyContract()
        {
            InitializeComponent();
            this.Loaded += MyContract_Loaded;
        }

        private void MyContract_Loaded(object sender, RoutedEventArgs e)
        {
            List<Passenger> passengerList = SystemCache.GetCache().GetObjByKey(Constant.PassengerList) as List<Passenger>;
            if (passengerList == null) return;
            foreach (var item in passengerList)
            {
                item.PassengerIDNo = StringHelper.ReplaceWithSpecialChar(item.PassengerIDNo, 6, 4, '*');
            }
            this.dataGridContracts.ItemsSource = passengerList;
        }

        //光标右键事件
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("我们还在完善中", "信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        //删除
        private void btnDelContract_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("对不起，该功能我们还在完善中请关注", "信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        //全选关注
        private void CheckBoxAllFocus_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in dataGridContracts.Items)
            {
                DataGridTemplateColumn templeColumn = dataGridContracts.Columns[0] as DataGridTemplateColumn;
                FrameworkElement fwElement = dataGridContracts.Columns[0].GetCellContent(item);

                if (fwElement != null)
                {
                    CheckBox cBox = templeColumn.CellTemplate.FindName("cbitem", fwElement) as CheckBox;
                    if (cBox.IsChecked == true)
                    {
                        cBox.IsChecked = false;
                    }
                    else
                    {
                        cBox.IsChecked = cBox != null;
                    }
                }

            }
        }
    }
}
