using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NamedayDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Boolean Edit = false;
        private string DeselectedTitleValue = " Current Note: Please select a note, or create one from the menu above";
        private string DeselectedBodyValue = "please select a note";

        public MainPage()
        {
            InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //btnSave.IsEnabled = false;
            //Title.IsEnabled = false;
            txtNoteBody.IsEnabled = false;
            btnEditNote.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageBoxTitle"></param>
        /// <returns></returns>
        private async Task<string> AskTitleMessageBoxAsync(string messageBoxTitle)
        {
            TextBox textBox = new TextBox();
            textBox.Height = 32;

            ContentDialog contentDialog= new ContentDialog();
            contentDialog.Title = messageBoxTitle;
            contentDialog.IsSecondaryButtonEnabled = true;
            contentDialog.PrimaryButtonText = "OK";
            contentDialog.SecondaryButtonText = "Cancel";
            contentDialog.Content = textBox;
            if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return textBox.Text;
            }
            else
            {
                return "";
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageToAskUser">the operation that you are asking the user to confirm</param>
        /// <returns>boolean value</returns>
        private async Task<bool> ConfirmMessageBoxAsync(string messageToAskUser)
        {
            ContentDialog contentDialog = new ContentDialog();
            contentDialog.Title = "Are you sure you would like to " + messageToAskUser + "?";
            contentDialog.IsSecondaryButtonEnabled = true;
            contentDialog.PrimaryButtonText = "Yes, " + messageToAskUser;
            contentDialog.SecondaryButtonText = "Cancel";

            if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// displays a message to the user
        /// </summary>
        /// <param name="messageToAskUser">the operation that you are asking the user to confirm</param>
        /// <returns>boolean value</returns>
        private async Task InfoMessageBoxAsync(string messageToUser)
        {
            ContentDialog contentDialog = new ContentDialog();

            contentDialog.Title = messageToUser;
            contentDialog.IsPrimaryButtonEnabled = true;
            contentDialog.PrimaryButtonText = "Continue";

            await contentDialog.ShowAsync();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tempText;

            if ((tempText = txtSearch.Text) != "")//if the filter isnt empty...
            {
                MainPageData.PerformNoteFiltering(tempText);
            }
            else //if it is empty
            {
                MainPageData.ShowAll();
            }
        }

        private static void GetNewNote(string newNoteName)
        {
            MainPageData.NewNote(newNoteName, "");
        }

        private static bool CheckForNote(String noteName)
        {
            return MainPageData.CheckForNote(noteName);
        }

        private async void btnNewNote_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                string noteName = await AskTitleMessageBoxAsync("Name of New Note");

                //short circuit if statement, checks for valid name, then if it already is in the notes collection
                if (noteName != null && noteName != "" && !CheckForNote(noteName))
                {

                    GetNewNote(noteName);
                    Debug.WriteLine("A new Note has been Created.");
                    txtNoteBody.Text = "";
                    apbNoteName.Text = " Current Note: " + noteName;

                    //this is the only friggin way to do this, if not, show me!!
                    //without this line the listview doesnt update quick enough 
                    //(because they are on separate tasks)
                    await Task.Delay(400);

                    lsvNoteList.SelectedIndex = lsvNoteList.Items.Count -1;

                    //make a loseNoteFocus() to set textbox 
                    //and title to display that there is 
                    //no note selected

                    break;
                }
                else if (noteName == "")
                {
                    break;
                }
                else
                {
                    await InfoMessageBoxAsync("The name \"" + noteName + "\" is currently in use \nPlease enter a unique name for this Note.");
                }
            }
            
        }

        private void btnEditNote_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditMode();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MainPageData.Save(txtNoteBody.Text);

        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //checks for notes, and gets confirmation from the user
            if ((lsvNoteList.SelectedIndex != -1) &&  await ConfirmMessageBoxAsync("Delete this Note"))
            {
                Debug.WriteLine("Note Deleted");
                apbNoteName.Text = DeselectedTitleValue;
                txtNoteBody.Text = DeselectedBodyValue;
                MainPageData.DeleteNote();
                btnDelete.IsEnabled = false;
                btnEditNote.IsEnabled = false;
            }
            else
            {
                btnDelete.IsEnabled = true;
                //handles canceled delete
                Debug.WriteLine("Note not Deleted");
            }
        }

        //now uses the about page navigation method
        //private async void btnAboutThisApp_Click(object sender, RoutedEventArgs e)
        //{
        //    await InfoMessageBoxAsync("Asynchronous message from Matt Williamson");
        //}

        private void btnExitApp_Click(object sender, RoutedEventArgs e)
        {
            //exit the app gracefully
        }

        private void lsvNoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lsvNoteList.SelectedIndex != -1)
            {
                ChangeEditMode(true);
                btnDelete.IsEnabled = true;
            }
            else
            {
                btnDelete.IsEnabled = false;
            }
        }

        private void ChangeEditMode(bool exitEdit = false)
        {
            if (Edit || exitEdit) //in edit mode
            {
                Edit = false;
                txtNoteBody.IsEnabled = false;
                btnEditNote.IsEnabled = true;
                btnSave.IsEnabled = false;
                Debug.WriteLine("edit mode : false");

            }
            else //not in edit mode
            {
                Edit = true;
                txtNoteBody.IsEnabled = true;
                btnEditNote.IsEnabled = false;
                btnSave.IsEnabled = true;
                Debug.WriteLine("edit mode : true");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void apbAboutPage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }

        //private void apbPreviousPage_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
