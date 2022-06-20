using System.Windows.Controls;
using DnD_Item_Manager.View_Model.Items_Tab;

namespace DnD_Item_Manager.View.Items_tab
{
    /// <summary>
    /// Logika interakcji dla klasy AG.xaml
    /// </summary>
    public partial class AG : UserControl
    {
        public AG()
        {
            InitializeComponent();
            DataContext = new AG_VM();
        }
    }
}
