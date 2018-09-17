namespace TDHelper
{
    /// <summary>
    /// A class to allow the items in a combobox to have a display value and a return value.
    /// </summary>
    public class ComboBoxItem
    {
        public ComboBoxItem(
            string name,
            object value = null)
        {
            Text = name;
            Value = value ?? name;
        }

        public ComboBoxItem()
        {
        }

        /// <summary>
        /// Gets or sets the text to be displayed in the control.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the value to be returned to the application.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Override the ToString method and return the text.
        /// </summary>
        /// <returns>The text to be displayed.</returns>
        public override string ToString()
        {
            return Text;
        }
    }
}