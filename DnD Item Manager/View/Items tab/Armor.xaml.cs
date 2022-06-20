using System.Windows.Controls;
using DnD_Item_Manager.View_Model.Items_Tab;

namespace DnD_Item_Manager.View.Items_tab
{
    /// <summary>
    /// Logika interakcji dla klasy Armor.xaml
    /// </summary>
    public partial class Armor : UserControl
    {
        public Armor()
        {
            InitializeComponent();
            DataContext = new Armor_VM();
        }
    }
}
