using System;

namespace SimpleHospitalApp.Helpers
{
    public class ComboBoxItem
    {
        public string Text { get; set; } = string.Empty;
        public object Value { get; set; } = 0;
        
        public override string ToString()
        {
            return Text;
        }
    }
}
