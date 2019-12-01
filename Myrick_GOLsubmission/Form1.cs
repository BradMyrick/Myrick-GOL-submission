using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Myrick_GOLsubmission
{
    
    public partial class Form1 : Form
    {
       static int rows = 50;
       static int cols = 50;
        // The universe array
        bool[,] universe = new bool[rows, cols];
        // Drawing colors
        Color gridColor = Color.White;
        Color cellColor = Color.Magenta;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

        }

        private float CheckStatus(int row, int col)
        {
            // converting these to floats greatly reduced the mouse click offset when in fullscreen.
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (1.0f * graphicsPanel1.ClientSize.Width) / (1.0f * universe.GetLength(0));
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight =(1.0f *  graphicsPanel1.ClientSize.Height) /(1.0f * universe.GetLength(1));
            float myRows = rows;
            float myCols = cols;
            float myCellWidth = cellWidth;
            int count = 0;

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

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            Form1 temp = new Form1();
            int myRows = rows;
            int myCols = cols;
            
            bool[,] newGrid = new bool[myRows, myCols];
            for (float r = 0; r < universe.GetLength(0); r++)
            {
                for (float c = 0; c < universe.GetLength(1); c++)
                {
                    int count = (int)CheckStatus((int)r,(int)c);


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

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            /*
             * trying to figure out the conversion from Int to double without using temp variables.
              public static System.Drawing.Rectangle Convert 
              (System.Drawing.Rectangle value, System.Drawing.Printing.PrinterUnit fromUnit, System.Drawing.Printing.PrinterUnit toUnit);
            
             */
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

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //add about and instructions
        }

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
            // Tell Windows you need to repaint
            graphicsPanel1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            /*
             saveFile();
             */
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            /*
             openFile();
             */
        }
    }
}
