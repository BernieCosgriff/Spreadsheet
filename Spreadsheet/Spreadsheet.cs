using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace SS
{



    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits.  Cell names
    /// are not case sensitive.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are cell names.  (Note that 
    /// "A15" and "a15" name the same cell.)  On the other hand, "Z", "X07", and 
    /// "hello" are not cell names."
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        //Keeps track of which cells depend on eachother
        private DependencyGraph dependencys;
        //Regex used to validate cell names
        private Regex validName;
        //Associates cells with their cell names
        private Dictionary<string, Cell> cells;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get; protected set;
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        /// </summary>
        public Spreadsheet()
        {
            // Initialize instance variables
            dependencys = new DependencyGraph();
            validName = new Regex(".*", RegexOptions.IgnoreCase);
            cells = new Dictionary<string, Cell>();
            Changed = false;
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        /// </summary>
        /// <param name="isValid"></param>
        public Spreadsheet(Regex isValid)
        {
            // Initialize instance variables
            dependencys = new DependencyGraph();
            validName = isValid;
            cells = new Dictionary<string, Cell>();
            Changed = false;
        }
        /// <summary>
        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  If there's a problem reading source, throws an IOException
        /// If the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  If there is an invalid cell name, or a 
        /// duplicate cell name, or an invalid formula in the source, throws a SpreadsheetReadException.
        /// If there's a Formula that causes a circular dependency, throws a SpreadsheetReadException. 
        /// </summary>
        /// <cell name="cell name goes here" contents="cell contents go here"></cell>
        public Spreadsheet(TextReader source)
        {
            // Initialize instance variables
            dependencys = new DependencyGraph();
            cells = new Dictionary<string, Cell>();

            XmlSchemaSet schema = new XmlSchemaSet();

            schema.Add(null, "Spreadsheet.xsd");

            XmlReaderSettings sets = new XmlReaderSettings();
            sets.ValidationType = ValidationType.Schema;
            sets.Schemas = schema;
            sets.ValidationEventHandler += ValidationCallback;
            try
            {
                using (XmlReader reader = XmlReader.Create(source, sets))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    validName = new Regex(reader["IsValid"]);
                                    break;

                                case "cell":
                                    string name = reader["name"];
                                    string content = reader["contents"];
                                    if (cells.ContainsKey(name))
                                    {
                                        throw new SpreadsheetReadException(name + " already exists in the spreadsheet (Duplicate cell name).");
                                    }
                                    if (!validName.IsMatch(name))
                                    {
                                        throw new SpreadsheetReadException(name + " is an invalid cell name.");
                                    }
                                    try
                                    {
                                        SetContentsOfCell(name, content);
                                    }
                                    catch (FormulaFormatException)
                                    {
                                        throw new SpreadsheetReadException(content + " is an invalid formula.");
                                    }
                                    catch (CircularException)
                                    {
                                        throw new SpreadsheetReadException(content + " caused a circular dependency.");
                                    }
                                    break;
                            }
                        }
                    }
                }

                Changed = false;
            }
            catch (Exception e)
            {

                throw new SpreadsheetReadException(e.Message);
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            Cell c;

            if (name == null || !validName.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            //Make sure all cell names are stored in UpperCase
            name = name.ToUpper();

            //If there is a cell return its contents
            if (cells.TryGetValue(name, out c))
            {
                return c.contents;
            }

            // Otherwise return an empty string
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            HashSet<string> set = new HashSet<string>();

            // Add each cell that currently is in cells to set and return set
            foreach (KeyValuePair<string, Cell> pair in cells)
            {
                set.Add(pair.Key);
            }
            return set;
        }

        /// <summary>
        /// If formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            Cell cell;

            bool contentChanged = false;

            object oldContent = null;

            object oldValue = null;

            if (formula.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (name == null || !validName.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            // Make sure that all cell names are stored upper case
            name = name.ToUpper();

            // If the cell already exists remove its formulas
            // dependencys and then replace its contents with 
            // the new formula
            if (cells.TryGetValue(name, out cell))
            {
                contentChanged = true;

                oldValue = cell.value;

                if ((oldContent = cell.contents) is Formula)
                {
                    Formula f = (Formula)cell.contents;

                    foreach (string s in f.GetVariables())
                    {
                        dependencys.RemoveDependency(s.ToUpper(), name);
                    }
                }
                cell.contents = formula;
            }

            //Otherwise just make a new cell with the new formula
            else
            {
                cell = new Cell(formula, Lookup);
                cells.Add(name, cell);
            }

            //Add the new dependencys
            foreach (string s in formula.GetVariables())
            {
                dependencys.AddDependency(s.ToUpper(), name);
            }

            try
            {
                return new HashSet<string>(GetCellsToRecalculate(name));
            }
            catch (CircularException)
            {
                if (contentChanged)
                {
                    foreach(string s in formula.GetVariables())
                    {
                        dependencys.RemoveDependency(s.ToUpper(), name);
                    }
                    cell.contents = oldContent;
                }
                throw new CircularException();
            }

        }

        /// <summary>
        /// Lookup method that uses the cells dictionary
        /// to get the values of variables in a formula.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private double Lookup(string s)
        {
            Cell cell;

            // If s is a cell name return and its contents is a double
            // return that double otherwise throw an exception.
            if (cells.TryGetValue(s, out cell))
            {
                if (cell.value is double)
                {
                    return (double)cell.value;
                }
            }
            throw new UndefinedVariableException(s);
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            Cell c;

            if (text == null)
            {
                throw new ArgumentNullException();
            }
            
            if (name == null || !validName.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            // Make sure all cell names are stored upper case
            name = name.ToUpper();

            // If c already exists and its contents is a formula
            // remove the dependencys from that formula then replace
            // its contents with text
            if (cells.TryGetValue(name, out c))
            {
                if (c.contents is Formula)
                {
                    Formula f = (Formula)c.contents;

                    foreach (string s in f.GetVariables())
                    {
                        dependencys.RemoveDependency(s.ToUpper(), name);
                    }
                }

                if (text.Equals(""))
                {
                    cells.Remove(name);
                    return new HashSet<string>(GetCellsToRecalculate(name));
                }

                c.contents = text;
            }

            // Otherwise make a new cell
            else if(!text.Equals(""))
            {
                c = new Cell(text);
                cells.Add(name, c);
            }

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            Cell c;

            if (name == null || !validName.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            //Make sure cell names are stored upper case
            name = name.ToUpper();

            // If c already exists and its contents is a formula
            // remove the dependencys from that formula then replace
            // its contents with the number
            if (cells.TryGetValue(name, out c))
            {
                if (c.contents is Formula)
                {
                    Formula f = (Formula)c.contents;

                    foreach (string s in f.GetVariables())
                    {
                        dependencys.RemoveDependency(s.ToUpper(), name);
                    }
                }

                c.contents = number;
            }

            //Otherwise make a new cell
            else
            {
                c = new Cell(number);
                cells.Add(name, c);
            }

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {

            name = name.ToUpper();

            HashSet<string> set = new HashSet<string>();

            if (name == null)
            {
                throw new ArgumentNullException();
            }

            if (!validName.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            return dependencys.GetDependents(name);
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the isvalid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            Changed = false;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            using (XmlWriter writer = XmlWriter.Create(dest, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("IsValid", validName.ToString());

                foreach (KeyValuePair<string, Cell> pair in cells)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteAttributeString("name", pair.Key);
                    if (pair.Value.contents is Formula)
                    {
                        writer.WriteAttributeString("contents", "=" + pair.Value.contents.ToString());
                    }
                    else
                    {
                        writer.WriteAttributeString("contents", pair.Value.contents.ToString());
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }


        private static void ValidationCallback(object sender, ValidationEventArgs e)
        {
            throw new SpreadsheetReadException("Validation Error");
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            Cell c;


            if (name == null || !validName.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            name = name.ToUpper();

            if (cells.TryGetValue(name, out c))
            {
                return c.value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            HashSet<string> set;

            double d;

            if (content == null)
            {
                throw new ArgumentNullException();
            }

            if (name == null || !validName.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            name = name.ToUpper();

            if (Double.TryParse(content, out (d)))
            {
                set = new HashSet<string>(SetCellContents(name, d));
            }

            else if (!String.IsNullOrEmpty(content) && content.ElementAt(0).Equals('='))
            {
                set = new HashSet<string>(SetCellContents(name, new Formula(content.Substring(1), s => s.ToUpper(), validName.IsMatch)));
            }

            else
            {
                set = new HashSet<string>(SetCellContents(name, content));
            }
            Changed = true;
            ReEvaluate(set);
            return set;
        }

        /// <summary>
        /// ReEvaluate each cell in cellsToRecalculate in 
        /// the order given in the set.
        /// </summary>
        /// <param name="cellsToRecalculate"></param>
        private void ReEvaluate(HashSet<string> cellsToRecalculate)
        {
            Cell c;

            foreach (string s in cellsToRecalculate)
            {
                if (cells.TryGetValue(s, out c))
                {
                    if (c.contents is Formula)
                    {
                        Formula f = (Formula)c.contents;
                        try
                        {
                            c.value = f.Evaluate(Lookup);
                        }
                        catch (FormulaEvaluationException)
                        {
                            c.value = new FormulaError();
                        }
                    }

                    else
                    {
                        c.value = c.contents;
                    }
                }
            }
        }

        /// <summary>
        /// Class representing a cell in a spreasheet.
        /// </summary>
        protected class Cell
        {
            /// <summary>
            /// Contents of the cell
            /// </summary>
            public object contents { get; set; }

            /// <summary>
            /// Value of the cell
            /// </summary>
            public object value { get; set; }

            /// <summary>
            /// Constructor that sets the name, contents and 
            /// value of the cell using text.
            /// </summary>
            /// <param name="_contents"></param>
            public Cell(string _contents)
            {
                value = contents = _contents;
            }

            /// <summary>
            /// Constructor that sets the name, contents and 
            /// value of the cell using a Formula.
            /// </summary>
            /// <param name="_contents"></param>
            /// /// <param name="lookup"></param>
            public Cell(Formula _contents, Lookup lookup)
            {
                contents = _contents;

                // If contents can be evaluated set value to the result
                try
                {
                    value = _contents.Evaluate(lookup);
                }

                // Otherwise set value to a FormulaError
                catch (FormulaEvaluationException e)
                {
                    value = new FormulaError(e.Message);
                }
            }

            /// <summary>
            /// Constructor that sets the name, contents and 
            /// value of the cell using a double.
            /// </summary>
            /// <param name="_contents"></param>
            public Cell(double _contents)
            {
                value = contents = _contents;
            }
        }
    }
}
