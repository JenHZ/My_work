/*
*Filename:		MainPage.xaml.cs
*Project:		WMP Final Project
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.12.11
*Description:	This file contain the code that is the code behind the MainPage.xaml page, it is the start page of the game
*               it will allow user to enter the name, and choose to start a new game or resume a previous game
*/



using TilePuzzle.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238



namespace TilePuzzle
{
   /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;                  // the NavigationHelper
        private ObservableDictionary defaultViewModel = new ObservableDictionary();     // initate a ObservableDictionary
        private Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings; // used to store info

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }



        ///Function:		MainPage constructor
        ///Description:     initiate the varibales that will be used in the game
        ///Parameters:      NONE
        ///Return Values:   NONE
        public MainPage()
        {
            this.InitializeComponent();                                     // Initialize the Component
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;  // assign the event with method
            this.navigationHelper.SaveState += navigationHelper_SaveState;  // assign the event with method
        }



        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // check if this is the first time to launch this app
            if (!roamingSettings.Values.ContainsKey("userName"))
            {
                HideButtons();              // hid the buttons
            }

            else
            {
                HideNameInput();
                ShowButtons();

                // game has been played
                if (roamingSettings.Values.ContainsKey("resume"))
                {
                    if (!roamingSettings.Values["resume"].ToString().Equals("true"))
                    {
                        resumeBtn.IsEnabled = false;
                    }
                    else
                    {
                        pageTitle.Text = "Hello: " + roamingSettings.Values["userName"].ToString() + "!";       // greeting message
                        resumeBtn.IsEnabled = true;
                    }
                }

                // a picture has been chose before
                if (roamingSettings.Values.ContainsKey("fileName"))
                {
                    String fileName = roamingSettings.Values["fileName"].ToString();

                    // if file has been chosen
                    if(fileName != "")
                    {
                        // load the file
                        StorageFile file = await Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(fileName);

                        // loas success
                        if (file != null)
                        {
                            // read the stream
                            Windows.Storage.Streams.IRandomAccessStream fileStream =
                                await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                            // new a bitmap
                            Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage =
                                new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                            // assign the image to the bitmap
                            bitmapImage.SetSource(fileStream);
                            imagePreview.Source = bitmapImage;

                            this.DataContext = file;
                        }
                    }
                }
            }
        }

        
        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion



        ///Function:		resumeBtn_Click
        ///Description:     called if the resume button is clicked, it will navigate to the game page
        ///Parameters:      object sender: the sender of the event
		///					RoutedEventArgs e: the event argument
        ///Return Values:   NONE
		///			
        private void resumeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(PhotoPage));         // change to the game page
            }
        }




        ///Function:		newGameBtn_Click
        ///Description:     called if the new game button is clicked, it will navigate to the game page
        ///Parameters:      object sender: the sender of the event
		///					RoutedEventArgs e: the event argument
        ///Return Values:   NONE
		///			
        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            // set all the information that will be stored in the file
            roamingSettings.Values["resume"] = "false";     // not resume the game
            roamingSettings.Values["fileName"] = "";        // reset the file name
            roamingSettings.Values["userName"] = "";        // reset the user name
            roamingSettings.Values["currentMove"] = "";     // reset the current move
            roamingSettings.Values["time"] = "";            // reset the time

            HideButtons();                  // hid the buttons
            ShowNameInput();                // allow for input name
        }



        ///Function:		CommandHandler
        ///Description:     called if the start a new game and will allow user to choose if they want a number game or a picture game
        ///Parameters:      IUICommand command: the command that inidicate user choice
        ///Return Values:   NONE
        ///			
        private void CommandHandler(IUICommand command)
        {
            // get the user choice
            var choice = command.Label;
            // user choose to play a number game
            if(choice.Equals("Numbers"))
            {
                roamingSettings.Values["number"] = "true";      // set the information in the file
            }
            else
            {
                roamingSettings.Values["number"] = "false";     // user choose the picture game
            }

            // after user choose the game, go to the game page
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(PhotoPage));
            }
        }



        ///Function:		rankingBtn_Click
        ///Description:     called if the ranking button is clicked, it will navigate to the ranking page
        ///Parameters:      object sender: the sender of the event
		///					RoutedEventArgs e: the event argument
        ///Return Values:   NONE
		///			
        private void rankingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(LeaderBoard));       // go to the ranking page
            }
        }



        ///Function:		rankingBtn_Click
        ///Description:     called if the name button is clicked, it will first check if user enter anything
        ///Parameters:      object sender: the sender of the event
		///					RoutedEventArgs e: the event argument
        ///Return Values:   NONE
		///			
        private async void nameOkBtn_Click(object sender, RoutedEventArgs e)
        {
            // user need to enter anything
            if (nameInput.Text.Length > 0)
            {
                HideNameInput();            // hid the input box

                // assign the name to the file
                Windows.Storage.ApplicationDataContainer roamingSettings =
                    Windows.Storage.ApplicationData.Current.RoamingSettings;
                roamingSettings.Values["userName"] = nameInput.Text;

                // greeing message
                var dialog = new MessageDialog("Hi, " + nameInput.Text + ".  Which tile puzzle would you like to try?");
                dialog.Commands.Add(new UICommand("Numbers", new UICommandInvokedHandler(CommandHandler)));
                dialog.Commands.Add(new UICommand("Picture", new UICommandInvokedHandler(CommandHandler)));

                await dialog.ShowAsync();
            }
        }





        private void HideNameInput()
        {
            hintForName.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            nameOkBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            nameInput.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ShowButtons()
        {
            newGameBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            imagePreview.Visibility = Windows.UI.Xaml.Visibility.Visible;
            resumeBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            rankingBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void HideButtons()
        {
            newGameBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            resumeBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            imagePreview.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            rankingBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ShowNameInput()
        {
            hintForName.Visibility = Windows.UI.Xaml.Visibility.Visible;
            nameOkBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            nameInput.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

       
    }
}
