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
        Boolean New = false;

        public MainPage()
        {
            this.InitializeComponent();
            //btnSave.IsEnabled = false;
            //Title.IsEnabled = false;
            txtNoteBody.IsEnabled = false;
        }

        private async Task<string> ShowMessageBoxAsync(string messageBoxTitle)
        {
            TextBox textBox = new TextBox();
            textBox.AcceptsReturn = true;
            textBox.Height = 42;

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

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine("App Bar Button");
        }

        private static void GetNewNote(string newNoteName)
        {
            MainPageData.NewNote(newNoteName, "");
        }

        private async void btnNewNote_Click(object sender, RoutedEventArgs e)
        {
            string noteName = await ShowMessageBoxAsync("Name of New Note");

            if (noteName != null && noteName != "")
            {
                Debug.WriteLine("A new Note has been Created.");
                txtNoteName.Text = noteName;
                txtNoteBody.Text = "";
                New = true;
                GetNewNote(noteName);
            }
        }

        private void btnEditNote_Click(object sender, RoutedEventArgs e)
        {
            if (Edit) //in edit mode
            {
                Edit = false;
                txtNoteBody.IsEnabled = false;
                Debug.WriteLine("edit mode : false");
                
            }
            else //not in edit mode
            {
                Edit = true;
                txtNoteBody.IsEnabled = true;
                Debug.WriteLine("edit mode : true");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MainPageData.Save(txtNoteBody.Text);

            Debug.WriteLine("your Note has been Saved");
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
    }
}
