using DnD_Item_Manager.View_Model.Items_Tab;
using System.Windows.Controls;

namespace DnD_Item_Manager.View.Items_tab
{
    /// <summary>
    /// Logika interakcji dla klasy Currency.xaml
    /// </summary>
    public partial class Currency : UserControl
    {
        public Currency()
        {
            InitializeComponent();
            DataContext = new Currency_VM();
        }
    }
}
