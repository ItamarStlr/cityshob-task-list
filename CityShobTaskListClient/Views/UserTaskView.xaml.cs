using CityShobTaskListClient.ViewModels;
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

namespace CityShobTaskListClient.Views
{
    /// <summary>
    /// Interaction logic for UserTaskView.xaml
    /// </summary>
    public partial class UserTaskView : UserControl
    {
        public UserTaskView()
        {
            InitializeComponent();
        }

        private void EditUserTask_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Cast DataContext and invoke your method
            if (DataContext is UserTaskViewModel vm)
            {
                vm.BeginSaveDescription();
            }
        }
    }
}
