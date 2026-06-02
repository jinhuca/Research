using System.Windows.Input;

namespace HomeModule.ViewModels;

public interface IHomeViewModel {
  ICommand NavigateCommand { get; set; }
}
