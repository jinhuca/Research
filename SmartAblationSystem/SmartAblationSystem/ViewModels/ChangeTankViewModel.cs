using Console;
using DataAccessLayer;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using static SmartAblationSystem.Helpers.Enumeration;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using Prism.Commands;
using SmartAblationSystem.Converters;
using SmartAblationSystem.Views;

namespace SmartAblationSystem.ViewModels
{
  /// <summary>
  /// This class is the Change Tank View Model
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  internal class ChangeTankViewModel : BindableBase
  {
    private static readonly string _closeTankWarningMessage = @"Tank is not properly closed. Please watch instruction video,  try again and press NEXT.";
    private static readonly string _closeTankMessageTitle = @"STEP 1 - INCOMPLETE";
    private static Dictionary<ChangeTankStep, string> _stepsVideoInstructions =
      new Dictionary<ChangeTankStep, string>()
      {
          { ChangeTankStep.CloseTank, "CLOSE TANK - HOW TO" },
          { ChangeTankStep.WaitTankPurge, "WAITING TANK PURGE" },
          { ChangeTankStep.ReplaceTank, "REPLACE TANK - HOW TO" },
          { ChangeTankStep.OpenTank, "OPEN TANK - HOW TO" },
          { ChangeTankStep.Completed, "CLOSE TANK - HOW TO" },
      };

    /// <summary>
    /// Change Tank Step enumeration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum ChangeTankStep
    {
      CloseTank = 0,
      WaitTankPurge = 1,
      ReplaceTank = 2,
      OpenTank = 3,
      Completed = 4
    }

    private int expectationTime = 0;
    private readonly int maxPurgTime = 15;
    private int purgTime = 15;
    private int removingTankTime = 3;
    private int replacingTankTime = 3;
    private int maxExpectationTime = 1; // Before it was 1 
    private bool isTimerGreen;
    private bool isRedSegmentEnabled;
    private bool isYellowSegmentEnabled;
    private bool isGreenLeftSegmentEnabled;
    private bool isGreenRightSegmentEnabled;
    private bool isGangerRedSegmentEnabled;
    private bool isGangerGreenSegmentEnabled;

    private bool isRedLightOn = true;
    private bool isGreenLightOn = false;

    private bool isFinishVisible = false;

    private bool isStartVisible = true;
    private bool isWarningVisible = false;
    private bool isCancelVisible = false;

    private bool isTankClosed = false;
    private bool isTankPurged = false;
    private bool isTankReplaced = false;
    private bool isTankOpened = false;
    private bool isResetVisible = false;

    private double pt1InitialValue = 0;
    private const double PT1PurgedValue = 25;
    private const double PT1MinimumDecreaseValue = 50;
    private const double PT1OpenedValue = 500;
    private const double PT1ClosedValue = 600; // if we open the valves the PT1 shall goes at list at 600 PSI after 2 sec

    private double LC1RemovedValue = 1;
    private double LC1PlacedValue = 3;

    private uint svLevel1 = 0;
    private uint svLevel2 = 0;
    private uint svLevel3 = 0;
    private uint svLevel4 = 0;
    private uint svLevel5 = 0;
    private uint svLevel6 = 0;
    private uint svLevel7 = 0;
    private uint svLevel8 = 0;
    private uint svLevel9 = 0;
    private uint fanLevel = 0;
    private uint svLevel10 = 0;
    private uint svLevel11 = 0;

    private bool tank10PoundsVisible = false;
    private bool tank15PoundsVisible = false;
    private bool tankSelectionVisible = false;
    private double selectedTankOpacity = 0.2;
    private string selectedTankWeight = "";

    private bool isTankSelecetd = false;
    private bool istank10PoundsSelected = false;
    private bool istank15PoundsSelected = false;


    public ICommand StartCommand { get; private set; }
    public ICommand FinishCommand { get; private set; }
    public ICommand ResetCommand { get; private set; }
    public ICommand SelectTank { get; private set; }

    private DispatcherTimer pt1Timer = new DispatcherTimer();
    private Helpers.Enumeration.TankWeight gasState = Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;

    private DataAccess dataAccess;

    private Uri videoSourcePath;
    const string VideoFolderName = "VideoTutorial";
    private ChangeTankStep currentStep = ChangeTankStep.CloseTank;
    private List<string> videoPathList;
    private bool isVideoPlaying = false;
    private DispatcherTimer timer;
    private string videoDuration;
    private double minuteDuration = 0;
    private double secondDuration = 0;

    private ViewsEventArgs viewsEvent;
    private WeightUnit weightUnit;

    /// <summary>
    /// This constructor initializes the Change Tank View Model's properties, Timer and commands
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary
    public ChangeTankViewModel()
    {
      this.StartCommand = new DelegateCommand<object>(this.OnStartCommand, this.CanStartCommand);
      this.FinishCommand = new DelegateCommand<object>(this.OnFinishCommand, this.CanFinishCommand);
      this.ResetCommand = new DelegateCommand<object>(this.OnResetCommand, this.CanResetCommand);
      this.SelectTank = new DelegateCommand<object>(this.OnSelectTankCommand, this.CanSelectTankCommand);
      //
      Pt1Timer.Interval = TimeSpan.FromMilliseconds(1000);
      Pt1Timer.Tick += new EventHandler(Pt1Timer_Tick);
      CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;

      this.dataAccess = CommonViewModel.Current.Data.DataAccess;

      InitializeVideoList();

      CurrentStep = ChangeTankStep.CloseTank;

      //Automatically start the first video tutorial
      if (videoPathList != null && videoPathList.Count > 0)
      {
        LoadTutorial(currentStep, false);
      }

      viewsEvent = new ViewsEventArgs();

      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(IsWarningVisible))
        .Select(_ => IsWarningVisible)
        .Where(w => w)
        .ObserveOnDispatcher()
        .Subscribe(_ => ShowWarningMessage());
    }

