using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        public string NoteTitle
        {
            get
            {
                return " Current Note: " + Greeting;
            }

            set
            {

            }
        }

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
                    Greeting = "Please select a note, or create one from the menu above";
                }
                else
                {
                    Greeting = _selectedNote.NoteName;
                    BodyContent = _selectedNote.NoteBody;
                }
                
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("NoteTitle"));

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

        ////combined this functionality into the perform note filtering

        //public string Filter
        //{
        //    get
        //    {
        //        return _filter;
        //    }
        //    set
        //    {
        //        if (value == _filter)
        //        {
        //            return;
        //        }
        //        _filter = value;
        //        PropertyChanged?.Invoke(this,
        //            new PropertyChangedEventArgs(nameof(Filter)));
        //        //PerformFiltering();
        //        PerformNoteFiltering();
        //    }
        //}

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
            string noteName = _selectedNote.NoteName;

            StorageFile tempFile = await storageFolder.GetFileAsync(noteName + ".txt");
            await tempFile.DeleteAsync();

            //delete that note from Notes and FilteredNotes
            // got this line from http://stackoverflow.com/questions/20403162/remove-one-item-in-observablecollection
            Notes.Remove(Notes.Where(i => i.NoteName == _selectedNote.NoteName).Single());
            FilteredNotes.Remove(FilteredNotes.Where(i => i.NoteName == _selectedNote.NoteName).Single());
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

        public static void ShowAll()
        {
            FilteredNotes.Clear();
            foreach (NoteModel note in Notes)
            {
                FilteredNotes.Add(note);
            }
        }

        public static void PerformNoteFiltering(string Filter)
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
    }
}
