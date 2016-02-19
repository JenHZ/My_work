/*
*Filename:		PhotoPage.xaml.cs
*Project:		WMD Final Project
*By:			ZhengHua/Shaohua Mao
*Date:			2015.12.18
*Description:	The file includes code for PhotoPage.
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
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;
using Windows.Media.Capture;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using System.Threading;
using Windows.Devices.Sensors;
using System.Runtime.InteropServices;
using Windows.UI.Core;
using Windows.Graphics.Display;
using Windows.ApplicationModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace TilePuzzle
{
    /// <summary>
    /// A page on which user can play the game.
    /// </summary>
    public sealed partial class PhotoPage : Page
    {
        // constants
        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;
        private int orientation = DMDO_DEFAULT;
        static public int OFFSET = 125;
        static public String LEADERBOARD = "leader.txt";

        private WriteableBitmap writeableBitmap;            // to store the file from camera or gallary
        private Image currentImg = new Image();             // to remember the image chosen
        private Image[] images;                             // a list which holds all images in imageGrid
        private Popup popUp;                                // a popup windows shows user the result
        private Point oldP = new Point();                   // a Point to remember where the user touch on screen
        private Tile tile = new Tile();                     // a tile object to do all the calculation
        private Windows.Storage.ApplicationDataContainer roamingSettings =
                    Windows.Storage.ApplicationData.Current.RoamingSettings;

        private DispatcherTimer dispatcherTimer;            // a timer to constantly check the reading of accelerometer
        private DispatcherTimer tiltingTimer;               // a timer used to record user score
        DateTimeOffset startTime;                           // used to calculate the time span
        DateTimeOffset lastTime;                            // used to calculate the time span
        int timesTicked = 1;                                // how many seconds has gone

        public ManualResetEvent done = new ManualResetEvent(false);     // used to control the threads
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Accelerometer accelerometer;                // an Accelerometer to read the compass reading
        private DEVMODE dm;                                 // used to hold the readings from accelerometer
        private bool canMove;                               // if user using tilte, need to move the default position before go to the next step
        private static Mutex m = new Mutex();               // a mutex used to control all the threads

        // PInvoke declaration for EnumDisplaySettings Win32 API
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        private string mruToken = null;                      // a string to hold the file name priviously used
        private string currentMove = null;                   // a string used to record current tile positions
        private StorageFile file;                            // a local file
        private MediaElement snd;                            // used to play sound
        private StorageFolder folder;                        // a folder holds all local files



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



        ///Function:		PhotoPage -- constructor
        ///Description:     to instantiate a PhotoPage Page.
        ///Parameters:      NONE
        ///Return Values:   NONE
        public PhotoPage()
        {
            this.InitializeComponent();

            // initialize navigationHelper and DispatcherTimer here.
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            DispatcherTimerSetup();

            // initialize the images list
            images = new Image[15];
            images[0] = image01;
            images[1] = image02;
            images[2] = image03;
            images[3] = image04;
            images[4] = image05;
            images[5] = image06;
            images[6] = image07;
            images[7] = image08;
            images[8] = image09;
            images[9] = image10;
            images[10] = image11;
            images[11] = image12;
            images[12] = image13;
            images[13] = image14;
            images[14] = image15;

            // initialize accelerometer.
            accelerometer = Accelerometer.GetDefault();
            dm = new DEVMODE();
            dm.dmDeviceName = new string(new char[32]);
            dm.dmFormName = new string(new char[32]);
            canMove = true;                         

            // if we get a accelerometer object 
            if (accelerometer != null)
            {
                uint minReportInterval = accelerometer.MinimumReportInterval;
                accelerometer.ReportInterval = 200;

                // Set up a DispatchTimer
                tiltingTimer = new DispatcherTimer();
                tiltingTimer.Tick += tiltingMove;
                tiltingTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            }
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
            // initialize some buttons and images.
            displayImage1.Source = null;
            displayImage2.Source = null;
            finishBtn.IsEnabled = false;
            InitilizeImages();

            // if the user comes back from a unfinished game
            if (roamingSettings.Values["resume"].ToString().Equals("true"))
            {
                // get the file the user played with last time
                mruToken = roamingSettings.Values["fileName"].ToString();
                Windows.Storage.StorageFile file =
                    await Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(mruToken);

                // if we get the file
                if (file != null)
                {
                    // put this image into two preview images.
                    Windows.Storage.Streams.IRandomAccessStream fileStream =
                        await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage =
                        new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                    bitmapImage.SetSource(fileStream);
                    displayImage1.Source = bitmapImage;
                    displayImage2.Source = bitmapImage;

                    this.DataContext = file; 

                    // get the privious positions of all tiles 
                    String[] move = roamingSettings.Values["currentMove"].ToString().Split('|');
                    int[] randomInt = new int[16];
                    for (int i = 0; i < 16; i++)
                    {
                        randomInt[i] = Int32.Parse(move[i]);
                    }
                    tile.SetCurrentMove(roamingSettings.Values["currentMove"].ToString());

                    // rearrange the image into pieces.
                    await CutImageToPieces(images, randomInt);
                }
            }

            // user starts a new game here
            else
            {
                timesTicked = 0;
               
                // check which kind of game user selects
                if (roamingSettings.Values["number"].ToString().Equals("true"))
                {
                    // get the default  number picture
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await localFolder.GetFileAsync("number.jpg");

                    if (file != null)
                    {
                        // save it to recently list
                        roamingSettings.Values["fileName"] = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);

                        Windows.Storage.Streams.IRandomAccessStream fileStream =
                            await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                        Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage =
                            new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                        
                        // set this number picture as resource to two preview image
                        bitmapImage.SetSource(fileStream);
                        displayImage1.Source = bitmapImage;
                        displayImage2.Source = bitmapImage;
                        this.DataContext = file;

                        // rearrange those pieces
                        int[] randomInt = tile.GetArray();
                        roamingSettings.Values["currentMove"] = tile.GetCurrentMove();
                        await CutImageToPieces(images, randomInt);
                    }
                }
                // user wants to play a photo game
                else
                {
                    cameraBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    gallaryBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    
                }
            }

            LoadSoundFile();
        }



        ///Function:		InitilizeImages
        ///Description:     to initialize the imageGrid
        ///Parameters:      NONE
        ///Return Values:   NONE
        private void InitilizeImages()
        {
            foreach (Image img in images)
            {
                img.Source = null;
            }

            // put all images into associated cells
            for (int i = 0; i < 15; i ++)
            {
                Grid.SetRow(images[i], i / 4);
                Grid.SetColumn(images[i], i % 4);
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
            if (!string.IsNullOrEmpty(mruToken))
            {
                e.PageState["mruToken"] = mruToken;
            }

            // stop two timers when the user leaves this page
            if (accelerometer != null)
            {
                tiltingTimer.Stop();
            }
            dispatcherTimer.Stop();
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
            DisplayInformation.GetForCurrentView().OrientationChanged += OnOrientationChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
            DisplayInformation.GetForCurrentView().OrientationChanged -= OnOrientationChanged;
        }

        #endregion



        ///Function:		OnOrientationChanged
        ///Description:     to change the layout when the user changes the orientation
        ///Parameters:      Point robotPos:    the position of the robot
        ///Return Values:   NONE
        private void OnOrientationChanged(DisplayInformation sender, object args)
        {
            TransitionStoryboardState();
        }



        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// display orientation.
        /// </summary>
        private void TransitionStoryboardState()
        {
            string displayOrientation;

            switch (DisplayInformation.GetForCurrentView().CurrentOrientation)
            {
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                    displayOrientation = "Portrait";
                    break;

                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                default:
                    displayOrientation = "Landscape";
                    break;
            }

            VisualStateManager.GoToState(this, displayOrientation, false);
        }



        ///Function:		GetPhotoButton_Click
        ///Description:     select a picture from galary for the user
        ///Parameters:      object sender:     the object arises the event
        ///Return Values:   RoutedEventArgs e: the event argument
        private async void GetPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // ask user to choose a picture
            Windows.Storage.Pickers.FileOpenPicker openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");

            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();

            // if the file chosen is not null
            if (file != null)
            {
                Windows.Storage.Streams.IRandomAccessStream fileStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage();

                // set this file as the resource of preview images
                bitmapImage.SetSource(fileStream);
                displayImage1.Source = bitmapImage;
                displayImage2.Source = bitmapImage;
                this.DataContext = file;

                // save to system folder so we can use it next time.
                Windows.Storage.Streams.IRandomAccessStream stream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                writeableBitmap = new WriteableBitmap(1, 1);
                writeableBitmap.SetSource(stream);
                writeableBitmap.Invalidate();
                SaveImage();
                
                // get a random array and cut the picture according to this array
                int[] randomInt = tile.GetArray();
                InitilizeImages();
                roamingSettings.Values["currentMove"] = tile.GetCurrentMove();
                await CutImageToPieces(images, randomInt);
            }
        }



        ///Function:		GetPhotoButton_Click
        ///Description:     select a picture from galary for the user
        ///Parameters:      Image[] images:     the images in the imageGrid
        ///Return Values:   int[] randomInt:    the sequence of those pictures
        private async Task CutImageToPieces(Image[] images, int[] randomInt)
        {
            // set a decoder for this file
            Windows.Storage.StorageFile file = (StorageFile)this.DataContext;
            var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

            // set up the loop counters
            int i = 0;
            int j = 0;
            int imageCounter = 0;

            // loop through the array to cut the file
            for (int k = 0; k < 16; k++)
            {
                // if the current position is empty
                if(randomInt[k] == 0)
                {
                    // if user gets back from a unfinished game
                    if (roamingSettings.Values["resume"].ToString().Equals("true"))
                    {
                        // push all images one cell back
                        for (int o = 14; o >= k; o--)
                        {
                            if (o % 4 == 3)
                            {
                                Grid.SetRow(images[o], o / 4 + 1);
                                Grid.SetColumn(images[o], 0);
                            }
                            else
                            {
                                Grid.SetColumn(images[o], o % 4 + 1);
                                Grid.SetRow(images[o], o / 4);
                            }
                        }
                    }
                    continue;
                }
                // find which piece to feed in to this cell
                else
                {
                    j = (randomInt[k] - 1) % 4;
                    i = (randomInt[k] - 1) / 4;
                }
             
                // set a new boundary for this image
                InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
                BitmapEncoder enc = await BitmapEncoder.CreateForTranscodingAsync(ras, decoder);
                enc.BitmapTransform.ScaledHeight = 500;
                enc.BitmapTransform.ScaledWidth = 500;
                
                // find which piece we want to cut
                BitmapBounds bounds = new BitmapBounds();
                bounds.Height = 125;
                bounds.Width = 125;
                bounds.X = (uint)(125 * j);
                bounds.Y = (uint)(125 * i);
                enc.BitmapTransform.Bounds = bounds;

                try
                {
                    await enc.FlushAsync();
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }

                // put this little piece to a cell
                BitmapImage bImg = new BitmapImage();
                bImg.SetSource(ras);
                images[imageCounter].Source = bImg;
                imageCounter++;
            }

            if(roamingSettings.Values.ContainsKey("pause"))
            {
                // if user didn't pause last time
                if (!(roamingSettings.Values["pause"].ToString().Equals("true")))
                {
                    if (accelerometer != null)
                    {
                        tiltingTimer.Start();
                    }
                    dispatcherTimer.Start();
                }
            }
            // if user didn't pause last time
            else
            {
                if (accelerometer != null)
                {
                    tiltingTimer.Start();
                }
                dispatcherTimer.Start();
            }

            // disable camera and gallary buttons.
            roamingSettings.Values["resume"] = "true";
            cameraBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            gallaryBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            finishBtn.IsEnabled = true;
        }



        ///Function:		imageGrid_PointerReleased
        ///Description:     move the tile when the user released finger
        ///Parameters:      object sender:              the event riser
        ///Return Values:   PointerRoutedEventArgs e:   event arguments
        private void imageGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // set variable to detect where the user left the screen
            double endX;
            double endY;
            char direction = ' ';

            // calculate the distance and the direction the user moves his finger
            endX = e.GetCurrentPoint((Grid)sender).Position.X;
            endY = e.GetCurrentPoint((Grid)sender).Position.Y;
            double deltaX = endX - oldP.X;
            double deltaY = endY - oldP.Y;

            int isWin = 0;

            // user just pressed this image
            if (Math.Abs(deltaY) <= 0.5 && Math.Abs(deltaX) <= 0.5)
            {
                // calculate which image is pressed
                int row = (int)endY / 125;
                int coloumn = (int)endX / 125;
                int newR = row;
                int newC = coloumn;
                int newOffset = OFFSET;

                // check if this tile can be move or not
                String result = tile.CheckMove(row, coloumn);
                direction = result.ElementAt(0);
                char win = ' ';

                // user wins after this move
                if (result.Length == 2)
                {
                    win = result.ElementAt(1);
                }

                // calculate the new position of this image after the move
                if (direction == 'L')
                {
                    newC = coloumn - 1;
                }
                else if (direction == 'R')
                {
                    newC = coloumn + 1;
                    newOffset = OFFSET * (-1);
                }
                else if (direction == 'U')
                {
                    newR = row - 1;

                }
                else if (direction == 'D')
                {
                    newR = row + 1;
                    newOffset = OFFSET * (-1);
                }

                // put the image into new position
                Grid.SetRow(currentImg, newR);
                Grid.SetColumn(currentImg, newC);
                Storyboard sb = new Storyboard();
                RepositionThemeAnimation rt = new RepositionThemeAnimation();

                // set the distance the image is going to move
                if (direction == 'U' || direction == 'D')
                {
                    rt.FromVerticalOffset = newOffset;
                }
                else if (direction == 'L' || direction == 'R')
                {
                    rt.FromHorizontalOffset = newOffset;
                }

                // add animation effect and play sound
                sb.Children.Add(rt);
                Storyboard.SetTarget(rt, currentImg);
                sb.Begin();
                if(direction!='O')
                {
                    PlaySound();
                }

                // user does't win yet after this move
                if (win == ' ')
                {
                    roamingSettings.Values["currentMove"] = tile.GetCurrentMove();
                }

                // user wins after this move
                else if(win == 'W')
                {
                    // stop the timers and save user score
                    if (accelerometer != null)
                    {
                        tiltingTimer.Stop();
                    }
                    dispatcherTimer.Stop();
                    String userScore = roamingSettings.Values["userName"].ToString() + "|" + timesTicked + "|";
                    CleanUp(userScore);
                }
            }
            
            // user slides on this image
            else
            {
                // calculate which direction user move
                if (deltaY < 0 && Math.Abs(deltaY) > Math.Abs(deltaX) && Math.Abs(deltaY) > 20)
                {
                    direction = 'U';
                }
                if (deltaY > 0 && deltaY > Math.Abs(deltaX) && deltaY > 20)
                {
                    direction = 'D';
                }
                if (deltaX < 0 && Math.Abs(deltaX) > Math.Abs(deltaY) && Math.Abs(deltaX) > 20)
                {
                    direction = 'L';
                }
                if (deltaX > 0 && deltaX > Math.Abs(deltaY) && Math.Abs(deltaX) > 20)
                {
                    direction = 'R';
                }

                // get the current position of this image
                int row = Grid.GetRow(currentImg);
                int coloumn = Grid.GetColumn(currentImg);
                int newR = row;
                int newC = coloumn;
                int newOffset = OFFSET;

                // check whether this tile can move or not
                isWin = tile.CheckMove(row, coloumn, direction);

                // if this tile can be moved
                if (isWin == 1 || isWin == 2)
                {
                    // calculate the new position of this image
                    if (direction == 'L')
                    {
                        newC = coloumn - 1;
                    }
                    else if (direction == 'R')
                    {
                        newC = coloumn + 1;
                        newOffset = OFFSET * (-1);
                    }
                    else if (direction == 'U')
                    {
                        newR = row - 1;
                    }
                    else if (direction == 'D')
                    {
                        newR = row + 1;
                        newOffset = OFFSET * (-1);
                    }

                    // put this image to new position
                    Grid.SetRow(currentImg, newR);
                    Grid.SetColumn(currentImg, newC);
                    Storyboard sb = new Storyboard();
                    RepositionThemeAnimation rt = new RepositionThemeAnimation();

                    // calculate how far this tile will move
                    if (direction == 'U' || direction == 'D')
                    {
                        rt.FromVerticalOffset = newOffset;
                    }
                    else if (direction == 'L' || direction == 'R')
                    {
                        rt.FromHorizontalOffset = newOffset;
                    }

                    // play the animation and the sound
                    sb.Children.Add(rt);
                    Storyboard.SetTarget(rt, currentImg);
                    sb.Begin();
                    PlaySound();
                }
            }
            
            // user doesn't after this move 
            if (isWin != 2)
            {
                roamingSettings.Values["currentMove"] = tile.GetCurrentMove();
            }
            // user wins after this move
            else
            {
                // stop the timers and save the scores
                if (accelerometer != null)
                {
                    tiltingTimer.Stop();
                }
                dispatcherTimer.Stop();
                String userScore = roamingSettings.Values["userName"].ToString() + "|" + timesTicked + "|";
                CleanUp(userScore);
            }
        }



        ///Function:		Image_PointerPressed
        ///Description:     remember which tile is pressed by user
        ///Parameters:      object sender:              the event riser
        ///Return Values:   PointerRoutedEventArgs e:   event arguments
        private void Image_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentImg = (Image)sender;
        }



        ///Function:		imageGrid_PointerPressed
        ///Description:     remember the position when the user points at this imageGrid
        ///Parameters:      object sender:              the event riser
        ///Return Values:   PointerRoutedEventArgs e:   event arguments
        private void imageGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            double startX = e.GetCurrentPoint((Grid)sender).Position.X;
            double startY = e.GetCurrentPoint((Grid)sender).Position.Y;
            oldP = new Point(startX, startY);

            // capture the pointer
            bool isCaptured = ((Grid)sender).CapturePointer(e.Pointer);
        }



        ///Function:		tiltingMove
        ///Description:     move the tiles when the user tilting the tablet
        ///Parameters:      object sender:   the event riser
        ///Return Values:   object e:        event arguments
        async private void tiltingMove(object sender, object e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                List<int> whichOne = new List<int>();       // a list to store result from tile class
                bool willMove = false;                      // check if this tile can move or not
                int newOffset = OFFSET;                     // how far this tile will move
                char direction = ' ';                       // which direction the tile will move
                bool readCanMove;                           // a copy of will move
                bool playSound = true;                      // whethere we will play the sound or not

                m.WaitOne();
                readCanMove = canMove;
                m.ReleaseMutex();

                // get reading from Accelerometer
                AccelerometerReading reading = accelerometer.GetCurrentReading();

                // we got readings
                if (reading != null)
                {
                    EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm);
                    orientation = dm.dmDisplayOrientation;

                    // reset the value of canMove
                    if (reading.AccelerationX <= 0.1 && reading.AccelerationX >= -0.1 && reading.AccelerationY <= 0.1 && reading.AccelerationY >= -0.1)
                    {
                        m.WaitOne();
                        canMove = true;
                        m.ReleaseMutex();
                    }
                    else if (readCanMove)
                    {
                        // we calculate the move by current orientation, DMDO_DEFAULT means landscape
                        if (orientation == DMDO_DEFAULT)
                        {
                            // tile move right
                            if (reading.AccelerationX > 0.1 && reading.AccelerationY <= 0.2 && reading.AccelerationY >= -0.2)
                            {
                                whichOne = tile.CheckMove('R');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'R';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move up
                            else if (reading.AccelerationY > 0.1 && reading.AccelerationX <= 0.2 && reading.AccelerationX >= -0.2)
                            {
                                whichOne = tile.CheckMove('U');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    direction = 'U';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move down
                            else if (reading.AccelerationY < -0.1 && reading.AccelerationX <= 0.2 && reading.AccelerationX >= -0.2)
                            {
                                whichOne = tile.CheckMove('D');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    playSound = true;
                                    direction = 'D';
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move left
                            else if (reading.AccelerationX < -0.1 && reading.AccelerationY <= 0.2 && reading.AccelerationY >= -0.2)
                            {
                                whichOne = tile.CheckMove('L');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    direction = 'L';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                        }

                        // if current orientation is landscapte
                        else if (orientation == DMDO_180)
                        {
                            // tile move left
                            if (reading.AccelerationX > 0.1 && reading.AccelerationY <= 0.2 && reading.AccelerationY >= -0.2)
                            {
                                whichOne = tile.CheckMove('L');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'L';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move down
                            else if (reading.AccelerationY > 0.1 && reading.AccelerationX <= 0.2 && reading.AccelerationX >= -0.2)
                            {
                                whichOne = tile.CheckMove('D');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'D';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move up
                            else if (reading.AccelerationY < -0.1 && reading.AccelerationX <= 0.2 && reading.AccelerationX >= -0.2)
                            {
                                whichOne = tile.CheckMove('U');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'U';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }

                            }
                            // tile move up
                            else if (reading.AccelerationX < -0.1 && reading.AccelerationY <= 0.2 && reading.AccelerationY >= -0.2)
                            {
                                whichOne = tile.CheckMove('R');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'R';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }

                            }
                        }
                        // if current orientation is potrait mode
                        else if (orientation == DMDO_90)
                        {
                            // tile move down
                            if (reading.AccelerationX > 0.1 && reading.AccelerationY <= 0.2 && reading.AccelerationY >= -0.2)
                            {
                                whichOne = tile.CheckMove('D');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'D';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move right
                            else if (reading.AccelerationY > 0.1 && reading.AccelerationX <= 0.2 && reading.AccelerationX >= -0.2)
                            {
                                whichOne = tile.CheckMove('R');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'R';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move left
                            else if (reading.AccelerationY < -0.1 && reading.AccelerationX <= 0.2 && reading.AccelerationX >= -0.2)
                            {
                                whichOne = tile.CheckMove('L');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'L';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move up
                            else if (reading.AccelerationX < -0.1 && reading.AccelerationY <= 0.2 && reading.AccelerationY >= -0.2)
                            {
                                whichOne = tile.CheckMove('U');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'U';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                        }
                        // if current orientation is in another potrait mode
                        else if (orientation == DMDO_270)
                        {
                            // tile move up
                            if (reading.AccelerationX > 0.1 && reading.AccelerationY <= 0.2 && reading.AccelerationY >= -0.2)
                            {
                                whichOne = tile.CheckMove('U');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'U';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move left
                            else if (reading.AccelerationY > 0.1 && reading.AccelerationX <= 0.2 && reading.AccelerationX >= -0.2)
                            {
                                whichOne = tile.CheckMove('L');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'L';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move right
                            else if (reading.AccelerationY < -0.1 && reading.AccelerationX <= 0.2 && reading.AccelerationX >= -0.2)
                            {
                                whichOne = tile.CheckMove('R');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'R';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                            // tile move down
                            else if (reading.AccelerationX < -0.1 && reading.AccelerationY <= 0.2 && reading.AccelerationY >= -0.2)
                            {
                                whichOne = tile.CheckMove('D');

                                // tile can move, calculate new position
                                if (whichOne.Count != 0)
                                {
                                    willMove = true;
                                    newOffset = OFFSET * (-1);
                                    direction = 'D';
                                    playSound = true;
                                    m.WaitOne();
                                    canMove = false;
                                    m.ReleaseMutex();
                                }
                            }
                        }

                        // this tile can move
                        if(willMove)
                        {
                            // check which image to mvoe
                            foreach(Image img in images)
                            {
                                if(Grid.GetRow(img) == whichOne[0] && Grid.GetColumn(img) == whichOne[1] )
                                {
                                    // get the new poistion of this tile
                                    int newC = whichOne[1];
                                    int newR = whichOne[0];
                                    if(direction == 'L')
                                    {
                                        newC = whichOne[1] - 1;
                                    }
                                    else if (direction == 'R')
                                    {
                                        newC = whichOne[1] + 1;
                                    }
                                    else if (direction == 'U')
                                    {
                                        newR = whichOne[0] - 1;
                                    }
                                    else if (direction == 'D')
                                    {
                                        newR = whichOne[0] + 1;
                                    }

                                    // move this tile to a new cell
                                    Grid.SetRow(img, newR);
                                    Grid.SetColumn(img,newC);
                                    Storyboard sb = new Storyboard();
                                    RepositionThemeAnimation rt = new RepositionThemeAnimation();

                                    // calculate the distance the tile will move
                                    if (direction == 'U' || direction == 'D')
                                    {
                                        rt.FromVerticalOffset = newOffset;
                                    }
                                    else if (direction == 'L' || direction == 'R')
                                    {
                                        rt.FromHorizontalOffset = newOffset;
                                    }

                                    // play the sound and animation
                                    sb.Children.Add(rt);
                                    Storyboard.SetTarget(rt, img);
                                    sb.Begin();
                                    if(playSound)
                                    {
                                        PlaySound();
                                        playSound = false;
                                    }
                                }
                            }

                            // user doesn't win after this move
                            if (whichOne.Count == 2)
                            {
                                roamingSettings.Values["currentMove"] = tile.GetCurrentMove();
                            }
                            // user wins after this move
                            else if(whichOne.Count == 3 && whichOne[2] == 1)
                            {
                                // stop timers and clean everything up
                                if (accelerometer != null)
                                {
                                    tiltingTimer.Stop();
                                }
                                dispatcherTimer.Stop();
                                String userScore = roamingSettings.Values["userName"].ToString() + "|" + timesTicked + "|";
                                CleanUp(userScore);
                            }
                        }
                    }
                }
            });
        }



        ///Function:		CleanUp
        ///Description:     clean up and show some information to user if the user wins.
        ///Parameters:      string userScore:    the name and score of current user
        ///Return Values:   NONE
        private async void CleanUp(string userScore)
        {
            roamingSettings.Values["resume"] = "false";
            try
            {
                // save the name and score of current user to leader.txt
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync("leader.txt", CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.AppendTextAsync(file, userScore);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            // instanciate a StackPanel
            StackPanel sp = new StackPanel();

            // rating the user according to their scores
            Image star = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            Uri uri;

            // very bad score
            if (timesTicked > 480)
            {
                uri = new Uri("ms-appx:///Assets/1star.jpg");
            }
            // 3 star user
            else if (timesTicked < 240)
            {
                uri = new Uri("ms-appx:///Assets/3star.jpg");
            }
            // so so score
            else
            {
                uri = new Uri("ms-appx:///Assets/2star.jpg");
            }

            // set a star image to the user and add it into stackpanel
            bitmapImage.UriSource = uri;
            star.Source = bitmapImage;
            star.MaxHeight = 500;
            star.MaxWidth = 500;
            star.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            star.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            sp.Children.Add(star);

            // instanciate a popup 
            popUp = new Popup();
            popUp.Height = 600;
            popUp.Width = 600;

            // create a button for the user to go back to main menu
            Button mainMenuBtn = new Button();
            mainMenuBtn.Content = "MainMenu";
            mainMenuBtn.FontSize = 25;
            mainMenuBtn.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;

            // create a button for the user to go back to quit menu
            Button quitBtn = new Button();
            quitBtn.Content = "Quit";
            quitBtn.FontSize = 25;
            quitBtn.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;

            // create another stackpanel to put these two buttons.
            StackPanel newSp = new StackPanel();
            newSp.Orientation = Orientation.Horizontal;
            newSp.Children.Add(mainMenuBtn);
            newSp.Children.Add(quitBtn);

            // put the smaller Horizontal stackpanel into the big one.
            sp.Children.Add(newSp);

            // set the eventhandler of these two buttons.
            quitBtn.Click += quitBtn_Click;
            mainMenuBtn.Click += mainBtn_Click;

            // put the big stackpanel into pupup window
            popUp.Child = sp;
            popUp.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            popUp.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;

            // put this popup window into the big grid
            contentGrid.Children.Add(popUp);
            Grid.SetRow(popUp, 0);
            Grid.SetColumn(popUp, 0);

            // show the popup and hide other stuff
            InitilizeImages();
            popUp.IsOpen = true;
            bar.IsEnabled = false;
            finishBtn.IsEnabled = false;
            backButton.IsEnabled = false;
        }



        ///Function:		cameraBtn_Click -- borrowed from Norbort's ImageHelper example
        ///Description:     user wants to take a picture using camera
        ///Parameters:      object sender:      the control rises this event
        ///                 RoutedEventArgs e:  event argument
        ///Return Values:   NONE
        public async void cameraBtn_Click(object sender, RoutedEventArgs e)
        {
            var camera = new CameraCaptureUI();
            var result = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (result != null)
            {
                this.DataContext = result;
                await LoadBitmap(await result.OpenAsync(
                    FileAccessMode.Read));
            }
        }


        ///Function:		LoadBitmap -- also borrowed from Norbort's ImageHelper example
        ///Description:     set picture from camera to image controls
        ///Parameters:      IRandomAccessStream stream:      the control rises this event
        ///Return Values:   Task
        private async Task LoadBitmap(IRandomAccessStream stream)
        {
            // create a new writeableBitmap and set it to the source of displayImage1 and displayImage2
            writeableBitmap = new WriteableBitmap(1, 1);
            writeableBitmap.SetSource(stream);
            writeableBitmap.Invalidate();
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => displayImage1.Source = writeableBitmap); ;

            displayImage2.Source = writeableBitmap;
            
            // get a random array and cut the image according to this array
            int[] randomInt = tile.GetArray();
            InitilizeImages();
            roamingSettings.Values["currentMove"] = tile.GetCurrentMove();
            SaveImage();
            await CutImageToPieces(images, randomInt);
        }



        ///Function:		SaveImage -- again borrowed from Norbort's ImageHelper example
        ///Description:     save a picture to a local folder so we can use it next time
        ///Parameters:      NONE
        ///Return Values:   NONE
        public async void SaveImage()
        {
            // use the writeableBitmap
            if (writeableBitmap != null)
            {
                // create a new jpg file with a unique identifier
                Guid photoID = System.Guid.NewGuid();
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                String fileName = photoID + ".jpg";
                StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                
                try
                {
                    if (file != null)
                    {
                        // get the stream of output file
                        using (var output = await
                            file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            // get pixels from input file
                            var encoder =
                                await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, output);
                            byte[] pixels;

                            using (var stream = writeableBitmap.PixelBuffer.AsStream())
                            {
                                pixels = new byte[stream.Length];
                                await stream.ReadAsync(pixels, 0, pixels.Length);
                            }

                            encoder.SetPixelData(BitmapPixelFormat.Rgba8,
                                                    BitmapAlphaMode.Straight,
                                                    (uint)writeableBitmap.PixelWidth,
                                                    (uint)writeableBitmap.PixelHeight,
                                                    96.0, 96.0,
                                                    pixels);

                            // write stream from input file into output file
                            await encoder.FlushAsync();
                            await output.FlushAsync();
                            this.DataContext = file;

                            // put this file into recent access list and remember its name
                            mruToken = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                            Windows.Storage.ApplicationDataContainer roamingSettings =
                                Windows.Storage.ApplicationData.Current.RoamingSettings;
                            roamingSettings.Values["fileName"] = mruToken;
                            done.Set();
                        }
                    }
                }
                
                catch (Exception ex)
                {
                    var s = ex.ToString();
                    Debug.WriteLine(s);
                }
            }
        }



		///Function:		DispatcherTimerSetup
        ///Description:     to setup the game timer to check the time of the user
        ///Parameters:      NONE
        ///Return Values:   NONE
        public void DispatcherTimerSetup()
        {
			// initialize the DispatcherTimer
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            startTime = DateTimeOffset.Now;
            lastTime = startTime;
            
			// user doesn't start a new game
            if (roamingSettings.Values["resume"].ToString().Equals("true"))
            {
				// we have time record
                if (roamingSettings.Values.ContainsKey("time"))
                {
					// time record is empty
					if(roamingSettings.Values["time"].ToString().Equals(""))
					{
						timesTicked = 0;
						timerText.Text = "Game Start! Have Fun.\n";
						
					}
					else
					{
						timesTicked = Int32.Parse(roamingSettings.Values["time"].ToString());
					}
                }
                else
                {
                    timesTicked = 0;
					timerText.Text = "Game Start! Have Fun.\n";
                }
            }
        }

		

		///Function:		dispatcherTimer_Tick
        ///Description:     to setup the game timer to check the time of the user
        ///Parameters:      object sender:      the sender of the event
        ///                 object e:           event argument
        ///Return Values:   NONE
        void dispatcherTimer_Tick(object sender, object e)
        {
			// setup some basic variables
            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;
            lastTime = time;
            timesTicked++;

			// translate timesTicked to minutes and seconds then save the time.
            int seconds = timesTicked % 60;
            int minutes = timesTicked / 60;
            timerText.Text = String.Format("  {0} min {1} sec\n", minutes, seconds);
            roamingSettings.Values["time"] = timesTicked;
        }



        ///Function:		leaderBoardBtn_Click
        ///Description:     show leaderboard to the user
        ///Parameters:      object sender:       the sender of the event
        ///                 RoutedEventArgs e:   event argument
        ///Return Values:   NONE
        private void leaderBoardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(LeaderBoard));
            }
        }



        ///Function:		pauseBtn_Click
        ///Description:     to PAUSE the game
        ///Parameters:      object sender:       the sender of the event
        ///                 RoutedEventArgs e:   event argument
        ///Return Values:   NONE
        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (accelerometer != null)
            {
                tiltingTimer.Stop();
            }
            dispatcherTimer.Stop();
            roamingSettings.Values["pause"] = "true";
            imageGrid.PointerPressed -= imageGrid_PointerPressed;
            imageGrid.PointerReleased -= imageGrid_PointerReleased;
        }



        ///Function:		PhotoPage_SizeChanged
        ///Description:     to play the game from PAUSE status
        ///Parameters:      object sender:       the sender of the event
        ///                 RoutedEventArgs e:   event argument
        ///Return Values:   NONE
        private void PhotoPage_SizeChanged(object sender, RoutedEventArgs e)
        {
        }



        ///Function:		playBtn_Click
        ///Description:     to play the game from PAUSE status
        ///Parameters:      object sender:       the sender of the event
        ///                 RoutedEventArgs e:   event argument
        ///Return Values:   NONE
        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (accelerometer != null)
            {
                tiltingTimer.Start();
            }
            dispatcherTimer.Start();
            roamingSettings.Values["pause"] = "false";
            imageGrid.PointerPressed += imageGrid_PointerPressed;
            imageGrid.PointerReleased += imageGrid_PointerReleased;
        }



        ///Function:		quitBtn_Click
        ///Description:     to quit the program
        ///Parameters:      object sender:       the sender of the event
        ///                 RoutedEventArgs e:   event argument
        ///Return Values:   NONE
        private void quitBtn_Click(object sender, RoutedEventArgs e)
        {
            // close the popup window and enable buttons
            if (popUp != null)
            {
                // close the popup window and enable buttons
                bar.IsEnabled = true;
                finishBtn.IsEnabled = true;
                backButton.IsEnabled = true;

                if (popUp.IsOpen)
                {
                    popUp.IsOpen = false;
                }
            }
            Application.Current.Exit();
        }



        ///Function:		mainBtn_Click
        ///Description:     to go to main menu
        ///Parameters:      object sender:       the sender of the event
        ///                 RoutedEventArgs e:   event argument
        ///Return Values:   NONE
        private void mainBtn_Click(object sender, RoutedEventArgs e)
        {
            // navigate to mainpage
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(MainPage));
            }

            // close the popup window and enable buttons
            if (popUp != null)
            {
                bar.IsEnabled = true;
                finishBtn.IsEnabled = true;
                backButton.IsEnabled = true;

                if (popUp.IsOpen)
                {
                    popUp.IsOpen = false;
                }
            }
        }



        // a struct used to read all readings of accelarator.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        };



        ///Function:		finishBtn_Click
        ///Description:     user wants to give up
        ///Parameters:      object sender:       the sender of the event
        ///                 RoutedEventArgs e:   event argument
        ///Return Values:   NONE
        private async void finishBtn_Click(object sender, RoutedEventArgs e)
        {
            // get a final right array and put the pieces in right order
            int[] randomInt = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 };
            InitilizeImages();
            await CutImageToPieces(images, randomInt);

            // stop all timers and clean up 
            if (accelerometer != null)
            {
                tiltingTimer.Stop();
            }
            dispatcherTimer.Stop();
            String userScore = roamingSettings.Values["userName"].ToString() + "|" + timesTicked + "|";
            CleanUp(userScore);
        }



        ///Function:		LoadSoundFile
        ///Description:     loads sound file for this game
        ///Parameters:      NONE
        ///Return Values:   NONE
        async void LoadSoundFile()
        {
            snd = new MediaElement();
            folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            file = await folder.GetFileAsync("soundEffect.wav");
        }



        ///Function:		PlaySound
        ///Description:     plays sound file for this game
        ///Parameters:      NONE
        ///Return Values:   NONE
        async private void PlaySound()
        {
            var stream = await file.OpenAsync(FileAccessMode.Read);
            snd.SetSource(stream, file.ContentType);
            snd.Play();
        }

    }
}
