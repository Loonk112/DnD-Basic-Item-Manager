using DnD_Item_Manager.View_Model.Items_Tab;
using System.Windows.Controls;

namespace DnD_Item_Manager.View.Items_tab
{
    /// <summary>
    /// Logika interakcji dla klasy Tool.xaml
    /// </summary>
    public partial class Tool : UserControl
    {
        public Tool()
        {
            InitializeComponent();
            DataContext = new Tool_VM();
        }
    }
}
