using DnD_Item_Manager.DAL;
using DnD_Item_Manager.Model;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows.Input;

namespace DnD_Item_Manager.View_Model.Items_Tab
{
    // Dostosowana do tabelii klasa
    internal class Currency_VM : PropertyChangedNotify
    {
        private SQLBasicDataCombo _currency;

        // Zmienne unikatowe do klasy tabeli
        private string _itemName;
        private int _itemWeight;
        private int _itemValue;
        private string _itemDesc;

        private ICommand _updateEditCommand;
        private ICommand _newCurrencyCommand;
        private ICommand _saveCurrencyCommand;
        #region Właściwości
        public SQLBasicDataCombo Currency
        {
            get
            {
                if (_currency == null)
                    Currency = new SQLBasicDataCombo("currency_view");
                return _currency;
            }
            set
            {
                if (_currency != value)
                {
                    _currency = value;
                    OnPropertyChanged("Currency");
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
                if (_itemWeight != value & value < 65535)
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

        public int ItemValue
        {
            get { return _itemValue; }
            set
            {
                // Pilnuje aby nie wyjść poza granice.
                if (value > 65534)
                    value = 65534;
                else if (value < 0)
                    value = 0;
                if (value != _itemValue)
                {
                    _itemValue = value;
                    OnPropertyChanged("ItemValue");
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

        public ICommand NewCurrencyCommand
        {
            get
            {
                if (_newCurrencyCommand == null)
                    _newCurrencyCommand = new RelayCommand(param => NewCurrency());
                return _newCurrencyCommand;
            }
        }

        public ICommand SaveCurrencyCommand
        {
            get
            {
                if (_saveCurrencyCommand == null)
                    _saveCurrencyCommand = new RelayCommand(param => SaveCurrency());
                return _saveCurrencyCommand;
            }
        }
        #endregion

        #region Fun

        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_currency.Selected != null)
            {

                ItemName = _currency.Selected["Name"].ToString();
                ItemWeight = int.Parse(_currency.Selected["Weight"].ToString());
                ItemValue = int.Parse(_currency.Selected["Value"].ToString());
                ItemDesc = _currency.Selected["Description"].ToString();
            }
        }

        private void NewCurrency()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandText = $"INSERT INTO item (name, weight, description, type) VALUE (\"{ItemName}\",{ItemWeight},\"{ItemDesc}\",\"Armor\")";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                string itemId;
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    itemId = reader.GetValue(0).ToString();
                }
                cmd.CommandText = $"INSERT INTO currency (item_id, value) VALUE ({itemId}, {ItemValue})";
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            _currency.UpdateMain();
        }

        private void SaveCurrency()
        {
            if (_currency.Selected != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"UPDATE item SET name=\"{ItemName}\", weight={ItemWeight},description=\"{ItemDesc}\",type=\"Armor\" WHERE id={_currency.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"UPDATE currency SET value={ItemValue} WHERE item_id={_currency.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                _currency.UpdateMain();
            }
        }
        #endregion
    }
}