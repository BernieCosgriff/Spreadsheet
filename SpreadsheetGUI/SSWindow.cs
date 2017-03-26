using System;
using SSGui;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;

namespace SpreadsheetGUI
{
    public partial class SSWindow : Form, ISSWindow
    {

        private string lastSave;

        public SSWindow()
        {
            InitializeComponent();
            //Register the method with the event
            ssPanel.SelectionChanged += SelectionChanged;

            FormClosing += close_Click;

            currentCell.Text = "A1";

            this.Text = "Spreadsheet";
        }

        //Close option chosen
        public event Action CloseEvent;
        //Save option chosen
        public event Action<string> SaveEvent;
        //Open option chosen
        public event Action<string> OpenEvent;
        //New Window optiom chosen
        public event Action NewWindowEvent;
        //Cell selected
        public event Action CellEvent;
        //Content of a cell changed
        public event Action ContentBoxEvent;
        //How To Use option selected
        public event Action HelpEvent;

        /// <summary>
        /// Contents of the value textbox
        /// </summary>
        public string ValueBox { set { valueBox.Text = value; } get { return valueBox.Text; } }

        /// <summary>
        /// Contents of the content textbox
        /// </summary>
        public string ContentBox { set { contentBox.Text = value; } get { return contentBox.Text; } }

        /// <summary>
        /// Displays the Current Cell's name
        /// </summary>
        public string CurrentCell { set { currentCell.Text = value; } get { return currentCell.Text; } }

        /// <summary>
        /// The last save location
        /// </summary>
        public string Filename { set { lastSave = value; } get { return lastSave; } }


        public string Message { set { MessageBox.Show(value); } }

        public SpreadsheetPanel SPanel { get { return ssPanel; } }

        /// <summary>
        /// Fires the SaveEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void save_Click(object sender, EventArgs e)
        {
            //If the Spreadsheet has never been saved prompt to choose a location
            if(lastSave == null)
            {
                saveAs_Click(new object(), new EventArgs());
            }
            //Otherwise just overwrite the old one
            else if(SaveEvent != null)
            {
                SaveEvent(lastSave);
            }
        }

        /// <summary>
        /// Fires the OpenEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, EventArgs e)
        {
            try
            {
                //Option to show only .ss files or all files
                openFileDialog.Filter = "Spreadsheet Files (*.ss)|*.ss|All files (*.*)|*.*";
                //Show the dialogue
                DialogResult result = openFileDialog.ShowDialog();
                //See what file the user wants
                if (result == DialogResult.Yes || result == DialogResult.OK)
                {
                    OpenEvent(openFileDialog.FileName);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Specified file could not be opened.");
            }
        }

        /// <summary>
        /// Fires the CellEvent
        /// </summary>
        /// <param name="ss"></param>
        private void SelectionChanged(SpreadsheetPanel ss)
        {
            if(CellEvent != null)
            {
                CellEvent();
            }
        }

        public void SaveBeforeClose()
        {
            //Show the save dialogue
            DialogResult closeResult = MessageBox.Show("Would you like to save before closing?", "Warning", MessageBoxButtons.YesNo);
            //See what the user wants to do
            if(closeResult == DialogResult.Yes || closeResult == DialogResult.OK)
            {
                try
                {
                    //Set the default extension
                    saveFileDialog.DefaultExt = "ss";
                    //Add the extension by default
                    saveFileDialog.AddExtension = true;
                    //Option to show only .ss files for all files
                    saveFileDialog.Filter = "Spreadsheet Files (*.ss)|*.ss|All files (*.*)|*.*";
                    //Enable the overwrite prompt
                    saveFileDialog.OverwritePrompt = true;
                    //Show the dialogue
                    DialogResult result = saveFileDialog.ShowDialog();
                    //See where the user wants to save
                    if (result == DialogResult.Yes || result == DialogResult.OK)
                    {
                        lastSave = saveFileDialog.FileName;
                        SaveEvent(saveFileDialog.FileName);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("File could not be saved at the specified location.");
                }
            }
        }

        /// <summary>
        /// Opens a new window
        /// </summary>
        public void OpenNew()
        {
            SSAplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Opens a new window
        /// </summary>
        public void OpenNew(string filename)
        {
            SSAplicationContext.GetContext().RunNew(filename);
        }

        /// <summary>
        /// Fires the SaveEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAs_Click(object sender, EventArgs e)
        {
            try
            {
                //Set the default extension
                saveFileDialog.DefaultExt = "ss";
                //Add the .ss extension by default
                saveFileDialog.AddExtension = true;
                //Option to show only .ss files or all files
                saveFileDialog.Filter = "Spreadsheet Files (*.ss)|*.ss|All files (*.*)|*.*";
                //Enable the overwrite prompt
                saveFileDialog.OverwritePrompt = true;
                //Get the dialogue result
                DialogResult result = saveFileDialog.ShowDialog();
                //See where the user wants to save
                if (result == DialogResult.Yes || result == DialogResult.OK)
                {
                    
                    //Save the last save location
                    lastSave = saveFileDialog.FileName;
                    //Fire the save event
                    SaveEvent(saveFileDialog.FileName);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("File could not be saved at the specified location.");
            }
        }

        /// <summary>
        /// Fires the NewWindowEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newWindow_Click(object sender, EventArgs e)
        {
            if(NewWindowEvent != null)
            {
                NewWindowEvent();
            }
        }

        /// <summary>
        /// Fires the CloseEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_Click(object sender, EventArgs e)
        {
            if(CloseEvent != null)
            {
                CloseEvent();
            }
        }

        /// <summary>
        /// Fires the CloseEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_Click2(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Opens the How To Use message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void howToUse_Click(object sender, EventArgs e)
        {
            if(HelpEvent != null)
            {
                HelpEvent();
            }
        }

        /// <summary>
        /// Fires the ContentBoxEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calculateButton_Click(object sender, EventArgs e)
        {
            if (ContentBoxEvent != null)
            {
                ContentBoxEvent();
            }
        }
    }
}
