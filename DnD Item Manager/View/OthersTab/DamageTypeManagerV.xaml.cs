using DnD_Item_Manager.View_Model.Others_Tab;
using System.Windows.Controls;

namespace DnD_Item_Manager.View.OthersTab
{
    /// <summary>
    /// Logika interakcji dla klasy DamageTypeManagerV.xaml
    /// </summary>
    public partial class DamageTypeManagerV : UserControl
    {
        public DamageTypeManagerV()
        {
            InitializeComponent();
            DataContext = new DamageTypeManagerVM();
        }
    }
}
