using DnD_Item_Manager.View_Model.Others_Tab;
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

namespace DnD_Item_Manager.View.OthersTab
{
    /// <summary>
    /// Logika interakcji dla klasy DiceManagerv.xaml
    /// </summary>
    public partial class DiceManagerv : UserControl
    {
        public DiceManagerv()
        {
            InitializeComponent();
            DataContext = new DiceManagerVM();
        }
    }
}
