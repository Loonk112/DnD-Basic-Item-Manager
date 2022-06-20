using DnD_Item_Manager.DAL;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Input;

namespace DnD_Item_Manager.View_Model.Others_Tab
{
    internal class PropertiesManagerVM : PropertyChangedNotify
    {
        private DataRowView _selectedProperty;
        private DataView _properties;
        private string _propertyName;
        private string _propertyDescription;
        private ICommand _newPropertyComand;
        private ICommand _savePropertyComand;
        private ICommand _deletePropertyComand;

        #region Właściwości

        public DataRowView SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                if (_selectedProperty != value)
                {
                    _selectedProperty = value;
                    OnPropertyChanged("SelectedProperty");
                    UpdateEdit();
                }
            }
        }

        public DataView Properties
        {
            get
            {
                if (this._properties == null)
                    UpdateProperties();
                return this._properties;
            }
            set
            {
                if ((this._properties != value))
                {
                    this._properties = value;
                    OnPropertyChanged("Properties");
                }
            }
        }

        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if ((_propertyName != value))
                {
                    // Pozbywa się ' i " które mogłyby wpłynąć na funkcje
                    _propertyName = value.Replace("'", "").Replace("\"", "").Replace("(","").Replace(")","");
                    OnPropertyChanged("PropertyName");
                }
            }
        }

        public string PropertyDescription
        {
            get { return _propertyDescription; }
            set
            {
                if ((_propertyDescription != value))
                {
                    // Pozbywa się ' i " które mogłyby wpłynąć na funkcje
                    _propertyDescription = value.Replace("'", "").Replace("\"", "").Replace("(","").Replace(")","");
                    OnPropertyChanged("PropertyDescription");
                }
            }
        }

        #endregion
        #region ICommand
        public ICommand NewPropertyCommand
        {
            get
            {
                if (_newPropertyComand == null)
                    _newPropertyComand = new RelayCommand(param => NewProperty());
                return _newPropertyComand;
            }
        }

        public ICommand SavePropertyCommand
        {
            get
            {
                if (_savePropertyComand == null)
                    _savePropertyComand = new RelayCommand(param => SaveProperty());
                return _savePropertyComand;
            }
        }

        public ICommand DeletePropertyCommand
        {
            get
            {
                if (_deletePropertyComand == null)
                    _deletePropertyComand = new RelayCommand(param => DeleteProperty());
                return _deletePropertyComand;
            }
        }

        #endregion
        #region Funkcje


        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_selectedProperty != null)
            {
                PropertyName = _selectedProperty["name"].ToString();
                PropertyDescription = _selectedProperty["description"].ToString();
            }
        }

        private void UpdateProperties()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                string cmdStr = $"SELECT * from property";
                using (MySqlCommand cmdSel = new MySqlCommand(cmdStr, Conn))
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmdSel))
                    da.Fill(dt);
                Properties = dt.DefaultView;
                Conn.Close();
            }
        }

        private void NewProperty()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;

                cmd.CommandText = $"INSERT INTO property (name, description) VALUES (\"{PropertyName}\",\"{PropertyDescription}\")";
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            UpdateProperties();
        }

        private void SaveProperty()
        {
            if (_selectedProperty != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;
                    cmd.CommandText = $"UPDATE property SET name=\"{PropertyName}\", description=\"{PropertyDescription}\" WHERE id={_selectedProperty["id"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                UpdateProperties();
            }
        }

        private void DeleteProperty()
        {
            if (_selectedProperty != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"DELETE FROM property WHERE id={_selectedProperty["id"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                UpdateProperties();
            }
        }

        #endregion
    }
}
