using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Checkers_DW8.Resources;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Checkers_DW8
{
    public partial class MainPage : PhoneApplicationPage
    {
        public List<Pieces> boardPieces;           
        public Ellipse ellipse { get; set; }
        public bool canMove { get; set; }
        public bool turn { get; set; }
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;                    
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            boardPieces = new List<Pieces>();            
            LoadBoardPieces();
            this.canMove = false;
            this.turn = false;
        }

        private void LoadBoardPieces()
        {
            boardPieces.Clear();            
            boardPieces.Add(new Pieces("r1", 1, ColorPeace.black, false)); boardPieces.Add(new Pieces("r2", 3, ColorPeace.black, false));
            boardPieces.Add(new Pieces("r3", 5, ColorPeace.black, false)); boardPieces.Add(new Pieces("r4", 7, ColorPeace.black, false));
            boardPieces.Add(new Pieces("r5", 8, ColorPeace.black, false)); boardPieces.Add(new Pieces("r6", 10, ColorPeace.black, false));
            boardPieces.Add(new Pieces("r7", 12, ColorPeace.black, false)); boardPieces.Add(new Pieces("r8", 14, ColorPeace.black, false));
            boardPieces.Add(new Pieces("r9", 17, ColorPeace.black, false)); boardPieces.Add(new Pieces("r10", 19, ColorPeace.black, false));
            boardPieces.Add(new Pieces("r11", 21, ColorPeace.black, false)); boardPieces.Add(new Pieces("r12", 23, ColorPeace.black, false));

            boardPieces.Add(new Pieces("w1", 62, ColorPeace.white, false)); boardPieces.Add(new Pieces("w2", 60, ColorPeace.white, false));
            boardPieces.Add(new Pieces("w3", 58, ColorPeace.white, false)); boardPieces.Add(new Pieces("w4", 56, ColorPeace.white, false));
            boardPieces.Add(new Pieces("w5", 55, ColorPeace.white, false)); boardPieces.Add(new Pieces("w6", 53, ColorPeace.white, false));
            boardPieces.Add(new Pieces("w7", 51, ColorPeace.white, false)); boardPieces.Add(new Pieces("w8", 49, ColorPeace.white, false));
            boardPieces.Add(new Pieces("w9", 46, ColorPeace.white, false)); boardPieces.Add(new Pieces("w10", 44, ColorPeace.white, false));
            boardPieces.Add(new Pieces("w11", 42, ColorPeace.white, false)); boardPieces.Add(new Pieces("w12", 40, ColorPeace.white, false));
            
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {            
            //Reinicio la Lista que contene las piezas
            LoadBoardPieces();            
            foreach (var item in Board.Children)
            {
                // Solo nos interesan los objetos tipo Ellipse
                if (item.GetType() == typeof(Ellipse))
                {
                    Ellipse eli = item as Ellipse;
                    Pieces p = (from b in boardPieces
                                where b.Name.Equals(eli.Name)
                             select b).Single();
                    RowColl rc = new RowColl(p.Position);                    
                    eli.SetValue(Grid.ColumnProperty, rc.col);
                    eli.SetValue(Grid.RowProperty, rc.row);
                    ImageBrush ib = new ImageBrush();
                    if(p.Color == ColorPeace.black)
                    {
                        ib.ImageSource = new BitmapImage(new Uri(@"pieceBlack.png", UriKind.Relative));
                    }        
                    else
                    {
                        ib.ImageSource = new BitmapImage(new Uri(@"pieceWhite.png", UriKind.Relative));
                    }
                    eli.Fill = ib;
                }
            }
            Board.UpdateLayout();
            boardPieces.ForEach(p => p.IsActive = true);
            this.txt.Text = "";
            this.txt1.Text = "";
            this.canMove = false;
            this.turn = false;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Terminate();
        }             

        /// <summary>
        /// Inicia la Manipulacion de la ficha, determinando si puede o no mover.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ellipse_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            this.ellipse = sender as Ellipse;
            Pieces p = (from b in boardPieces
                     where b.Name.Equals(this.ellipse.GetValue(NameProperty))
                     select b).Single();
            if (!this.turn && p.Color == ColorPeace.white || this.turn && p.Color == ColorPeace.black)
            {
                this.txt.Text = "No es tu turno.";
                this.canMove = false;
            }
            else
            {
                this.txt.Text = "";
                this.canMove = PlayMove.CanMove(p, boardPieces);
                if (!this.canMove)
                    this.txt.Text = "La ficha seleccionada no se puede mover.";
            }
        }
        
        /// <summary>
        /// Se mueve la ficha.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ellipse_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.canMove)
            {
                CompositeTransform cp = this.ellipse.RenderTransform as CompositeTransform;
                cp.TranslateX += e.DeltaManipulation.Translation.X;
                cp.TranslateY += e.DeltaManipulation.Translation.Y;                
            }
        }

        /// <summary>
        /// Se termina la manipuralción de la ficha y determina si puede o no ubicarse en el lugar
        /// indicado, de lo contrario vuelve a su posición inicial.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ellipse_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (this.canMove)
            {                
                Pieces p = (from b in boardPieces
                            where b.Name.Equals(this.ellipse.GetValue(NameProperty))
                            select b).Single();                

                RowColl initialPosition = new RowColl(p.Position);
                RowColl finalPosition = new RowColl(p.Position);                                                
                
                CompositeTransform cp = this.ellipse.RenderTransform as CompositeTransform;

                #region Set_Move
                if (p.Color == ColorPeace.black)
                {                    
                    if (e.TotalManipulation.Translation.Y > 0)
                    {                        
                        //Evaluamos si debemos sumar o restar una columna
                        if (e.TotalManipulation.Translation.X > 0)
                        {
                            if (e.TotalManipulation.Translation.Y > 40.5 && e.TotalManipulation.Translation.X > 40.5)
                            {
                                finalPosition.col = initialPosition.col + 1;
                                finalPosition.row = initialPosition.row + 1;
                            }                            
                        }
                        else
                        {
                            if (e.TotalManipulation.Translation.Y > 40.5 && e.TotalManipulation.Translation.X < -40.5)
                            {
                                finalPosition.col = initialPosition.col - 1;
                                finalPosition.row = initialPosition.row + 1;
                            }   
                        }

                    }
                }
                else
                {
                    if (e.TotalManipulation.Translation.Y < 0)
                    {                        
                        //Evaluamos si debemos sumar o restar una columna
                        if (e.TotalManipulation.Translation.X > 0)
                        {
                            if (e.TotalManipulation.Translation.Y < -40.5 && e.TotalManipulation.Translation.X > 40.5)
                            {
                                finalPosition.col = initialPosition.col + 1;
                                finalPosition.row = initialPosition.row - 1;
                            }
                        }
                        else
                        {
                            if (e.TotalManipulation.Translation.Y < -40.5 && e.TotalManipulation.Translation.X < -40.5)
                            {
                                finalPosition.col = initialPosition.col - 1;
                                finalPosition.row = initialPosition.row - 1;
                            }
                        }

                    }
                }
                #endregion

                #region Movement
                if (PlayMove.IsEmptyPlace(finalPosition.GetPosition(), boardPieces))
                {
                    cp.TranslateX -= e.TotalManipulation.Translation.X;
                    cp.TranslateY -= e.TotalManipulation.Translation.Y;
                    this.ellipse.SetValue(Grid.ColumnProperty, finalPosition.col);
                    this.ellipse.SetValue(Grid.RowProperty, finalPosition.row);
                    (boardPieces.Where(piece => piece.Name.Equals(this.ellipse.GetValue(NameProperty))).ToList<Pieces>()).ForEach(pi => pi.Position = finalPosition.GetPosition());
                    this.turn = !this.turn;
                }
                else
                {
                    if (PlayMove.TakePiece(finalPosition.GetPosition(), p, boardPieces))
                    {   
                        Pieces tp = (from t in boardPieces where t.Position.Equals(finalPosition.GetPosition()) && t.IsActive select t).Single();
                        bool takedPiece = false;

                        #region Take_Piece
                        if (p.Color == ColorPeace.black)
                        {
                            if (e.TotalManipulation.Translation.Y > 0)
                            {
                                //Evaluamos si debemos sumar o restar una columna
                                if (e.TotalManipulation.Translation.X > 0)
                                {
                                    if (e.TotalManipulation.Translation.Y > 94.5 && e.TotalManipulation.Translation.X > 94.5)
                                    {
                                        finalPosition.col = initialPosition.col + 2;
                                        finalPosition.row = initialPosition.row + 2;
                                        takedPiece = true;
                                    }
                                }
                                else
                                {
                                    if (e.TotalManipulation.Translation.Y > 94.5 && e.TotalManipulation.Translation.X < -94.5)
                                    {
                                        finalPosition.col = initialPosition.col - 2;
                                        finalPosition.row = initialPosition.row + 2;
                                        takedPiece = true;
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (e.TotalManipulation.Translation.Y < 0)
                            {                                
                                //Evaluamos si debemos sumar o restar una columna
                                if (e.TotalManipulation.Translation.X > 0)
                                {
                                    if (e.TotalManipulation.Translation.Y < -94.5 && e.TotalManipulation.Translation.X > 94.5)
                                    {
                                        finalPosition.col = initialPosition.col + 2;
                                        finalPosition.row = initialPosition.row - 2;
                                        takedPiece = true;
                                    }
                                }
                                else
                                {
                                    if (e.TotalManipulation.Translation.Y < -94.5 && e.TotalManipulation.Translation.X < -94.5)
                                    {
                                        finalPosition.col = initialPosition.col - 2;
                                        finalPosition.row = initialPosition.row - 2;
                                        takedPiece = true;
                                    }
                                }

                            }
                        }                        
                        #endregion

                        if (takedPiece)
                        {                                             
                            foreach (var item in Board.Children)
                            {
                                // Solo nos interesan los objetos tipo Ellipse
                                if (item.GetType() == typeof(Ellipse))
                                {
                                    Ellipse eli = item as Ellipse;
                                    if (eli.GetValue(NameProperty).ToString() == tp.Name)
                                    {
                                        ImageBrush hib = new ImageBrush();
                                        hib.ImageSource = new BitmapImage(new Uri(@"pieceEmpty.png", UriKind.Relative));
                                        eli.Fill = hib;
                                        eli.SetValue(Grid.ColumnProperty, 0);
                                        eli.SetValue(Grid.RowProperty, 0);
                                        boardPieces.Where(pc => pc.Name.Equals(tp.Name)).ToList().ForEach(ph => ph.IsActive = false);
                                        break;
                                    }
                                }
                            }
                            cp.TranslateX -= e.TotalManipulation.Translation.X;
                            cp.TranslateY -= e.TotalManipulation.Translation.Y;
                            this.ellipse.SetValue(Grid.ColumnProperty, finalPosition.col);
                            this.ellipse.SetValue(Grid.RowProperty, finalPosition.row);
                            boardPieces.Where(pc=>pc.Name.Equals(this.ellipse.Name)).ToList().ForEach(ph => ph.Position = finalPosition.GetPosition());
                            this.turn = !this.turn;
                        }
                        else
                        {
                            cp.TranslateX -= e.TotalManipulation.Translation.X;
                            cp.TranslateY -= e.TotalManipulation.Translation.Y;
                        }
                    }
                    else
                    {
                        cp.TranslateX -= e.TotalManipulation.Translation.X;
                        cp.TranslateY -= e.TotalManipulation.Translation.Y;
                    }
                }
                #endregion

                if (PlayMove.IsCrow(finalPosition.GetPosition(), p.Color))
                {
                    ImageBrush qib = new ImageBrush();
                    boardPieces.Where(pc=>pc.Name.Equals(this.ellipse.Name)).ToList<Pieces>().ForEach(pi => pi.IsQueen = true);
                    if (p.Color == ColorPeace.black)
                    {
                        qib.ImageSource = new BitmapImage(new Uri(@"blackCrown.png", UriKind.Relative));
                    }
                    else
                    {
                        qib.ImageSource = new BitmapImage(new Uri(@"whiteCrown.png", UriKind.Relative));
                    }
                    this.ellipse.Fill = qib;
                    this.txt1.Text = string.Format("Damas Blancas {0} vs Damas Negras {1}",
                        boardPieces.Where(pc => pc.Color == ColorPeace.white && pc.IsActive && pc.IsQueen).ToList().Count.ToString(),
                        boardPieces.Where(pc => pc.Color == ColorPeace.black && pc.IsActive && pc.IsQueen).ToList().Count.ToString());
                }                
            }

            this.txt.Text = string.Format("Blancas {0} vs Negras {1}",
                boardPieces.Where(p => p.Color == ColorPeace.white && p.IsActive).ToList().Count.ToString(),
                boardPieces.Where(p => p.Color == ColorPeace.black && p.IsActive).ToList().Count.ToString());            

            if (boardPieces.Where(p => p.Color == ColorPeace.white && p.IsActive).ToList().Count == 0)
            {
                this.txt.Text = "Ganan las Rojas";
            }
            if (boardPieces.Where(p => p.Color == ColorPeace.black && p.IsActive).ToList().Count == 0)
            {
                this.txt.Text = "Ganan las Amarillas";                
            }
            
        }
        
    }
}