using System;
using System.Windows.Forms;

namespace SimpleHospitalApp.Helpers
{
    /// <summary>
    /// Utility class for standardized error handling across the application.
    /// </summary>
    public static class ErrorHandlingHelper
    {
        /// <summary>
        /// Displays a standard error message for exceptions.
        /// </summary>
        /// <param name="operation">Description of the operation that failed</param>
        /// <param name="ex">The exception that occurred</param>
        public static void ShowError(string operation, Exception ex)
        {
            MessageBox.Show(
                $"Error {operation}: {ex.Message}", 
                "Error", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error
            );
        }
        
        /// <summary>
        /// Displays a validation error message.
        /// </summary>
        /// <param name="message">The validation error message</param>
        public static void ShowValidationError(string message)
        {
            MessageBox.Show(
                message, 
                "Validation Error", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Warning
            );
        }
        
        /// <summary>
        /// Displays a message when a selection is required.
        /// </summary>
        /// <param name="itemType">The type of item that needs to be selected</param>
        /// <param name="action">The action that requires a selection</param>
        public static void ShowSelectionRequired(string itemType, string action = "edit")
        {
            MessageBox.Show(
                $"Please select a {itemType} to {action}.", 
                "No Selection", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information
            );
        }
        
        /// <summary>
        /// Displays a confirmation dialog for deletion.
        /// </summary>
        /// <param name="itemName">Name of the item to delete</param>
        /// <param name="itemType">Type of the item to delete</param>
        /// <returns>The dialog result (Yes/No)</returns>
        public static DialogResult ConfirmDelete(string itemName, string itemType)
        {
            return MessageBox.Show(
                $"Are you sure you want to delete the {itemType} '{itemName}'?", 
                "Confirm Delete", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question
            );
        }
        
        /// <summary>
        /// Displays a message indicating why an item cannot be deleted.
        /// </summary>
        /// <param name="itemName">Name of the item that can't be deleted</param>
        /// <param name="itemType">Type of the item that can't be deleted</param>
        /// <param name="reason">Reason why the item can't be deleted</param>
        public static void ShowCannotDelete(string itemName, string itemType, string reason)
        {
            MessageBox.Show(
                $"Cannot delete {itemType} '{itemName}' because {reason}", 
                "Cannot Delete", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Warning
            );
        }
        
        /// <summary>
        /// Displays a success message after an operation completes.
        /// </summary>
        /// <param name="message">The success message</param>
        public static void ShowSuccess(string message)
        {
            MessageBox.Show(
                message, 
                "Success", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information
            );
        }
    }
}
