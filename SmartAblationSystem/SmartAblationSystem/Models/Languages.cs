using DataAccessLayer;
using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class handles language translation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static  class Languages
    {
        private static Dictionary<string, string> guiFieldTranslation = new Dictionary<string, string>();
        private static Dictionary<int, string> catheterDescription = new Dictionary<int, string>();
        private static Dictionary<long, string> errorsTranslation = new Dictionary<long, string>();
        private static List<Tuple<long, string, string>> errorsAndSolutionTranslations = new List<Tuple<long, string, string>>();

        private static Data Data = new Data();
        private static Language currentLanguage;
        private static Language selectedUserManualLanguage;
        private static Enumeration enumeration = new Enumeration();

        public static event EventHandler<EventArgs> LanguageChangedEvent;
   
        private static bool guiFieldTranslationInitialized = false;

        static Languages()
        {
          InitializeGuiFieldTranslation();
          guiFieldTranslationInitialized = true;
        }

        /// <summary>
        /// This function handles the sender's PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void OnLanguageChangedEvent(object sender, EventArgs e)
        {
            LanguageChangedEvent?.Invoke(sender, e);
            GuiFieldTranslation = Data.DataAccess.GetAllTranslationsForCurrentLanguage();
            guiFieldTranslationInitialized = false;
        }

        /// <summary>
        /// Initializes GUI field translation
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void InitializeGuiFieldTranslation()
        {
            try
            {
                GuiFieldTranslation = Data.DataAccess.GetAllTranslationsForCurrentLanguage();

                CatheterDescription = Data.DataAccess.GetCathetersDesctiption();

                SelectedUserManualLanguage = Data.DataAccess.GetSelectedUserManualLanguage();
            }

            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        /// <summary>
        /// Returns true if GuiFieldTranslation.Count>0 else return false
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool GuiFieldTranslationInitialized
        {
            get
            {
                return guiFieldTranslationInitialized;
                // return GuiFieldTranslation.Count > 0;
            }
            set
            {
                guiFieldTranslationInitialized = value;
            }
            
        }


        /// <summary>
        /// Initializes error and solution translation
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void InitializeErrorAndSolutionTranslation()
        {
            ErrorsAndSolutionTranslations = Data.DataAccess.GetAllErrorsAndSolutionTranslationsForCurrentLanguage();
        }

        /// <summary>
        /// Initializes error translation
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void InitializeErrorTranslation()
        {
            ErrorsTranslation = Data.DataAccess.GetAllErrorsTranslationsForCurrentLanguage();
        }


        /// <summary>
        /// Gets all languages
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>list of language</returns>
        public static List<Language> GetAllLanguage()
        {
           return Data.DataAccess.GetAllLanguage();
        }

        /// <summary>
        /// Gets a list of translated language objects.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns></returns>
        public static List<Language> GetAllUserManualLanguage()
        {
            List<Language> languageTrans = new List<Language>();
            languageTrans = Data.DataAccess.GetAllUserManualLanguage();
            for (int i = 0; i < languageTrans.Count; i++)
            {
                string lDescriptionUID = languageTrans[i].Description.ToString();
                lDescriptionUID += "UID";
                string lDescriptionTran = GuiFieldTranslation.ContainsKey(lDescriptionUID) ? GuiFieldTranslation[lDescriptionUID] : lDescriptionUID;
                languageTrans[i].Description = lDescriptionTran;
            }
            return languageTrans;
        }


        /// <summary>
        /// Gets errors and Cryterion Solution Translations
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="errorId">error id</param>
        /// <param name="errorType">error type</param>
        /// <returns>translation tuple</returns>
        public static Tuple<long, string, string, string> ErrorsAndCryterionSolutionTranslations(int errorId, int errorType)
        {
            return Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage(errorId, errorType);
        }

        /// <summary>
        /// Gets GUI field translations
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Dictionary<string, string> GuiFieldTranslation
        {
            get
            {
                return guiFieldTranslation;
            }

            set
            {
                guiFieldTranslation = value;
            }
        }



        /// <summary>
        /// Gets or sets current language
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Language CurrentLanguage
        {
            get
            {
                return currentLanguage;
            }

            set
            {
                currentLanguage = value;
                Data.DataAccess.SetCurrentLanguage(currentLanguage.Id);
                OnLanguageChangedEvent(null, null);
            }
        }
        /// <summary>
        /// Gets errors translations
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Dictionary<long, string> ErrorsTranslation
        {
            get
            {
                return errorsTranslation;
            }

            set
            {
                errorsTranslation = value;
            }
        }
        /// <summary>
        /// Gets errors and solution translations
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static List<Tuple<long, string, string>> ErrorsAndSolutionTranslations
        {
            get
            {
                return errorsAndSolutionTranslations;
            }

            set
            {
                errorsAndSolutionTranslations = value;
            }
        }
        /// <summary>
        /// Gets or sets Enumeration value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Enumeration Enumeration { get => enumeration; set => enumeration = value; }
        /// <summary>
        /// Gets or sets catheter description value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Dictionary<int, string> CatheterDescription
        {
            get => catheterDescription;
            set => catheterDescription = value;
        }

        /// <summary>
        /// Gets or sets selected user manual language value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Language SelectedUserManualLanguage
        {
            get => selectedUserManualLanguage;
            set => selectedUserManualLanguage = value;
        }
    }
}
