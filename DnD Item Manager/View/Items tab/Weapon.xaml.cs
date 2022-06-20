using DnD_Item_Manager.View_Model.Items_Tab;
using System.Windows.Controls;

namespace DnD_Item_Manager.View.Items_tab
{
    /// <summary>
    /// Logika interakcji dla klasy Weapons.xaml
    /// </summary>
    public partial class Weapon : UserControl
    {
        public Weapon()
        {
            InitializeComponent();
            DataContext = new Weapon_VM();
        }
    }
}
