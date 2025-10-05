using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is the Catheter Validator's Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class CatheterValidator
    {
        private readonly Data Data; 
        private const double maximumHoursAfterUse = 12;

        /// <summary>
        /// Initializes a new instance of the Action Log Record Model class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="data">A Data representing a catheter's data.</param>
        public CatheterValidator(Data data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Validates a catheter ID
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheterID">An integer representing a catheter's ID.</param>
        /// <returns>A boolean value that states whether the catheter's ID is valid or not.</returns>
        public bool ValidateCatheterID(int catheterID)
        {
            bool isValid = false;
            var catheter = this.Data.DataAccess.GetCatheterAccordingToCatheterId(catheterID);

            isValid = (catheter != null) ? true : false;

            return isValid;
        }

        /// <summary>
        /// Validates whether a catheter has expired (date) or not using its ID
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheterID">An integer representing a catheter's ID.</param>
        /// <returns>A boolean value that states whether the catheter has expired or not.</returns>
        public bool ValidateCatheterExpirationDate(int catheterID)
        {
            bool isValid = false;
            DateTime expirationDate = this.Data.DataAccess.GetCatheterExpirationDate(catheterID);

            isValid = (expirationDate > DateTime.Now) ? true : false;

            return isValid;
        }

        /// <summary>
        /// Validates whether a catheter has expired (date) or not using its expiration date
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="expirationDate">An DateTime representing a catheter's expiration date.</param>
        /// <returns>A boolean value that states whether the catheter has expired or not.</returns>
        public bool ValidateCatheterExpirationDate(DateTime expirationDate)
        {
            // New Requirment: Remove catheter expiration date validation. In the new generation remove the next line:

            return true;

            bool isValid = false;

            isValid = (expirationDate > DateTime.Now) ? true : false;

            return isValid;
        }

        /// <summary>
        /// Validates whether a catheter is valid considering its last use date
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheterLastUseDate">An DateTime representing a catheter's last usage date.</param>
        /// <returns>A boolean value that states whether the catheter is valid or not.</returns>
        public bool ValidateCatheterLastUseDate(DateTime catheterLastUseDate)
        {
            bool isValid = false;

            isValid = ((DateTime.Now - catheterLastUseDate).TotalHours < maximumHoursAfterUse) ? true : false;

            return isValid;
        }

        /// <summary>
        /// Validates whether a catheter is valid considering its last use date
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheterID">An integer representing the catheter's ID.</param>
        /// <returns>A boolean value that states whether the catheter is valid or not.</returns>
        public bool ValidateCatheterLastUseDate(int catheterID)
        {
            bool isValide = false;

            DateTime catheterLastUseDate = this.Data.DataAccess.GetCatheterLastUseDate(catheterID);

            isValide = ((DateTime.Now - catheterLastUseDate).TotalHours > maximumHoursAfterUse) ? true : false;

            return isValide;
        }

        /// <summary>
        /// Compares a catheter's date and the database catheter's date
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheterDate">A DateTime representing a catheter's date.</param>
        /// <param name="dataBaseCatheterDate">A DateTime representing a catheter's date.</param>
        /// <returns>A boolean represengint if both dates are equals or not.</returns>
        public bool CompareDateDate(DateTime catheterDate, DateTime dataBaseCatheterDate)
        {
            if (catheterDate.Hour != dataBaseCatheterDate.Hour ||
                catheterDate.Day != dataBaseCatheterDate.Day ||
                catheterDate.Month != dataBaseCatheterDate.Month ||
                catheterDate.Year != dataBaseCatheterDate.Year)
                return false;

            return true;
        }

        /// <summary>
        /// Validates a catheter using its ID, expiration date and last usage date
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheterID">An integer representing a Catheter Id.</param>
        /// <param name="expirationDate">A DateTime representing a catheter's expiration date.</param>
        /// <param name="catheterLastUseDate">A DateTime representing a catheter's last usage date.</param>
        /// <returns>A boolean representing if a catheter is valid or not.</returns>
        public bool ValidateCatheter(int catheterID, DateTime expirationDate, DateTime catheterLastUseDate)
        {
            ErrorWarningAndMessage errorWarningAndMessage = new ErrorWarningAndMessage();
            bool isCatheterInError = CommonViewModel.Current.IsCatheterInError;

            if (!this.ValidateCatheterID(catheterID))
            {
                if (!isCatheterInError)
                {
                    CommonViewModel.Current.IsCatheterInError = true;
                    errorWarningAndMessage.DisplayErrorMessage("Invalid catheter.");
                }
                return false;
            }

            if (!this.ValidateCatheterExpirationDate(expirationDate))
            {
                if (!isCatheterInError)
                {
                    CommonViewModel.Current.IsCatheterInError = true;
                    errorWarningAndMessage.DisplayErrorMessage("Catheter Expired");
                }
                return false;
            }

            if (!this.ValidateCatheterLastUseDate(catheterLastUseDate))
            {
                if (!isCatheterInError)
                {
                    CommonViewModel.Current.IsCatheterInError = true;
                    errorWarningAndMessage.DisplayErrorMessage("Catheter has been used for more than 12 hours.");
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates a catheter when already in use
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheterID">An integer representing a Catheter's Id.</param>
        /// <param name="expirationDate">A DateTime representing a catheter's expiration date.</param>
        /// <param name="catheterLastUseDate">A DateTime representing a catheter's last usage date.</param>
        /// <param name="dataBaseExpirationDate">A DateTime representing a catheter's database expiration date.</param>
        /// <param name="dataBaseCatheterLastUseDate">A DateTime representing a catheter's database last useage date.</param>
        /// <returns>A boolean representing if a catheter is valid or not.</returns>
        public bool ValidateCatheterWhenAlreadyUsed(int catheterID, DateTime expirationDate, DateTime catheterLastUseDate, DateTime dataBaseExpirationDate, DateTime dataBaseCatheterLastUseDate)
        {
            ErrorWarningAndMessage errorWarningAndMessage = new ErrorWarningAndMessage();
            bool isCatheterInError = CommonViewModel.Current.IsCatheterInError;

            if (!this.CompareDateDate(expirationDate, dataBaseExpirationDate))
            {
                if (!isCatheterInError)
                {
                    CommonViewModel.Current.IsCatheterInError = true;
                    errorWarningAndMessage.DisplayErrorMessage("Catheter data does not match the expected value.");
                }
                return false;
            }

            if (!this.CompareDateDate(catheterLastUseDate, dataBaseCatheterLastUseDate))
            {
                if (!isCatheterInError)
                {
                    CommonViewModel.Current.IsCatheterInError = true;
                    errorWarningAndMessage.DisplayErrorMessage("Catheter data does not match the expected value.");
                }
                return false;
            }

            if (!this.ValidateCatheterID(catheterID))
            {
                if (!isCatheterInError)
                {
                    CommonViewModel.Current.IsCatheterInError = true;
                    errorWarningAndMessage.DisplayErrorMessage("Invalid catheter.");
                }
                return false;
            }

            if (!this.ValidateCatheterExpirationDate(expirationDate))
            {
                if (!isCatheterInError)
                {
                    CommonViewModel.Current.IsCatheterInError = true;
                    errorWarningAndMessage.DisplayErrorMessage("Catheter Expired");
                }
                return false;
            }

            if (!this.ValidateCatheterLastUseDate(catheterLastUseDate))
            {
                if (!isCatheterInError)
                {
                    CommonViewModel.Current.IsCatheterInError = true;
                    errorWarningAndMessage.DisplayErrorMessage("Catheter has been used for more than 12 hours.");
                }
                return false;
            }

            return true;
        }
    }
}