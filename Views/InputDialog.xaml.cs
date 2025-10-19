using System.Windows;

namespace Finance.Views
{
    public partial class InputDialog : Window
    {
        public string ResponseText { get; private set; }

        public InputDialog(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            PromptText.Text = question;
            ResponseTextBox.Text = defaultAnswer;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = ResponseTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}