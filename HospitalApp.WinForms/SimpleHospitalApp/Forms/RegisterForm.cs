using SimpleHospitalApp.Data;
using SimpleHospitalApp.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleHospitalApp.Forms
{
    public class RegisterForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtEmail;
        private ComboBox cmbRole;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblConfirmPassword;
        private Label lblEmail;
        private Label lblRole;
        private Label lblError;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Hospital Management System - Register";
            this.Size = new Size(450, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create title
            lblTitle = new Label();
            lblTitle.Text = "Create New Account";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(350, 40);
            lblTitle.Location = new Point(50, 20);

            // Create username label and textbox
            lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Size = new Size(120, 25);
            lblUsername.Location = new Point(30, 80);

            txtUsername = new TextBox();
            txtUsername.Size = new Size(250, 25);
            txtUsername.Location = new Point(150, 80);

            // Create email label and textbox
            lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Size = new Size(120, 25);
            lblEmail.Location = new Point(30, 120);

            txtEmail = new TextBox();
            txtEmail.Size = new Size(250, 25);
            txtEmail.Location = new Point(150, 120);

            // Create password label and textbox
            lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Size = new Size(120, 25);
            lblPassword.Location = new Point(30, 160);

            txtPassword = new TextBox();
            txtPassword.Size = new Size(250, 25);
            txtPassword.Location = new Point(150, 160);
            txtPassword.PasswordChar = '*';

            // Create confirm password label and textbox
            lblConfirmPassword = new Label();
            lblConfirmPassword.Text = "Confirm Password:";
            lblConfirmPassword.Size = new Size(120, 25);
            lblConfirmPassword.Location = new Point(30, 200);

            txtConfirmPassword = new TextBox();
            txtConfirmPassword.Size = new Size(250, 25);
            txtConfirmPassword.Location = new Point(150, 200);
            txtConfirmPassword.PasswordChar = '*';

            // Create role label and combobox
            lblRole = new Label();
            lblRole.Text = "Role:";
            lblRole.Size = new Size(120, 25);
            lblRole.Location = new Point(30, 240);

            cmbRole = new ComboBox();
            cmbRole.Size = new Size(250, 25);
            cmbRole.Location = new Point(150, 240);
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Items.AddRange(new string[] { "Receptionist", "Doctor", "Admin" });
            cmbRole.SelectedIndex = 0;

            // Create register button
            btnRegister = new Button();
            btnRegister.Text = "Register";
            btnRegister.Size = new Size(100, 35);
            btnRegister.Location = new Point(120, 290);
            btnRegister.BackColor = SystemColors.ButtonFace;
            btnRegister.Click += BtnRegister_Click;

            // Create cancel button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(230, 290);
            btnCancel.BackColor = SystemColors.ButtonFace;
            btnCancel.Click += BtnCancel_Click;

            // Create error label
            lblError = new Label();
            lblError.Text = "";
            lblError.ForeColor = Color.Red;
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Size = new Size(350, 40);
            lblError.Location = new Point(50, 340);

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblConfirmPassword);
            this.Controls.Add(txtConfirmPassword);
            this.Controls.Add(lblRole);
            this.Controls.Add(cmbRole);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnCancel);
            this.Controls.Add(lblError);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrEmpty(txtUsername.Text) || 
                string.IsNullOrEmpty(txtEmail.Text) || 
                string.IsNullOrEmpty(txtPassword.Text) || 
                string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                lblError.Text = "Please fill in all fields.";
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblError.Text = "Passwords do not match.";
                return;
            }

            try
            {
                using (var context = new HospitalContext())
                {
                    // Check if username already exists
                    bool usernameExists = context.Users.Any(u => u.Username == txtUsername.Text.Trim());
                    if (usernameExists)
                    {
                        lblError.Text = "Username already exists. Please choose another.";
                        return;
                    }

                    // Create new user
                    var newUser = new User
                    {
                        Username = txtUsername.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        PasswordHash = txtPassword.Text.Trim(), // In a real app, use password hashing
                        Role = cmbRole.SelectedItem.ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    context.Users.Add(newUser);
                    context.SaveChanges();

                    MessageBox.Show("Registration successful! You can now login.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
