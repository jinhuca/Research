using System.Windows;
using System.Windows.Input;
using SmartAblationSystem.ViewModels;

namespace SmartAblationSystem.Views
{
	public partial class Home
  {
		public Home()
		{
			InitializeComponent();
		}


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      this.Opacity = 0.5;
      //var vm_ = (HomeViewModel)DataContext;
      //if(string.IsNullOrEmpty(vm_.UserName))
      //  return;
      //else
      //{

      //  // invoke command
      //}
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      
    }
  }
}
