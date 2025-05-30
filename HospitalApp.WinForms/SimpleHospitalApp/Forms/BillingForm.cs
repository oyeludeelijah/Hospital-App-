using SimpleHospitalApp.Data;
using SimpleHospitalApp.Models;
using SimpleHospitalApp.Helpers;
using SimpleHospitalApp.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleHospitalApp.Forms
{
    public class BillingForm : Form
    {
        private Label lblTitle;
        private Label lblPatient;
        private Label lblAmount;
        private Label lblDescription;
        private Label lblPaymentStatus;
        private Label lblPaymentMethod;
        private ComboBox cmbPatient;
        private NumericUpDown numAmount;
        private TextBox txtDescription;
        private ComboBox cmbPaymentStatus;
        private ComboBox cmbPaymentMethod;
        private Button btnSave;
        private Button btnCancel;
        
        private Billing? _billing;
        private bool _isEdit;
        
        public BillingForm(Billing? billing = null)
        {
            _billing = billing;
            _isEdit = billing != null;
            
            InitializeComponent();
            LoadPatients();
            
            if (_isEdit)
            {
                LoadBillingData();
            }
        }
        
        private void InitializeComponent()
        {
            // Form settings
            this.Text = _isEdit ? "Edit Billing" : "Add Billing";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Create title
            lblTitle = new Label();
            lblTitle.Text = _isEdit ? "Edit Billing" : "Add Billing";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Size = new Size(400, 30);
            lblTitle.Location = new Point(20, 20);
            
            // Create patient label and combobox
            lblPatient = new Label();
            lblPatient.Text = "Patient:";
            lblPatient.Size = new Size(100, 25);
            lblPatient.Location = new Point(20, 70);
            
            cmbPatient = new ComboBox();
            cmbPatient.Size = new Size(250, 25);
            cmbPatient.Location = new Point(160, 70);
            cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // Create amount label and numeric updown
            lblAmount = new Label();
            lblAmount.Text = "Amount ($):";
            lblAmount.Size = new Size(100, 25);
            lblAmount.Location = new Point(20, 110);
            
            numAmount = new NumericUpDown();
            numAmount.Size = new Size(250, 25);
            numAmount.Location = new Point(160, 110);
            numAmount.Minimum = 0;
            numAmount.Maximum = 10000;
            numAmount.DecimalPlaces = 2;
            numAmount.Value = 0;
            
            // Create description label and textbox
            lblDescription = new Label();
            lblDescription.Text = "Description:";
            lblDescription.Size = new Size(100, 25);
            lblDescription.Location = new Point(20, 150);
            
            txtDescription = new TextBox();
            txtDescription.Size = new Size(250, 25);
            txtDescription.Location = new Point(160, 150);
            
            // Create payment status label and combobox
            lblPaymentStatus = new Label();
            lblPaymentStatus.Text = "Payment Status:";
            lblPaymentStatus.Size = new Size(100, 25);
            lblPaymentStatus.Location = new Point(20, 190);
            
            cmbPaymentStatus = new ComboBox();
            cmbPaymentStatus.Size = new Size(250, 25);
            cmbPaymentStatus.Location = new Point(160, 190);
            cmbPaymentStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentStatus.Items.AddRange(new string[] { "Pending", "Paid", "Overdue" });
            cmbPaymentStatus.SelectedIndex = 0;
            
            // Create payment method label and combobox
            lblPaymentMethod = new Label();
            lblPaymentMethod.Text = "Payment Method:";
            lblPaymentMethod.Size = new Size(100, 25);
            lblPaymentMethod.Location = new Point(20, 230);
            
            cmbPaymentMethod = new ComboBox();
            cmbPaymentMethod.Size = new Size(250, 25);
            cmbPaymentMethod.Location = new Point(160, 230);
            cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentMethod.Items.AddRange(new string[] { "Cash", "Credit Card", "Debit Card", "Insurance", "Bank Transfer" });
            cmbPaymentMethod.SelectedIndex = 0;
            
            // Create buttons
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Size = new Size(100, 35);
            btnSave.Location = new Point(160, 280);
            btnSave.BackColor = SystemColors.ButtonFace;
            btnSave.Click += BtnSave_Click;
            
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(270, 280);
            btnCancel.BackColor = SystemColors.ButtonFace;
            btnCancel.Click += (s, e) => this.Close();
            
            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPatient);
            this.Controls.Add(cmbPatient);
            this.Controls.Add(lblAmount);
            this.Controls.Add(numAmount);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblPaymentStatus);
            this.Controls.Add(cmbPaymentStatus);
            this.Controls.Add(lblPaymentMethod);
            this.Controls.Add(cmbPaymentMethod);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }
        
        private void LoadPatients()
        {
            try
            {
                // Get patients from DataService instead of direct database access
                var patients = DataService.Instance.GetAllPatients();
                
                // Create a simple list for the dropdown
                cmbPatient.Items.Clear();
                
                // Add a default item
                cmbPatient.Items.Add("-- Select Patient --");
                
                // Add each patient
                foreach (var patient in patients)
                {
                    string displayName = $"{patient.FirstName} {patient.LastName} (ID: {patient.Id})";
                    cmbPatient.Items.Add(displayName);
                }
                
                // Select the first item
                if (cmbPatient.Items.Count > 0)
                {
                    cmbPatient.SelectedIndex = 0;
                }
                
                // If no patients were found, inform the user
                if (patients.Count == 0)
                {
                    MessageBox.Show("No patients found. Please add patients first.", 
                        "No Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading patients: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadBillingData()
        {
            if (_billing == null) return;
            
            try
            {
                using (var context = new HospitalContext())
                {
                    var patient = context.Patients.FirstOrDefault(p => p.Id == _billing.PatientId);
                    
                    if (patient != null)
                    {
                        // Find and select the patient in the combobox
                        for (int i = 0; i < cmbPatient.Items.Count; i++)
                        {
                            var item = (ComboBoxItem)cmbPatient.Items[i];
                            if ((int)item.Value == patient.Id)
                            {
                                cmbPatient.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    
                    numAmount.Value = _billing.Amount;
                    txtDescription.Text = _billing.Description;
                    
                    // Set payment status
                    for (int i = 0; i < cmbPaymentStatus.Items.Count; i++)
                    {
                        if (cmbPaymentStatus.Items[i]?.ToString() == _billing?.PaymentStatus)
                        {
                            cmbPaymentStatus.SelectedIndex = i;
                            break;
                        }
                    }
                    
                    // Set payment method
                    for (int i = 0; i < cmbPaymentMethod.Items.Count; i++)
                    {
                        if (cmbPaymentMethod.Items[i]?.ToString() == _billing?.PaymentMethod)
                        {
                            cmbPaymentMethod.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading billing data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (numAmount.Value <= 0)
            {
                MessageBox.Show("Please enter a valid amount.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Validate patient selection
            int patientId = GetSelectedPatientId();
            if (patientId <= 0)
            {
                MessageBox.Show("Please select a valid patient.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // Use the DataService for data operations
                if (_isEdit && _billing != null)
                {
                    // Update existing billing
                    _billing.PatientId = GetSelectedPatientId();
                    _billing.Amount = numAmount.Value;
                    _billing.Description = txtDescription.Text.Trim();
                    _billing.PaymentStatus = cmbPaymentStatus.SelectedItem?.ToString() ?? "Pending";
                    _billing.PaymentMethod = cmbPaymentMethod.SelectedItem?.ToString() ?? string.Empty;
                    
                    // Update the billing
                    DataService.Instance.UpdateBilling(_billing);
                    MessageBox.Show("Billing updated successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Create new billing
                    var newBilling = new Billing
                    {
                        PatientId = GetSelectedPatientId(),
                        BillingDate = DateTime.Now,
                        Amount = numAmount.Value,
                        Description = txtDescription.Text.Trim(),
                        PaymentStatus = cmbPaymentStatus.SelectedItem?.ToString() ?? "Pending",
                        PaymentMethod = cmbPaymentMethod.SelectedItem?.ToString() ?? string.Empty
                    };
                    
                    // Add the billing
                    DataService.Instance.AddBilling(newBilling);
                    MessageBox.Show("Billing added successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving billing: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private int GetSelectedPatientId()
        {
            if (cmbPatient.SelectedIndex <= 0) // Not selected or default item
                return 0;
            
            try
            {
                string selectedText = cmbPatient.SelectedItem.ToString();
                // Extract ID from format: "FirstName LastName (ID: X)"
                int startPos = selectedText.LastIndexOf("ID: ");
                if (startPos < 0) return 0; // Pattern not found
                
                startPos += 4; // Move past "ID: "
                int endPos = selectedText.LastIndexOf(")");
                if (endPos < 0 || endPos <= startPos) return 0; // Invalid format
                
                string idString = selectedText.Substring(startPos, endPos - startPos);
                if (int.TryParse(idString, out int id))
                    return id;
                return 0;
            }
            catch
            {
                return 0; // Return default value if parsing fails
            }
        }
    }
}
