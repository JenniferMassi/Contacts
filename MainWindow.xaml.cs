using Dapper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace Contacts {

    public partial class MainWindow : Window {
        private string connectionString = "Server=localhost;Database=contacts;Trusted_Connection=true";
        public event Action<Contact> OnAddContact;
        public string newContactImagePath;
        public MainWindow() {
            InitializeComponent();
            Contact.contacts = GetData();
            bool connected = TestConnection();
            listContacts.ItemsSource = Contact.contacts;
            OnAddContact += HandleNewContact;
            //display most recently added contact
            if (Contact.contacts.Count > 0) {
                listContacts.SelectedItem = Contact.contacts.Last();
            }//end if

        }//end function

        private void HandleNewContact(Contact newContact) {
            AddContact(newContact);
            Contact.contacts = GetData();
            bool connected = TestConnection();
            listContacts.ItemsSource = Contact.contacts;
        }

      
        private bool TestConnection() {
            try {
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    connection.Open();
                    return true;
                }
            } catch (Exception exception) {
                MessageBox.Show(exception.ToString());
                return false;
            }
        }

        public List<Contact> GetData(string searchText = null) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                if (string.IsNullOrEmpty(searchText)) {
                    return connection.Query<Contact>("SELECT * FROM tblContact WHERE isActive = 1").ToList();
                } else {
                    var query = "SELECT * FROM tblContact WHERE isActive = '1' AND (firstName LIKE @SearchText OR lastName LIKE @SearchText)";
                    return connection.Query<Contact>(query, new { SearchText = "%" + searchText + "%" }).ToList();
                }
            }
        }

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
                ImageControl.ImageSource = image;
            }

        }


        public void AddContact(Contact contact) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                string query = "INSERT INTO tblContact (firstName, middleName, lastName, notes, phone, picture, birthday, isFavorite, isActive) VALUES (@firstName, @middleName, @lastName, @notes, @phone, @picture, @birthday, @isFavorite, 1)";
                connection.Execute(query, contact);
            }
        }

        private void AddContactButton_Click(object sender, RoutedEventArgs e) {
            Contact contact = new Contact {
                firstName = txtAddFirstName.Text,
                middleName = txtAddMiddleName.Text,
                lastName = txtAddLastName.Text,
                notes = txtAddNotes.Text,
                phone = txtAddPhoneNumber.Text,
                picture = newContactImagePath,
                birthday = string.IsNullOrWhiteSpace(txtAddBirthday.Text) ? (DateTime?)null : DateTime.Parse(txtAddBirthday.Text)
            };
            OnAddContact?.Invoke(contact);//if function is not null then execute/invoke it
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            AddContactBox.Visibility = Visibility.Visible;
            EditContactBox.Visibility = Visibility.Hidden;
            ViewContactBox.Visibility = Visibility.Hidden;
        }//end ADDBUTTON CLICK FUNCTION



        private void EditContactButton_Click(object sender, RoutedEventArgs e) {
            Contact selectedContact = (Contact)listContacts.SelectedItem;
            if (selectedContact != null) {
                txtEditFirstName.Text = selectedContact.firstName;
                txtEditMiddleName.Text = selectedContact.middleName;
                txtEditLastName.Text = selectedContact.lastName;
                txtEditPhoneNumber.Text = selectedContact.phone;
                txtEditNotes.Text = selectedContact.notes;

                if (!string.IsNullOrEmpty(selectedContact.picture)) {
                    BitmapImage image = new BitmapImage(new Uri(selectedContact.picture));
                    ImageControl2.ImageSource = image; // edit image control...upload new image to pre existing one
                }

                txtEditBirthday.Text = selectedContact.birthday?.ToShortDateString() ?? ""; //display birthday

                Contact.currentContact = selectedContact;
                AddContactBox.Visibility = Visibility.Hidden;
                EditContactBox.Visibility = Visibility.Visible;
                ViewContactBox.Visibility = Visibility.Hidden;
            }
        }
        private void CancelEditButton_Click(object sender, RoutedEventArgs e) {
            EditContactBox.Visibility = Visibility.Hidden;
            AddContactBox.Visibility = Visibility.Hidden;
            ViewContactBox.Visibility = Visibility.Visible;
        }
      
        public void UpdateContact(Contact contact) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                string query = "UPDATE tblContact SET firstName = @firstName, middleName = @middleName, lastName = @lastName, phone = @phone, notes = @notes, picture = @picture, birthday = @birthday WHERE id = @id";
                connection.Execute(query, contact);

                //update image
                Contact.currentContact.picture = newContactImagePath; //****************************************************

            }
        }

        private void UpdateContactButton_Click(object sender, RoutedEventArgs e) {
            if (Contact.currentContact != null) {
                Contact.currentContact.firstName = txtEditFirstName.Text;
                Contact.currentContact.middleName = txtEditMiddleName.Text;
                Contact.currentContact.lastName = txtEditLastName.Text;
                Contact.currentContact.phone = txtEditPhoneNumber.Text;
                Contact.currentContact.notes = txtEditNotes.Text;
                Contact.currentContact.birthday = string.IsNullOrWhiteSpace(txtEditBirthday.Text) ? (DateTime?)null : DateTime.Parse(txtEditBirthday.Text);
                UpdateContact(Contact.currentContact);
                Contact.contacts = GetData();
                bool connected = TestConnection();
                listContacts.ItemsSource = Contact.contacts;
                AddContactBox.Visibility = Visibility.Hidden;
                EditContactBox.Visibility = Visibility.Hidden;
                ViewContactBox.Visibility = Visibility.Visible;
            }
        }

        private void EditPhotoButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".jpg";
            openFileDialog.Filter = "JPG Files (.jpg)|*.jpg";

            // Show file dialog
            bool? result = openFileDialog.ShowDialog();

            // determine if file was opened
            if (result == true) {
                // Store file path
                string selectedFile = openFileDialog.FileName;

                //display image
                BitmapImage image = new BitmapImage(new Uri(selectedFile));
                ImageControl2.ImageSource = image;

                // update currentContacts picture
                if (Contact.currentContact != null) {
                    Contact.currentContact.picture = selectedFile;
                }
            }
        }


        private void listContacts_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // first check if any item is selected
            if (listContacts.SelectedItem != null) {
                // convert the selected item to a Contact object
                Contact selectedContact = (Contact)listContacts.SelectedItem;

                // populate the selected contacts details into the viewcontactbox fields
                txtViewFirstName.Content = selectedContact.firstName;
                txtViewMiddleName.Content = selectedContact.middleName;
                txtViewLastName.Content = selectedContact.lastName;
                txtViewPhoneNumber.Content = selectedContact.phone;
                txtViewBirthday.Content = selectedContact.birthday?.ToShortDateString() ?? "";
                txtViewNotes.Text = selectedContact.notes;

                if (!string.IsNullOrEmpty(selectedContact.picture)) {
                    BitmapImage image = new BitmapImage(new Uri(selectedContact.picture));
                    ImageControl3.ImageSource = image; //display/save image**********************
                } else {
                    ImageControl3.ImageSource = new BitmapImage(new Uri("C:\\Users\\Jenni\\" +
                        "source\\repos\\Contacts\\images\\Portrait_Placeholder.png"));
                }

                // set button color based on favorite status
                if (selectedContact.isFavorite) {
                    FavoriteButton.Foreground = Brushes.Red; // change to red for favorite.
                } else {
                    FavoriteButton.Foreground = Brushes.Black; 
                }

                // hide AddContactBox and EditContactBox
                AddContactBox.Visibility = Visibility.Hidden;
                EditContactBox.Visibility = Visibility.Hidden;

                // show ViewContactBox
                ViewContactBox.Visibility = Visibility.Visible;
            }
        }


        public void UpdateFavorite(Contact contact) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                string query = "UPDATE tblContact SET isFavorite = @isFavorite WHERE id = @id";
                connection.Execute(query, contact);
            }
        }//end function

        private void FavoriteButton_Click(object sender, RoutedEventArgs e) {
            //get the selected contact
            Contact selectedContact = (Contact)listContacts.SelectedItem;
            if (selectedContact != null) {
                //toggle the isFavorite property
                selectedContact.isFavorite = !selectedContact.isFavorite;

                //save change to database
                UpdateFavorite(selectedContact);

                // Set button color based on favorite status:
                if (selectedContact.isFavorite) {
                    FavoriteButton.Foreground = Brushes.Magenta; 
                } else {
                    FavoriteButton.Foreground = Brushes.Black; 
                }

                //refresh list of contacts
                Contact.contacts = GetData();
                listContacts.ItemsSource = Contact.contacts;
            }
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            string searchText = SearchBar.Text;
            Contact.contacts = GetData(searchText);
            listContacts.ItemsSource = Contact.contacts;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e) {
            var searchText = SearchBar.Text;
            Contact.contacts = GetData(searchText);
            listContacts.ItemsSource = Contact.contacts;
        }

        private void OnViewDeletedClick(object sender, RoutedEventArgs e) {
            if (btnViewDeleted.Content is "View Deleted") {
                btnViewDeleted.Content = "View Active";
                btnRestore.Visibility = Visibility.Visible;
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    listContacts.ItemsSource = connection.Query<Contact>("SELECT * FROM tblContact WHERE isActive = 0").ToList();
                }
            }
            
            else {
                btnViewDeleted.Content = "View Deleted";
                btnRestore.Visibility = Visibility.Hidden;
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    listContacts.ItemsSource = connection.Query<Contact>("SELECT * FROM tblContact WHERE isActive = 1").ToList();
                }//end using
            }//end else
        }//end function

        private void DeleteContactButton_Click(object sender, RoutedEventArgs e) {
            // get selected contact from listContacts
            Contact selectedContact = listContacts.SelectedItem as Contact;
            if (selectedContact != null) {
                DeleteContact(selectedContact);
                Contact.contacts = GetData();
                bool connected = TestConnection();
                listContacts.ItemsSource = Contact.contacts;
            }
        }

        public void DeleteContact(Contact contact) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                string query = "UPDATE tblContact SET isActive = 0 WHERE id = @id";
                connection.Execute(query, new { id = contact.id });
            }
        }


        private void OnRestoreClick(object sender, RoutedEventArgs e) {
            // get selected contact from listContacts
            Contact selectedContact = listContacts.SelectedItem as Contact;
            if (selectedContact != null) {
                RestoreContact(selectedContact);
                //Contact.contacts = GetData();
                //bool connected = TestConnection();
                //listContacts.ItemsSource = connection.Query<Contact>("SELECT * FROM tblContact WHERE isActive = 0").ToList();
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    listContacts.ItemsSource = connection.Query<Contact>("SELECT * FROM tblContact WHERE isActive = 0").ToList();
                }
            }
        }

        private void RestoreContact(Contact contact) {
            if (contact != null) {
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    string query = "UPDATE tblContact SET isActive = 1 WHERE id = @id";
                    connection.Execute(query, new { id = contact.id });
                }
            }
        }
    }//end main
}//end class

    




