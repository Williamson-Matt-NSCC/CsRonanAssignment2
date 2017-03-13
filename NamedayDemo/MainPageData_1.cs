using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedayDemo
{
    public class MainPageData : INotifyPropertyChanged
    {
        // Defaults to "Hello" if not set
        public string Greeting { get; set; } = "Hello NSCC!";
        // List of NamedayModel classes
        public ObservableCollection<NamedayModel> Namedays
        {
            get; set;
        }

        private List<NamedayModel> _allNamedays = new List<NamedayModel>();

        private NamedayModel _selectedNameday;
        private string _filter;

        public event PropertyChangedEventHandler PropertyChanged;

        public NamedayModel SelectedNameday
        {
            get
            {
                return _selectedNameday;
            }
            set
            {
                _selectedNameday = value;
                if (value == null)
                {
                    Greeting = "Hello World!";
                } else
                {
                    Greeting = "Hello " + value.NamesAsString;
                }
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs("Greeting"));
                CheckCommand.FireCanExecuteChanged();
            }
        }
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (value == _filter)
                {
                    return;
                }
                _filter = value;
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(Filter)));
                PerformFiltering();
            }
        }

        private void PerformFiltering()
        {
            if (_filter == null)
            {
                _filter = "";
            }
            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();
            var result = _allNamedays.Where(
                d => d.NamesAsString.ToLowerInvariant()
                        .Contains(lowerCaseFilter)).ToList();
            var toRemove = Namedays.Except(result).ToList();
            foreach (var x in toRemove)
            {
                Namedays.Remove(x);
            }
            // Add back in the correct order.
            var resultcount = result.Count;
            for (int i = 0; i < resultcount; i++)
            {
                var resultItem = result[i];
                if (i+1 > Namedays.Count || !Namedays[i].Equals(resultItem))
                {
                    Namedays.Insert(i, resultItem);
                }
            }
        }

        public CheckCommand CheckCommand { get; }
        public MainPageData()
        {
            CheckCommand = new CheckCommand(this);
            Namedays = new ObservableCollection<NamedayModel>();
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                for (int month = 1; month <= 12; month++)
                {
                    _allNamedays.Add(new NamedayModel(month, 1, new string[] { "Adam" }));
                    _allNamedays.Add(new NamedayModel(month, 24, new string[] { "Eve", "Ronan" }));
                }
                PerformFiltering();
            }
            else
            {
                LoadData();
            }
        }

        private async void LoadData()
        {
            _allNamedays = await Repository.GetAllNamedaysAsync();
            PerformFiltering();
        }
    }
}
