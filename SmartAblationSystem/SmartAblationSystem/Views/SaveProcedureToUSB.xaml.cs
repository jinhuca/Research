using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using SmartAblationSystem.Helpers;

namespace SmartAblationSystem.Views
{
	public partial class SaveProcedureToUSB
	{
		private readonly IDataExportable _context;

		public SaveProcedureToUSB(IDataExportable context)
		{
			InitializeComponent();
			_context = context;
			DataContext = context;
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				DialogResult = true;
				if(_context != null)
				{
					int startYear_ = 0;
					int endYear_ = 0;

					if(ComboxBoxStartYear.SelectedValue != null)
					{
						startYear_ = int.Parse(ComboxBoxStartYear.SelectedValue.ToString());
					}

					if(ComboxBoxEndYear.SelectedValue != null)
					{
						endYear_ = int.Parse(ComboxBoxEndYear.SelectedValue.ToString());
					}

					if(startYear_ > endYear_)
					{
						endYear_ = startYear_;
					}

					_context.ProcedureEndTime = endYear_.ToString();
					_context.ProcedureStartTime = startYear_.ToString();
				}

				Close();
			}
			catch(Exception ex_)
			{
				LogSystem.LogService.LogException(ex_);
			}
		}

		private bool PopulateYearList()
		{
			bool hasData = false;
			List<string> yearlist = new List<string>();
			yearlist = CommonViewModel.Current?.Data?.DataAccess?.GetAllProceduresYear();

			if(yearlist.Count > 0)
			{
				ComboxBoxEndYear.ItemsSource = yearlist;
				ComboxBoxStartYear.ItemsSource = yearlist;
				hasData = true;
			}
			return hasData;
		}

		private void No_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private readonly Regex _pattern = new Regex(@"^(?=.*\d)(?=.*).{8,16}$", RegexOptions.Compiled);

		private bool CheckPassword(string pw)
		{
			return pw != null && _pattern.IsMatch(pw);
		}

		private void SaveProcedureToUSB_OnLoaded(object sender, RoutedEventArgs e)
		{
			if(_context != null)
			{
				_context.SaveLogSelected = false;
				_context.SaveToJSONSelected = false;
				_context.SaveToPDFSelected = false;
				_context.SaveToCSVSelected = false;
				_context.SaveToReportSelected = false;
				_context.IsPasswordValid = false;
				_context.IsPasswordConfirmed = false;
				_context.FilePassword = string.Empty;
				_context.ConfirmPassword = string.Empty;
				_context.IsPatientInfoAnonymized = false;

        CaseSummaryReportOption.Visibility = (_context.IsCryterionUser || _context.IsBSCADMINUser) && PopulateYearList()
          ? Visibility.Visible
          : Visibility.Hidden;

				CaseSummaryRange.Visibility = (_context.IsCryterionUser || _context.IsBSCADMINUser) && PopulateYearList()
          ? Visibility.Visible
          : Visibility.Hidden;
      }

			Top = 100;
		}

		private void SaveProcedureToUSB_OnUnloaded(object sender, RoutedEventArgs e)
		{
			if(_context != null)
			{
				_context.IsPasswordConfirmed = false;
			}
		}

    private void EnteredFilePasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
      if (_context == null)
      {
				return;
      }
			_context.FilePassword = EnteredFilePasswordBox.Password;
      _context.IsPasswordValid = CheckPassword(_context.FilePassword);
			_context.IsPasswordConfirmed = 
        _context.FilePassword == _context.ConfirmPassword && 
        _context.FilePassword != string.Empty && 
        _context.ConfirmPassword != string.Empty;
      EnteredFilePasswordBox.SetSelection(EnteredFilePasswordBox.Password.Length,0);
    }

    private void ConfirmFilePasswordBox_OnConfirmChanged(object sender, RoutedEventArgs e)
    {
      if (_context == null)
      {
				return;
      }
      _context.ConfirmPassword = ConfirmPasswordBox.Password;
      _context.IsPasswordConfirmed = 
        _context.FilePassword == _context.ConfirmPassword && 
        _context.FilePassword != string.Empty && 
        _context.ConfirmPassword != string.Empty;
			ConfirmPasswordBox.SetSelection(ConfirmPasswordBox.Password.Length, 0);
    }
  }
}