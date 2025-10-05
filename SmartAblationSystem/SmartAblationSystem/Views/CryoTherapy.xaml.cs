using FileSerializer;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using static LogSystem.LogService;

namespace SmartAblationSystem.Views
{
  /// <summary>
  /// Interaction logic for CryoTherapy.xaml.
  /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public partial class CryoTherapy
  {
    private CryoTherapyViewModel cryoTherapyViewModel;
    private readonly FileAction fileAction;

    public CryoTherapy()
		{
			InitializeComponent();

      fileAction = new FileAction();
    }

    private void CryoTherapy_OnLoaded(object sender, RoutedEventArgs e)
    {
      DispatcherBeginInvoke(DispatcherPriority.Loaded, () => CryoTherapy_OnLoadedAction(sender, e));
    }

    private void CryoTherapy_OnLoadedAction(object sender, RoutedEventArgs e)
    {
#if Simulator
      CommonViewModel.Current.EcgChannel1And2Reading = 100;
      CommonViewModel.Current.TC1Reading = 25;
#endif
      cryoTherapyViewModel = DataContext as CryoTherapyViewModel;

      CommonViewModel localCommonViewModel = CommonViewModel.Current;
      ProcedureLogModel.IsUserAccessRecord = false;

      try
      {
        cryoTherapyViewModel.ElapsedTimeLastValue = 0;
        cryoTherapyViewModel.ElapsedTimeLastValueForFlowReading = 0;
        cryoTherapyViewModel.ElapsedTimeLastValueForIBPReading = 0;

        cryoTherapyViewModel.SystemState = CommonViewModel.Current.SystemState;
        // cryoTherapyViewModel.TimerProcedureElapsedTime.Start();
        cryoTherapyViewModel.IsTreatmentNumberAndPlayBackVisible = false;
        cryoTherapyViewModel.GasState = Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;

        if (!localCommonViewModel.CanstartDiaphragmMovementMonitoring)
        {
          localCommonViewModel.CanstartDiaphragmMovementMonitoring = true;
          localCommonViewModel.Console.AskForVitalParameters = true;
          CommonViewModel.Current.ResetCanTwoStopWatch();
          CommonViewModel.Current.StopCanTwoStopWatchCommunicationMonitoring();
          Task.Delay(3000).ContinueWith(t => CommonViewModel.Current.StartCanTwoStopWatchCommunicationMonitoring());
        }

        if (!cryoTherapyViewModel.IsFromReturnToProcedure)
        {
          cryoTherapyViewModel.ResetDisplayWithPhysicianPreferences();
          DispatcherBeginInvoke(DispatcherPriority.Normal, () => cryoTherapyViewModel.NotificationsCommand.Execute(this));
        }

        cryoTherapyViewModel.BloodPressureMaximumValueDuringOneSecond = cryoTherapyViewModel.EcgChannel1And2Reading;
        SensorReadingMananger.AllowRemoteControl = true;
        cryoTherapyViewModel.AllowUserToActivateLowFlow = false;
        cryoTherapyViewModel.IsLowFlowActivated = false;

      }
      catch (Exception ex)
      {
        LogException(ex);
      }

      SensorReadingMananger.AllowPlayback = true;
      localCommonViewModel.Console.ConnectTheCanTwo();

      if (cryoTherapyViewModel.TotalTreatmentNumber == 0)
      {
        localCommonViewModel.IsPlayBackModeDeactivted = false;
        cryoTherapyViewModel.PreviuosTotalTreatmentNumber = 0;
      }
      else
      {
        localCommonViewModel.AreSensorsInPlayBackMode = false;
        localCommonViewModel.IsPlayBackModeDeactivted = true;
      }

      localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled = false;
      cryoTherapyViewModel.DASBalloonEnabled = false;
      cryoTherapyViewModel.LockTheFootSwitch = true;
      cryoTherapyViewModel.DiaphragmAmplitudeThresholdReached = false;

      CommonViewModel.Current.SkinToSkinAblationTimer.Stop();

      //Reload Last Information 
      if (ProcedureLogModel.CanReloadProcudreInformation)
      {
        CommonViewModel.Current.CurrentProcedure = ProcedureLogModel.PreviousProcedure;
        CommonViewModel.Current.SkinToSkinAblationTimer.Stop();

        if (ProcedureLogModel.AblationTimersSet.Item2)
        {
          cryoTherapyViewModel.ISTTIFixedTimerSelected = true;
        }
        else if (ProcedureLogModel.AblationTimersSet.Item3)
        {
          cryoTherapyViewModel.ISTTIDurationTimerSelected = true;
        }

        CommonViewModel.Current.AllAblationDataList = ProcedureLogModel.PreviousAllAblationDataList;
        if (ProcedureLogModel.PreviousAllAblationDataList != null)
        {
          int treatmentNumber = ProcedureLogModel.PreviousAllAblationDataList.Count;
          CommonViewModel.Current.AblationSummary = ProcedureLogModel.PreviousAblationSummary;

          cryoTherapyViewModel.IsReloadingPreviuosProcdure = true;
          cryoTherapyViewModel.SkinToSkinDuration = ProcedureLogModel.SkinToSkinDuration;
          cryoTherapyViewModel.RefreshTheInBodyTime();

          if (treatmentNumber != 0)
          {
            AblationInformation.IsThereAbltionHistoricalData = true;
            SensorReadingMananger.AllowPlayback = true;
            cryoTherapyViewModel.IsLastAblationDataLoaded = false;
            cryoTherapyViewModel.TreatmentNumber = treatmentNumber;
            cryoTherapyViewModel.TotalTreatmentNumber = treatmentNumber;
            cryoTherapyViewModel.AblationNumber = treatmentNumber;
            cryoTherapyViewModel.PreviuosTotalTreatmentNumber = treatmentNumber;
            cryoTherapyViewModel.ManageExceptionDataLoading(null);
          }
        }
      }
      else
      {
        ProcedureLogModel.SkinToSkinDuration = 0;
      }

      if (!CommonViewModel.Current.IsCanOneInError)
      {
        CommonViewModel.Current.ReadPMCAndCMCUFirmware(1);
      }

      cryoTherapyViewModel.RefreshUIProperties();
      cryoTherapyViewModel.IsSiteUsingDefalteAfterThaw = CommonViewModel.Current.Console.EnableDefalteAfterThaw;

      string basePath = fileAction.GetBasePath();
      fileAction.CreateNewFolderWithPermission(basePath, "FileStore");

      //AppTrace.Log("CryoTherapy loaded.", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(UserControl_Loaded));
      //   ETSLoad();
    }

    private void CryoTherapy_OnUnloaded(object sender, RoutedEventArgs e)
    {
      CommonViewModel localCommonViewModel = CommonViewModel.Current;

      try
      {

        localCommonViewModel.AreSensorsInPlayBackMode = false;
        SensorReadingMananger.ConnectSensors();

        localCommonViewModel.Console.Stop();
        localCommonViewModel.Console.InjectionDisable();
        Thread.Sleep(10);
        localCommonViewModel.Console.Stop();
        SensorReadingMananger.AllowPlayback = false;
        localCommonViewModel.CanstartDiaphragmMovementMonitoring = false;
        localCommonViewModel.Console.AskForVitalParameters = false;
        localCommonViewModel.Console.DisconnectTheCanTwo();
        Thread.Sleep(500);

        AblationInformation.IsThereAbltionHistoricalData = false;
        localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled = false;
        CommonViewModel.Current.GUIIsRunning = true;

        //Save the skin to skin time
        if(CommonViewModel.Current.SkinToSkinDuration != 0 && CommonViewModel.Current.CurrentProcedure != null)
        {
          short skinToSkinDuration = (short)CommonViewModel.Current.SkinToSkinDuration;

          if(skinToSkinDuration > 0)
          {
            ProcedureLogModel.SkinToSkinDuration = skinToSkinDuration;
            CommonViewModel.Current.SkinToSkinAblationTimer.Start();

            ProcedureLogModel.SkinToSkinDurationBeforeLeavingTheCryoScreen = skinToSkinDuration;
          }

          CommonViewModel.Current.CurrentProcedure.SkinToSkinDuration = skinToSkinDuration;
          CommonViewModel.Current.Data.DataAccess.UpdateProcedure(CommonViewModel.Current.CurrentProcedure);
        }

        //ProcedureLogModel.CanReloadProcudreInformation = false;
        cryoTherapyViewModel.IsReloadingPreviuosProcdure = false;
        cryoTherapyViewModel.IsFromReturnToProcedure = false;

        ProcedureLogModel.PreviousAllAblationDataList = new List<List<AblationDataDetails>>(CommonViewModel.Current.AllAblationDataList);
        ProcedureLogModel.PreviousAblationSummary = CommonViewModel.Current.AblationSummary;

        cryoTherapyViewModel.IsAblationSiteChanged = false;
      }
      catch(Exception ex)
      {
        LogException(ex);
      }
    }

    protected void DispatcherBeginInvoke(DispatcherPriority priority, System.Action action)
    {
      Dispatcher.BeginInvoke(priority, action);
    }
  }
}