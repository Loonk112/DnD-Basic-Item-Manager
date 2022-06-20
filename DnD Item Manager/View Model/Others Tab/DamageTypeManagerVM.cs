using DnD_Item_Manager.DAL;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Input;

namespace DnD_Item_Manager.View_Model.Others_Tab
{
    internal class DamageTypeManagerVM : PropertyChangedNotify
    {
        private DataRowView _selectedDamageType;
        private DataView _damageTypes;
        private string _typeName;
        private ICommand _newDTCommand;
        private ICommand _saveDTCommand;
        private ICommand _deleteDTCommand;

        //DT - DamageType - typ obrażeń

        #region Właściwości

        public DataRowView SelectedDamageType
        {
            get { return _selectedDamageType; }
            set
            {
                if (_selectedDamageType != value)
                {
                    _selectedDamageType = value;
                    OnPropertyChanged("SelectedDamageType");
                    UpdateEdit();
                }
            }
        }

        public DataView DamageTypes
        {
            get
            {
                if (this._damageTypes == null)
                    UpdateDamageTypes();
                return this._damageTypes;
            }
            set
            {
                if ((this._damageTypes != value))
                {
                    this._damageTypes = value;
                    OnPropertyChanged("DamageTypes");
                }
            }
        }

        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if ((_typeName != value))
                {
                    // Pozbywa się ' i " które mogłyby wpłynąć na funkcje
                    _typeName = value.Replace("'", "").Replace("\"", "").Replace("(","").Replace(")","");
                    OnPropertyChanged("TypeName");
                }
            }
        }

        #endregion
        #region ICommand
        public ICommand NewDTCommand
        {
            get
            {
                if (_newDTCommand == null)
                    _newDTCommand = new RelayCommand(param => NewDT());
                return _newDTCommand;
            }
        }

        public ICommand SaveDTCommand
        {
            get
            {
                if (_saveDTCommand == null)
                    _saveDTCommand = new RelayCommand(param => SaveDT());
                return _saveDTCommand;
            }
        }

        public ICommand DeleteDTCommand
        {
            get
            {
                if (_deleteDTCommand == null)
                    _deleteDTCommand = new RelayCommand(param => DeleteDT());
                return _deleteDTCommand;
            }
        }

        #endregion
        #region Funkcje

        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_selectedDamageType != null)
            {
                TypeName = _selectedDamageType["name"].ToString();
            }
        }

        private void UpdateDamageTypes()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                string cmdStr = $"SELECT * from damage_type";
                using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                    da.Fill(dt);
                DamageTypes = dt.DefaultView;
                Conn.Close();
            }
        }

        private void NewDT()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;

                cmd.CommandText = $"INSERT INTO damage_type (name) VALUES (\"{TypeName}\")";
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            UpdateDamageTypes();
        }

        private void SaveDT()
        {
            if (_selectedDamageType != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"UPDATE damage_type SET name=\"{TypeName}\" WHERE id={_selectedDamageType["id"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                UpdateDamageTypes();
            }
        }

        private void DeleteDT()
        {
            if (_selectedDamageType != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"DELETE FROM damage_type WHERE id={_selectedDamageType["id"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                UpdateDamageTypes();
            }
        }

        #endregion
    }
}
