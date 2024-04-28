using System;
using System.Collections.Generic;
using System.Diagnostics; 
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace WikiApplicationFINAL
{
    public partial class WikiApplicationFINAL : Form
    {
        public WikiApplicationFINAL()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "Welcome to the Wiki Application!"; // Prints message everytime user opens form
            ComboBoxCategory(); // When form is opened, the user should be able to select drop down box and all the categories should appear
             
        }
        #region Global Variable 
        // 6.2 Create a global List<T> of type Information called Wiki.
        List<Information> WikiList = new List<Information>();
        const string fileName = "definitions.dat"; // Used for the Open/Save file methods
        #endregion

        #region Methods for DisplayList, ValidName, ComboBoxCategory & ShowData
        // 6.11 Create a ListView event so a user can select a Data Structure Name from the list of Names
        // and the associated information will be displayed in the related text boxes combo box and radio button.
        private void DisplayList()
        {
            
            listViewWiki.Items.Clear();

            foreach (Information item in WikiList)
            {
                // Creates a new ListViewItem and add sub-items (columns) to it
                ListViewItem listViewItem = new ListViewItem(item.GetName()); // Assuming name is displayed in the first column

                // Add additional sub-items as needed for other properties
                listViewItem.SubItems.Add(item.GetCategory());
                listViewItem.SubItems.Add(item.GetStructure());
                listViewItem.SubItems.Add(item.GetDefinition());

                // Adds the ListViewItem to the ListView
                listViewWiki.Items.Add(listViewItem);
            }
        }

        // 6.5 Create a custom ValidName method which will take a parameter string value from the Textbox Name and returns a Boolean after checking for duplicates.
        // Use the built in List<T> method “Exists” to answer this requirement.
        private bool ValidName(string a)
        {
            /* StreamWriter stream = File.AppendText("Trace_6.5.txt");
            TextWriterTraceListener writer = new TextWriterTraceListener(stream);
            Trace.Listeners.Add(writer);
            Trace.WriteLine("ValidName Method");
            Trace.WriteLine("Check Name: " + a); */
            if (!string.IsNullOrEmpty(txtBoxName.Text))
            {
                // If statement used to see if there is any duplications when user inputs the same name as any of the other ones within list
                if (WikiList.Exists(dup => dup.GetName() == a.ToUpper()))
                {
                    //Trace.WriteLine("Return False: Duplicate Found");
                    //Trace.Flush();
                    //stream.Close();
                    return false;
                }
                else
                {
                    //Trace.WriteLine("Return True: Duplicate Not Found");
                    //Trace.Flush();
                    //stream.Close();
                    return true;
                }
            }
            else
            {
                return false;
            }
           

        }


        // 6.4 Create a custom method to populate the ComboBox when the Form Load method is called. The six categories must be read from a simple text file.
        private void ComboBoxCategory()
        {
            // Variable for .txt file
            string filePath = "Category.txt";

            try
            {
                // Read all lines from the text file
                string[] lines = File.ReadAllLines(filePath);

                // Add each category to the ComboBox
                comboBoxCategory.Items.AddRange(lines);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        // 6.9 Create a single custom method that will sort and then display the Name and Category from the wiki information in the list.
        private void ShowData()
        {
            // Get the selected item from the ListView
            ListViewItem item = listViewWiki.SelectedItems[0];

            // Get the index of the selected item
            int indx = item.Index;

            // Set the text of txtBoxName to the name from WikiList at the selected index
            txtBoxName.Text = WikiList[indx].GetName();

            // Set the text of comboBoxCategory to the category from WikiList at the selected index
            comboBoxCategory.Text = WikiList[indx].GetCategory();

            // Check if the structure from WikiList at the selected index is "Linear"
            if (WikiList[indx].GetStructure() == "Linear")
            {
                // If it's "Linear", check the radio button for linear structures
                radioBtnLinear.Checked = true;
            }
            else
            {
                // If it's not "Linear", check the radio button for non-linear structures
                radioBtnNonLinear.Checked = true;
            }

            // Set the text of txtBoxDefinition to the definition from WikiList at the selected index
            txtBoxDefinition.Text = WikiList[indx].GetDefinition();
        }
        #endregion

        #region Add, Edit, Delete, Reset & Radio Button
        // 6.3 Create a button method to ADD a new item to the list.
        // Use a TextBox for the Name input, ComboBox for the Category, Radio group for the Structure and Multiline TextBox for the Definition.
        private void AddWiki()
        {
            // Check if all required fields are filled out
            if (!string.IsNullOrEmpty(txtBoxName.Text) &&
                !string.IsNullOrEmpty(comboBoxCategory.Text) &&
                !string.IsNullOrEmpty(GetRadioButton()) &&
                !string.IsNullOrEmpty(txtBoxDefinition.Text))
            {
                // Check if the name is valid (not a duplicate)
                if (ValidName(txtBoxName.Text))
                {
                    try
                    {
                        // Creates a new Information object with the input data
                        Information newInformation = new Information(txtBoxName.Text.ToUpper(), comboBoxCategory.Text, GetRadioButton(), txtBoxDefinition.Text);

                        // Add the newInformation object to the WikiList
                        WikiList.Add(newInformation);
                        
                        // Clears everything after data has been added
                        ResetTextBoxes();
                        
                        //Prints message
                        toolStripStatusLabel1.Text = "Successfully added data to the list!";

                    }
                    catch (Exception ex)
                    {
                        toolStripStatusLabel1.Text = ex.Message;
                    }
                }
                else
                {
                    // Prints message if name already exists
                    toolStripStatusLabel1.Text = "Name already exists. Please enter a different name.";
                }
            }
            else
            {
                // Prints message if all fields have not been filled
                toolStripStatusLabel1.Text = "Please fill out all required fields.";
                return;
            }
            txtBoxName.Focus();
            WikiList.Sort();
            DisplayList();
            

        }

        // 6.8 Create a button method that will save the edited record of the currently selected item in the ListView.
        // All the changes in the input controls will be written back to the list. Display an updated version of the sorted list at the end of this process.
        private void EditWiki()
        {
            // Check if any item is selected in the ListView
            if (listViewWiki.SelectedIndices.Count == 0)
            {
                // Display a message if no item is selected and return from the method
                toolStripStatusLabel1.Text = "Must select data to edit.";
                return;
            }

            // Get the index of the selected item and the corresponding Information object from WikiList
            int selectedIndex = listViewWiki.SelectedIndices[0];
            Information selectedData = WikiList[selectedIndex];

            // Check if the name is valid (not a duplicate)
            if (ValidName(txtBoxName.Text))
            {
                // Check if any required field is empty
                if (string.IsNullOrEmpty(txtBoxName.Text) || string.IsNullOrEmpty(comboBoxCategory.Text) ||
                    string.IsNullOrEmpty(GetRadioButton()) || string.IsNullOrEmpty(txtBoxDefinition.Text))
                {
                    // Display a message if any required field is empty
                    toolStripStatusLabel1.Text = "Fill all fields to edit data.";
                }
                else
                {
                    // Updates the information
                    selectedData.SetName(txtBoxName.Text.ToUpper());
                    selectedData.SetCategory(comboBoxCategory.Text);
                    selectedData.SetStructure(GetRadioButton());
                    selectedData.SetDefinition(txtBoxDefinition.Text);

                    // Sort the WikiList and update the displayed list
                    WikiList.Sort();
                    DisplayList();
                    // Reset text boxes and prints message
                    ResetTextBoxes();
                    toolStripStatusLabel1.Text = "Successfully edited data";
                }
            }
            else
            {
                // Prints message if the name entered in txtBoxName is a duplicate
                toolStripStatusLabel1.Text = "Duplication!";
            }
        }

        // 6.7 Create a button method that will delete the currently selected record in the ListView.
        // Ensure the user has the option to backout of this action by using a dialog box. Display an updated version of the sorted list at the end of this process.
        private void DeleteWiki()
        {
            // Check if any item is selected in the ListView
            if (listViewWiki.SelectedIndices.Count > 0)
            {
                // Get the index of the selected data
                int selectedIndex = listViewWiki.SelectedIndices[0];

                // Ask for confirmation before deleting
                DialogResult result = MessageBox.Show("Are you sure you want this data to be deleted?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // If the user confirms deletion
                if (result == DialogResult.Yes)
                {
                    // Remove the selected item from WikiList
                    WikiList.RemoveAt(selectedIndex);

                    // Reset text boxes
                    ResetTextBoxes();

                    // Update the displayed list and prints message
                    DisplayList();
                    toolStripStatusLabel1.Text = "Data has been deleted!";
                }
            }
            else
            {
                // Prints message if no item is selected for deletion
                toolStripStatusLabel1.Text = "Must select data to delete.";
            }
        }

        // 6.12 Create a custom method that will clear and reset the TextBoxes, ComboBox and Radio button
        private void ResetTextBoxes()
        {
            // Clears each individual text box, combo box and radio button
            txtBoxName.Clear();
            comboBoxCategory.Text = "";
            radioBtnLinear.Checked = false;
            radioBtnNonLinear.Checked = false;
            txtBoxSearch.Clear();
            txtBoxDefinition.Clear();
            // Prints message
            toolStripStatusLabel1.Text = "All required fields have been cleared!"; 

        }

        // 6.6 Create two methods to highlight and return the values from the Radio button GroupBox.
        // The first method must return a string value from the selected radio button (Linear or Non-Linear).
        // The second method must send an integer index which will highlight an appropriate radio button.
        private string GetRadioButton()
        {
            string radiotxt = "";
            if (radioBtnLinear.Checked)
            {
                radiotxt = radioBtnLinear.Text;
            }
            else if (radioBtnNonLinear.Checked)
            {
                radiotxt = radioBtnNonLinear.Text;
            }
            return radiotxt;
        }

        //6.6 Create two methods to highlight and return the values from the Radio button GroupBox.
        //The first method must return a string value from the selected radio button (Linear or Non-Linear).
        //The second method must send an integer index which will highlight an appropriate radio button.
        private void SetRadioButton(int info)
        {
            if (WikiList[info].GetStructure() == "Linear")
                radioBtnLinear.Checked = true;
            if (WikiList[info].GetStructure() == "Non-Linear")
                radioBtnNonLinear.Checked = true;
        }
        #endregion

        #region Search, Open & Save
        // 6.10 Create a button method that will use the builtin binary search to find a Data Structure name.
        // If the record is found the associated details will populate the appropriate input controls and highlight the name in the ListView. At the end of the search process the search input TextBox must be cleared.
        private void SearchWiki()
        {
            // Create a new Information object for searching
            Information searchName = new Information();

            // Set the name from the textbox and convert it to uppercase
            searchName.SetName(txtBoxSearch.Text.ToUpper());

            // Perform a binary search on WikiList to find the index of the searchName
            int foundIndex = WikiList.BinarySearch(searchName);

            // If the searchName is found
            if (foundIndex >= 0)
            {
                // Clear any previous selections in the ListView
                listViewWiki.SelectedItems.Clear();

                // Select the data at the found index in the ListView
                listViewWiki.Items[foundIndex].Selected = true;

                // Set focus to the ListView
                listViewWiki.Focus();

                // Update text boxes and radio buttons with data from WikiList at the found index
                txtBoxName.Text = WikiList[foundIndex].GetName();
                comboBoxCategory.Text = WikiList[foundIndex].GetCategory();
                txtBoxDefinition.Text = WikiList[foundIndex].GetDefinition();
                SetRadioButton(foundIndex); // Set radio button based on structure
                toolStripStatusLabel1.Text = "Found at index " + foundIndex;
            }
            else
            {
                // Prints message if the searchName is not found
                toolStripStatusLabel1.Text = "Data cannot be found";
            }
        }

        // 6.14 Create two buttons for the manual open and save option; this must use a dialog box to select a file or rename a saved file.
        // All Wiki data is stored/retrieved using a binary reader/writer file format.
        private void OpenBinaryFile()
        {
            // Create and configure an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            openFileDialog.DefaultExt = ".dat";
            openFileDialog.Filter = "DAT files (*.dat)|*.dat|All files (*.*)|*.*";

            // Show the file dialog and wait for user input
            DialogResult result = openFileDialog.ShowDialog();

            // Process the user's choice
            if (result == DialogResult.OK)
            {
                BinaryReader br = null;
                try
                {
                    // Open the selected file for reading
                    br = new BinaryReader(new FileStream(openFileDialog.FileName, FileMode.Open));

                    // Clear existing data before adding new data from the file
                    WikiList.Clear();
                    listViewWiki.Items.Clear();

                    // Read data from the file until the end is reached
                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        try
                        {
                            // Read data from the file
                            string name = br.ReadString().ToUpper();
                            string category = br.ReadString();
                            string structure = br.ReadString();
                            string definition = br.ReadString();

                            // Create a new Information object with the read data
                            Information newInformation = new Information(name, category, structure, definition);

                            // Add the newInformation object to the WikiList
                            WikiList.Add(newInformation);
                        }
                        catch (Exception fe)
                        {
                            // Display an error message if data cannot be read from the file or if the end of file is reached unexpectedly
                            MessageBox.Show("Cannot read data from file or EOF: " + fe.Message);
                            break; // Exit the loop if an error occurs
                        }
                    }

                    // Display the updated list in the ListView
                    DisplayList();
                }
                catch (Exception ex)
                {
                    // MessageBox error message if there is a general exception
                    MessageBox.Show("Error opening file: " + ex.Message);
                }
                finally
                {
                    // Close the BinaryReader if it was successfully opened
                    if (br != null)
                    {
                        br.Close();
                    }
                }
            }
        }

        // 6.14 Create two buttons for the manual open and save option; this must use a dialog box to select a file or rename a saved file.
        // All Wiki data is stored/retrieved using a binary reader/writer file format.
        private void SaveBinaryFile()
        {
            // Create and configure a SaveFileDialog instance
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath; // Set initial directory
            saveFileDialog.FileName = "definitions"; // Default file name
            saveFileDialog.DefaultExt = ".dat"; // Default file extension
            saveFileDialog.Filter = "DAT files (*.dat)|*.dat|All files (*.*)|*.*"; // Filter for file types

            // Show the save file dialog and wait for user input
            DialogResult result = saveFileDialog.ShowDialog();

            // Process the user's choice
            if (result == DialogResult.OK) // If the user chooses to save
            {
                // Create a FileStream and BinaryWriter to write data to the file
                using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    using (var bw = new BinaryWriter(fs))
                    {
                        // Loop through each Information object in WikiList and save its details to the file.
                        foreach (Information item in WikiList)
                        {
                            // Writes Name, Category, Structure & Definition
                            bw.Write(item.GetName()); 
                            bw.Write(item.GetCategory()); 
                            bw.Write(item.GetStructure()); 
                            bw.Write(item.GetDefinition()); 
                        }
                    }
                }
            }
            else // If the user cancels or closes the dialog without saving
            {
                MessageBox.Show("File was not saved");
            }
        }
        #endregion

        #region Button Click Methods
        // All methods from above are assigned to each button listed below
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenBinaryFile();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveBinaryFile();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetTextBoxes();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchWiki(); 
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddWiki();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditWiki(); 
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteWiki();
        }

        private void listViewWiki_Click(object sender, EventArgs e)
        {
            ShowData(); 
        }

        // 6.13 Create a double click event on the Name TextBox to clear the TextBboxes, ComboBox and Radio button.
        private void TextBoxName_DoubleClick(object sender, MouseEventArgs e)
        {
            
            ResetTextBoxes(); 
        }

        // 6.15 The Wiki application will save data when the form closes. 
        private void WikiApplicationFINAL_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveBinaryFile();
        }
    }
}
#endregion




