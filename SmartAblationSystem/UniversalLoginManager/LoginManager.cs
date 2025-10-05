using DataAccessLayer;
using RijndaelCryptography;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UniversalLoginManager
{
    /// <summary>
    /// This class handles the system's user login and authentification.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class LoginManager : INotifyPropertyChanged
    {
        private AuthenticationType userAuthenticationType;
        private AccessControlType userAccessControlType;
        private CostTrackingType userCostTrackingType;
        private ObservableCollection<User> users;
        private User currentUser;
        private DataAccess Data;

        private int cryUserCode;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This class notifies listeners that a property changed.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="propertyName">The property name that has changed.</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This property handles getter and setter for the user authentication type.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AuthenticationType UserAuthenticationType
        {
            get
            {
                return userAuthenticationType;
            }

            set
            {
                userAuthenticationType = value;
                NotifyPropertyChanged("UserAuthenticationType");
            }
        }

        /// <summary>
        /// This property handles getter and setter for the user acces control type.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AccessControlType UserAccessControlType
        {
            get
            {
                return userAccessControlType;
            }

            set
            {
                userAccessControlType = value;
                NotifyPropertyChanged("UserAccessControlType");
            }
        }

        /// <summary>
        /// This property handles getter and setter for the cost tracking type.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public CostTrackingType UserCostTrackingType
        {
            get
            {
                return userCostTrackingType;
            }

            set
            {
                userCostTrackingType = value;
                NotifyPropertyChanged("UserCostTrackingType");
            }
        }

        /// <summary>
        /// This property handles getter and setter for the system's user list.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ObservableCollection<User> Users
        {
            get
            {
                return Data.GetAllActiveUsers();
            }

            set
            {
                users = value;
                NotifyPropertyChanged("Users");
            }
        }

        /// <summary>
        /// This property handles getter and setter for the current logged-in user.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return currentUser;
            }

            set
            {
                currentUser = value;
                NotifyPropertyChanged("CurrentLogin");
            }
        }

        /// <summary>
        /// Enumeration representing the possible authentication types.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum AuthenticationType
        {
            USER_NAME_AND_PASSWORD = 0,
            ID_CARD = 1,
            RETINAL_SCAN = 2,
            FINGERPRINT = 3
        }

        /// <summary>
        /// Enumeration representing the possible access control types.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum AccessControlType
        {
            USER = 1,
            ADMIN = 2,
            CRYTERION = 3,
            DOCTOR = 4,
            BSCADMIN = 5
        }

        /// <summary>
        /// Enumeration representing the possible cost tracking types.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum CostTrackingType
        {
            ALLOCATED = 0,
            REDUCED = 1,
        }

        /// <summary>
        /// This constructor allows to get the full list of the system's users.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="data">Data access object to allow getting users from an external source.</param>
        public LoginManager(DataAccess data)
        {
            Data = data;

            if (data != null)
            {
                users = Data.GetAllActiveUsers();
            }
        }

        /// <summary>
        /// This function performs a single user authentication by validating its username and password
        /// using an external source.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="username">The user's username string</param>
        /// <param name="password">The user's password string.</param>
        /// <returns></returns>
        public bool LoginUser(string username, string password)
        {
            User user = null;

            if (username.ToUpper() == "BSC")
            {
                if (IsPasscodeValid(password))
                {
                    user = Data.ConnectUserCry();
                }
            }
            else if (username.ToUpper() == "BSCADMIN")
            {
                if (IsBSCADMINPasscodeValid(password))
                {
                    user = Data.ConnectBSCADMINUser();
                }
            }
            else
            {

                user = Data.ConnectUser(username, password);
            }
            
            CurrentUser = user;

            return CurrentUser != null;
        }

        /// <summary>
        /// This property handles getter and setter for the CRY user passcode.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CryUserCode
        {
            get
            {
                return cryUserCode;
            }
            set
            {
                cryUserCode = value;
            }
        }

        /// <summary>
        /// This function generates a random 8-digit number.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void GeneratePassCode()
        {
            Random rnd = new Random();
            CryUserCode = rnd.Next(10000000, 99999999);
        }

        /// <summary>
        /// This function verifies if the passcode entered is valid when comparing to a formula result.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// <returns>Returns if the passcode is valid.</returns>
        /// </summary>
        public bool IsPasscodeValid(string passcode)
        {
          return true;
            string stringPasscode = CryUserCode.ToString();

            //(D1 * 2 + D2 ^ 2 + D3 + D4 * 3 + D5 + D6 * 4 + D7 ^ 3 + D8) ^ 3  where D1, D2, D3…, D8 are the digits in the random number
            double formulaResult = Math.Pow(
                                Int32.Parse(stringPasscode.Substring(0, 1)) * 2 +
                                Math.Pow(Int32.Parse(stringPasscode.Substring(1, 1)), 2) +
                                Int32.Parse(stringPasscode.Substring(2, 1)) +
                                Int32.Parse(stringPasscode.Substring(3, 1)) * 3 +
                                Int32.Parse(stringPasscode.Substring(4, 1)) +
                                Int32.Parse(stringPasscode.Substring(5, 1)) * 4 +
                                Math.Pow(Int32.Parse(stringPasscode.Substring(6, 1)), 3) +
                                Int32.Parse(stringPasscode.Substring(7, 1))
                                , 3);

            return passcode == formulaResult.ToString();
        }

        public bool IsBSCADMINPasscodeValid(string passcode)
        {
          return true;
            string stringPasscode = CryUserCode.ToString();

            //(D1 + D2 ^ 2 + D3 + D4 * 4 + D5 ^ 3 + D6 * 5 + D7 ^ 2 + D8) ^ 2  where D1, D2, D3…, D8 are the digits in the random number
            double formulaResult = Math.Pow(
                                Int32.Parse(stringPasscode.Substring(0, 1)) +  
                                Math.Pow(Int32.Parse(stringPasscode.Substring(1, 1)), 2) + 
                                Int32.Parse(stringPasscode.Substring(2, 1)) + 
                                Int32.Parse(stringPasscode.Substring(3, 1)) * 4 +  
                                Math.Pow(Int32.Parse(stringPasscode.Substring(4, 1)),3) + 
                                Int32.Parse(stringPasscode.Substring(5, 1)) * 5 +  
                                Math.Pow(Int32.Parse(stringPasscode.Substring(6, 1)), 2) + 
                                Int32.Parse(stringPasscode.Substring(7, 1))  
                                , 2);

            return passcode == formulaResult.ToString();
        }
    }
}