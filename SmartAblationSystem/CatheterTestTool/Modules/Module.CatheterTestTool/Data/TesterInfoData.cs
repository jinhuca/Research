
using System;
using Prism.Mvvm;

namespace Module.CatheterTestTool.Data
{
    public class TesterInfoData : BindableBase
    {
        private string _firstName;
        public string FirstName 
        { 
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName;

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private DateTime _logTime;

        public DateTime LogonTime
        {
            get => _logTime; 
            set => SetProperty(ref _logTime, value);
        }
    }
}
