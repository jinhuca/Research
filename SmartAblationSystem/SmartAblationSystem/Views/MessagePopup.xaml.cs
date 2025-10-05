using System.Windows;

namespace SmartAblationSystem.Views
{
  using System;
  using System.Collections.Generic;

  using SmartAblationSystem.Helpers;
  using SmartAblationSystem.ViewModels;

  /// <summary>
  /// Interaction logic for MessagePopup.xaml
  /// </summary>
  public partial class MessagePopup
  {
    public enum MessageType
    {
      SystemMessage = 0,  // blue
      WarningMessage = 1, // yellow
      ErrorMessage = 2 // Red
    }

    /// <summary>
    /// Button type enumeration.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum ButtonType
    {
      YesNo = 0,
      OkCancel = 1,
      Ok = 2
    }

    private readonly MessagePopupViewModel _viewModel;
    protected MessagePopup()
    {
      InitializeComponent();
      _viewModel = DataContext as MessagePopupViewModel;
    }

    /// <summary>
    /// Initializes the MessagePopup components.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="message">A string representing the message to display.</param>
    /// <param name="messageType">A MessageType enum represenging the message type.</param>
    /// <param name="buttonType">A ButtonType enum representing the button type to display.</param>
    /// <param name="messageTitle">A string representing the message's title.</param>
    public MessagePopup(string message, 
                         MessageType messageType = MessageType.SystemMessage, 
                         ButtonType buttonType = ButtonType.YesNo, 
                         string messageTitle = "")
    : this()
    {
      _viewModel?.InitializeMessageSettings(message, messageType, buttonType, messageTitle);
    }

    /// <summary>
    /// Initializes the MessagePopup components.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="errors">A typle representing the error ID, its message and its solution.</param>
    /// <param name="messageType">A MessageType enum represenging the message type.</param>
    /// <param name="buttonType">A ButtonType enum representing the button type to display.</param>
    /// <param name="messageTitle">A string representing the message's title.</param>
    public MessagePopup(List<Tuple<long, string, string, string>> errors, 
                        MessageType messageType = MessageType.SystemMessage,
                        ButtonType buttonType = ButtonType.YesNo,
                        string messageTitle = "",
                        bool isActionRequired = false,
                        Enumeration.ErrorTypes errorType = Enumeration.ErrorTypes.Unknown)
      : this()
    {
      _viewModel?.InitializeMessageSettings(errors, messageType, buttonType, messageTitle, isActionRequired, errorType);
    }

    /// <summary>
    /// Initializes the MessagePopup components.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="errors">A typle representing the error ID, its message and its solution.</param>
    /// <param name="messageType">A MessageType enum represenging the message type.</param>
    /// <param name="buttonType">A ButtonType enum representing the button type to display.</param>
    /// <param name="messageTitle">A string representing the message's title.</param>
    public MessagePopup(Tuple<long, string, string, string> errors,
                        MessageType messageType = MessageType.SystemMessage,
                        ButtonType buttonType = ButtonType.YesNo,
                        string messageTitle = "",
                        bool isActionRequired = false)
    : this()
    {
      _viewModel?.InitializeMessageSettings(new List<Tuple<long, string, string, string>> { errors }, messageType, buttonType, messageTitle, isActionRequired);
    }

    public void No_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      this.Close();
    }

    public void Yes_Click(object sender, RoutedEventArgs e)
    {
      _viewModel?.ResetVolumeValue(false);
      this._viewModel?.CleanUpForClose();
      
      this.Dispatcher.Invoke(() =>
        {
          try
          {
            DialogResult = true;
            this.Close();
          }
          catch (Exception ex)
          {
            ex.ToString();
          }
        });
    }
  }
}