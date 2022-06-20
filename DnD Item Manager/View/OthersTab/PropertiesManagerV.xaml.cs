using DnD_Item_Manager.View_Model.Others_Tab;
using System.Windows.Controls;

namespace DnD_Item_Manager.View.OthersTab
{
    /// <summary>
    /// Logika interakcji dla klasy PropertiesManagerV.xaml
    /// </summary>
    public partial class PropertiesManagerV : UserControl
    {
        public PropertiesManagerV()
        {
            InitializeComponent();
            DataContext = new PropertiesManagerVM();
        }
    }
}
