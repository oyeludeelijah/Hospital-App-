using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleHospitalApp.Helpers
{
    /// <summary>
    /// Utility class for creating standardized UI elements.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Creates a standardized label with the specified properties.
        /// </summary>
        public static Label CreateLabel(string text, Point location, Size size, Font? font = null)
        {
            var label = new Label();
            label.Text = text;
            label.Location = location;
            label.Size = size;
            
            if (font != null)
            {
                label.Font = font;
            }
            
            return label;
        }
        
        /// <summary>
        /// Creates a standardized title label.
        /// </summary>
        public static Label CreateTitleLabel(string text, Point location)
        {
            var label = new Label();
            label.Text = text;
            label.Font = new Font("Arial", 14, FontStyle.Bold);
            label.Location = location;
            label.Size = new Size(300, 30);
            
            return label;
        }
        
        /// <summary>
        /// Creates a standardized text box.
        /// </summary>
        public static TextBox CreateTextBox(Point location, Size size, string placeholder = "")
        {
            var textBox = new TextBox();
            textBox.Location = location;
            textBox.Size = size;
            
            if (!string.IsNullOrEmpty(placeholder))
            {
                textBox.PlaceholderText = placeholder;
            }
            
            return textBox;
        }
        
        /// <summary>
        /// Creates a standardized button.
        /// </summary>
        public static Button CreateButton(string text, Point location, Size size, EventHandler? clickHandler = null)
        {
            var button = new Button();
            button.Text = text;
            button.Location = location;
            button.Size = size;
            button.BackColor = SystemColors.ButtonFace;
            
            if (clickHandler != null)
            {
                button.Click += clickHandler;
            }
            
            return button;
        }
        
        /// <summary>
        /// Creates a standardized DataGridView.
        /// </summary>
        public static DataGridView CreateDataGridView(Point location, Size size)
        {
            var dataGridView = new DataGridView();
            dataGridView.Location = location;
            dataGridView.Size = size;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.RowHeadersVisible = false;
            dataGridView.BackgroundColor = Color.White;
            
            return dataGridView;
        }
        
        /// <summary>
        /// Creates a standardized ComboBox.
        /// </summary>
        public static ComboBox CreateComboBox(Point location, Size size, bool dropDownList = true)
        {
            var comboBox = new ComboBox();
            comboBox.Location = location;
            comboBox.Size = size;
            
            if (dropDownList)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            
            return comboBox;
        }
        
        /// <summary>
        /// Creates a standardized DateTimePicker.
        /// </summary>
        public static DateTimePicker CreateDateTimePicker(Point location, Size size)
        {
            var dateTimePicker = new DateTimePicker();
            dateTimePicker.Location = location;
            dateTimePicker.Size = size;
            dateTimePicker.Format = DateTimePickerFormat.Short;
            
            return dateTimePicker;
        }
        
        /// <summary>
        /// Creates a standardized NumericUpDown.
        /// </summary>
        public static NumericUpDown CreateNumericUpDown(Point location, Size size, decimal minimum = 0, decimal maximum = 1000000)
        {
            var numericUpDown = new NumericUpDown();
            numericUpDown.Location = location;
            numericUpDown.Size = size;
            numericUpDown.Minimum = minimum;
            numericUpDown.Maximum = maximum;
            numericUpDown.DecimalPlaces = 2;
            
            return numericUpDown;
        }
    }
}
