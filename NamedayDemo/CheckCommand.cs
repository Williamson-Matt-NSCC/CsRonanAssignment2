using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NamedayDemo
{
    public class CheckCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainPageData mpd;

        public CheckCommand(MainPageData mpd)
        {
            this.mpd = mpd;
        }

        public bool CanExecute(object parameter)
        {
            return mpd.SelectedNote != null;
        }

        public void Execute(object parameter)
        {
            Debug.WriteLine("check Complete");
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
