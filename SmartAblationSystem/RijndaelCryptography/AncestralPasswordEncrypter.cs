using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RijndaelCryptography
{
    /// <summary>
    /// Represents the ancestral password encrypter class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AncestralPasswordEncrypter
    {
        private static AncestralPasswordEncrypter instance;

        private static readonly Dictionary<string, string> alphabetAndNumbersDictionary = new Dictionary<string, string>();

        public static Dictionary<string, string> AlphabetAndNumbersDictionary => alphabetAndNumbersDictionary;

        private static readonly string encryptionKey = "ⵜⴰⵣⴰⵢⴰⵔⵜ";
        private static readonly string siganture = "ⵏⵏⴰⴳ";

        /// <summary>
        ///   Represents ancestral password encrypter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static AncestralPasswordEncrypter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AncestralPasswordEncrypter();
                    //InitializealphabetAndNumbersDictionary();

                }

                return instance;
            }
        }

        /// <summary>
        /// Initializes alphabet and numbers dictionary
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private static void InitializealphabetAndNumbersDictionary()
        {
            AlphabetAndNumbersDictionary.Add("A", "ⴰ");

            AlphabetAndNumbersDictionary.Add("B", "ⴱ");

            AlphabetAndNumbersDictionary.Add("C", "ⵛ");

            AlphabetAndNumbersDictionary.Add("D", "ⴷ");

            AlphabetAndNumbersDictionary.Add("E", "ⴻ");

            AlphabetAndNumbersDictionary.Add("F", "ⴼ");

            AlphabetAndNumbersDictionary.Add("G", "ⴳ");

            AlphabetAndNumbersDictionary.Add("H", "ⵀ");

            AlphabetAndNumbersDictionary.Add("I", "ⵉ");

            AlphabetAndNumbersDictionary.Add("J", "ⵊ");

            AlphabetAndNumbersDictionary.Add("K", "ⴽ");

            AlphabetAndNumbersDictionary.Add("L", "ⵍ");

            AlphabetAndNumbersDictionary.Add("M", "ⵎ");

            AlphabetAndNumbersDictionary.Add("N", "ⵏ");

            AlphabetAndNumbersDictionary.Add("O", "O");

            AlphabetAndNumbersDictionary.Add("P", "P");

            AlphabetAndNumbersDictionary.Add("Q", "ⵇ");

            AlphabetAndNumbersDictionary.Add("R", "ⵔ");

            AlphabetAndNumbersDictionary.Add("S", "ⵙ");

            AlphabetAndNumbersDictionary.Add("T", "ⵜ");

            AlphabetAndNumbersDictionary.Add("U", "ⵓ");

            AlphabetAndNumbersDictionary.Add("V", "V");

            AlphabetAndNumbersDictionary.Add("W", "ⵡ");

            AlphabetAndNumbersDictionary.Add("X", "ⵅ");

            AlphabetAndNumbersDictionary.Add("Y", "ⵢ");

            AlphabetAndNumbersDictionary.Add("Z", "ⵣ");

            AlphabetAndNumbersDictionary.Add("0", "ϕ");

            AlphabetAndNumbersDictionary.Add("1", "๑");

            AlphabetAndNumbersDictionary.Add("2", "๒");

            AlphabetAndNumbersDictionary.Add("3", "๓");

            AlphabetAndNumbersDictionary.Add("4", "๔");

            AlphabetAndNumbersDictionary.Add("5", "๕");

            AlphabetAndNumbersDictionary.Add("6", "๖");

            AlphabetAndNumbersDictionary.Add("7", "๗");

            AlphabetAndNumbersDictionary.Add("8", "๘");

            AlphabetAndNumbersDictionary.Add("9", "๙");

        }

        /// <summary>
        /// Encrypts password
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static string EncryptPassword(string password)
        {
            password = password + siganture;

            string encryptedstring = StringCipher.Encrypt(password, encryptionKey);

            return encryptedstring;
        }

        /// <summary>
        /// Encrypts patient first name and last name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Tuple<string, string> EncryptPatientFirstNameAndLastName(string FirstName, string LastName)
        {
            try
            {

                string firstName = StringCipher.Encrypt(FirstName, encryptionKey);
                string lastName = StringCipher.Encrypt(LastName, encryptionKey);

                return new Tuple<string, string>(firstName, lastName);
            }

            catch 
            {
                return new Tuple<string, string>("Invalid First Name ", "Invalid Last Name ");
            }
        }

        /// <summary>
        /// Encrypts doctor name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static string EncryptDoctorName(string Name)
        {
            try
            {

                string name = StringCipher.Encrypt(Name, encryptionKey);

                return name;
            }

            catch
            {

                return "Invalid Doctor Name";
            }
        }

        /// <summary>
        /// Decrypts password
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static string DecryptPassword(string encryptedstring)
        {

            string decryptedstring = string.Empty;
            try
            {
                 decryptedstring = StringCipher.Decrypt(encryptedstring, encryptionKey);

                if (decryptedstring.Contains(siganture))
                {
                    decryptedstring = decryptedstring.Replace(siganture, string.Empty);
                }

                else
                {
                    decryptedstring = "Invalid Password";
                }
            }

            catch
            {
                decryptedstring = "Invalid Password";
            }

            return decryptedstring;
        }

        /// <summary>
        /// Decrypts patient first name and last name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Tuple<string, string> DecryptPatientFirstNameAndLastName(string FirstName, string LastName)
        {
            string firstName = StringCipher.Decrypt(FirstName, encryptionKey);

            string lastName = StringCipher.Decrypt(LastName, encryptionKey);


            return new Tuple<string, string>(firstName, lastName);
        }

        /// <summary>
        /// Decrypts doctor name
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static string DecryptDoctorName(string Name)
        {
            string name = StringCipher.Decrypt(Name, encryptionKey);

            return name;
        }



    }
}
