using System.Collections.Generic;
using System.Windows.Input;
using DnD_Item_Manager.DAL;
using DnD_Item_Manager.Model;
using MySql.Data.MySqlClient;

namespace DnD_Item_Manager.View_Model.Items_Tab
{
    // Dostosowana do tabelii klasa
    internal class Tool_VM : PropertyChangedNotify
    {
        private SQLBasicDataCombo _tool;

        // Zmienne unikatowe do klasy tabeli
        private string _itemName;
        private int _itemWeight;
        private string _itemGroup;
        private string _itemDesc;
        private List<string> _toolGroupList = new List<string>() { "", "Artisan", "Game", "Music"};

        private ICommand _updateEditCommand;
        private ICommand _newToolCommand;
        private ICommand _saveToolCommand;
        #region Właściwości
        public SQLBasicDataCombo Tool
        {
            get
            {
                if (_tool == null)
                    Tool = new SQLBasicDataCombo("tool_view");
                return _tool;
            }
            set
            {
                if (_tool != value)
                {
                    _tool = value;
                    OnPropertyChanged("Tool");
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

        public List<string> ToolGroupList
        {
            get { return _toolGroupList; }
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

        public ICommand NewToolCommand
        {
            get
            {
                if (_newToolCommand == null)
                    _newToolCommand = new RelayCommand(param => NewTool());
                return _newToolCommand;
            }
        }

        public ICommand SaveToolCommand
        {
            get
            {
                if (_saveToolCommand == null)
                    _saveToolCommand = new RelayCommand(param => SaveTool());
                return _saveToolCommand;
            }
        }
        #endregion

        #region Funkcje

        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_tool.Selected != null)
            {

                ItemName = _tool.Selected["Name"].ToString();
                ItemWeight = int.Parse(_tool.Selected["Weight"].ToString());
                ItemGroup = _tool.Selected["Group"].ToString();
                ItemDesc = _tool.Selected["Description"].ToString();
            }
        }

        private void NewTool()
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
                cmd.CommandText = $"INSERT INTO tool (item_id, tool_group) VALUE ({itemId},{ItemGroup})";
                cmd.ExecuteNonQuery();
                ItemGroup = tmpGroup;
                Conn.Close();
            }
            _tool.UpdateMain();
        }

        private void SaveTool()
        {
            if (_tool.Selected != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"UPDATE item SET name=\"{ItemName}\", weight={ItemWeight},description=\"{ItemDesc}\",type=\"Armor\" WHERE id={_tool.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    string tmpGroup = ItemGroup;
                    if (ItemGroup == "")
                        ItemGroup = "NULL";
                    else
                        ItemGroup = $"\"{ItemGroup}\"";
                    cmd.CommandText = $"UPDATE tool SET tool_group={ItemGroup} WHERE item_id={_tool.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    ItemGroup = tmpGroup;
                    Conn.Close();
                }
                _tool.UpdateMain();
            }
        }
        #endregion
    }
}
