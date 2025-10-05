using Prism.Commands;
using Prism.Mvvm;
using SmartAblationSystem.Views;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartAblationSystem.ViewModels
{
    internal class MainTreatmentRecordViewModel : BindableBase, IDisposable
    {
        private const string TreatmentRecordName = "TreatmentRecord";
        private const string TreatmentRecordViewName = "Records";
        private const string ReportName = "Report";
        private const string ReportViewName = "Summary Report";

        private readonly TreatmentRecordsViewModel _treatmentRecordViewModel;
        private readonly ViewsEventArgs _viewsEvent;
        public ICommand NavigateToViewCommand { get; }

        public MainTreatmentRecordViewModel()
        {
            TreatmentRecordView = new TreatmentRecords();
            _treatmentRecordViewModel = (TreatmentRecordsViewModel)TreatmentRecordView?.DataContext;
            NavigateToViewCommand = new DelegateCommand<object>(OnNavigateToView, CanNavigateToView);
            if(_treatmentRecordViewModel != null)
            {
                _treatmentRecordViewModel.PropertyChanged += TreatmentTreatmentRecordViewModelPropertyChanged;
                _treatmentRecordViewModel.NavigateToViewCommand = NavigateToViewCommand;
            }
            CurrentMainTreatmentRecordView = TreatmentRecordView;
            _viewsEvent = new ViewsEventArgs();
        }

        private bool _isSummaryReportVisible;
        public bool IsSummaryReportVisible
        {
            get => _isSummaryReportVisible;
            set => SetProperty(ref _isSummaryReportVisible, value);
        }

        private bool _isBackToTreatmentRecordVisible;
        public bool IsBackToTreatmentRecordVisible
        {
            get => _isBackToTreatmentRecordVisible;
            set => SetProperty(ref _isBackToTreatmentRecordVisible, value);
        }

        private UserControl _currentMainTreatmentRecordView;
        public UserControl CurrentMainTreatmentRecordView
        {
            get => _currentMainTreatmentRecordView;
            set => SetProperty(ref _currentMainTreatmentRecordView, value);
        }

        private UserControl _treatmentRecordView;
        public UserControl TreatmentRecordView
        {
            get => _treatmentRecordView;
            set => SetProperty(ref _treatmentRecordView, value);
        }

        private UserControl _reportView;
        private bool disposedValue;

        public UserControl ReportView
        {
            get => _reportView;
            set => SetProperty(ref _reportView, value);
        }

        public TreatmentRecordsViewModel TreatmentRecordViewModel => this._treatmentRecordViewModel;

        private void TreatmentTreatmentRecordViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(TreatmentRecordsViewModel.NavigatedProcedureRecords):
                    IsSummaryReportVisible = _treatmentRecordViewModel.NavigatedProcedureRecords != null;
                    break;
            }
        }

        private void OnNavigateToView(object arg)
        {
            switch(arg.ToString())
            {
                case TreatmentRecordName:
                    if(TreatmentRecordView == null)
                    {
                        TreatmentRecordView = new TreatmentRecords();
                    }
                    CurrentMainTreatmentRecordView = _treatmentRecordView;
                    IsSummaryReportVisible = true;
                    IsBackToTreatmentRecordVisible = false;
                    _treatmentRecordViewModel.SetToTreatment(_treatmentRecordViewModel.TreatmentNumber);
                    _viewsEvent.ViewName = TreatmentRecordViewName;
                    CommonViewModel.Current.OnViewchanged(_viewsEvent);
                    break;
                case ReportName:
                    if(ReportView == null)
                    {
                        ReportView = new Report();
                    }
                    if(CommonViewModel.Current.GenerateAblationSummary())
                    {
                        CurrentMainTreatmentRecordView = ReportView;
                        IsBackToTreatmentRecordVisible = true;
                        IsSummaryReportVisible = false;
                        if(ReportView.DataContext is ReportViewModel reportVm_)
                        {
                            reportVm_.IsPatientInfoVisibilityMutable = false;
                            reportVm_.IsPatientInfoVisible = true;
                        }
                        _viewsEvent.ViewName = ReportViewName;
                        CommonViewModel.Current.OnViewchanged(_viewsEvent);
                    }
                    break;
            }
        }

        private bool CanNavigateToView(object arg) => true;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    _treatmentRecordViewModel.PropertyChanged -= TreatmentTreatmentRecordViewModelPropertyChanged;
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~MainTreatmentRecordViewModel()
        {
            Dispose(disposing: false);
        }

        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}