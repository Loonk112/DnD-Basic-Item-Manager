using DnD_Item_Manager.DAL;
using DnD_Item_Manager.Model;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows.Input;

namespace DnD_Item_Manager.View_Model.Items_Tab
{
    // Dostosowana do tabelii klasa
    internal class Other_VM : PropertyChangedNotify
    {
        private SQLBasicDataCombo _other;

        // Zmienne unikatowe do klasy tabeli
        private string _itemName;
        private int _itemWeight;
        private string _itemDesc;

        private ICommand _updateEditCommand;
        private ICommand _newOtherCommand;
        private ICommand _saveOtherCommand;
        #region Właściwości
        public SQLBasicDataCombo Other
        {
            get
            {
                if (_other == null)
                    Other = new SQLBasicDataCombo("other_view");
                return _other;
            }
            set
            {
                if (_other != value)
                {
                    _other = value;
                    OnPropertyChanged("Other");
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

        public ICommand NewOtherCommand
        {
            get
            {
                if (_newOtherCommand == null)
                    _newOtherCommand = new RelayCommand(param => NewOther());
                return _newOtherCommand;
            }
        }

        public ICommand SaveOtherCommand
        {
            get
            {
                if (_saveOtherCommand == null)
                    _saveOtherCommand = new RelayCommand(param => SaveOther());
                return _saveOtherCommand;
            }
        }
        #endregion

        #region Funkcje

        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_other.Selected != null)
            {

                ItemName = _other.Selected["Name"].ToString();
                ItemWeight = int.Parse(_other.Selected["Weight"].ToString());
                ItemDesc = _other.Selected["Description"].ToString();
            }
        }

        private void NewOther()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandText = $"INSERT INTO item (name, weight, description, type) VALUE (\"{ItemName}\",{ItemWeight},\"{ItemDesc}\",\"Other\")";
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            _other.UpdateMain();
        }

        private void SaveOther()
        {
            if (_other.Selected != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;
                    cmd.CommandText = $"UPDATE item SET name=\"{ItemName}\", weight={ItemWeight},description=\"{ItemDesc}\",type=\"Other\" WHERE id={_other.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                _other.UpdateMain();
            }
        }
        #endregion
    }
}