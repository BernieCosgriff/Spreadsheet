using Formulas;
using SS;
using SSGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Represents a controller that coordinates operations between
    /// the Model (Spreadsheet) and the View (SpreadsheetGUI)
    /// </summary>
    public class Controller
    {
        //The View of the Spreadsheet
        private ISSWindow window;

        //The Model of the Spreadsheet
        private AbstractSpreadsheet ss;

        /// <summary>
        /// Constructs a Controller with an empty Spreadsheet
        /// </summary>
        /// <param name="_window"></param>
        public Controller(ISSWindow _window)
        {
            //Initialize variables
            window = _window;
            ss = new Spreadsheet(new Regex("^[A-Z][1-9][0-9]{0,1}$"));

            //Register all event handlers
            window.NewWindowEvent += HandleNew;
            window.OpenEvent += HandleOpen;
            window.SaveEvent += HandleSave;
            window.ContentBoxEvent += HandleContentBoxChange;
            window.CellEvent += HandleCellSelect;
            window.CloseEvent += HandleClose;
            window.HelpEvent += HandleHelp;
        }

        /// <summary>
        /// Constructs a controller with a Spreadsheet
        /// built from the specified file.
        /// </summary>
        /// <param name="_window"></param>
        /// <param name="filename"></param>
        public Controller(ISSWindow _window, string filename)
        {
            //Initialize variables
            window = _window;
            ss = new Spreadsheet(new StreamReader(filename));
            ReDisplay(new HashSet<string>(ss.GetNamesOfAllNonemptyCells()), window.SPanel);

            //Register all event handlers
            window.NewWindowEvent += HandleNew;
            window.OpenEvent += HandleOpen;
            window.SaveEvent += HandleSave;
            window.ContentBoxEvent += HandleContentBoxChange;
            window.CellEvent += HandleCellSelect;
            window.CloseEvent += HandleClose;
            window.HelpEvent += HandleHelp;
        }

        /// <summary>
        /// Handles closing the Form
        /// </summary>
        private void HandleClose()
        {
            //If the spreadsheet has changed ask if the user wants to save
            if(ss.Changed)
            {
                window.SaveBeforeClose();
            }
        }

        /// <summary>
        /// Updates the value and content boxes
        /// when a new cell is selected
        /// </summary>
        private void HandleCellSelect()
        {
            //Out vars for GetSelection
            int col;
            int row;

            //Get the selected cell's address
            window.SPanel.GetSelection(out col, out row);

            //Convert the address to a cell name
            window.CurrentCell = numToLetter(col, row);

            //Get the cell's contents
            object content = ss.GetCellContents(window.CurrentCell);

            //If its a formula append an '=' to it and display it
            if(content is Formula)
            {
                window.ContentBox = "=" + content.ToString();
            }

            //Otherwise just display it
            else
            {
                window.ContentBox = content.ToString();
            }

            object value = ss.GetCellValue(window.CurrentCell);

            if(value is FormulaError)
            {
                value = "Formula Error";
            }
            //Update the value box
            window.ValueBox = value.ToString();
        }

        /// <summary>
        /// Updates the spreadsheet when the contents of
        /// a cell are changed
        /// </summary>
        private void HandleContentBoxChange()
        {
            //Out vars for GetSelection
            int col;
            int row;

            ISet<string> set;

            //Value to be displayed
            string value;

            //Set the contents of the cell
            try
            {
                set = ss.SetContentsOfCell(window.CurrentCell, window.ContentBox);
            }
            catch (FormulaFormatException)
            {
                window.Message = "Invalid Formula.";
                return;
            }
            catch (CircularException)
            {
                window.Message = "Circular Dependencies are not Allowed.";
                return;
            }

            object val = ss.GetCellValue(window.CurrentCell);

            //Store the value
            if (val is FormulaError)
            {
                value = "Formula Error";
            }
            else
            {
                value = val.ToString();
            }
            

            //Display the value in the value box
            window.ValueBox = value;

            //Get the col and row values for the cell
            window.SPanel.GetSelection(out col, out row);

            //Display the value in the cell
            window.SPanel.SetValue(col, row, value);

            //Change all of the cells depending on this one
            ReDisplay(set, window.SPanel);
        }

        /// <summary>
        /// Prompts the help menu
        /// </summary>
        private void HandleHelp()
        {
            window.Message = "To select a cell simply click on it.\n\nTo edit the contents of a cell select the cell and then use the contents box located in the"
                + " top right to edit the contents.\n\nUse the calculate button to commit your changes and calculate the cell's value.\n\nThe value of the selected"
                + " cell will be displayed in the value box which is located to the left of the content box.\n\nTo open or save a Spreadsheet file, open a new window,"
                + " or close the window use the \"file\" menu.";
        }

        /// <summary>
        /// Saves the spreadsheet at the specified file name
        /// </summary>
        /// <param name="filename"></param>
        private void HandleSave(string filename)
        {
            //If the file has changed save it
            if(ss.Changed)
            {
                ss.Save(new StreamWriter(filename));
                window.Message = "Save Successful.";
            }
            else
            {
                window.Filename = null;
                window.Message = "The file was not saved becasue the file has not been changed since the last save or since being opened.";
            }
        }

        /// <summary>
        /// Opens a new windows from a saved spreadsheet file
        /// </summary>
        /// <param name="filename"></param>
        private void HandleOpen(string filename)
        {
            window.OpenNew(filename);
        }

        /// <summary>
        /// Opens new empty window
        /// </summary>
        private void HandleNew()
        {
            window.OpenNew();
        }

        /// <summary>
        /// Converts the column and row numbers into a cell address
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private string numToLetter(int col, int row)
        {
            return ((char)(col + 65)) + (row+1).ToString();
        }

        /// <summary>
        /// Replaces all of the old cell values with their
        /// updated successors.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="sp"></param>
        private void ReDisplay(ISet<string> set, SpreadsheetPanel sp)
        {
            int result;

            //For each string in the set update its cell
            foreach (string s in set)
            {
                //Get the row number connected to the column letter
                int.TryParse(s.Substring(1), out result);

                object value;

                if((value = ss.GetCellValue(s)) is FormulaError)
                {
                    value = "Formula Error";
                }
                //Set the value of the cell
                sp.SetValue(s.First() - 65, result-1, value.ToString());
            }
        }
    }
}
