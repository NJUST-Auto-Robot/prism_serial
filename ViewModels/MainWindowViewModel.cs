using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace prism_serial.ViewModels
{
    /// <summary>
    /// Represents the view model for the main window.
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            this._regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(OnNavigate);
            GoBackCommand = new DelegateCommand(() =>
            {
                if (_journal is { CanGoBack: true })
                    _journal.GoBack();
            });
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (_journal is { CanGoForward: true })
                    _journal.GoForward();
            });
        }

        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal _journal;

        /// <summary>
        /// Gets the command to navigate to a specific region.
        /// </summary>
        public DelegateCommand<string> NavigateCommand { set; get; }

        /// <summary>
        /// Gets the command to navigate back in the region's navigation history.
        /// </summary>
        public DelegateCommand GoBackCommand { set; get; }

        /// <summary>
        /// Gets the command to navigate forward in the region's navigation history.
        /// </summary>
        public DelegateCommand GoForwardCommand { set; get; }

        private void OnNavigate(string obj)
        {
            _regionManager.Regions["ContentRegion"].RequestNavigate(obj, callback =>
            {
                if (callback.Result != null && (bool)callback.Result)
                {
                    _journal = callback.Context.NavigationService.Journal;
                }
            });
        }
    }
}