using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Myrick_GOLsubmission
{
    //Main Form
    public partial class Form1 : Form
    {
        static int rows = 50;
        static int cols = 50;
        // The universe array
        bool[,] universe = new bool[rows, cols];
        // Drawing colors
        Color gridColor = Properties.Settings.Default.OutlineColor;
        Color cellColor = Properties.Settings.Default.CellColor;
        // The Timer class
        Timer timer = new Timer();
        //Generation count
        int generations = 0;
        //Form Constructor
        public Form1()
        {
            
            InitializeComponent();
            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false;
        }
        //Count Neighbors 
        private float CheckStatus(int row, int col)
        {
            float myRows = rows;
            float myCols = cols;
            int count = 0;
            //counting neighbors
            if ((row - 1 >= 0 && col - 1 > 0)&& universe[row - 1, col - 1] == true)
                count++;
            if ((row - 1 >= 0) && universe[row - 1, col] == true)
                count++;
            if ((row - 1 >= 0 && col + 1 < myCols) && universe[row - 1, col + 1] == true)
                count++;
            if ((col - 1 >= 0) && universe[row, col - 1] == true)
                count++;
            if ((col + 1 < myCols) && universe[row, col + 1] == true)
                count++;
            if ((row + 1 < myRows && col - 1 >= 0) && universe[row + 1, col - 1] == true)
                count++;
            if ((row + 1 < myRows) && universe[row + 1, col] == true)
                count++;
            if ((row + 1 < myRows && col + 1 < myCols) && universe[row + 1, col + 1] == true)
                count++;

            return count;
        }
        //Calculate the next generation of cells
        private void NextGeneration()
        {
            Form1 temp = new Form1();
            int myRows = rows;
            int myCols = cols;
            int count = 0;            
            bool[,] newGrid = new bool[myRows, myCols];
            for (float r = 0; r < universe.GetLength(0); r++)
            {
                for (float c = 0; c < universe.GetLength(1); c++)
                {
                    count = (int)CheckStatus((int)r, (int)c);
                   

                    if (universe[(int)r, (int)c])
                    {
                        if (count == 2 || count == 3)
                            newGrid[(int)r, (int)c] = true;
                        if (count < 2 || count > 3)
                            newGrid[(int)r,(int) c] = false;
                    }
                    else
                    {
                        if (count == 3)
                            newGrid[(int)r,(int) c] = true;
                    }
                }
            }
            universe = newGrid;
            graphicsPanel1.Invalidate();
            // Increment generation count
            generations++;
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }
        //The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            //loop to update # of alive cells
            int alive = 0;
            for (float y = 0; y < (1.0f * universe.GetLength(1)); y++)
            {
                for (float x = 0; x < (1.0f * universe.GetLength(1)); x++)
                {
                    if (universe[(int)x, (int)y] == true)
                    {
                        alive++;
                        break;
                    }
                }
            }
            toolStripStatusLabel2.Text = "Alive Cells = " + alive;
            NextGeneration();
            graphicsPanel1.Invalidate();
        }
        //Paint the cells
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (1.0f * graphicsPanel1.ClientSize.Width) / (1.0f * universe.GetLength(0));
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (1.0f * graphicsPanel1.ClientSize.Height) / (1.0f * universe.GetLength(1));

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (float y = 0; y <(1.0f * universe.GetLength(1)); y++)
            {
                // Iterate through the universe in the x, left to right
                for (float x = 0; x < (1.0f * universe.GetLength(1)); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                   
                    // Fill the cell with a brush if alive
                    if (universe[(int)x,(int) y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }
        //File New
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (1.0f * graphicsPanel1.ClientSize.Width) / (1.0f * universe.GetLength(0));
                float cellHeight = (1.0f * graphicsPanel1.ClientSize.Height) /(1.0f * universe.GetLength(1));

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x,(int) y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        //Clear the Universe
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x,y] = false;
                }
            }
            generations = 0;
            // Tell Windows you need to repaint
            graphicsPanel1.Invalidate();
        }
        //play button
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }
        //pause button
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        //move forward 1 generation
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        //about information under Help
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string about = "Welcome to Conway's Game of life.\nRandomize the Univers under the Tools Tab.";
            MessageBox.Show(about);
        }
        //Clear the universe 
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            graphicsPanel1.Invalidate();
        }
        //Save File
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells"; 
                    dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);
               // writer.WriteLine("!This is my comment.");
                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;
                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x,y])
                        {
                            currentRow += 'O';
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else
                        {
                            currentRow += '.';
                        }                                   
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }
        //Open File
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    //if (row.Contains('!'))
                    //{
                    //    continue;//
                    //}
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    //else
                    //{
                        maxHeight++;
                    //}
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    if (maxWidth < row.Length)
                    {
                        maxWidth = row.Length;
                    }
                }
                int a = maxWidth;
                int b = maxHeight;
                bool[,] temp = new bool[a, b];
                universe = temp;
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    //if (row.StartsWith("!"))
                    //{
                    //    break;//do nothing
                    //}
                    //else
                    //{
                        int y = 0;
                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // set the corresponding cell in the universe to alive
                            if (row[xPos] == 'O')
                            {
                                universe[xPos,y] = true;
                            }
                            // set the rest to !alive
                             else
                            {
                                universe[xPos,y] = !true;   
                            }
                        }
                    //}
                }
                // Close the file.
                reader.Close();
                graphicsPanel1.Invalidate();
            }
        }
        //file -> exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Change the background color
        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }
        //turn grid lines on/off
        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showGridToolStripMenuItem.Checked)
            {
                if (graphicsPanel1.BackColor == Color.Black)
                {
                    gridColor = Color.White;
                }
                else
                {
                    gridColor = Color.Black;
                }
            }
            else { gridColor = Properties.Settings.Default.OutlineColor;}   

        }
        //save file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);
               // writer.WriteLine("!This is my comment.");
                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;
                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y])
                        {
                            currentRow += 'O';
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else
                        {
                            currentRow += '.';
                        }
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }
        //open file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);
                //variables to calculate the width and height of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;
                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    //if (row.Contains('!'))
                    //{
                    //    continue;
                    //}
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    //else
                    //{
                        maxHeight++;
                    //}
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    if (maxWidth <= row.Length)
                    {
                        maxWidth = row.Length;
                    }
                }
                int a = maxWidth;
                int b = maxHeight;
                bool[,] temp = new bool[a, b];
                universe = temp;
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {

                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    //if (row.StartsWith("!"))
                    //{
                    //    break;//do nothing
                    //}
                    //else
                    //{
                        int y = 0;
                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // set the corresponding cell in the universe to alive
                            if (row[xPos] == 'O')
                            {
                                universe[xPos, y] = true;
                            }
                            // set the rest to !alive
                            else
                            {
                                universe[xPos, y] = !true;
                            }
                        }
                    //}
                }
                // Close the file.
                reader.Close();
                graphicsPanel1.Invalidate();
            }

        }
        //change alive cell color
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = Properties.Settings.Default.CellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                
                cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }
        //randomize universe in tool menu
        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Random randomGenerator = new Random();
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j ++)
                {
                    int randomNumber = randomGenerator.Next(8);
                    if (randomNumber == 0)
                        universe[i, j] = true;
                }
            }
            graphicsPanel1.Invalidate();
        }
        //save settings on exit
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Background = graphicsPanel1.BackColor;
            Properties.Settings.Default.OutlineColor = Properties.Settings.Default.Clear;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.Save();
        }
        //25*25 grid
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            rows = 25;
            cols = 25;
            bool[,] temp = new bool[rows, cols];
            universe = temp;
            graphicsPanel1.Invalidate();
        }
        //50*50 grid
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            rows = 50;
            cols = 50;
            bool[,] temp = new bool[rows, cols];
            universe = temp;
            graphicsPanel1.Invalidate();
        }
        //100*100 grid
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            rows = 100;
            cols = 100;
            bool[,] temp = new bool[rows, cols];
            universe = temp;
            graphicsPanel1.Invalidate();
        }
        // 25% time
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            timer.Interval = 400;
        }
        // 50% time
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            timer.Interval = 300;
        }
        // 75% time
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            timer.Interval = 200;
        }
        // 100% time
        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            timer.Interval = 100;
        }
        //contextual show grid
        private void showGridToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (showGridToolStripMenuItem.Checked)
            {
                if (graphicsPanel1.BackColor == Color.Black)
                {
                    gridColor = Color.White;
                }
                else
                {
                    gridColor = Color.Black;
                }
            }
            else { gridColor = Properties.Settings.Default.OutlineColor; }
        }
        //contextual background color
        private void backgroundColorToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }
        //contextual alive cell color
        private void cellColorToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = Properties.Settings.Default.CellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {

                cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }
        //contextual Exit Program
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Life Lexicon Glider Gun
        private void gasperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly _assembly;
            StreamReader _textStreamReader;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("Myrick_GOLsubmission.Gosper_glider_gun.cells"));
            }
            catch
            {
                MessageBox.Show("Error accessing resources!");
            }
        }
        //Life Lexicon R2D2
        private void r2D2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly _assembly;
            StreamReader _textStreamReader;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("Myrick_GOLsubmission.R2D2.cells"));
            }
            catch
            {
                MessageBox.Show("Error accessing resources!");
            }
        }
        //Life Lexicon ring of fire
        private void ringOfFireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly _assembly;
            StreamReader _textStreamReader;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("Myrick_GOLsubmission.ring_of_fire.cells"));
            }
            catch
            {
                MessageBox.Show("Error accessing resources!");
            }
        }
    }
}
