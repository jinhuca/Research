using Console;
using Prism.Commands;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using System;
using System.Windows.Input;
using System.Threading.Tasks;

using static SmartAblationSystem.Helpers.Enumeration;

namespace SmartAblationSystem.ViewModels
{

  /// <summary>
    /// This class is the TC and LC calibration View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class TCsAndLCCalibrationViewModel : BindableBase
    {
        public ICommand StartLoadCellCalibrationCommand { get; private set; }
        public ICommand Tank10PoundsSelectedCommand { get; private set; }
        public ICommand Tank15PoundsSelectedCommand { get; private set; }
        public ICommand CalibrationFactorCommand { get; private set; }
        public ICommand ReadBackCalibrationFactorCommand { get; private set; }

        private bool isLoadCellEnabled = false;
        private bool is10PoundTankSelected = false;
        private bool is15PoundTankSelected = true;
        private bool calibrationSucceeded = false;
        private bool isTareCompleted = false;
        private double calibrationFactorValue = 0;
        private const int maxWritingTime = 2;
        private CommonViewModel localCommonViewModel = CommonViewModel.Current;
        private WeightUnit weightUnit;

        private Helpers.Enumeration.TankWeight gasState = Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;

        /// <summary>
        /// This constructor initializes commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public TCsAndLCCalibrationViewModel()
        {
            localCommonViewModel.PropertyChanged += Current_PropertyChanged;

            this.StartLoadCellCalibrationCommand = new DelegateCommand<object>(this.OnStartLoadCellCalibration, this.CanStartLoadCellCalibration);
            this.Tank10PoundsSelectedCommand = new DelegateCommand<object>(this.OnTank10PoundsSelectedCommand, this.CanTank10PoundsSelectedCommand);
            this.Tank15PoundsSelectedCommand = new DelegateCommand<object>(this.OnTank15PoundsSelectedCommand, this.CanTank15PoundsSelectedCommand);
            CalibrationFactorValue = CommonViewModel.Current.Data.DataAccess.LoadCellCalibrationFactor();
            this.CalibrationFactorCommand = new DelegateCommand<object>(this.OnCalibrationFactorCommand, this.CanCalibrationFactorCommand);
            ReadBackCalibrationFactorCommand = new DelegateCommand(this.OnReadBackCalibrationFactor, ()=>true);
        }


        /// <summary>
        /// Function that returns if the system can invoke the calibration factor command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>true</returns>
        private bool CanCalibrationFactorCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles purge command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void  OnCalibrationFactorCommand(object obj)
        {
            CommonViewModel.Current.Data.DataAccess.SetLoadCellCalibrationFactor(CalibrationFactorValue);        
        }

        /// <summary>
        /// Gets or sets the calibration factor value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CalibrationFactorValue
        {
            get
            {
                return calibrationFactorValue;
            }

            set
            {
                calibrationFactorValue = double.Parse(value.ToString("#.0000"));
                RaisePropertyChanged("CalibrationFactorValue");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we purge the console
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CalibrationFactor
        {
            get
            {
                return true; //CommonViewModel.Current;
            }

            set
            {
                CommonViewModel.Current.Console.PurgeTheConsole = value;
                RaisePropertyChanged("PurgeTheConsole");
            }
        }

        private double _loadCellCalibrationFactor = CommonViewModel.Current.LoadCellCalibrationFactor;
        public double LoadCellCalibrationFactor
        {
          get => this._loadCellCalibrationFactor;
          set => SetProperty(ref this._loadCellCalibrationFactor, value);
        }

        private double _loadCellCalibrationOffset=CommonViewModel.Current.LoadCellCalibrationOffset;
        public double LoadCellCalibrationOffset
        {
          get => _loadCellCalibrationOffset;
          set => SetProperty(ref _loadCellCalibrationOffset, value);
        }

        /// <summary>
        /// Function/Command that manages Tank values when a 10 pounds tank is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's argument (not used in this function).</param>
        private void OnTank10PoundsSelectedCommand(object arg)
        {
            if (!IsTareCompleted)
                return;

            DataAccessLayer.Tank tank = new DataAccessLayer.Tank();
            tank.Type = (int)TankType.Tank_10pounds;
            tank.WeightAtReplacementDate = CommonViewModel.Current.LC1Reading;
            tank.WeightAtEndOfUseDate = -1;
            tank.ReplacementDate = DateTime.Now;
            tank.EndOfUseDate = DateTime.MaxValue;

            DataAccessLayer.Tank _tank = CommonViewModel.Current.Data.DataAccess.AddTankToTheConsole(tank);

            if (_tank != null)
            {
                TankBuilder tankBuilder = new TankBuilder(_tank, CommonViewModel.Current.Data);
             
                CommonViewModel.Current.Data.DataAccess.SetCurrentTank(_tank.Id);
                CommonViewModel.Current.CurrentTank = _tank;
                CommonViewModel.Current.Console.Tank.MetalWeight = tankBuilder.MetalWeight;
            }

            Is10PoundsTankSelected = true;
            Is15PoundsTankSelected = false;

            IsTareCompleted = false;
            CalibrationSucceeded = true;
        }

        /// <summary>
        /// Function that returns if the system can invoke the tank 10 pounds selected command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>true</returns>
        public bool CanTank10PoundsSelectedCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that manages Tank values when a 15 pounds tank is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's argument (not used in this function).</param>
        private void OnTank15PoundsSelectedCommand(object arg)
        {
            if (!IsTareCompleted)
                return;

            DataAccessLayer.Tank tank = new DataAccessLayer.Tank();

            tank.Type = (int)TankType.Tank_15pounds;
            tank.WeightAtReplacementDate = CommonViewModel.Current.LC1Reading;
            tank.WeightAtEndOfUseDate = -1;
            tank.ReplacementDate = DateTime.Now;
            tank.EndOfUseDate = DateTime.MaxValue;

            DataAccessLayer.Tank _tank = CommonViewModel.Current.Data.DataAccess.AddTankToTheConsole(tank);

            if (_tank != null)
            {
                TankBuilder tankBuilder = new TankBuilder(_tank, CommonViewModel.Current.Data);

                CommonViewModel.Current.Data.DataAccess.SetCurrentTank(_tank.Id);
                CommonViewModel.Current.CurrentTank = _tank;
                CommonViewModel.Current.Console.Tank.MetalWeight = tankBuilder.MetalWeight;
            }

            Is10PoundsTankSelected = false;
            Is15PoundsTankSelected = true;

            IsTareCompleted = false;
            CalibrationSucceeded = true;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Tank 15 Pounds Selected Command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's argument (not used in this function).</param>
        /// <returns></returns>
        public bool CanTank15PoundsSelectedCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that manages Start Load Cell Calibration Command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's argument (not used in this function).</param>
        private void OnStartLoadCellCalibration(object arg)
        {
            int localCalibrationFactorValue = (int)(CalibrationFactorValue * 10000);

            for (int i = 0; i < maxWritingTime; i++)
            {
                CommonViewModel.Current.Console.CalibrateComponent(CalibrationComponentANDCPLDRegister.CalibrationComponentId.CMCU_Load_Cell, localCalibrationFactorValue);
                System.Threading.Thread.Sleep(20);
            }

            Task.Delay(TimeSpan.FromMilliseconds(2000)).ContinueWith(_ => this.OnReadBackCalibrationFactor());
            IsTareCompleted = true;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Start Load Cell Calibration Command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's argument (not used in this function).</param>
        /// <returns></returns>
        public bool CanStartLoadCellCalibration(object arg)
        {
            return true;
        }

        /// <summary>
        /// This property gets/sets the Load Cell Enabled boolean
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsLoadCellEnabled
        {
            get
            {
                return isLoadCellEnabled;
            }
            set
            {
                isLoadCellEnabled = value;
                RaisePropertyChanged("IsLoadCellEnabled");
            }
        }

        /// <summary>
        /// This property gets/sets the Is 10 Pounds Tank Selected boolean
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool Is10PoundsTankSelected
        {
            get
            {
                return is10PoundTankSelected;
            }
            set
            {
                is10PoundTankSelected = value;
                RaisePropertyChanged("Is10PoundsTankSelected");
            }
        }

        /// <summary>
        /// This property gets/sets the Is 15 Pounds Tank Selected boolean
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool Is15PoundsTankSelected
        {
            get
            {
                return is15PoundTankSelected;
            }
            set
            {
                is15PoundTankSelected = value;
                RaisePropertyChanged("Is15PoundsTankSelected");
            }
        }

        /// <summary>
        /// This property gets/sets the Calibration Succeeded boolean
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CalibrationSucceeded
        {
            get
            {
                return calibrationSucceeded;
            }
            set
            {
                calibrationSucceeded = value;
                RaisePropertyChanged("CalibrationSucceeded");
            }
        }

        /// <summary>
        /// This property gets/sets the Is Tare Completed boolean
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsTareCompleted
        {
            get
            {
                return isTareCompleted;
            }

            set
            {
                isTareCompleted = value;
                RaisePropertyChanged("IsTareCompleted");
            }
        }
        /// <summary>
        /// This property gets/sets the LC1 value
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
        /// Gets/set value for weight unit
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
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
        /// This function resets properties values so the view is reset
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetDisplay()
        {
            Is10PoundsTankSelected = false;
            Is15PoundsTankSelected = true;
            IsLoadCellEnabled = false;
            CalibrationSucceeded = false;

            RaisePropertyChanged("WeightUnit");
        }
        /// <summary>
        /// Occurs when the current proerty changed event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
          CommonViewModel commonviewmodel = sender as CommonViewModel;

          switch (e.PropertyName)
          {
            case nameof(commonviewmodel.LC1Reading):
              RaisePropertyChanged("LC1Reading");
              break;

            case nameof(commonviewmodel.GasState):
              RaisePropertyChanged("GasState");
              break;

            case nameof(commonviewmodel.LoadCellCalibrationFactor):
              this.LoadCellCalibrationFactor = commonviewmodel?.LoadCellCalibrationFactor ?? 0d;
              break;

            case nameof(commonviewmodel.LoadCellCalibrationOffset):
              this.LoadCellCalibrationOffset = commonviewmodel?.LoadCellCalibrationOffset ?? 0d;
              break;
          }
        }

        private void OnReadBackCalibrationFactor()
        {
          CommonViewModel.Current.Console.ReadCalibrateComponent();
        }
    }
}