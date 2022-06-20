using DnD_Item_Manager.DAL;
using DnD_Item_Manager.Model;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows.Input;

namespace DnD_Item_Manager.View_Model.Items_Tab
{
    // Dostosowana do tabelii klasa
    internal class MV_VM : PropertyChangedNotify
    {
        private SQLBasicDataCombo _mV;

        // Zmienne unikatowe do klasy tabeli
        private string _itemName;
        private int _itemWeight;
        private int _itemSpeed;
        private int _itemCarryweight;
        private string _itemDesc;

        private ICommand _updateEditCommand;
        private ICommand _newMVCommand;
        private ICommand _saveMVCommand;
        #region Właściwości
        public SQLBasicDataCombo MV
        {
            get
            {
                if (_mV == null)
                    _mV = new SQLBasicDataCombo("mv_view");
                return _mV;
            }
            set
            {
                if (_mV != value)
                {
                    _mV = value;
                    OnPropertyChanged("MV");
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

        public int ItemSpeed
        {
            get { return _itemSpeed; }
            set
            {
                // Pilnuje aby nie wyjść poza granice.
                if (value > 65534)
                    value = 65534;
                else if (value < 0)
                    value = 0;
                if (value != _itemSpeed)
                {
                    _itemSpeed = value;
                    OnPropertyChanged("ItemSpeed");
                }
            }
        }

        public int ItemCarryweight
        {
            get { return _itemCarryweight; }
            set
            {
                // Pilnuje aby nie wyjść poza granice.
                if (value > 65534)
                    value = 65534;
                else if (value < 0)
                    value = 0;
                if (value != _itemCarryweight)
                {
                    _itemCarryweight = value;
                    OnPropertyChanged("ItemCarryweight");
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

        public ICommand NewMVCommand
        {
            get
            {
                if (_newMVCommand == null)
                    _newMVCommand = new RelayCommand(param => NewMV());
                return _newMVCommand;
            }
        }

        public ICommand SaveMVCommand
        {
            get
            {
                if (_saveMVCommand == null)
                    _saveMVCommand = new RelayCommand(param => SaveMV());
                return _saveMVCommand;
            }
        }
        #endregion

        #region Funkcje

        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_mV.Selected != null)
            {

                ItemName = _mV.Selected["Name"].ToString();
                ItemWeight = int.Parse(_mV.Selected["Weight"].ToString());
                ItemSpeed = int.Parse(_mV.Selected["Speed"].ToString());
                ItemCarryweight = int.Parse(_mV.Selected["Carryweight"].ToString());
                ItemDesc = _mV.Selected["Description"].ToString();
            }
        }

        private void NewMV()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandText = $"INSERT INTO item (name, weight, description, type) VALUE (\"{ItemName}\",{ItemWeight},\"{ItemDesc}\",\"Mount or vehicle\")";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                string itemId;
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    itemId = reader.GetValue(0).ToString();
                }
                cmd.CommandText = $"INSERT INTO mount_or_vehicle (item_id, speed, carryweight) VALUE ({itemId},{ItemSpeed},{ItemCarryweight})";
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            _mV.UpdateMain();
        }

        private void SaveMV()
        {
            if (_mV.Selected != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"UPDATE item SET name=\"{ItemName}\", weight={ItemWeight},description=\"{ItemDesc}\",type=\"Armor\" WHERE id={_mV.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"UPDATE mount_or_vehicle SET speed={ItemSpeed}, carryweight={ItemCarryweight} WHERE item_id={_mV.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                _mV.UpdateMain();
            }
        }
        #endregion
    }
}