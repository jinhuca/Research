using SmartAblationSystem.Views;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles the ErrorWarningAndMessage.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ErrorWarningAndMessage
    {
        /// <summary>
        /// Initializes a new instance of the ErrorWarningAndMessage class and sets all the properties to their initial values. 
        /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void DisplayErrorMessage(string error)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
	            MessagePopup dialogPopup = new MessagePopup(error, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok);
	            dialogPopup.ShowDialog();
            });
        }

        public MessagePopup DisplayWarningMessage(List<Tuple<long, string, string, string>> errors)
        {
            MessagePopup dialogPopup = new MessagePopup(errors, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok,"",true,Enumeration.ErrorTypes.Unknown);
            Application.Current.Dispatcher.InvokeAsync(() =>
            {

                dialogPopup.ShowDialog();
            });
            return dialogPopup;
        }



    }


    public class MessagePopupHandler
    {

        private Thread StatusThread = null;

        private MessagePopup Popup = null;

        public void Start(List<Tuple<long, string, string, string>> errors)
        {
            //create the thread with its ThreadStart method
            this.StatusThread = new Thread(() =>
            {
                try
                {
                    this.Popup = new MessagePopup(errors, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "", true, Enumeration.ErrorTypes.Unknown); ;
                    this.Popup.Show();
                    this.Popup.Closed += (sender, e) =>
                    {
                        //when the window closes, close the thread invoking the shutdown of the dispatcher
                        this.Popup.Dispatcher.InvokeShutdown();
                        this.Popup = null;
                        this.StatusThread = null;
                    };

                    //this call is needed so the thread remains open until the dispatcher is closed
                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
            });

            //run the thread in STA mode to make it work correctly
            this.StatusThread.SetApartmentState(ApartmentState.STA);
            this.StatusThread.Priority = ThreadPriority.Normal;
            this.StatusThread.Start();
        }

        public void Stop()
        {
            if (this.Popup != null)
            {
                //need to use the dispatcher to call the Close method, because the window is created in another thread, and this method is called by the main thread
                this.Popup.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Popup.Close();
                }));
            }
        }
    }

}