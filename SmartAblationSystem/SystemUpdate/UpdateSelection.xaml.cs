using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SystemUpdate.Utils.Class1;

namespace SystemUpdate
{
    /// <summary>
    /// Interaction logic for UpdateSelection.xaml
    /// </summary>
    public partial class UpdateSelection : Window
    {

        public ObservableCollection<BoolStringClass> TheList { get; set; }
        public List<UpdateObj> Updates;
        private List<string> RemovedUpdates;
        public UpdateSelection(List<UpdateObj> allUpdates)
        {
            Updates = allUpdates;
            RemovedUpdates = new List<string>();
            InitializeComponent();
            CreateCheckBoxList(allUpdates);
        }
        public class BoolStringClass
        {
            public string TheText { get; set; }
            public int TheValue { get; set; }
        }
        
        public void CreateCheckBoxList(List<UpdateObj> allUpdates)
        {
            TheList = new ObservableCollection<BoolStringClass>();
            
            int i = 0;
            foreach (UpdateObj obj in allUpdates)
            {
                if(obj.type == "gui")
                {
                    TheList.Add(new BoolStringClass { TheText = "Update GUI", TheValue = i });
                }
                i++;
                TheList.Add(new BoolStringClass { TheText = obj.title, TheValue = i });
                i++;
            }
            this.DataContext = this;
        }
        private void CheckBoxZone_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            if((string)chkZone.Content == "Update GUI")
            {
                var item = listBoxZone.ItemContainerGenerator.ContainerFromItem(listBoxZone.Items[(int)chkZone.Tag+1]) as ListBoxItem;
                var template = item.ContentTemplate as DataTemplate;

                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(item);

                CheckBox myCheckBox = (CheckBox)template.FindName("CheckBoxZone", myContentPresenter);

                myCheckBox.IsChecked = true;

            }
            RemovedUpdates.Remove((string)chkZone.Content);
        }
        private void CheckBoxZone_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            if ((string)chkZone.Content == "Update GUI")
            {
                var item = listBoxZone.ItemContainerGenerator.ContainerFromItem(listBoxZone.Items[(int)chkZone.Tag + 1]) as ListBoxItem;
                var template = item.ContentTemplate as DataTemplate;

                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(item);

                CheckBox myCheckBox = (CheckBox)template.FindName("CheckBoxZone", myContentPresenter);

                myCheckBox.IsChecked = false;
            }
            RemovedUpdates.Add((string)chkZone.Content);
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            //Confirm updates

            foreach (string name in RemovedUpdates)
            {
                UpdateObj upd = Updates.FindLast(x => x.title.Equals(name));
                Updates.Remove(upd);
            }

            this.Close();
        }
    }
}
