using Prism.Commands;
using Prism.Mvvm;
using SD.UI.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace SD.UI.Helpers
{
    public enum WindowDockPosition
    {
        Undocked,
        Left,
        Right,
    }
    public class WindowResizer : BindableBase, IWindowBehavior
    {
        #region Private Members
        /// <summary>
        /// The window to handle the resizing for
        /// </summary>
        private Window _mWindow;

        /// <summary>
        /// The last calculated available screen size
        /// </summary>
        private Rect mScreenSize = new Rect();

        /// <summary>
        /// How close to the edge the window has to be to be detected as at the edge of the screen
        /// </summary>
        private int mEdgeTolerance = 2;

        /// <summary>
        /// The transform matrix used to convert WPF sizes to screen pixels
        /// </summary>
        private Matrix mTransformToDevice;

        /// <summary>
        /// The last screen the window was on
        /// </summary>
        private nint mLastScreen;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mLastDock = WindowDockPosition.Undocked;
        private Visibility _maxButtonVisibility = Visibility.Hidden;
        private Visibility _restoreButtonVisibility = Visibility.Visible;
        #endregion

        #region Public Members
        public Window MWindow
        {
            get { return _mWindow; }
            set
            {
                if (_mWindow == null)
                {
                    _mWindow = value;
                }
            }
        }

        public DelegateCommand WindowLoaded { get; set; }
        public DelegateCommand MenuCommand { get; set; }
        public DelegateCommand MinimizeCommand { get; set; }
        public DelegateCommand MaximizeCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        public DelegateCommand WindowClosing { get; set; }


        public Visibility MaxButtonVisibility
        {
            get { return _maxButtonVisibility; }
            set
            {
                _maxButtonVisibility = value;
                RaisePropertyChanged(nameof(MaxButtonVisibility));
            }
        }

        public Visibility RestoreButtonVisibility
        {
            get { return _restoreButtonVisibility; }
            set
            {
                _restoreButtonVisibility = value; RaisePropertyChanged(nameof(RestoreButtonVisibility));
            }
        }
        #endregion

        private void InitilizeCommands()
        {
            WindowLoaded = new DelegateCommand(OnWindowLoaded);
            MenuCommand = new DelegateCommand(OnMenuCommand);
            MinimizeCommand = new DelegateCommand(OnMinimizeCommand);
            MaximizeCommand = new DelegateCommand(OnMaximizeCommand);
            CloseCommand = new DelegateCommand(OnCloseCommand);
        }
        public void OnWindowLoaded()
        {
        }
        public void LoadMainWindow()
        {
        }
        public void OnMenuCommand()
        {
            SystemCommands.ShowSystemMenu(MWindow, MWindow.PointToScreen(Mouse.GetPosition(MWindow)));
        }
        public void OnMinimizeCommand()
        {
            MWindow.WindowState = WindowState.Minimized;
        }
        public void OnMaximizeCommand()
        {
            MWindow.WindowState ^= WindowState.Maximized;
        }
        public void OnCloseCommand()
        {
            MWindow.Close();
        }


        #region Dll Imports

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(nint hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        static extern nint MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("user32.dll")]
        public static extern nint MonitorFromWindow(nint hMonitor, int flags);
        #endregion

        #region Public Events

        /// <summary>
        /// Called when the window dock position changes
        /// </summary>
        public event Action<WindowDockPosition> WindowDockChanged = (dock) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window">The window to monitor and correctly maximize</param>
        /// <param name="adjustSize">The callback for the host to adjust the maximum available size if needed</param>
        public WindowResizer(Window shellWindow)
        {
            _mWindow = shellWindow;
            InitilizeCommands();
            InitializeWindowResizer();
        }


        public void InitializeWindowResizer()
        {
            if (_mWindow == null)
                return;

            // Create transform visual (for converting WPF size to pixel size)
            GetTransform();

            // Listen out for source initialized to setup
            _mWindow.SourceInitialized += Window_SourceInitialized;

            // Monitor for edge docking
            _mWindow.SizeChanged += Window_SizeChanged;

            _mWindow.StateChanged += (sender, e) =>
            {
                RestoreButtonVisibility ^= Visibility.Hidden;
                MaxButtonVisibility ^= Visibility.Hidden;
            };
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Gets the transform object used to convert WPF sizes to screen pixels
        /// </summary>
        private void GetTransform()
        {
            // Get the visual source
            var source = PresentationSource.FromVisual(MWindow);

            // Reset the transform to default
            mTransformToDevice = default;

            // If we cannot get the source, ignore
            if (source == null)
                return;

            // Otherwise, get the new transform object
            mTransformToDevice = source.CompositionTarget.TransformToDevice;
        }

        /// <summary>
        /// Initialize and hook into the windows message pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            // Get the handle of this window
            var handle = new WindowInteropHelper(MWindow).Handle;
            var handleSource = HwndSource.FromHwnd(handle);

            // If not found, end
            if (handleSource == null)
                return;

        }
        #endregion

        #region Edge Docking

        /// <summary>
        /// Monitors for size changes and detects if the window has been docked (Aero snap) to an edge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // We cannot find positioning until the window transform has been established
            if (mTransformToDevice == default)
                return;

            // Get the WPF size
            var size = e.NewSize;

            // Get window rectangle
            var top = MWindow.Top;
            var left = MWindow.Left;
            var bottom = top + size.Height;
            var right = left + MWindow.Width;

            // Get window position/size in device pixels
            var windowTopLeft = mTransformToDevice.Transform(new System.Windows.Point(left, top));
            var windowBottomRight = mTransformToDevice.Transform(new System.Windows.Point(right, bottom));

            // Check for edges docked
            var edgedTop = windowTopLeft.Y <= mScreenSize.Top + mEdgeTolerance;
            var edgedLeft = windowTopLeft.X <= mScreenSize.Left + mEdgeTolerance;
            var edgedBottom = windowBottomRight.Y >= mScreenSize.Bottom - mEdgeTolerance;
            var edgedRight = windowBottomRight.X >= mScreenSize.Right - mEdgeTolerance;

            // Get docked position
            var dock = WindowDockPosition.Undocked;

            // Left docking
            if (edgedTop && edgedBottom && edgedLeft)
                dock = WindowDockPosition.Left;
            else if (edgedTop && edgedBottom && edgedRight)
                dock = WindowDockPosition.Right;
            // None
            else
                dock = WindowDockPosition.Undocked;

            // If dock has changed
            if (dock != mLastDock)
                // Inform listeners
                WindowDockChanged(dock);

            // Save last dock position
            mLastDock = dock;
        }

        #endregion

        #region Windows Proc

        /// <summary>
        /// Listens out for all windows messages for this window
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private nint WindowProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            switch (msg)
            {
                // Handle the GetMinMaxInfo of the Window
                case 0x0024:/* WM_GETMINMAXINFO */
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return 0;
        }

        #endregion

        /// <summary>
        /// Get the min/max window size for this window
        /// Correctly accounting for the taskbar size and position
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        private void WmGetMinMaxInfo(nint hwnd, nint lParam)
        {
            // Get the point position to determine what screen we are on
            //POINT lMousePosition;

            // Get the primary monitor at cursor position 0,0
            var lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);

            // Try and get the primary screen information
            var lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
                return;

            // Now get the current screen
            var lCurrentScreen = MonitorFromWindow(hwnd, (int)MonitorOptions.MONITOR_DEFAULTTONEAREST);

            // If this has changed from the last one, update the transform
            if (lCurrentScreen != mLastScreen || mTransformToDevice == default)
                GetTransform();

            // Try and get the current screen information
            var lCurrentScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lCurrentScreen, lCurrentScreenInfo) == false)
                return;

            // Store last know screen
            mLastScreen = lCurrentScreen;

            // Get min/max structure to fill with information
            var lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            //If it is the primary screen, use the rcWork variable
            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            //Otherwise it's the rcMonitor values
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lCurrentScreenInfo.rcWork.Right - lCurrentScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lCurrentScreenInfo.rcWork.Bottom - lCurrentScreenInfo.rcWork.Top;
            }

            // Set min size
            var minSize = mTransformToDevice.Transform(new System.Windows.Point(MWindow.MinWidth, MWindow.MinHeight));

            lMmi.ptMinTrackSize.X = (int)minSize.X;
            lMmi.ptMinTrackSize.Y = (int)minSize.Y;

            lMmi.ptMaxTrackSize.X = lMmi.ptMaxSize.X;
            lMmi.ptMaxTrackSize.Y = lMmi.ptMaxSize.Y;

            // Store new size
            mScreenSize = new Rect(lMmi.ptMaxPosition.X, lMmi.ptMaxPosition.Y, lMmi.ptMaxSize.X, lMmi.ptMaxSize.Y);

            // Now we have the max size, allow the host to tweak as needed
            Marshal.StructureToPtr(lMmi, lParam, true);
        }
    }

    #region Dll Helper Structures

    enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public Rectangle rcMonitor = new Rectangle();
        public Rectangle rcWork = new Rectangle();
        public int dwFlags = 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left, Top, Right, Bottom;

        public Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// x coordinate of point.
        /// </summary>
        public int X;
        /// <summary>
        /// y coordinate of point.
        /// </summary>
        public int Y;

        /// <summary>
        /// Construct a point of coordinates (x,y).
        /// </summary>
        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    internal static class SystemHelper
    {
        public static int GetCurrentDPI()
        {
            var dPI = (int)typeof(SystemParameters)?.GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null, null);
            return dPI;
        }

        public static double GetCurrentDPIScaleFactor()
        {
            return Convert.ToDouble(GetCurrentDPI()) / 96;
        }

        public static System.Windows.Point GetMouseScreenPosition()
        {
            GetCursorPos(out var point);
            return new System.Windows.Point(point.X, point.Y);
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Windows.Point point);
    }
    #endregion
}
