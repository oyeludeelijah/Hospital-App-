using SimpleHospitalApp.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleHospitalApp.Controls
{
    /// <summary>
    /// Base class for all management controls to reduce code duplication and standardize UI.
    /// </summary>
    public abstract class BaseManagementControl : UserControl
    {
        protected DataGridView dataGridView;
        protected Button btnAdd;
        protected Button btnEdit;
        protected Button btnDelete;
        protected Label lblTitle;
        protected TextBox txtSearch;
        protected Button btnSearch;
        
        protected BaseManagementControl()
        {
            InitializeBaseComponent();
            SetupEventHandlers();
        }
        
        /// <summary>
        /// Initializes the common UI components used across all management controls.
        /// </summary>
        protected virtual void InitializeBaseComponent()
        {
            // Setup control
            this.Size = new Size(940, 400);
            
            // Create title label using UIHelper
            lblTitle = UIHelper.CreateTitleLabel("Management", new Point(10, 10));
            
            // Create search controls using UIHelper
            txtSearch = UIHelper.CreateTextBox(
                new Point(530, 10), 
                new Size(200, 25), 
                "Search...");
            
            btnSearch = UIHelper.CreateButton(
                "Search", 
                new Point(740, 10), 
                new Size(80, 25));
            
            // Create DataGridView using UIHelper
            dataGridView = UIHelper.CreateDataGridView(
                new Point(10, 50), 
                new Size(810, 280));
            
            // Create buttons using UIHelper
            btnAdd = UIHelper.CreateButton(
                "Add", 
                new Point(830, 50), 
                new Size(100, 30));
            
            btnEdit = UIHelper.CreateButton(
                "Edit", 
                new Point(830, 90), 
                new Size(100, 30));
            
            btnDelete = UIHelper.CreateButton(
                "Delete", 
                new Point(830, 130), 
                new Size(100, 30));
        }
        
        /// <summary>
        /// Sets up event handlers for common buttons.
        /// </summary>
        protected virtual void SetupEventHandlers()
        {
            btnSearch.Click += (s, e) => Search();
            btnAdd.Click += (s, e) => Add();
            btnEdit.Click += (s, e) => Edit();
            btnDelete.Click += (s, e) => Delete();
        }
        
        /// <summary>
        /// Displays a standard error message with the provided exception details.
        /// </summary>
        protected void ShowError(string operation, Exception ex)
        {
            ErrorHandlingHelper.ShowError(operation, ex);
        }
        
        /// <summary>
        /// Displays a selection required message.
        /// </summary>
        protected void ShowSelectionRequired(string itemType)
        {
            ErrorHandlingHelper.ShowSelectionRequired(itemType, btnEdit.Text.ToLower());
        }
        
        /// <summary>
        /// Confirms deletion of an item.
        /// </summary>
        protected DialogResult ConfirmDelete(string itemName, string itemType)
        {
            return ErrorHandlingHelper.ConfirmDelete(itemName, itemType);
        }
        
        /// <summary>
        /// Shows a message indicating why an item cannot be deleted.
        /// </summary>
        protected void ShowCannotDelete(string itemName, string itemType, string reason)
        {
            ErrorHandlingHelper.ShowCannotDelete(itemName, itemType, reason);
        }
        
        /// <summary>
        /// Handles the search functionality. Must be implemented by derived classes.
        /// </summary>
        protected abstract void Search();
        
        /// <summary>
        /// Handles the add functionality. Must be implemented by derived classes.
        /// </summary>
        protected abstract void Add();
        
        /// <summary>
        /// Handles the edit functionality. Must be implemented by derived classes.
        /// </summary>
        protected abstract void Edit();
        
        /// <summary>
        /// Handles the delete functionality. Must be implemented by derived classes.
        /// </summary>
        protected abstract void Delete();
    }
}
