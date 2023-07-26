using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Contacts {
    public partial class usControl : UserControl {
        static string connectionString = "Server=localhost;Database=contacts;Trusted_Connection=true";
        public event Action<Contact> OnAddContact;
        public usControl() {
            InitializeComponent();
        }

        public void AddContactButton_Click(object sender, RoutedEventArgs e) {
            Contact contact = new Contact {
                firstName = txtFirstName.Text,
                middleName = txtMiddleName.Text,
                lastName = txtLastName.Text,
                notes = txtNotes.Text,
                phone = txtPhoneNumber.Text
            };
            OnAddContact?.Invoke(contact);
        }

        private string newContactImagePath;

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".jpg";
            openFileDialog.Filter = "JPG Files (.jpg)|*.jpg";

            // Show file dialog
            bool? result = openFileDialog.ShowDialog();

            // determine if file was opened
            if (result == true) {
                // Store file path
                string selectedFile = openFileDialog.FileName;
                newContactImagePath = selectedFile;

               //display image
                BitmapImage image = new BitmapImage(new Uri(newContactImagePath));
                imageControl.Source = image; 
            }
        }


        //private void ReturnToMainWindow() {
        //    // Switch back to main page
        //    if (Window.GetWindow(this) is MainWindow mainWindow) {
        //        mainWindow.NavigateToOriginalView();
        //    }// End if
        //}// End function
    }
}