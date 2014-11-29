using ItalianCheckers.Models;
using ItalianCheckers.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ItalianCheckers
{
    public partial class BoardForm : Form
    {
        FigureView[,] figureViews = new FigureView[Board.SIZE, Board.SIZE];

        Board board;
        List<Motion> motions;
        Motion selectedMotion;

        Motion SelectedMotion 
        {
            get { return selectedMotion; }
            set
            {
                selectedMotion = value;
                UpdateBoardView();
            }
        }


        public BoardForm()
        {
            InitializeComponent();
            board = new Board();            
            InitBoard();
            UpdateListView();

            listView.SelectedIndexChanged += listView_SelectedIndexChanged;
        }

        void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMotion = motions[listView.SelectedIndex];
        }


        private void InitBoard()
        {
            this.SuspendLayout();

            bool white = true;
            for(int i = 0; i < Board.SIZE; i++)
            {
                for(int j = 0; j < Board.SIZE; j++)
                {
                    var view = new FigureView(white);
                    view.Location = GetViewPosition(i, j, view.Size.Width, view.Size.Height);
                    view.Figure = board[j,i];
                    figureViews[i, j] = view;
                    this.Controls.Add(view);
                    white = !white;
                }
                white = !white;
            }
            this.ResumeLayout(false);
        }

        private System.Drawing.Point GetViewPosition(int i, int j, int width, int height)
        {
            var p = new System.Drawing.Point();
            p.X = j * width;
            p.Y = i * height;
            return p;
        }

        bool isWhite = true;

        private void OnNextButtonClick(object sender, EventArgs e)
        {
            if (listView.SelectedIndex == -1)
            {
                return;
            }

            board = Rules.ApplyMotion(board, motions[listView.SelectedIndex], isWhite);
            UpdateBoardView();
            isWhite = !isWhite;
            
            UpdateListView();
        }

        private void UpdateListView()
        {
            motions = Rules.GetAllMotions(board, isWhite);
            listView.DataSource = motions;
        }
        
        private void UpdateBoardView()
        {
            for (int i = 0; i < Board.SIZE; i++)
                for (int j = 0; j < Board.SIZE; j++)
                {
                    figureViews[j, i].Figure = board[i, j];
                    figureViews[j, i].Selected = false;
                }

            if(SelectedMotion != null)
            {
                foreach(var m in SelectedMotion.Moves)
                {
                    figureViews[m.Y, m.X].Selected = true;
                }
            }
        }
       
    }
}
