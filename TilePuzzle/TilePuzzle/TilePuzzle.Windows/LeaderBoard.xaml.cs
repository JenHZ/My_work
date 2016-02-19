/*
*Filename:		LeaderBoard.xaml.cs
*Project:		WMD Final Project
*By:			ZhengHua/Shaohua Mao
*Date:			2015.12.18
*Description:	The file includes code for LeaderBoard Page.
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
using Windows.Storage;
using System.Diagnostics;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace TilePuzzle
{
    /// <summary>
    /// A page to show the rankings of all users
    /// </summary>
    public sealed partial class LeaderBoard : Page
    {
        static public String LEADERBOARD = "leader.txt";      
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();



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



        ///Function:		LeaderBoard -- constructor
        ///Description:     to instantiate a LeaderBoard Page.
        ///Parameters:      NONE
        ///Return Values:   NONE
        public LeaderBoard()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
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

            // everytime when user comes to this page we load the file which saves all user information
            StorageFolder localFolder = null;
            StorageFile file = null;
            String errormessage = "";

            try
            {
                // try to get the file from local folder
                localFolder = ApplicationData.Current.LocalFolder;
                file = await localFolder.GetFileAsync(LEADERBOARD);
            }
            catch(Exception ex)
            {
                errormessage = ex.Message;
            }

            // file doesn't exist, then we create the file
            if(errormessage != "")
            {
                file = await localFolder.CreateFileAsync(LEADERBOARD, CreationCollisionOption.ReplaceExisting);
            }
            
            if(file != null)
            {
                // parse the string and then fill them into user list.
				String result = await Windows.Storage.FileIO.ReadTextAsync(file);
				String[] fields = result.Split('|');
				List<User> users = new List<User>();

                // loop to get all users from file
				for (int i = 0; i < fields.Count() - 1; i +=2 )
				{
					User aUser = new User();
					aUser.name = fields[i];
					aUser.score = Int32.Parse(fields[i + 1]);
					users.Add(aUser);
				}

                // sort the list according to their time ASCD
				users.Sort(SortByScoreHelper.SortByScore());
				int rank = 1;

                // put all information into the grid to show user
				foreach(User aUser in users)
				{
                    // textBlocks to hold text 
					TextBlock tbRank = new TextBlock();
					TextBlock tbName = new TextBlock();
					TextBlock tbScore = new TextBlock();

                    // create a new RowDefinition
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength(50);

                    // translate seconds to minutes and seconds
                    int seconds = aUser.score % 60;
                    int minutes = aUser.score / 60;
					tbRank.Text = "" + rank;
					tbName.Text = aUser.name;
                    tbScore.Text = String.Format("  {0} min {1} sec", minutes, seconds);

                    // set font size
					tbRank.FontSize = 20;
					tbName.FontSize = 20;
					tbScore.FontSize = 20;

                    // put textBlocks and new RowDefinition into Grid
                    rankingGrid.RowDefinitions.Add(rd);
					rankingGrid.Children.Add(tbRank);
					rankingGrid.Children.Add(tbName);
					rankingGrid.Children.Add(tbScore);

                    // put textBlocks into specific cells of this grid
					Grid.SetRow(tbRank, rank);
					Grid.SetColumn(tbRank, 0);
					Grid.SetRow(tbName, rank);
					Grid.SetColumn(tbName, 1);
					Grid.SetRow(tbScore, rank);
					Grid.SetColumn(tbScore, 2);

					rank++;
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



        ///Inner Class:		User
        ///Description:     to hold the information of a user
        public class User
        {
            public String name;
            public int score;
        }



        ///SortByLastNameHelper
        ///The purpose of this class is to provide a mechanism to sort the List by score.\n
        ///
        ///This class implements IComparer interface
        public class SortByScoreHelper : IComparer<User>
        {
            ///Function:		Compare
            ///Description:     Called to compare two user's score in a container
            ///Parameters:      NONE
            ///Return Values:   int
            int IComparer<User>.Compare(User left, User right)
            {
                return  left.score - right.score;
            }



            ///Function:		SortByScore
            ///Description:     called to get an instance of this SortByScoreHelper class
            ///Parameters:      NONE
            ///Return Values:   IComparer<Employee>
            public static IComparer<User> SortByScore()
            {
                return (IComparer<User>)new SortByScoreHelper();
            }
        }
    }
}
