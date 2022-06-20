using System.Collections.Generic;
using System.Windows.Input;
using DnD_Item_Manager.DAL;
using DnD_Item_Manager.Model;
using MySql.Data.MySqlClient;

namespace DnD_Item_Manager.View_Model.Items_Tab
{
    // Dostosowana do tabelii klasa
    internal class AG_VM : PropertyChangedNotify
    {
        
        private SQLBasicDataCombo _aG;
        // Zmienne unikatowe do klasy tabeli
        private string _itemName;
        private int _itemWeight;
        private string _itemGroup;
        private int _itemCount;
        private string _itemDesc;
        private List<string> _aGGroupList = new List<string>() { "", "Ammuntion", "Mage focus", "Druid focus", "Holy symbol", "Container" };

        private ICommand _updateEditCommand;
        private ICommand _newAGCommand;
        private ICommand _saveAGCommand;
        #region Właściwości
        public SQLBasicDataCombo AG
        {
            get
            {
                if (_aG == null)
                    AG = new SQLBasicDataCombo("ag_view"); // Uzupełnia odpowiednio dane przy pierwszym wyborze
                return _aG;
            }
            set
            {
                if (_aG != value)
                {
                    _aG = value;
                    OnPropertyChanged("AG");
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
                    _itemName = value.Replace("'", "").Replace("\"", "").Replace("(","").Replace(")","");
                    OnPropertyChanged("ItemName");
                }
            }
        }

        public int ItemWeight
        {
            get { return _itemWeight; }
            set
            {
                if (_itemWeight != value)
                {
                    // Pilnuje aby nie wyjść poza granice.
                    if (value > 65534)
                        value = 65534;
                    else if (value < 0)
                        value = 0;
                    _itemWeight = value;
                    OnPropertyChanged("ItemWeight");
                }
            }
        }

        public string ItemGroup
        {
            get { return _itemGroup; }
            set
            {
                if (value != _itemGroup)
                {
                    _itemGroup = value;
                    OnPropertyChanged("ItemGroup");
                }
            }
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                // Pilnuje aby nie wyjść poza granice.
                if (value > 65534)
                    value = 65534;
                else if (value < 0)
                    value = 0;
                if (value != _itemCount)
                {
                    _itemCount = value;
                    OnPropertyChanged("ItemCount");
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

        public List<string> AGGroupList
        {
            get { return _aGGroupList; }
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

        public ICommand NewAGCommand
        {
            get
            {
                if (_newAGCommand == null)
                    _newAGCommand = new RelayCommand(param => NewAG());
                return _newAGCommand;
            }
        }

        public ICommand SaveAGCommand
        {
            get
            {
                if (_saveAGCommand == null)
                    _saveAGCommand = new RelayCommand(param => SaveAG());
                return _saveAGCommand;
            }
        }
        #endregion

        #region Fun

        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_aG.Selected != null)
            {

                ItemName = _aG.Selected["Name"].ToString();
                ItemWeight = int.Parse(_aG.Selected["Weight"].ToString());
                ItemGroup = _aG.Selected["Group"].ToString();
                ItemCount = int.Parse(_aG.Selected["Count"].ToString());
                ItemDesc = _aG.Selected["Description"].ToString();
            }
        }

        // Tworzy nowy AG(Adventurer gear)
        private void NewAG()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;
                string tmpGroup = ItemGroup;
                if (ItemGroup == null | ItemGroup == "")
                    ItemGroup = "NULL";
                else
                    ItemGroup = $"\"{ItemGroup}\"";

                cmd.CommandText = $"INSERT INTO item (name, weight, description, type) VALUE (\"{ItemName}\",{ItemWeight},\"{ItemDesc}\",\"Adventure gear\")";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                string itemId;
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    itemId = reader.GetValue(0).ToString();
                }
                cmd.CommandText = $"INSERT INTO adventure_gear (item_id, count, gear_group) VALUE ({itemId},{ItemCount},{ItemGroup})";
                cmd.ExecuteNonQuery();
                ItemGroup = tmpGroup;
                Conn.Close();
            }
            _aG.UpdateMain();
        }

        //Edytuje wybrany AG na podstawie elementów edytowalnych
        private void SaveAG()
        {
            if (_aG.Selected != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"UPDATE item SET name=\"{ItemName}\", weight={ItemWeight},description=\"{ItemDesc}\",type=\"Armor\" WHERE id={_aG.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    string tmpGroup = ItemGroup;
                    if (ItemGroup == "")
                        ItemGroup = "NULL";
                    else
                        ItemGroup = $"\"{ItemGroup}\"";
                    cmd.CommandText = $"UPDATE adventure_gear SET gear_group={ItemGroup}, count={ItemCount} WHERE item_id={_aG.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    ItemGroup = tmpGroup;
                    Conn.Close();
                }
                _aG.UpdateMain();
            }
        }
        #endregion
    }
}
