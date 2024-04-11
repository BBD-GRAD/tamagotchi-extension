using System.Windows;

namespace tamagotchi_pet.Dialogs
{
    public partial class CreatePetDialog : Window
    {
        public string ResponseText { get; private set; }

        public CreatePetDialog()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = InputTextBox.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}