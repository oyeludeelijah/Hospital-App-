using SimpleHospitalApp.Data;
using SimpleHospitalApp.Models;
using SimpleHospitalApp.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleHospitalApp.Forms
{
    public class DepartmentForm : Form
    {
        private Label lblTitle;
        private Label lblName;
        private Label lblDescription;
        private TextBox txtName;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnCancel;
        
        private Department? _department;
        private bool _isEdit;
        
        public DepartmentForm(Department? department = null)
        {
            _department = department;
            _isEdit = department != null;
            
            InitializeComponent();
            
            if (_isEdit)
            {
                LoadDepartmentData();
            }
        }
        
        private void InitializeComponent()
        {
            // Form settings
            this.Text = _isEdit ? "Edit Department" : "Add Department";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Create title
            lblTitle = new Label();
            lblTitle.Text = _isEdit ? "Edit Department" : "Add Department";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Size = new Size(350, 30);
            lblTitle.Location = new Point(20, 20);
            
            // Create name label and textbox
            lblName = new Label();
            lblName.Text = "Name:";
            lblName.Size = new Size(100, 25);
            lblName.Location = new Point(20, 70);
            
            txtName = new TextBox();
            txtName.Size = new Size(250, 25);
            txtName.Location = new Point(130, 70);
            
            // Create description label and textbox
            lblDescription = new Label();
            lblDescription.Text = "Description:";
            lblDescription.Size = new Size(100, 25);
            lblDescription.Location = new Point(20, 110);
            
            txtDescription = new TextBox();
            txtDescription.Multiline = true;
            txtDescription.Size = new Size(250, 80);
            txtDescription.Location = new Point(130, 110);
            
            // Create buttons
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Size = new Size(100, 35);
            btnSave.Location = new Point(130, 210);
            btnSave.BackColor = SystemColors.ButtonFace;
            btnSave.Click += BtnSave_Click;
            
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(240, 210);
            btnCancel.BackColor = SystemColors.ButtonFace;
            btnCancel.Click += (s, e) => this.Close();
            
            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }
        
        private void LoadDepartmentData()
        {
            if (_department == null) return;
            
            txtName.Text = _department.Name;
            txtDescription.Text = _department.Description;
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Please enter a department name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // Use DataService for data operations
                if (_isEdit && _department != null)
                {
                    // Update existing department
                    _department.Name = txtName.Text.Trim();
                    _department.Description = txtDescription.Text.Trim();
                    
                    // Update in DataService
                    DataService.Instance.UpdateDepartment(_department);
                    MessageBox.Show("Department updated successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Create new department
                    var newDepartment = new Department
                    {
                        Name = txtName.Text.Trim(),
                        Description = txtDescription.Text.Trim()
                    };
                    
                    // Add to DataService
                    DataService.Instance.AddDepartment(newDepartment);
                    MessageBox.Show("Department added successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving department: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
