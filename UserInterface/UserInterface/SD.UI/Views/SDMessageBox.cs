using SD.UI.Windows;
using System.Windows;

namespace SD.UI.Views;
public class SDMessageBox
{
    /// <summary>
    /// Displays a message box that has a message and returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
    public static MessageBoxResult Show(string messageBoxText, bool canCopyMessage = false)
    {
        SetIncomingTextFormat(ref messageBoxText);
        return System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            var msg = new SDMessageBoxWindow(messageBoxText, canCopyMessage);
            msg.ShowDialog();

            return msg.Result;
        });
    }

    /// <summary>
    /// Displays a message box that has a message and a title bar caption; and that returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
    public static MessageBoxResult Show(string messageBoxText, string caption, bool canCopyMessage = false)
    {
        SetIncomingTextFormat(ref messageBoxText);
        return System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            var msg = new SDMessageBoxWindow(messageBoxText, caption, canCopyMessage);
            msg.ShowDialog();

            return msg.Result;
        });
    }

    /// <summary>
    /// Displays a message box in front of the specified window. The message box displays a message and returns a result.
    /// </summary>
    /// <param name="owner">A System.Windows.Window that represents the owner window of the message box.</param>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
    public static MessageBoxResult Show(Window owner, string messageBoxText, bool canCopyMessage = false)
    {
        SetIncomingTextFormat(ref messageBoxText);
        return System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            var msg = new SDMessageBoxWindow(messageBoxText, canCopyMessage);
            msg.Owner = owner;
            msg.ShowDialog();

            return msg.Result;
        });
    }

    /// <summary>
    /// Displays a message box in front of the specified window. The message box displays a message and title bar caption; and it returns a result.
    /// </summary>
    /// <param name="owner">A System.Windows.Window that represents the owner window of the message box.</param>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
    public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, bool canCopyMessage = false)
    {
        SetIncomingTextFormat(ref messageBoxText);
        return System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            var msg = new SDMessageBoxWindow(messageBoxText, caption, canCopyMessage);
            msg.Owner = owner;
            msg.ShowDialog();

            return msg.Result;
        });
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, and button; and that returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A System.Windows.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, bool canCopyMessage = false)
    {
        SetIncomingTextFormat(ref messageBoxText);
        return System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            var msg = new SDMessageBoxWindow(messageBoxText, caption, button, canCopyMessage);
            msg.ShowDialog();

            return msg.Result;
        });
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and that returns a result.
    /// </summary>
    /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
    /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
    /// <param name="button">A System.Windows.MessageBoxButton value that specifies which button or buttons to display.</param>
    /// <param name="icon">A System.Windows.MessageBoxImage value that specifies the icon to display.</param>
    /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, bool canCopyMessage = false)
    {
        SetIncomingTextFormat(ref messageBoxText);
        return System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            var msg = new SDMessageBoxWindow(messageBoxText, caption, button, icon, canCopyMessage);
            msg.ShowDialog();

            return msg.Result;
        });
    }
    public static MessageBoxResult Show(string messageBoxText, string caption, string yesButtonText, string noButtonText, string cancelButtonText, MessageBoxImage icon)
    {
        SetIncomingTextFormat(ref messageBoxText);
        return System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            var msg = new SDMessageBoxWindow(messageBoxText, caption, MessageBoxButton.YesNoCancel, icon, false);
            msg.YesButtonText = yesButtonText;
            msg.NoButtonText = noButtonText;
            msg.CancelButtonText = cancelButtonText;

            msg.ShowDialog();
            return msg.Result;

        });
    }
    private static void SetIncomingTextFormat(ref string messageBoxText)
    {
        if (!string.IsNullOrWhiteSpace(messageBoxText) && messageBoxText.Contains("\\n"))
            messageBoxText = $"{messageBoxText.Replace("\\n", "\n")}";
    }

}
