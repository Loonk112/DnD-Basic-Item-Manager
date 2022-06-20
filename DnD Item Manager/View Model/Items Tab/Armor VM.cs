using DnD_Item_Manager.DAL;
using DnD_Item_Manager.Model;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows.Input;

namespace DnD_Item_Manager.View_Model.Items_Tab
{
    // Dostosowana do tabelii klasa
    internal class Armor_VM : PropertyChangedNotify
    {
        private SQLBasicDataCombo _armorDG;

        // Zmienne unikatowe do klasy tabeli
        private string _itemName;
        private int _itemWeight;
        private string _itemClass;
        private int _itemAC;
        private string _itemDesc;
        private List<string> _armorClassList = new List<string>() { "", "Light", "Medium", "Heavy" };

        private ICommand _updateEditCommand;
        private ICommand _newArmorCommand;
        private ICommand _saveArmorCommand;
        #region Właściwości
        public SQLBasicDataCombo ArmorDG
        {
            get
            {
                if (_armorDG == null)
                    ArmorDG = new SQLBasicDataCombo("armor_view");// Uzupełnia odpowiednio dane przy pierwszym wyborze
                return _armorDG;
            }
            set
            {
                if (_armorDG != value)
                {
                    _armorDG = value;
                    OnPropertyChanged("ArmorDG");
                }

            }
        }

        public string ItemName
        {
            get { return _itemName; }
            set
            {
                if (value != _itemName)
                {
                    // Pozbywa się ' i " które mogłyby wpłynąć na funkcje
                    _itemName = value.Replace("'","").Replace("\"","");
                    OnPropertyChanged("ItemName");
                }
            }
        }

        public int ItemWeight
        {
            get { return _itemWeight; }
            set
            {
                // Pilnuje aby nie wyjść poza granice.
                if (value > 65534)
                    value = 65534;
                else if (value < 0)
                    value = 0;
                if (_itemWeight != value)
                {
                    _itemWeight = value;
                    OnPropertyChanged("ItemWeight");
                }
            }
        }

        public string ItemClass
        {
            get { return _itemClass; }
            set
            {
                if (value != _itemClass)
                {
                    _itemClass = value;
                    OnPropertyChanged("ItemClass");
                }
            }
        }

        public int ItemAC
        {
            get { return _itemAC; }
            set
            {
                // Pilnuje aby nie wyjść poza granice.
                if (value > 65534)
                    value = 65534;
                else if (value < 0)
                    value = 0;
                if (value != _itemAC)
                {
                    _itemAC = value;
                    OnPropertyChanged("ItemAC");
                }
            }
        }

        public string ItemDesc
        {
            get { return _itemDesc; }
            set
            {
                if ((value != _itemDesc))
                {
                    // Pozbywa się ' i " które mogłyby wpłynąć na funkcje
                    _itemDesc = value.Replace("'", "").Replace("\"", "").Replace("(","").Replace(")","");
                    OnPropertyChanged("ItemDesc");
                }
            }
        }

        public List<string> ArmorClassList {
            get { return _armorClassList; }
        }
        #endregion

        #region Commands
        public ICommand UpdateEditCommand
        {
            get
            {
                if (_updateEditCommand == null)
                    _updateEditCommand = new RelayCommand(param => UpdateEdit());
                return _updateEditCommand;
            }
        }

        public ICommand NewArmorCommand
        {
            get
            {
                if (_newArmorCommand == null)
                    _newArmorCommand = new RelayCommand(param => NewArmor());
                return _newArmorCommand;
            }
        }

        public ICommand SaveArmorCommand
        {
            get
            {
                if (_saveArmorCommand == null)
                    _saveArmorCommand = new RelayCommand(param => SaveArmor());
                return _saveArmorCommand;
            }
        }
        #endregion

        #region Funkcje

        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_armorDG.Selected != null)
            {

                ItemName = _armorDG.Selected["Name"].ToString();
                ItemWeight = int.Parse(_armorDG.Selected["Weight"].ToString());
                ItemClass = _armorDG.Selected["Class"].ToString();
                ItemAC = int.Parse(_armorDG.Selected["AC"].ToString());
                ItemDesc = _armorDG.Selected["Description"].ToString();
            }
        }

        // Tworzy nową zbroje
        private void NewArmor()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;
                string tmpClass = ItemClass;
                if (ItemClass == null | ItemClass == "")
                    ItemClass = "NULL";
                else
                    ItemClass = $"\"{ItemClass}\"";
                
                cmd.CommandText = $"INSERT INTO item (name, weight, description, type) VALUE (\"{ItemName}\",{ItemWeight},\"{ItemDesc}\",\"Armor\")";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                string itemId;
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    itemId = reader.GetValue(0).ToString();
                }
                cmd.CommandText = $"INSERT INTO armor (item_id, class, ac) VALUE ({itemId},{ItemClass},{ItemAC})";
                cmd.ExecuteNonQuery();
                ItemClass = tmpClass;
                Conn.Close();
            }
            //Odświerzenie głównej listy
            _armorDG.UpdateMain();
        }

        //Edytuje wybraną zbroje
        private void SaveArmor()
        {
            if (_armorDG.Selected != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"UPDATE item SET name=\"{ItemName}\", weight={ItemWeight},description=\"{ItemDesc}\",type=\"Armor\" WHERE id={_armorDG.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    string tmpClass = ItemClass;
                    if (ItemClass == "")
                        ItemClass = "NULL";
                    else
                        ItemClass = $"\"{ItemClass}\"";
                    cmd.CommandText = $"UPDATE armor SET class={ItemClass}, ac={ItemAC} WHERE item_id={_armorDG.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    ItemClass = tmpClass;
                    Conn.Close();
                }
                //Odświerza liste
                _armorDG.UpdateMain();
            }
        }
        #endregion
    }
}