    private void ShowWarningMessage()
    {
      var messagePopup = new MessagePopup(_closeTankWarningMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, _closeTankMessageTitle);
      messagePopup.ShowDialog();
    }

    /// <summary>
    /// Load tutorial video
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary
    private void LoadTutorial(ChangeTankStep step, bool startTimer)
    {
      try
      {
        CurrentStep = step;

        if (videoPathList != null && videoPathList.Count > 0)
        {
          var videoIndex = CurrentStep == ChangeTankStep.Completed ? 0 : (int)currentStep;
          VideoSourcePath = new Uri(videoPathList[videoIndex], UriKind.RelativeOrAbsolute);
        }
      }

      catch (Exception exception)
      {
        exception.ToString();
      }
    }
    /// <summary>
    /// Initialize video list
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary
    private void InitializeVideoList()
    {
      videoPathList = new List<string>();

      string videoPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                                              VideoFolderName);

      if (!string.IsNullOrWhiteSpace(videoPath) && Directory.Exists(videoPath))
      {
        string[] filesList = Directory.GetFiles(videoPath);

        foreach (string path in filesList)
        {
          videoPathList.Add(path);
        }
      }
    }

    /// <summary>
    /// Get or set video duration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary
    public string VideoDuration
    {
      get
      {
        return videoDuration;
      }
      set
      {
        videoDuration = value;
        RaisePropertyChanged("VideoDuration");
      }
    }

    /// <summary>
    /// Get or set video weight unit
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary
    public Enumeration.WeightUnit WeightUnit
    {
      get
      {
        return weightUnit;
      }
      set
      {
        weightUnit = value;
        RaisePropertyChanged("WeightUnit");
      }
    }

    // private string _currentVideoDescription = _stepsVideoInstructions[ChangeTankStep.CloseTank];
    public string CurrentVideoDescription
    {
      get => _stepsVideoInstructions[currentStep];
    }

    public string HospitalName => dataAccess.GetHospitalName();

    public ChangeTankStep CurrentStep
    {
      get => currentStep;
      set
      {
        SetProperty(ref currentStep, value);
        RaisePropertyChanged(nameof(CurrentVideoDescription));
      }
    }

    /// <summary>
    /// This property gets/sets the Video Source Path value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Uri VideoSourcePath
    {
      get
      {
        return videoSourcePath;
      }
      set
      {
        videoSourcePath = value;
        RaisePropertyChanged("VideoSourcePath");
      }
    }

    /// <summary>
    /// This property gets/sets the Tank Selection Visibility boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool TankSelectionVisible
    {
      get
      {
        return tankSelectionVisible;
      }
      set
      {
        tankSelectionVisible = value;
        RaisePropertyChanged("TankSelectionVisible");
      }
    }

    /// <summary>
    /// This property gets/sets the Tank 10 Pounds Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool Tank10PoundsVisible
    {
      get
      {
        return tank10PoundsVisible;
      }
      set
      {
        tank10PoundsVisible = value;
        RaisePropertyChanged("Tank10PoundsVisible");
      }
    }

    /// <summary>
    /// This property gets/sets the Tank 15 Pounds Visibility boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool Tank15PoundsVisible
    {
      get
      {
        return tank15PoundsVisible;
      }
      set
      {
        tank15PoundsVisible = value;
        RaisePropertyChanged("Tank15PoundsVisible");
      }
    }

    /// <summary>
    /// Function that is invoked at each PT1 Timer Tick.  It handles the Tank change flow states and actions
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments.</param>
    private void Pt1Timer_Tick(object sender, EventArgs e)
    {
      ExpectationTime++;


      if (ChangeTankFSM.CurrentState == TankStates.Tank_Opened)
      {
        //ChangeTankFSM.CurrentState = TankStates.Tank_Closing;
      }
      else if (ChangeTankFSM.CurrentState == TankStates.Tank_Closing)
      {
        IsFinishVisible = false;

        IsStartVisible = false;


        if (((Pt1InitialValue - PT1Reading) >= PT1MinimumDecreaseValue || PT1Reading < PT1PurgedValue) && (ExpectationTime >= maxExpectationTime && !IsTankClosed))
        {
          IsTankClosed = true;
          ChangeTankFSM.CurrentState = TankStates.Tank_Closed;
          PurgTime = maxPurgTime;
          SetTankGPIOLevel(false);
          return;
        }
        else if (ExpectationTime >= maxExpectationTime && ((Pt1InitialValue - PT1Reading) <= PT1MinimumDecreaseValue))
        {
          SetTankGPIOLevel(false);
          Pt1Timer.Stop();
          IsStartVisible = true;
          IsWarningVisible = true;
          PurgTime = maxPurgTime;
          LoadTutorial(ChangeTankStep.CloseTank, false);
          return;
        }
      }
      else if (ChangeTankFSM.CurrentState == TankStates.Tank_Closed && ChangeTankFSM.CurrentState != TankStates.Tank_Purging)
      {
        PurgTime--;
        IsFinishVisible = false;
        IsStartVisible = false;
        ChangeTankFSM.CurrentState = TankStates.Tank_Purging;
        SetTankGPIOLevel(true);
        IsTankClosed = true;
        //IsSytemRepurged = false;
        return;
      }
      else if (ChangeTankFSM.CurrentState == TankStates.Tank_Purging)
      {
        PurgTime--;
        IsStartVisible = false;
        IsTankClosed = true;

        if (PurgTime <= 13 && PT1Reading > PT1ClosedValue)  // after  2 sec purge the PurgeTime = 13
        {
          SetTankGPIOLevel(false);
          Pt1Timer.Stop();
          IsStartVisible = true;
          IsWarningVisible = true;
          PurgTime = maxPurgTime;
          ResetWithoutRemovingTheWarnning();
          return;
        }

        // We want at list wait for 10 sec

        if ((PT1Reading <= PT1PurgedValue && PurgTime < 10) || PurgTime == 0) //Purge at list for 5 sec
        {


          if (PurgTime == 0 && PT1Reading > PT1PurgedValue)
          {
            SetTankGPIOLevel(false);
            Pt1Timer.Stop();
            IsStartVisible = true;
            IsWarningVisible = true;
            PurgTime = maxPurgTime;
            ResetWithoutRemovingTheWarnning();
            return;
          }

          IsFinishVisible = false;
          ChangeTankFSM.CurrentState = TankStates.Tank_Purged;
          SetTankGPIOLevel(false);
          return;
        }

      }
      else if (ChangeTankFSM.CurrentState == TankStates.Tank_Purged)
      {
        IsRedLightOn = false;
        IsGreenLightOn = true;
        IsTankClosed = true;
        IsTankPurged = true;

        //When tank is purged, display next tutorial video
        LoadTutorial(ChangeTankStep.ReplaceTank, true);

        IsTimerGreen = true;
        ChangeTankFSM.CurrentState = TankStates.TanK_Removing;
        return;
      }
      else if (ChangeTankFSM.CurrentState == TankStates.TanK_Removing)
      {
        IsTankClosed = true;
        IsTankPurged = true;

        if (LC1Reading < LC1RemovedValue && IsTankClosed)
        {
          RemovingTankTime--;

          if (RemovingTankTime <= 0)
            ChangeTankFSM.CurrentState = TankStates.Tank_Removed;

          return;
        }
      }
      else if (ChangeTankFSM.CurrentState == TankStates.Tank_Removed)
      {
        IsTankClosed = true;
        IsTankPurged = true;

        if (LC1Reading > LC1RemovedValue && IsTankClosed)
        {
          ChangeTankFSM.CurrentState = TankStates.Tank_Replacing;
          return;
        }

        //Allow the user to select Tank type.
        if (SelectedTankWeight == "")
        {
          //No tank has been selected
          TankSelectionVisible = true;
          Tank10PoundsVisible = true;
          Tank15PoundsVisible = true;
          SelectedTankOpacity = 0.2;
        }
        else
        {
          //A tank has been selected
          TankSelectionVisible = true;
          Tank10PoundsVisible = SelectedTankWeight.Contains("15");
          Tank15PoundsVisible = SelectedTankWeight.Contains("10");
          SelectedTankOpacity = 1;
        }
      }
      else if (ChangeTankFSM.CurrentState == TankStates.Tank_Replacing)
      {
        if (LC1Reading > LC1PlacedValue && IsTankClosed && IsTankSelecetd)
        {
          ReplacingTankTime--;
          if (ReplacingTankTime <= 0)
          {
            ChangeTankFSM.CurrentState = TankStates.Tank_Replaced;
            IsTankReplaced = true;

            if (Istank10PoundsSelected)
            {
              CommonViewModel.Current.LC1Reading = 10;
            }
            else if (Istank15PoundsSelected)
            {
              CommonViewModel.Current.LC1Reading = 15;
            }

            return;
          }
          else
          {
            return;
          }
        }
        else
        {

          //No tank has been selected
          TankSelectionVisible = true;
          Tank10PoundsVisible = true;
          Tank15PoundsVisible = true;
          SelectedTankOpacity = 0.2;
        }
      }
      else if (ChangeTankFSM.CurrentState == TankStates.Tank_Replaced)
      {
        if (LC1Reading > LC1PlacedValue && IsTankReplaced)
        {
          ChangeTankFSM.CurrentState = TankStates.Tank_Openning;
          IsTankReplaced = true;

          //When tank is replaced, display next tutorial video
          LoadTutorial(ChangeTankStep.OpenTank, true);
          return;
        };
      }
      else if (ChangeTankFSM.CurrentState == TankStates.Tank_Openning)
      {
        if (PT1Reading > PT1OpenedValue && IsTankReplaced)
        {
          ChangeTankFSM.CurrentState = TankStates.Tank_Opened;
          IsTankOpened = true;

          LoadTutorial(ChangeTankStep.Completed, false);

          Pt1Timer.Stop();
          IsStartVisible = false;
          IsFinishVisible = true;
          PurgTime = maxPurgTime;
          IsGreenLightOn = false;
          IsRedLightOn = true;
          Tank15PoundsVisible = false;
          Tank10PoundsVisible = false;
          TankSelectionVisible = false;
          return;
        }
      }
    }

    /// <summary>
    /// This read-only property returns the Current Tank value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public DataAccessLayer.Tank CurrentTank
    {
      get
      {
        return CommonViewModel.Current.CurrentTank;
      }
    }

    private TankType _selectedTankType = TankType.Unknown;

    public TankType SelectedTankType
    {
      get => _selectedTankType;
      set => SetProperty(ref _selectedTankType, value);
    }

    /// <summary>
    /// Function that returns if the system can invoke the Start command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanStartCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Change Tank operation Start when the Start
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command's parameter (not used in this function).</param>
    private void OnStartCommand(object obj)
    {
      ExpectationTime = 0;
      RemovingTankTime = 3;
      ReplacingTankTime = 3;

      Pt1Timer.Start();
      CommonViewModel.Current.Console.GUIInMaintenanceMode = true;
      Pt1InitialValue = CommonViewModel.Current.PT1Reading;


      IsWarningVisible = false;
      SetTankGPIOLevel(true);
      ChangeTankFSM.CurrentState = TankStates.Tank_Closing;

      IsTankSelecetd = false;

      Istank10PoundsSelected = false;
      Istank15PoundsSelected = false;

      TankSelectionVisible = false;
      Tank10PoundsVisible = false;
      Tank15PoundsVisible = false;

      PurgTime = maxPurgTime;

      if (Pt1InitialValue > PT1PurgedValue)
      {
        ChangeTankFSM.CurrentState = TankStates.Tank_Closing;
        LoadTutorial(ChangeTankStep.WaitTankPurge, true);
      }
      else
      {
        ChangeTankFSM.CurrentState = TankStates.Tank_Purging;
      }

    }

    /// <summary>
    /// Function that returns if the system can invoke the Finish command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanFinishCommand(object obj)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Change Tank operation Finish when the Finish
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command's parameter (not used in this function).</param>
    private void OnFinishCommand(object obj)
    {
      IsTankClosed = false;
      IsTankPurged = false;
      IsTankReplaced = false;
      IsTankOpened = false;
      IsFinishVisible = false;
      IsStartVisible = true;
      IsRedLightOn = true;
      IsGreenLightOn = false;
      PurgTime = maxPurgTime;

      Istank10PoundsSelected = false;
      Istank15PoundsSelected = false;

      CommonViewModel.Current.Console.GUIInMaintenanceMode = false;

      viewsEvent.ViewName = "Home";
      CommonViewModel.Current.OnViewchanged(viewsEvent);
    }

    /// <summary>
    /// Function that returns if the system can invoke the Reset command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanResetCommand(object obj)
    {
      return true;
    }


    private void ResetSettings(bool resetWarning = true)
    {
      Pt1Timer.Stop();
      IsTankClosed = false;
      IsTankPurged = false;
      IsTankReplaced = false;
      IsTankOpened = false;
      IsStartVisible = true;
      IsFinishVisible = false;
      IsWarningVisible = !resetWarning;
      IsRedLightOn = true;
      IsGreenLightOn = false;

      SelectedTankWeight = "";
      Tank10PoundsVisible = false;
      Tank15PoundsVisible = false;
      TankSelectionVisible = false;

      IsTankSelecetd = false;

      Istank10PoundsSelected = false;
      Istank15PoundsSelected = false;
      SelectedTankType = TankType.Unknown;

      PurgTime = maxPurgTime;

      //Reset videos
      CurrentStep = ChangeTankStep.CloseTank;
      LoadTutorial(currentStep, true);

    }
    /// <summary>
    /// Function/Command that handles the Reset operations when the Reset
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command's parameter (not used in this function).</param>
    private void OnResetCommand(object obj)
    {
      ResetSettings();
    }

    /// <summary>
    /// Rest the system warning
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void ResetWithoutRemovingTheWarnning()
    {
      ResetSettings(false);
    }

    /// <summary>
    /// Function that returns if the system can invoke the Select Tank command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanSelectTankCommand(object obj)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Tank Selection when the Select Tank
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="tank">The Tank size (string).</param>
    private void OnSelectTankCommand(object tank)
    {
      string tankSize = "";

      if (tank is string && !string.IsNullOrWhiteSpace((string)tank))
      {
        IsTankSelecetd = true;

        tankSize = (string)tank;

        SelectedTankOpacity = 1;

        if (tankSize == "10")
        {
          DataAccessLayer.Tank newTank = new DataAccessLayer.Tank();

          newTank.Type = (int)TankType.Tank_10pounds;
          newTank.WeightAtReplacementDate = CommonViewModel.Current.LC1Reading;
          newTank.WeightAtEndOfUseDate = -1;
          newTank.ReplacementDate = DateTime.Now;
          newTank.EndOfUseDate = DateTime.MaxValue;

          DataAccessLayer.Tank _tank = CommonViewModel.Current.Data.DataAccess.AddTankToTheConsole(newTank);

          if (_tank != null)
          {
            TankBuilder tankBuilder = new TankBuilder(_tank, CommonViewModel.Current.Data);

            CommonViewModel.Current.Data.DataAccess.SetCurrentTank(_tank.Id);
            CommonViewModel.Current.CurrentTank = _tank;
            CommonViewModel.Current.Console.Tank.MetalWeight = tankBuilder.MetalWeight;
          }

          Tank10PoundsVisible = false;
          Tank15PoundsVisible = true;

          Istank10PoundsSelected = true;
          Istank15PoundsSelected = false;
        }
        else if (tankSize == "15")
        {
          DataAccessLayer.Tank newTank = new DataAccessLayer.Tank();

          newTank.Type = (int)TankType.Tank_15pounds;
          newTank.WeightAtReplacementDate = CommonViewModel.Current.LC1Reading;
          newTank.WeightAtEndOfUseDate = -1;
          newTank.ReplacementDate = DateTime.Now;
          newTank.EndOfUseDate = DateTime.MaxValue;

          DataAccessLayer.Tank _tank = CommonViewModel.Current.Data.DataAccess.AddTankToTheConsole(newTank);

          if (_tank != null)
          {
            TankBuilder tankBuilder = new TankBuilder(_tank, CommonViewModel.Current.Data);

            CommonViewModel.Current.Data.DataAccess.SetCurrentTank(_tank.Id);
            CommonViewModel.Current.CurrentTank = _tank;
            CommonViewModel.Current.Console.Tank.MetalWeight = tankBuilder.MetalWeight;
          }

          Tank10PoundsVisible = true;
          Tank15PoundsVisible = false;

          Istank10PoundsSelected = false;
          Istank15PoundsSelected = true;
        }

        FieldToTextConverter converter = new FieldToTextConverter();

        if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
        {
          if (tankSize == "10")
          {
            SelectedTankWeight = (string)converter.Convert("TenPoundsLabel", null, null, null);
          }
          else
          {
            SelectedTankWeight = "BSC"; // (string)converter.Convert("FifteenPoundsLabel", null, null, null);
          }
        }
        else
        {
          if (tankSize == "10")
          {
            SelectedTankWeight = "4.5 Kg";
          }
          else
          {
            SelectedTankWeight = "BSC"; // 6.8 Kg";
          }

        }

        CommonViewModel.Current.initializeLoadCellRegisters();

        CommonViewModel.Current.SendTheLC1Thresholds();  // we suppose that  the console has at least one tank selected
      }
    }

    /// <summary>
    /// Update loading cell threshold values
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void UpdateLoadCellThresholdValues(Enumeration.TankType tankType)
    {


      DataAccessLayer.CatheterType catheterType = this.dataAccess.GetCatheterAccordingToCatheterId(CommonViewModel.Current.CatheterID);

      if (tankType == TankType.Tank_15pounds)
      {

      }

      else
      {

      }

      if (catheterType != null)
      {
        for (int state = 1; state < 8; state++)
        {
          this.dataAccess.UpdateLoadCellThresholdValues(state, 1, 1, 1, 1, catheterType.ID);
        }
      }
    }

    /// <summary>
    /// This property gets/sets Select Tank Weight value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string SelectedTankWeight
    {
      get
      {
        return selectedTankWeight;
      }
      set
      {
        selectedTankWeight = value;
        RaisePropertyChanged("SelectedTankWeight");
      }
    }

    /// <summary>
    /// This property gets/sets the Selected Tank opacity value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double SelectedTankOpacity
    {
      get
      {
        return selectedTankOpacity;
      }
      set
      {
        selectedTankOpacity = value;
        RaisePropertyChanged("SelectedTankOpacity");
      }
    }

    /// <summary>
    /// This property gets/sets Expectation Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ExpectationTime
    {
      get
      {
        return expectationTime;
      }

      set
      {
        expectationTime = value;
        RaisePropertyChanged("ExpectationTime");
      }
    }

    /// <summary>
    /// This property gets/sets Timer Green boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTimerGreen
    {
      get
      {
        return isTimerGreen;
      }
      set
      {
        if (value != isTimerGreen)
        {
          isTimerGreen = value;
          RaisePropertyChanged("IsTimerGreen");
        }
      }
    }

    /// <summary>
    /// This property gets/sets Red Segment Enabled boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsRedSegmentEnabled
    {
      get
      {
        return isRedSegmentEnabled;
      }
      set
      {
        isRedSegmentEnabled = value;
        RaisePropertyChanged("IsRedSegmentEnabled");
      }
    }

    /// <summary>
    /// This property gets/sets the Yellow Segment Enabled boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsYellowSegmentEnabled
    {
      get
      {
        return isYellowSegmentEnabled;
      }

      set
      {
        isYellowSegmentEnabled = value;
        RaisePropertyChanged("IsYellowSegmentEnabled");
      }
    }

    /// <summary>
    /// This property gets/sets the Green Left Segment Enabled boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsGreenLeftSegmentEnabled
    {
      get
      {
        return isGreenLeftSegmentEnabled;
      }
      set
      {
        isGreenLeftSegmentEnabled = value;
        RaisePropertyChanged("IsGreenLeftSegmentEnabled");
      }
    }

    /// <summary>
    /// This property gets/sets the Green Right Segment Enabled boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsGreenRightSegmentEnabled
    {
      get
      {
        return isGreenRightSegmentEnabled;
      }

      set
      {
        isGreenRightSegmentEnabled = value;
        RaisePropertyChanged("IsGreenRightSegmentEnabled");
      }
    }

    /// <summary>
    /// This property gets/sets the Ganger Red Segment Enabled boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsGangerRedSegmentEnabled
    {
      get
      {
        return isGangerRedSegmentEnabled;
      }

      set
      {
        isGangerRedSegmentEnabled = value;
        RaisePropertyChanged("IsGangerRedSegmentEnabled");
      }
    }

    /// <summary>
    /// This property gets/sets the Ganger Green Segment Enabled boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsGangerGreenSegmentEnabled
    {
      get
      {
        return isGangerGreenSegmentEnabled;
      }

      set
      {
        isGangerGreenSegmentEnabled = value;
        RaisePropertyChanged("IsGangerGreenSegmentEnabled");
      }
    }

    /// <summary>
    /// This property gets/sets the Red Light On boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsRedLightOn
    {
      get
      {
        return isRedLightOn;
      }
      set
      {
        if (value != isRedLightOn)
        {
          isRedLightOn = value;
          RaisePropertyChanged("IsRedLightOn");
        }
      }
    }

    /// <summary>
    /// This property gets/sets the PT1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT1Reading
    {
      get
      {
        return CommonViewModel.Current.PT1Reading;
      }
      set
      {
        CommonViewModel.Current.PT1Reading = value;
        RaisePropertyChanged("PT1Reading");
      }
    }

    /// <summary>
    /// This property gets/sets the Gas State value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Helpers.Enumeration.TankWeight GasState
    {
      get
      {
        return gasState;
      }

      set
      {
        gasState = value;
        RaisePropertyChanged("GasState");
      }
    }

    /// <summary>
    /// This property gets/sets the LC1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LC1Reading
    {
      get
      {
        return CommonViewModel.Current.LC1Reading;
      }
      set
      {
        CommonViewModel.Current.LC1Reading = value;
        RaisePropertyChanged("LC1Reading");
      }
    }

    /// <summary>
    /// This property gets/sets the Green Light On boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsGreenLightOn
    {
      get
      {
        return isGreenLightOn;
      }
      set
      {
        if (value != isGreenLightOn)
        {
          isGreenLightOn = value;
          RaisePropertyChanged("IsGreenLightOn");
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Finish Visible boolean Flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsFinishVisible
    {
      get
      {
        return isFinishVisible;
      }

      set
      {
        if (value != isFinishVisible)
        {
          isFinishVisible = value;
          RaisePropertyChanged("IsFinishVisible");
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Start Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsStartVisible
    {
      get
      {
        return isStartVisible;
      }
      set
      {
        isStartVisible = value;
        RaisePropertyChanged("IsStartVisible");
        IsCancelVisible = !value;

      }
    }

    /// <summary>
    /// This property gets/sets the Tank Closed boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTankClosed
    {
      get
      {
        return isTankClosed;
      }
      set
      {

        isTankClosed = value;
        RaisePropertyChanged("IsTankClosed");

      }
    }

    /// <summary>
    /// This property gets/sets the Tank Purged boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTankPurged
    {
      get
      {
        return isTankPurged;
      }
      set
      {

        isTankPurged = value;
        RaisePropertyChanged("IsTankPurged");

      }
    }

    /// <summary>
    /// This property gets/sets the Purge Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int PurgTime
    {
      get
      {
        return purgTime;
      }
      set
      {
        if (value < 0)
          purgTime = 0;
        else
        {
          purgTime = value;
          RaisePropertyChanged("PurgTime");
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Tank Replaced boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTankReplaced
    {
      get
      {
        return isTankReplaced;
      }
      set
      {
        isTankReplaced = value;
        RaisePropertyChanged("IsTankReplaced");
      }
    }

    /// <summary>
    /// This property gets/sets the Tank Opened boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTankOpened
    {
      get
      {
        return isTankOpened;
      }
      set
      {
        isTankOpened = value;
        RaisePropertyChanged("IsTankOpened");
      }
    }

    /// <summary>
    /// This property gets/sets the PT1 Initial value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double Pt1InitialValue
    {
      get
      {
        return pt1InitialValue;
      }

      set
      {
        pt1InitialValue = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Reset Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsResetVisible
    {
      get
      {
        return isResetVisible;
      }
      set
      {
        isResetVisible = value;
        RaisePropertyChanged("IsResetVisible");
      }
    }

    /// <summary>
    /// This property gets/sets the Warning Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsWarningVisible
    {
      get => isWarningVisible;
      set => SetProperty(ref isWarningVisible, value);
    }

    /// <summary>
    /// This property gets/sets the Sv Level 7 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel7
    {
      get
      {
        return SvLevel71;
      }

      set
      {
        SvLevel71 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 1 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel1
    {
      get
      {
        return svLevel1;
      }

      set
      {
        svLevel1 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 2 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel2
    {
      get
      {
        return svLevel2;
      }

      set
      {
        svLevel2 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 3 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel3
    {
      get
      {
        return svLevel3;
      }

      set
      {
        svLevel3 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 4 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel4
    {
      get
      {
        return svLevel4;
      }

      set
      {
        svLevel4 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 5 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel5
    {
      get
      {
        return svLevel5;
      }

      set
      {
        svLevel5 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 6 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel6
    {
      get
      {
        return svLevel6;
      }

      set
      {
        svLevel6 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 71 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel71
    {
      get
      {
        return svLevel7;
      }

      set
      {
        svLevel7 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 8 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel8
    {
      get
      {
        return svLevel8;
      }

      set
      {
        svLevel8 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 9 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel9
    {
      get
      {
        return svLevel9;
      }

      set
      {
        svLevel9 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Fan Level value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint FanLevel
    {
      get
      {
        return fanLevel;
      }

      set
      {
        fanLevel = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 10 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel10
    {
      get
      {
        return svLevel10;
      }

      set
      {
        svLevel10 = value;
      }
    }

    /// <summary>
    /// This property gets/sets the Sv Level 11 value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint SvLevel11
    {
      get
      {
        return svLevel11;
      }

      set
      {
        svLevel11 = value;
      }
    }
    /// <summary>
    /// Get or set isCancelVisible boolean value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsCancelVisible
    {
      get
      {
        return isCancelVisible;
      }

      set
      {
        isCancelVisible = value;
        RaisePropertyChanged("IsCancelVisible");
      }
    }

    //public bool IsSytemRepurged { get => isSytemRepurged; set => isSytemRepurged = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the a tank is selecetd
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTankSelecetd
    {
      get => isTankSelecetd;
      set => isTankSelecetd = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the the 10 Pounds tank is selected
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool Istank10PoundsSelected
    {
      get => istank10PoundsSelected;
      set => istank10PoundsSelected = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the the 15 Pounds tank is selected
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool Istank15PoundsSelected
    {
      get => istank15PoundsSelected;
      set => istank15PoundsSelected = value;
    }

    /// <summary>
    /// Gets or sets the PT1 timer
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public DispatcherTimer Pt1Timer
    {
      get => pt1Timer;
      set => pt1Timer = value;
    }

    /// <summary>
    /// Gets or sets the timer
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public DispatcherTimer Timer
    {
      get => timer;
      set => timer = value;
    }

    /// <summary>
    /// Gets or sets a value indicating  the removing tank time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int RemovingTankTime
    {
      get => removingTankTime;
      set => removingTankTime = value;
    }

    /// <summary>
    /// Gets or sets a value indicating the replacing tank time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ReplacingTankTime
    {
      get => replacingTankTime;
      set => replacingTankTime = value;
    }

    /// <summary>
    /// Function that sets the Tank GPIO Level
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="IsActivatingTheSv1Sv6andSv7">Boolean if activating the SV 7.</param>
    private void SetTankGPIOLevel(bool IsActivatingTheSv1Sv6andSv7)
    {
      uint valuesCombinationTosend = 0;

      //Before Activating sv7 first we open sv6. we are using the same state  for sv 7, sv6 and sv1

      //SvLevel1 = ((IsActivatingTheSv1Sv6andSv7 == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV1 : (uint)0);

      //SvLevel6 = ((IsActivatingTheSv1Sv6andSv7 == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV6 : (uint)0);

      SvLevel7 = ((IsActivatingTheSv1Sv6andSv7 == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.Sv7 : (uint)0);

      valuesCombinationTosend = SvLevel1 | SvLevel2 | SvLevel3 | SvLevel4 | SvLevel5 | SvLevel6 | SvLevel7 | SvLevel8 | SvLevel9 | FanLevel | SvLevel10 | SvLevel11;

      CommonViewModel.Current.Console.SetCPLDSVLevel(valuesCombinationTosend);
    }

    /// <summary>
    /// This function handles the sender's PropertyChanged event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments.</param
    private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      CommonViewModel commonviewmodel = sender as CommonViewModel;
      switch (e.PropertyName)
      {
        case "PT1Reading":
          RaisePropertyChanged("PT1Reading");
          break;

        case "LC1Reading":
          RaisePropertyChanged("LC1Reading");
          break;

        case "CurrentTank":
          RaisePropertyChanged("CurrentTank");
          break;
      }
    }
  }
}