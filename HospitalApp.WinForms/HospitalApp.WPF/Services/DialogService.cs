using System;
using System.Threading.Tasks;
using System.Windows;

namespace HospitalApp.WPF.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "Information");
        void ShowError(string message, string title = "Error");
        void ShowWarning(string message, string title = "Warning");
        bool ShowConfirmation(string message, string title = "Confirmation");
        Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation");
        string? ShowInputDialog(string message, string title = "Input", string defaultValue = "");
    }

    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowConfirmation(string message, string title = "Confirmation")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation")
        {
            return await Task.Run(() => ShowConfirmation(message, title));
        }

        public string? ShowInputDialog(string message, string title = "Input", string defaultValue = "")
        {
            // Simple input dialog implementation
            // In a real application, you might want to create a custom dialog window
            var dialog = new Window
            {
                Title = title,
                Width = 350,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            var panel = new System.Windows.Controls.StackPanel
            {
                Margin = new Thickness(10)
            };

            var label = new System.Windows.Controls.TextBlock
            {
                Text = message,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var textBox = new System.Windows.Controls.TextBox
            {
                Text = defaultValue,
                Margin = new Thickness(0, 0, 0, 20)
            };

            var buttonPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var okButton = new System.Windows.Controls.Button
            {
                Content = "OK",
                Width = 75,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };

            var cancelButton = new System.Windows.Controls.Button
            {
                Content = "Cancel",
                Width = 75,
                IsCancel = true
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            panel.Children.Add(label);
            panel.Children.Add(textBox);
            panel.Children.Add(buttonPanel);

            dialog.Content = panel;

            bool? result = null;
            okButton.Click += (s, e) =>
            {
                result = true;
                dialog.Close();
            };

            dialog.ShowDialog();

            return result == true ? textBox.Text : null;
        }
    }
}
