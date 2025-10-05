using DataAccessLayer;
using SmartAblationSystem.ViewModels;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for ErrorLog.xaml
    /// </summary>
    public partial class ConsoleErrorLog : UserControl
    {
        private ConsoleErrorLogViewModel errorLogViewModel;

        /// <summary>
        /// Console Error Log.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ConsoleErrorLog()
        {
            InitializeComponent();
            this.errorLogViewModel  = this.DataContext as ConsoleErrorLogViewModel;
        }



        /// <summary>
        /// Occurs when UserControl_Unloaded event is raised.
        /// IEC 62304 Class A
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadErrorLog();
        }

        private void UserControl_UnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.errorLogViewModel.ErrorLogList = null;
            this.errorLogViewModel.FiltedErrorLogList = null;
        }

        private void LoadErrorLog()
        {
            if (this.errorLogViewModel != null)
            {
                ResetValues();
                this.errorLogViewModel.GetErrorLogValue();
                if (this.errorLogViewModel.ErrorLogList.Count == 0)
                {
                    btnSaveData.Visibility = Visibility.Hidden;
                    btnSummary.Visibility = Visibility.Hidden;
                }
                else
                {
                    btnSaveData.Visibility = Visibility.Visible;
                    btnSummary.Visibility = Visibility.Visible;
                }
            }

        }

        /// <summary>
        /// Handles Date Picker event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                System.Windows.Controls.Primitives.DatePickerTextBox datePickerTextBox = FindVisualChild<System.Windows.Controls.Primitives.DatePickerTextBox>(datePicker);
                if (datePickerTextBox != null)
                {

                    ContentControl watermark = datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox) as ContentControl;
                    if (watermark != null)
                    {
                        watermark.Content = string.Empty;
                    }
                }
            }
        }
        /// <summary>
        /// Find child component.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private T FindVisualChild<T>(DependencyObject depencencyObject) where T : DependencyObject
        {
            if (depencencyObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depencencyObject); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depencencyObject, i);
                    T result = (child as T) ?? FindVisualChild<T>(child);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Handles clear event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ResetValues();
        }


        private void ResetValues()
        {
            this.errorLogViewModel.FilterFrom = "";
            this.errorLogViewModel.FilterTo = "";
            this.errorLogViewModel.FilterErrorNum = "";
            cbxErrorType.SelectedValue = "0";
            ResetContent();
            ResetCathContent();
        }

        /// <summary>
        /// Handles search event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            ResetContent();
            ResetCathContent();
        }

        /// <summary>
        /// Handles reset content event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ResetContent()
        {
            txtSolutionMessage.Text = "";
            txtMessage.Text = "";
            txtCryterionMessage.Text = "";
            lblGuiV.Content = "";
            lblDBV.Content = "";
            lblControlBLV.Content = "";
            lblControlFV.Content = "";
            lblPatientFV.Content = "";
            lblPatientBLV.Content = "";
            lblRepeatorFV.Content = "";
            lblRepeatorBLV.Content = "";
            lblRemoteFV.Content = "";
            lblRemoteBLV.Content = "";
            lblICBFV.Content = "";
            lblICBLV.Content = "";
            lblCPLDV.Content = "";
            lblUserInfo.Content = "";
            if (dgErrorLogList.Items.Count>0)
                RemoveIsSelectedFromDataGridRows(dgErrorLogList);
        }

        /// <summary>
        /// Handles reset catheter content event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ResetCathContent()
        {
            lblCatheterIDV.Content = "--";
            lblCatheterSerialV.Content = "--";
            lblCatheterContainerV.Content = "--";
            lblCatheterLotV.Content = "--";
            lblCatheterFirstUsedDateV.Content = "--";
            lblIsUsedForTestV.Content = "--";
            lblFirmwareV.Content = "--";
        }

        /// <summary>
        /// Remove selected item style frontend .
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void RemoveIsSelectedFromDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) {; }
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (row !=null) row.IsSelected = false;
            }
        }
        /// <summary>
        /// Handles current cell changed event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void dgErrorList_CurrentCellChanged(object sender, EventArgs e)
        {
            System.Windows.Controls.DataGridCellInfo selectedCell = dgErrorLogList.CurrentCell;
            DataAccessLayer.ErrorLog errorLogRecord = selectedCell.Item as DataAccessLayer.ErrorLog ;
            try
            {
            if (errorLogRecord != null)
            {
                string id = errorLogRecord.Id.ToString();
                long errorCode  = Int64.Parse(errorLogRecord.ErrorCode.ToString());
                int errorType = Int32.Parse(errorLogRecord.ErrorType.ToString());
                int? CatId;
                int? UserID;
                UserID = errorLogRecord.UserID;
                lblUserInfo.Content = "--";
                if (UserID.HasValue)
                {
                    //int tempuserID = UserID.Value;
                    User userinfo = CommonViewModel.Current.Data.DataAccess.GetUserInfoById(UserID.Value);
                    if(userinfo !=null)
                    {
                        
                       var t = userinfo.Types.ToList();
                        if (t[0].Id == 4)
                        {
                            Physician physician = CommonViewModel.Current.Data.DataAccess.GetphysicianByID(UserID.Value);
                            lblUserInfo.Content = "Dr. " + physician.FirstName  + " " + physician.LastName;
                        }
                        else
                            lblUserInfo.Content = userinfo.UserName;
                    }                                  
                }
                CatId = errorLogRecord.CatheterID;
                ErrorMessage errorMessages = CommonViewModel.Current.Data.DataAccess.GetErrorMessagesByErrorID(errorCode, errorType);
                if (errorMessages != null)
                {
                    txtSolutionMessage.Text = errorMessages.SolutionMessage;
                    txtMessage.Text = errorMessages.Message;
                    txtCryterionMessage.Text = errorMessages.CryterionMessage;
                }
                else
                {
                    txtSolutionMessage.Text = "";
                    txtMessage.Text = "";
                    txtCryterionMessage.Text = "";
                }

                ResetCathContent();
                if (CatId.HasValue)
                {
                    CatheterInformation cathInfo = CommonViewModel.Current.Data.DataAccess.GetCathInfoByID(CatId.Value);

                    if (cathInfo != null)
                    {
                        lblCatheterIDV.Content = cathInfo.ID;
                        lblCatheterSerialV.Content = cathInfo.SerialNumber;
                        lblCatheterContainerV.Content = errorLogRecord.CatheterContainer?? string.Empty; 
                        lblCatheterLotV.Content = cathInfo.Lot;
                        lblCatheterFirstUsedDateV.Content = cathInfo.LastUseDate.ToString("MMM dd yyy HH:mm:ss");
                        lblIsUsedForTestV.Content = cathInfo.OverloadedCatheterID;   //cathInfo.IsUsedForEngineering?"Yes":"No";
                        lblFirmwareV.Content = errorLogRecord.ConsoleVersion.CatheterFirmware;       //cathInfo.FirmwareVersion;
                    }
                }

                lblGuiV.Content = errorLogRecord.ConsoleVersion.Software;
                lblDBV.Content = errorLogRecord.ConsoleVersion.DataBaseVersion;
                lblCPLDV.Content = errorLogRecord.ConsoleVersion.CPLDFirmware;

                lblControlFV.Content = "A : " + errorLogRecord.ConsoleVersion.ControlFirmware;
                lblControlBLV.Content = "B : " + errorLogRecord.ConsoleVersion.ControlFirmwareBootLoader;

                lblPatientFV.Content = "A : " + errorLogRecord.ConsoleVersion.PatientFirmware;
                lblPatientBLV.Content = "B : " + errorLogRecord.ConsoleVersion.PatientFirmwareBootLoader;

                lblRepeatorFV.Content = "A : " + errorLogRecord.ConsoleVersion.RepeaterFirmware;
                lblRepeatorBLV.Content = "B : " + errorLogRecord.ConsoleVersion.RepeaterFirmwareBootLoader;

                //if (errorLogRecord.IsUsingRemote)
                //{
                    lblRemoteFV.Content = "A : " + errorLogRecord.ConsoleVersion.RemoteFirmware;
                    lblRemoteBLV.Content = "B : --"; //+ errorLogRecord.ConsoleVersion.RemoteFirmwareBootLoader;
                //}
                //else
                //{
                //    lblRemoteFV.Content = "A : --";
                //    lblRemoteBLV.Content = "B : --";
                //}

                //if (errorLogRecord.IsUsingICB)
                //{
                    lblICBFV.Content = "A : " + errorLogRecord.ConsoleVersion.ICBFirmware;
                    lblICBLV.Content = "B : -- "; //+ errorLogRecord.ConsoleVersion.ICBFirmwareBootLoader;
                //}
                //else
                //{
                //    lblICBFV.Content = "A : --";
                //    lblICBLV.Content = "B : --";
                //}

            }
            }
            catch
            {

            }


        }

        /// <summary>
        /// Handles cancel button event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLogSummaryPopup.StaysOpen = false;
            ErrorLogSummaryPopup.IsOpen = false;
        }

        /// <summary>
        /// Handles summary click event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ErrorLogSummaryClick(object sender, EventArgs e)
        {
           SPGroupByErrorCode.Children.Clear();
           AddChildtoSP(SPGroupByErrorCode);
           SPGroupByDate.Children.Clear();
           AddChildtoSP(SPGroupByDate);
           ErrorLogSummaryPopup.IsOpen  = true;
        }


        /// <summary>
        /// Add Groupby element to StackPanel.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void AddChildtoSP(StackPanel SP)
        {

            var groupby = this.errorLogViewModel.GroupErrorLogListByDate;
            if (SP.Name == "SPGroupByErrorCode")
            {
                groupby = this.errorLogViewModel.GroupErrorLogListByErrorCode;
            }
            int count = groupby.Count;
            if (count > 0) 
            {
                for (int i = 0; i < count; i++)
                {
                    Label l = new Label();
                    
                    l.FontSize = 16;
                    l.HorizontalAlignment = HorizontalAlignment.Center;
                    l.Padding = new Thickness(6, 3, 0, 0);
                    l.Foreground = Brushes.White;
                    l.Width = 270;
                    if (SP.Name == "SPGroupByErrorCode")
                    {
                       
                        l.Content = groupby[i][0].ErrorInformation.ToString() + " Total: " + groupby[i].Count;
                    }
                    else if (SP.Name == "SPGroupByDate")
                    {
                        l.Content = groupby[i][0].ErrorDate.ToString("MMM dd yyy") + " Total error: " + groupby[i].Count;
                    }
                    SP.Children.Add(l);
                }
            }
        }

        private void BtnReloadErrorLog_Click(object sender, RoutedEventArgs e)
        {
          
            LoadErrorLog();
        }
               
    }
}

