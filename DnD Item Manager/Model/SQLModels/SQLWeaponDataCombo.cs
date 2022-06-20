using DnD_Item_Manager.DAL;
using DnD_Item_Manager.View_Model;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Input;

namespace DnD_Item_Manager.Model.SQLModels
{
    internal class SQLWeaponDataCombo : SQLBasicDataCombo
    {
        //Lista obrażeń
        private DataView _damageDGS;
        //Wybrane obraŻENIE
        private DataRowView _selectedDamage;
        //Wybrany typ obrażeń
        private DataRowView _selectedDamageType;
        // Wartość obrażeń
        private int _damageValue;
        //Wybrana kość
        private DataRowView _selectedDice;
        // Lista typów opvbrażeń
        private DataView _damageTypes;
        // Lista Kości
        private DataView _dices;
        // Komenda dodania obrażeń
        private ICommand _addDamageCommand;
        // Komenda usunięcie obrażeń
        private ICommand _deleteDamageCommand;

        #region Właściwości

        public DataView DamageDGS
        {
            get { return _damageDGS; }
            set
            {
                if (_damageDGS != value)
                {
                    _damageDGS = value;
                    OnPropertyChanged("DamageDGS");
                }
            }
        }

        public DataRowView SelectedDamage
        {
            get { return _selectedDamage; }
            set
            {
                if (value != _selectedDamage)
                {
                    _selectedDamage = value;
                    OnPropertyChanged("SelectedDamage");
                }
            }
        }

        #region Override
        // Nadpisanie 'Selected' z klasy bazowej
        public new DataRowView Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    //stare
                    _selected = value;
                    OnPropertyChanged("SelectedItem");
                    OnPropertyChanged("Desc");
                    // nowe
                    // sprawdza czy jest wybrany element listy bo jest wymagany do funkcji w if
                    if (_selected != null)
                    {
                        UpdateProperties();
                        UpdateDamage();
                    }
                    GetDices();
                    GetDamage();
                }
            }
        }
        #endregion

        public DataRowView SelectedDamageType
        {
            get { return _selectedDamageType; }
            set
            {
                if (_selectedDamageType != value)
                {
                    _selectedDamageType = value;
                    OnPropertyChanged("SelectedDamageType");
                }
            }
        }
        public int DamageValue
        {
            get { return _damageValue; }
            set
            {
                if (_damageValue != value)
                {
                    // Pilnuje aby wartośc nie wyszła poza limit
                    if (value > 65534)
                        value = 65534;
                    else if (value < 0)
                        value = 0;

                    _damageValue = value;
                    OnPropertyChanged("DamageValue");
                }
            }
        }
        public DataRowView SelectedDice
        {
            get { return _selectedDice; }
            set
            {
                if (_selectedDice != value)
                {
                    _selectedDice = value;
                    OnPropertyChanged("SelectedDice");
                }
            }
        }
        public DataView DamageTypes
        {
            get { return _damageTypes; }
            set
            {
                if (_damageTypes != value)
                {
                    _damageTypes = value;
                    OnPropertyChanged("DamageTypes");
                }
            }
        }
        public DataView Dices
        {
            get { return _dices; }
            set
            {
                if (_dices != value)
                {
                    _dices = value;
                    OnPropertyChanged("Dices");
                }
            }
        }
        #endregion
        #region Konstr

        public SQLWeaponDataCombo(string tableName)
            : base(tableName) { }

        #endregion
        #region ICommand

        public ICommand AddDamageCommand
        {
            get
            {
                if (_addDamageCommand == null)
                    _addDamageCommand = new RelayCommand(param => AddDamage());
                return _addDamageCommand;
            }
        }

        public ICommand DeleteDamageCommand
        {
            get
            {
                if (_deleteDamageCommand == null)
                    _deleteDamageCommand = new RelayCommand(param => DeleteDamage());
                return _deleteDamageCommand;
            }
        }

        #endregion
        #region Fun
        // Odświerza tabele obrażeń bronii
        public void UpdateDamage()
        {
            if (_selected != null)
            {
                DataTable dt = new DataTable();
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    string cmdStr = $"SELECT * FROM damage_view WHERE ID={_selected["ID"]};";
                    using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                        da.Fill(dt);
                    DamageDGS = dt.DefaultView;
                    Conn.Close();
                }
            }
        }

        // Pobiera liste kości
        private void GetDices()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                string cmdStr = $"SELECT * FROM dice;";
                using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                    da.Fill(dt);
                Dices = dt.DefaultView;
                Conn.Close();
            }
        }
        // Pobiera liste obrażeń
        private void GetDamage()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                string cmdStr = $"SELECT * FROM damage_type;";
                using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                    da.Fill(dt);
                DamageTypes = dt.DefaultView;
                Conn.Close();
            }
        }

        // Dodaje do bronii  nowe obrażenia
        private void AddDamage()
        {
            if (Selected != null & _selectedDamageType != null)
            {
                try
                {
                    using (MySqlConnection Conn = DBConnection.Instance.Connection)
                    {
                        Conn.Open();
                        using var cmd = new MySqlCommand();
                        cmd.Connection = Conn;
                        if (_selectedDice == null)
                            cmd.CommandText = $"INSERT INTO damage (item_id,value, type_id) VALUES ({_selected["ID"]},{_damageValue},\"{_selectedDamageType["id"]}\")";
                        else
                            cmd.CommandText = $"INSERT INTO damage (item_id,value, type_id, dice_id) VALUES ({_selected["ID"]},{_damageValue},\"{_selectedDamageType["id"]}\",\"{_selectedDice["id"]}\")";
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                } catch { }
                //Odświerza tabele
                GetDices();
                GetDamage();
                UpdateDamage();
            }
        }

        // Usuwa z broni dane obrażenia
        private void DeleteDamage()
        {
            if (_selectedDamage != null)
            {
                try
                {
                    using (MySqlConnection Conn = DBConnection.Instance.Connection)
                    {
                        Conn.Open();
                        using var cmd = new MySqlCommand();
                        cmd.Connection = Conn;
                        cmd.CommandText = $"DELETE FROM damage WHERE id={_selectedDamage["Self"]}";
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                } catch { }
                //Odświerza tabele
                GetDices();
                GetDamage();
                UpdateDamage();
            }
        }
        #endregion
    }
}
