using DnD_Item_Manager.View_Model.Items_Tab;
using System.Windows.Controls;

namespace DnD_Item_Manager.View.Items_tab
{
    /// <summary>
    /// Logika interakcji dla klasy MV.xaml
    /// </summary>
    public partial class MV : UserControl
    {
        public MV()
        {
            InitializeComponent();
            DataContext = new MV_VM();
        }
    }
}
