using DnD_Item_Manager.View_Model.Items_Tab;
using System.Windows.Controls;

namespace DnD_Item_Manager.View.Items_tab
{
    /// <summary>
    /// Logika interakcji dla klasy Other.xaml
    /// </summary>
    public partial class Other : UserControl
    {
        public Other()
        {
            InitializeComponent();
            DataContext = new Other_VM();
        }
    }
}
