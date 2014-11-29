using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ItalianCheckers.Models;

namespace ItalianCheckers.Views
{
    public partial class FigureView : UserControl
    {
        Color backWhiteColor = Color.Moccasin;
        Color backBlackColor = Color.Black;
        Color backSelectionColor = Color.RoyalBlue;

        FigureEnum figure;

        static System.Drawing.Image blackSenior;
        static System.Drawing.Image whiteSenior;
        static System.Drawing.Image blackPawn;
        static System.Drawing.Image whitePawn;

        bool isBlack;

        static FigureView()
        {
            System.ComponentModel.ComponentResourceManager resources;
            resources = new System.ComponentModel.ComponentResourceManager(typeof(FigureView));

            blackSenior = ((System.Drawing.Image)(resources.GetObject("BlackKing")));
            whiteSenior = ((System.Drawing.Image)(resources.GetObject("WhiteKing")));
            blackPawn = ((System.Drawing.Image)(resources.GetObject("BlackPawn")));
            whitePawn = ((System.Drawing.Image)(resources.GetObject("WhitePawn")));
        }

        public FigureView(bool blackOrWhiteColor)
        {
            InitializeComponent();

            isBlack = blackOrWhiteColor;
            this.BackColor = blackOrWhiteColor ? backBlackColor : backWhiteColor;
        }


        public FigureEnum Figure
        {
            get { return figure; }
            set { figure = value; UpdateView(); }
        }

        private void UpdateView()
        {
            switch(figure)
            {
                case FigureEnum.NONE:
                    this.pictureBox1.Image = null;
                    break;
                case FigureEnum.WHITE_PAWN:
                    this.pictureBox1.Image = whitePawn;
                    break;
                case FigureEnum.WHITE_SENIOR:
                    this.pictureBox1.Image = whiteSenior;
                    break;
                case FigureEnum.BLACK_PAWN:
                    this.pictureBox1.Image = blackPawn;
                    break;
                case FigureEnum.BLACK_SENIOR:
                    this.pictureBox1.Image = blackSenior;
                    break;
            }
            
           
        }

        bool selected = false;

        public bool Selected 
        {
            get { return selected; }
            set
            {
                selected = value;                
                this.BackColor = selected ? backSelectionColor : (isBlack ? backBlackColor : backWhiteColor);
            }
        }
    }
}
