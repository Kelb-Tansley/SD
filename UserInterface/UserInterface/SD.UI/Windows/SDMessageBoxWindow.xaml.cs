﻿using System.Windows;
using SD.UI.Utilities;

namespace SD.UI.Windows
{
    /// <summary>
    /// Interaction logic for EEMessageBoxWindow.xaml
    /// </summary>
    internal partial class SDMessageBoxWindow : Window
    {
        #region Properties

        internal string Caption
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value;
            }
        }

        internal string Message
        {
            get
            {
                return TextBlock_Message.Text;
            }
            set
            {
                TextBlock_Message.Text = value;
            }
        }

        internal string OkButtonText
        {
            get
            {
                return Label_Ok.Content.ToString();
            }
            set
            {
                Label_Ok.Content = value.TryAddKeyboardAccellerator();
            }
        }

        internal string CancelButtonText
        {
            get
            {
                return Label_Cancel.Content.ToString();
            }
            set
            {
                Label_Cancel.Content = value.TryAddKeyboardAccellerator();
            }
        }

        internal string YesButtonText
        {
            get
            {
                return Label_Yes.Content.ToString();
            }
            set
            {
                Label_Yes.Content = value.TryAddKeyboardAccellerator();
            }
        }

        internal string NoButtonText
        {
            get
            {
                return Label_No.Content.ToString();
            }
            set
            {
                Label_No.Content = value.TryAddKeyboardAccellerator();
            }
        }

        public MessageBoxResult Result { get; set; }
        #endregion
        #region Constructor

        internal SDMessageBoxWindow(string message, bool canCopyMessage = false)
        {
            InitializeComponent();

            Message = message;
            Image_MessageBox.Visibility = Visibility.Collapsed;
            DisplayButtons(MessageBoxButton.OK);
            ViewCopyMessageButton(canCopyMessage);
        }

        internal SDMessageBoxWindow(string message, string caption, bool canCopyMessage = false)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            Image_MessageBox.Visibility = Visibility.Collapsed;
            DisplayButtons(MessageBoxButton.OK);
            ViewCopyMessageButton(canCopyMessage);
        }

        internal SDMessageBoxWindow(string message, string caption, MessageBoxButton button, bool canCopyMessage = false)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            Image_MessageBox.Visibility = Visibility.Collapsed;

            DisplayButtons(button);
            ViewCopyMessageButton(canCopyMessage);
        }

        internal SDMessageBoxWindow(string message, string caption, MessageBoxImage image, bool canCopyMessage = false)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            DisplayImage(image);
            DisplayButtons(MessageBoxButton.OK);
            ViewCopyMessageButton(canCopyMessage);
        }

        internal SDMessageBoxWindow(string message, string caption, MessageBoxButton button, MessageBoxImage image, bool canCopyMessage = false)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            Image_MessageBox.Visibility = Visibility.Collapsed;

            DisplayButtons(button);
            DisplayImage(image);
            ViewCopyMessageButton(canCopyMessage);
        }

        #endregion

        #region Private methods
        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    // Hide all but OK, Cancel
                    Button_OK.Visibility = Visibility.Visible;
                    Button_OK.Focus();
                    Button_Cancel.Visibility = Visibility.Visible;

                    Button_Yes.Visibility = Visibility.Collapsed;
                    Button_No.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    // Hide all but Yes, No
                    Button_Yes.Visibility = Visibility.Visible;
                    Button_Yes.Focus();
                    Button_No.Visibility = Visibility.Visible;

                    Button_OK.Visibility = Visibility.Collapsed;
                    Button_Cancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    // Hide only OK
                    Button_Yes.Visibility = Visibility.Visible;
                    Button_Yes.Focus();
                    Button_No.Visibility = Visibility.Visible;
                    Button_Cancel.Visibility = Visibility.Visible;

                    Button_OK.Visibility = Visibility.Collapsed;
                    break;
                default:
                    // Hide all but OK
                    Button_OK.Visibility = Visibility.Visible;
                    Button_OK.Focus();

                    Button_Yes.Visibility = Visibility.Collapsed;
                    Button_No.Visibility = Visibility.Collapsed;
                    Button_Cancel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void DisplayImage(MessageBoxImage image)
        {
            Icon icon;

            switch (image)
            {
                case MessageBoxImage.Exclamation:       // Enumeration value 48 - also covers "Warning"
                    icon = SystemIcons.Exclamation;
                    break;
                case MessageBoxImage.Error:             // Enumeration value 16, also covers "Hand" and "Stop"
                    icon = SystemIcons.Hand;
                    break;
                case MessageBoxImage.Information:       // Enumeration value 64 - also covers "Asterisk"
                    icon = SystemIcons.Information;
                    break;
                case MessageBoxImage.Question:
                    icon = SystemIcons.Question;
                    break;
                default:
                    icon = SystemIcons.Information;
                    break;
            }

            Image_MessageBox.Source = icon.ToImageSource();
            Image_MessageBox.Visibility = Visibility.Visible;
        }

        private void ViewCopyMessageButton(bool canCopyMessage)
        {
            if (canCopyMessage)
            {
                Button_Clipboard_Copy.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Clickers
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void Button_Clipboard_Copy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(TextBlock_Message.Text);
        }
        #endregion
    }
}
