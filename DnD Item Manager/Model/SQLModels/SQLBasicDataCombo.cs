using DnD_Item_Manager.View_Model;
using DnD_Item_Manager.DAL;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Input;

namespace DnD_Item_Manager.Model
{
    internal class SQLBasicDataCombo : PropertyChangedNotify
    {
        // Wybrany wiersz
        protected DataRowView _selected;
        // Wybrana właściwość
        protected DataRowView _selectedProp;
        // Wybrana nie posiadane przez przedmiot właściwość
        protected DataRowView _selectedLeftoverProp;
        // Tabela przedmiotów
        protected DataView _itemDGS;
        // Tabela właściwości przedmiotu
        protected DataView _itemPropertiesDGS;
        // Tabela nie posiadanych przez przedmiot właściwości
        protected DataView _leftoverProperties;
        // Nazwa tabeli z której są pobierane dane
        protected string _mainTable;
        // Komenda usuwająca przedmiot
        private ICommand _deleteItemCommand;
        // komenda usuwająca wybraną właściwość
        private ICommand _deletePropertyCommand;
        // Komenda dodająca wybraną, nie posiadaną właściwość
        private ICommand _addPropertyCommand;

        #region Properties
        public DataRowView Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged("SelectedItem");
                    // Wywołuje funkcje Update Properties aby tabe;a posiadała aktualne dane
                    UpdateProperties();
                }
            }
        }

        public DataRowView SelectedProp
        {
            get { return _selectedProp; }
            set
            {
                if (_selectedProp != value)
                {
                    _selectedProp = value;
                    OnPropertyChanged("SelectedProp");
                }
            }
        }

        public DataRowView SelectedLeftoverProp
        {
            get { return _selectedLeftoverProp; }
            set
            {
                if (_selectedLeftoverProp != value)
                {
                    _selectedLeftoverProp = value;
                    OnPropertyChanged("SelectedLeftoverProp");
                }
            }
        }

        public DataView ItemDGS
        {
            get { return _itemDGS; }
            set
            {
                if (_itemDGS != value)
                {
                    _itemDGS = value;
                    OnPropertyChanged("ItemDGS");
                }
            }
        }

        public DataView ItemPropertiesDGS
        {
            get { return _itemPropertiesDGS; }
            set
            {
                if (_itemPropertiesDGS != value)
                {
                    _itemPropertiesDGS = value;
                    OnPropertyChanged("ItemPropertiesDGS");
                }
            }
        }

        public DataView LeftoverProperties
        {
            get { return _leftoverProperties; }
            set
            {
                if (_leftoverProperties != value)
                {
                    _leftoverProperties = value;
                    OnPropertyChanged("LeftoverProperties");
                }
            }
        }
        #endregion

        #region ICommand

        public ICommand DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                    _deleteItemCommand = new RelayCommand(param => DeleteItem());
                return _deleteItemCommand;
            }
        }



        public ICommand DeletePropertyCommand
        {
            get
            {
                if (_deletePropertyCommand == null)
                    _deletePropertyCommand = new RelayCommand(param => DeleteProperty());
                return _deletePropertyCommand;
            }
        }

        public ICommand AddPropertyCommand
        {
            get
            {
                if (_addPropertyCommand == null)
                    _addPropertyCommand = new RelayCommand(param => AddProperty());
                return _addPropertyCommand;
            }
        }

        #endregion

        #region Konstruktory

        public SQLBasicDataCombo(string tableName)
        {
            _mainTable = tableName;
            // Wypełnia tabele po raz pierwszy
            UpdateMain();
        }
        #endregion

        #region Funkcje

        //Odświerza główną tabele
        public void UpdateMain()
        {
            DataTable dt = new DataTable();
            // Wykorzystuje MySqlConnection w DBConnection aby połączyć z serwerem MySQL
            try
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    // Wybiera wszystkie kolumny i wiersze z konkretnej tabeli
                    string cmdStr = $"SELECT * from {_mainTable}";
                    using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                        da.Fill(dt);
                    //Wypełnia ITEMDGS wynikowymi danymi
                    ItemDGS = dt.DefaultView;
                    Conn.Close();
                }
            }
            catch { }
        }

        // Odświerza właściwości
        protected void UpdateProperties()
        {
            // Musi być wybrany element listy przedmiotrów aby przeprowadzić pozostałe funkcje
            if (_selected != null)
            {
                DataTable dt = new DataTable();
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    // Wybiera Nazwe, opis i identyfikatory z widoku 'property_view'
                    string cmdStr = $"SELECT Name, Description, item_id, property_id FROM property_view WHERE item_id={_selected["ID"]};";
                    using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                        da.Fill(dt);
                    ItemPropertiesDGS = dt.DefaultView;
                    Conn.Close();
                }
                dt = new DataTable();
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    // Wybiera nazwe, opis i id z właściwości dla właściwości nie posiadanych przez wybrany element
                    string cmdStr = $"SELECT name, description, id FROM property WHERE id NOT IN (Select property_id from item_property where item_id = {_selected["ID"]});";
                    using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                        da.Fill(dt);
                    LeftoverProperties = dt.DefaultView;
                    Conn.Close();
                }
            }
        }

        //Usuwa wybrany przedmiot
        private void DeleteItem()
        {
            if (Selected != null)
            {
                try
                {
                    using (MySqlConnection Conn = DBConnection.Instance.Connection)
                    {
                        Conn.Open();
                        using var cmd = new MySqlCommand();
                        cmd.Connection = Conn;
                        cmd.CommandText = $"DELETE FROM item WHERE id={Selected["ID"]}";
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                } catch { }
                UpdateMain();
            }
        }

        //Usuwa wybrana właściwość przedmiotu
        private void DeleteProperty()
        {
            if (Selected != null & SelectedProp != null)
            {
                try
                {
                    using (MySqlConnection Conn = DBConnection.Instance.Connection)
                    {
                        Conn.Open();
                        using var cmd = new MySqlCommand();
                        cmd.Connection = Conn;
                        cmd.CommandText = $"DELETE FROM item_property WHERE property_id={SelectedProp["property_id"]} AND item_id={_selected["ID"]}";
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                } catch { }
                UpdateProperties();
            }
        }

        // Dodaje właściwość do przedmiotu
        private void AddProperty()
        {
            if (Selected != null & SelectedLeftoverProp != null)
            {
                try
                {
                    using (MySqlConnection Conn = DBConnection.Instance.Connection)
                    {
                        Conn.Open();
                        using var cmd = new MySqlCommand();
                        cmd.Connection = Conn;
                        cmd.CommandText = $"INSERT INTO item_property VALUE ({_selected["ID"]}, {_selectedLeftoverProp["id"]});";
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                } catch { }
                UpdateProperties();
            }
        }

        #endregion
    }
}
