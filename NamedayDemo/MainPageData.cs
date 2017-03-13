using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace NamedayDemo
{
    public class MainPageData : INotifyPropertyChanged
    {
        static Windows.Storage.StorageFolder storageFolder = 
            Windows.Storage.ApplicationData.Current.LocalFolder;

        // Defaults to "Hello" if not set
        public string Greeting { get; set; } = "please select a note";
        public string BodyContent { get; set; } = "please select a note";
        // List of NamedayModel classes

        public ObservableCollection<NamedayModel> Namedays
        {
            get; set;
        }

        public static ObservableCollection<NoteModel> Notes
        {
            get; set;
        }

        public static ObservableCollection<NoteModel> FilteredNotes
        {
            get; set;
        }

        public static ObservableCollection<NoteModel> _allNotes
        {
            get; set;
        }

        private static NoteModel _selectedNote;

        private string _filter;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public NoteModel SelectedNote
        {
            get
            {
                return _selectedNote;
            }
            set
            {
                _selectedNote = value;
                if (value == null)
                {
                    Greeting = "Please select a note, or create one from the bar above";
                }
                else
                {
                    Greeting = _selectedNote.NoteName;
                    BodyContent = _selectedNote.NoteBody;
                }

                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("Greeting"));

                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("BodyContent"));

                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("FilterNotes"));

                CheckCommand.FireCanExecuteChanged();
            }
        }

        public CheckCommand CheckCommand { get; }

        //public MainPageData()
        //{
        //    CheckCommand = new CheckCommand(this);
        //    Namedays = new ObservableCollection<NamedayModel>();
        //    if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
        //    {
        //        for (int month = 1; month <= 12; month++)
        //        {
        //            _allNamedays.Add(new NamedayModel(month, 1, new string[] { "Adam" }));
        //            _allNamedays.Add(new NamedayModel(month, 24, new string[] { "Eve", "Ronan" }));
        //        }
        //        PerformFiltering();
        //    }
        //    else
        //    {
        //        LoadData();
        //    }
        //}

        public MainPageData()
        {
            CheckCommand = new CheckCommand(this);
            Notes = new ObservableCollection<NoteModel>();
            FilteredNotes = new ObservableCollection<NoteModel>();
            fillList();
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
                //PerformFiltering();
                PerformNoteFiltering();
            }
        }

        //private void PerformFiltering()
        //{
        //    if (_filter == null)
        //    {
        //        _filter = "";
        //    }
        //    var lowerCaseFilter = Filter.ToLowerInvariant().Trim();
        //    var result = _allNamedays.Where(
        //        d => d.NamesAsString.ToLowerInvariant()
        //                .Contains(lowerCaseFilter)).ToList();
        //    var toRemove = Namedays.Except(result).ToList();
        //    foreach (var x in toRemove)
        //    {
        //        Namedays.Remove(x);
        //    }
        //    // Add back in the correct order.
        //    var resultcount = result.Count;
        //    for (int i = 0; i < resultcount; i++)
        //    {
        //        var resultItem = result[i];
        //        if (i+1 > Namedays.Count || !Namedays[i].Equals(resultItem))
        //        {
        //            Namedays.Insert(i, resultItem);
        //        }
        //    }
        //}

        private void PerformNoteFiltering()
        {
            if (_filter == null)
            {
                _filter = "";
            }
            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();
            var result = _allNotes.Where(
                d => d.NoteNameAsString.ToLowerInvariant()
                        .Contains(lowerCaseFilter)).ToList();
            var toRemove = Notes.Except(result).ToList();
            foreach (var x in toRemove)
            {
                Notes.Remove(x);
            }
            // Add back in the correct order.
            var resultcount = result.Count;
            for (int i = 0; i < resultcount; i++)
            {
                var resultItem = result[i];
                if (i + 1 > Notes.Count || !Notes[i].Equals(resultItem))
                {
                    Notes.Insert(i, resultItem);
                }
            }
        }

        public async static void Save(string noteBody)
        {
            _selectedNote.NoteBody = noteBody;
            string noteName = _selectedNote.NoteName;
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var query = folder.CreateFileQuery();
            var files = await query.GetFilesAsync();
            var titleformat = noteName + ".txt";

            Windows.Storage.StorageFile sampleFile =
                        await storageFolder.CreateFileAsync(titleformat,
                            Windows.Storage.CreationCollisionOption.ReplaceExisting);

            await storageFolder.GetFileAsync(titleformat);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, noteBody);
        }

        public async static void SaveNew(string noteName, string noteBody)
        {
            String formattitle = noteName + ".txt";

            Windows.Storage.StorageFile newFile =
                await storageFolder.CreateFileAsync(formattitle,
                Windows.Storage.CreationCollisionOption.ReplaceExisting);
            
            await storageFolder.GetFileAsync(formattitle);
            await Windows.Storage.FileIO.WriteTextAsync(newFile, noteBody);
        }

        public static async void NewNote(string noteName, string noteBody)
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var query = folder.CreateFileQuery();
            var files = await query.GetFilesAsync();
            Boolean samename = false;

            foreach (Windows.Storage.StorageFile file in files)
            {
                string fileName = file.Name;
                fileName = fileName.Replace(".txt", "");

                if (fileName.Equals(noteName))
                {
                    samename = true;
                    var OkCommand = new UICommand("Ok");
                    var dialog = new MessageDialog("Please give your note a unique name", "Illegal Note Name");
                    dialog.Options = MessageDialogOptions.None;
                    dialog.Commands.Add(OkCommand);
                    dialog.DefaultCommandIndex = 0;
                    dialog.CancelCommandIndex = 0;
                }
            }

            if (samename != true)
            {
                int Id = Convert.ToInt32(Notes.LongCount()) + 1;
                Notes.Add(new NoteModel(Id, noteName, noteBody));
                FilteredNotes.Add(new NoteModel(Id, noteName, noteBody));
                SaveNew(noteName, noteBody);
            }
        }

        public async static void DeleteNote()
        {
            int noteNumber = _selectedNote.NoteNumber;
            string noteName = _selectedNote.NoteName;

            noteName = noteName + ".txt";

            StorageFile tempFile = await storageFolder.GetFileAsync(noteName);
            await tempFile.DeleteAsync();

            Notes.RemoveAt(noteNumber);
            FilteredNotes.RemoveAt(noteNumber);
        }

        public static void ShowAll()
        {
            FilteredNotes.Clear();
            foreach (NoteModel note in Notes)
            {
                FilteredNotes.Add(note);
            }
        }

        private async static void fillList()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var query = folder.CreateFileQuery();
            var files = await query.GetFilesAsync();
            foreach (Windows.Storage.StorageFile file in files)
            {
                string NoteBody = await Windows.Storage.FileIO.ReadTextAsync(file);
                string NoteName = file.Name;
                NoteName = NoteName.Replace(".txt", "");
                try
                {
                    int NoteNumber = Notes.Count + 1;
                    Notes.Add(new NoteModel(NoteNumber, NoteName, NoteBody));
                    FilteredNotes.Add(new NoteModel(NoteNumber, NoteName, NoteBody));
                }
                catch (Exception err)
                {
                    string error = err.Message;
                }
            }
        }

        public static void PerformFiltering(string Filter)
        {
            if (Filter.Trim() == null || Filter.Equals(""))
            {
                FilteredNotes.Clear();
                foreach (NoteModel note in Notes)
                {
                    FilteredNotes.Add(note);
                }
            }
            else
            {
                var lowerCaseFilter = Filter.ToLowerInvariant().Trim();

                var result = Notes.Where(d => d.NoteName.ToLowerInvariant()
                    .Contains(lowerCaseFilter)).ToList();

                var toRemove = FilteredNotes.Except(result).ToList();

                foreach (var x in toRemove)
                {
                    FilteredNotes.Remove(x);
                }

                var resultCount = result.Count;
                for (int i = 0; i < resultCount; i++)
                {
                    var resultItem = result[i];
                    if (i + 1 > FilteredNotes.Count || !FilteredNotes[i].Equals(resultItem))
                        FilteredNotes.Insert(i, resultItem);
                }
            }
        }

        //private async void LoadData()
        //{
        //    _allNamedays = await Repository.GetAllNamedaysAsync();
        //    PerformFiltering();
        //}
    }
}
