using DnD_Item_Manager.DAL;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Input;

namespace DnD_Item_Manager.View_Model.Others_Tab
{
    internal class DiceManagerVM : PropertyChangedNotify
    {
        private DataRowView _selectedDice;
        private DataView _dices;
        private string _diceName;
        private ICommand _newDiceComand;
        private ICommand _saveDiceComand;
        private ICommand _deleteDiceComand;

        #region Właściwości

        public DataRowView SelectedDice
        {
            get { return _selectedDice; }
            set
            {
                if (_selectedDice != value)
                {
                    _selectedDice = value;
                    OnPropertyChanged("SelectedDice");
                    UpdateEdit();
                }
            }
        }

        public DataView Dices
        {
            get
            {
                if (this._dices == null)
                    UpdateDices();
                return this._dices;
            }
            set
            {
                if ((this._dices != value))
                {
                    this._dices = value;
                    OnPropertyChanged("Dices");
                }
            }
        }

        public string DiceName
        {
            get { return _diceName; }
            set
            {
                if ((_diceName != value))
                {
                    // Pozbywa się ' i " które mogłyby wpłynąć na funkcje
                    _diceName = value.Replace("'", "").Replace("\"", "").Replace("(","").Replace(")","");
                    OnPropertyChanged("DiceName");
                }
            }
        }

        #endregion
        #region ICommand
        public ICommand NewDiceCommand
        {
            get
            {
                if (_newDiceComand == null)
                    _newDiceComand = new RelayCommand(param => NewDice());
                return _newDiceComand;
            }
        }

        public ICommand SaveDiceCommand
        {
            get
            {
                if (_saveDiceComand == null)
                    _saveDiceComand = new RelayCommand(param => SaveDice());
                return _saveDiceComand;
            }
        }

        public ICommand DeleteDiceCommand
        {
            get
            {
                if (_deleteDiceComand == null)
                    _deleteDiceComand = new RelayCommand(param => DeleteDice());
                return _deleteDiceComand;
            }
        }

        #endregion
        #region Funkcje


        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_selectedDice != null)
            {
                DiceName = _selectedDice["name"].ToString();
            }
        }

        private void UpdateDices()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                string cmdStr = $"SELECT * from dice";
                using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                    da.Fill(dt);
                Dices = dt.DefaultView;
                Conn.Close();
            }
        }

        private void NewDice()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;

                cmd.CommandText = $"INSERT INTO dice (name) VALUES (\"{DiceName}\")";
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            UpdateDices();
        }

        private void SaveDice()
        {
            if (_selectedDice != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;
                    cmd.CommandText = $"UPDATE dice SET name=\"{DiceName}\" WHERE id={_selectedDice["id"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                UpdateDices();
            }
        }

        private void DeleteDice()
        {
            if (_selectedDice != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"DELETE FROM dice WHERE id={_selectedDice["id"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                UpdateDices();
            }
        }

        #endregion
    }
}
