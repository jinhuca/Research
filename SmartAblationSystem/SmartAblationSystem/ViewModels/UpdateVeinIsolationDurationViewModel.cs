using DataAccessLayer;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SmartAblationSystem.ViewModels
{
  /// <summary>
  /// This class is the UpdateVeinIsolationDuration View Model
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class UpdateVeinIsolationDurationViewModel : BindableBase
  {
    private int maxDuration = 0;

    /// <summary>
    /// This constructor initializes the UpdateVeinIsolationDurationViewModel properties and commands
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public UpdateVeinIsolationDurationViewModel()
    {
      CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;
    }

    /// <summary>
    /// This function sets the current and max vein isolation duration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="currentDuration">The current vein isolation duration.</param>
    /// <param name="maxDuration">The maximum vein isolation duration.</param>
    public void SetCurrentAndMaxDuration(int currentDuration, int maxDuration)
    {
      this.maxDuration = maxDuration;
      this.VeinIsolationDuration = currentDuration.ToString();
    }

    /// <summary>
    /// This function handles the sender's PropertyChanged event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments.</param>
    private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      switch(e.PropertyName)
      {
        case "PhysicianList":
          RaisePropertyChanged("PhysiciansList");
          break;
      }
    }

    private string veinIsolationDuration = string.Empty;

    /// <summary>
    /// This property gets/sets Vein Isolation Duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string VeinIsolationDuration
    {
      get => veinIsolationDuration;

      set
      {
        SetProperty(ref veinIsolationDuration, value);
        ValidateInformation();
      }
    }

    private bool isInfoValid;

    /// <summary>
    /// This property gets/sets if the information is valid
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsInfoValid
    {
      get => isInfoValid;
      set => SetProperty(ref isInfoValid, value);
    }

    /// <summary>
    /// Function that validates the Patient information
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void ValidateInformation()
    {
      var isNumeric = int.TryParse(VeinIsolationDuration, out var duration);
      IsInfoValid = isNumeric && duration >= 0 && duration <= maxDuration;
    }
  }
}