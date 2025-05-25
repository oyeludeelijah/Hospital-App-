using SimpleHospitalApp.Data;
using SimpleHospitalApp.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleHospitalApp.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblError;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Hospital Management System - Login";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create title
            lblTitle = new Label();
            lblTitle.Text = "Hospital Management System";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(350, 40);
            lblTitle.Location = new Point(25, 20);

            // Create username label and textbox
            lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Size = new Size(100, 25);
            lblUsername.Location = new Point(30, 80);

            txtUsername = new TextBox();
            txtUsername.Size = new Size(220, 25);
            txtUsername.Location = new Point(130, 80);

            // Create password label and textbox
            lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Size = new Size(100, 25);
            lblPassword.Location = new Point(30, 120);

            txtPassword = new TextBox();
            txtPassword.Size = new Size(220, 25);
            txtPassword.Location = new Point(130, 120);
            txtPassword.PasswordChar = '*';

            // Create login button
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Size = new Size(100, 35);
            btnLogin.Location = new Point(150, 170);
            btnLogin.BackColor = SystemColors.ButtonFace;
            btnLogin.Click += BtnLogin_Click;

            // Create register button
            btnRegister = new Button();
            btnRegister.Text = "New User? Register";
            btnRegister.Size = new Size(150, 35);
            btnRegister.Location = new Point(125, 220);
            btnRegister.BackColor = SystemColors.ButtonFace;
            btnRegister.Click += BtnRegister_Click;

            // Create error label
            lblError = new Label();
            lblError.Text = "";
            lblError.ForeColor = Color.Red;
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Size = new Size(350, 25);
            lblError.Location = new Point(25, 270);

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
            this.Controls.Add(lblError);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter both username and password.";
                return;
            }

            try
            {
                using (var context = new HospitalContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

                    if (user != null)
                    {
                        // Set the current user for the application
                        Program.CurrentUser = user;

                        // Show the main form
                        var mainForm = new Form1();
                        this.Hide();
                        mainForm.FormClosed += (s, args) => this.Close();
                        mainForm.Show();
                    }
                    else
                    {
                        lblError.Text = "Invalid username or password.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm();
            this.Hide();
            registerForm.FormClosed += (s, args) => this.Show();
            registerForm.Show();
        }
    }
}
