using DnD_Item_Manager.DAL;
using DnD_Item_Manager.Model.SQLModels;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows.Input;

namespace DnD_Item_Manager.View_Model.Items_Tab
{
    // Dostosowana do tabelii klasa
    internal class Weapon_VM : PropertyChangedNotify
    {
        private SQLWeaponDataCombo _weapon;

        // Zmienne unikatowe do klasy tabeli
        private string _itemName;
        private int _itemWeight;
        private string _itemClass;
        private string _itemProficiency;
        private string _itemDesc;
        private List<string> _weaponClassList = new List<string>() { "Melee", "Ranged" };
        private List<string> _weaponProficiencyList = new List<string>() { "Simple", "Martial" };

        private ICommand _updateEditCommand;
        private ICommand _newWeaponCommand;
        private ICommand _saveWeaponCommand;
        #region Właściwości
        public SQLWeaponDataCombo Weapon
        {
            get
            {
                if (_weapon == null)
                    Weapon = new SQLWeaponDataCombo("weapon_view");
                return _weapon;
            }
            set
            {
                if (_weapon != value)
                {
                    _weapon = value;
                    OnPropertyChanged("Weapon");
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

        public string ItemClass
        {
            get
            {
                if (_itemClass == null | _itemClass == "")
                    ItemClass = "Melee";
                return _itemClass;
            }
            set
            {
                if (value != _itemClass)
                {
                    _itemClass = value;
                    OnPropertyChanged("ItemClass");
                }
            }
        }

        public string ItemProficiency
        {
            get
            {
                if (_itemProficiency == null | _itemProficiency == "")
                    ItemProficiency = "Simple";
                return _itemProficiency;
            }
            set
            {
                if (value != _itemProficiency)
                {
                    _itemProficiency = value;
                    OnPropertyChanged("ItemProficiency");
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

        public List<string> WeaponClassList
        {
            get { return _weaponClassList; }
        }

        public List<string> WeaponProficiencyList
        {
            get { return _weaponProficiencyList; }
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

        public ICommand NewWeaponCommand
        {
            get
            {
                if (_newWeaponCommand == null)
                    _newWeaponCommand = new RelayCommand(param => NewWeapon());
                return _newWeaponCommand;
            }
        }

        public ICommand SaveWeaponCommand
        {
            get
            {
                if (_saveWeaponCommand == null)
                    _saveWeaponCommand = new RelayCommand(param => SaveWeapon());
                return _saveWeaponCommand;
            }
        }
        #endregion

        #region Funkcje

        //Odświerza edytowalne zmioenne na te od wybranego elemntu głównej listy
        private void UpdateEdit()
        {
            if (_weapon.Selected != null)
            {

                ItemName = _weapon.Selected["Name"].ToString();
                ItemWeight = int.Parse(_weapon.Selected["Weight"].ToString());
                ItemClass = _weapon.Selected["Class"].ToString();
                ItemProficiency = _weapon.Selected["Proficiency"].ToString();
                ItemDesc = _weapon.Selected["Description"].ToString();
            }
        }

        private void NewWeapon()
        {
            using (MySqlConnection Conn = DBConnection.Instance.Connection)
            {
                Conn.Open();
                using var cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandText = $"INSERT INTO item (name, weight, description, type) VALUE (\"{ItemName}\",{ItemWeight},\"{ItemDesc}\",\"Weapon\")";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                string itemId;
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    itemId = reader.GetValue(0).ToString();
                }
                cmd.CommandText = $"INSERT INTO weapon (item_id, class, proficiency) VALUE ({itemId},\"{ItemClass}\",\"{ItemProficiency}\")";
                cmd.ExecuteNonQuery();
                Conn.Close();
            }
            _weapon.UpdateMain();
        }

        private void SaveWeapon()
        {
            if (_weapon.Selected != null)
            {
                using (MySqlConnection Conn = DBConnection.Instance.Connection)
                {
                    Conn.Open();
                    using var cmd = new MySqlCommand();
                    cmd.Connection = Conn;

                    cmd.CommandText = $"UPDATE item SET name=\"{ItemName}\", weight={ItemWeight},description=\"{ItemDesc}\",type=\"Weapon\" WHERE id={_weapon.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"UPDATE weapon SET class=\"{ItemClass}\", proficiency=\"{ItemProficiency}\" WHERE item_id={_weapon.Selected["ID"]}";
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                }
                _weapon.UpdateMain();
            }
        }
        #endregion
    }
}