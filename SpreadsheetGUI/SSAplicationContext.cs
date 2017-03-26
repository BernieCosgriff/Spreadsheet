using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS;
using SSGui;

namespace SpreadsheetGUI
{
    class SSAplicationContext : ApplicationContext
    {
        //Number of windows open
        private int windowsCount = 0;
        
        private static SSAplicationContext context;

        //Singleton pattern
        private SSAplicationContext()
        {
        }

        //Singleton pattern
        public static SSAplicationContext GetContext()
        {
            if(context == null)
            {
                context = new SSAplicationContext();
            }
            return context;
        }

        /// <summary>
        /// Opens a new window
        /// </summary>
        public void RunNew()
        {
            SSWindow window = new SSWindow();

            new Controller(window);

            windowsCount++;

            window.FormClosed += (o, e) => { if (--windowsCount <= 0) ExitThread(); };

            window.Show();
        }

        //Opens a new window from a saved spreadsheet
        public void RunNew(string filename)
        {
            SSWindow window = new SSWindow();

            new Controller(window, filename);

            windowsCount++;

            window.FormClosed += (o, e) => { if (--windowsCount <= 0) ExitThread(); };

            window.Show();
        }
    }
}
