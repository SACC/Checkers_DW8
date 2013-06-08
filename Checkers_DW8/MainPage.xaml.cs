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
            this.turn = true;
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
            //No se como reiniciar la aplicacion
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
                int pos = p.Position;

                RowColl rc = new RowColl(p.Position);

                double c = 0;
                double r = 0;

                CompositeTransform cp = this.ellipse.RenderTransform as CompositeTransform;

                #region Set_Move
                if (p.Color == ColorPeace.black)
                {
                    if (((0 < e.TotalManipulation.Translation.Y && e.TotalManipulation.Translation.Y < 67.5) && (-67.5 < e.TotalManipulation.Translation.X && e.TotalManipulation.Translation.X < 67.5)))
                    {
                        if (e.TotalManipulation.Translation.Y > 40.5 && e.TotalManipulation.Translation.X > 40.5)
                        {
                            c = 54;
                            r = 54;
                            pos = pos + 9;
                        }
                        if (e.TotalManipulation.Translation.Y > 40.5 && e.TotalManipulation.Translation.X < -40.5)
                        {
                            c = -54;
                            r = 54;
                            pos = pos + 7;
                        }
                    }
                }
                else
                {
                    if (((-67.5 < e.TotalManipulation.Translation.Y && e.TotalManipulation.Translation.Y < 0) && (-67.5 < e.TotalManipulation.Translation.X && e.TotalManipulation.Translation.X < 67.5)))
                    {
                        if (e.TotalManipulation.Translation.Y < -40.5 && e.TotalManipulation.Translation.X > 40.5)
                        {
                            c = 54;
                            r = -54;
                            pos = pos - 7;
                        }
                        if (e.TotalManipulation.Translation.Y < -40.5 && e.TotalManipulation.Translation.X < -40.5)
                        {
                            c = -54;
                            r = -54;
                            pos = pos - 9;
                        }
                    }
                }
                #endregion

                #region Movement
                if (PlayMove.IsEmptyPlace(pos, boardPieces))
                {
                    cp.TranslateX += c - e.TotalManipulation.Translation.X;
                    cp.TranslateY += r - e.TotalManipulation.Translation.Y;
                    (boardPieces.Where(piece => piece.Name.Equals(this.ellipse.GetValue(NameProperty))).ToList<Pieces>()).ForEach(pi => pi.Position = pos);
                    this.turn = !this.turn;
                }
                else
                {
                    if (PlayMove.TakePiece(pos, p, boardPieces))
                    {
                        int takePiece = pos;
                        #region Take_Piece
                        if (p.Color == ColorPeace.black)
                        {
                            if (((0 < e.TotalManipulation.Translation.Y && e.TotalManipulation.Translation.Y < 94.5) && (-94.5 < e.TotalManipulation.Translation.X && e.TotalManipulation.Translation.X < 94.5)))
                            {
                                if (e.TotalManipulation.Translation.Y > 40.5 && e.TotalManipulation.Translation.X > 40.5)
                                {
                                    c += 54;
                                    r += 54;
                                    pos = pos + 9;
                                }
                                if (e.TotalManipulation.Translation.Y > 40.5 && e.TotalManipulation.Translation.X < -40.5)
                                {
                                    c += -54;
                                    r += 54;
                                    pos = pos + 7;
                                }
                            }
                        }
                        else
                        {
                            if (((-94.5 < e.TotalManipulation.Translation.Y && e.TotalManipulation.Translation.Y < 0) && (-94.5 < e.TotalManipulation.Translation.X && e.TotalManipulation.Translation.X < 94.5)))
                            {
                                if (e.TotalManipulation.Translation.Y < -40.5 && e.TotalManipulation.Translation.X > 40.5)
                                {
                                    c += 54;
                                    r += -54;
                                    pos = pos - 7;
                                }
                                if (e.TotalManipulation.Translation.Y < -40.5 && e.TotalManipulation.Translation.X < -40.5)
                                {
                                    c += -54;
                                    r += -54;
                                    pos = pos - 9;
                                }
                            }
                        }
                        #endregion
                        Pieces tp = (from t in boardPieces where t.Position.Equals(takePiece) select t).Single();
                        foreach (var item in Board.Children)
                        {
                            // Solo nos interesan los objetos tipo Ellipse
                            if (item.GetType() == typeof(Ellipse))
                            {
                                Ellipse ellipse = item as Ellipse;
                                if (ellipse.GetValue(NameProperty).ToString() == tp.Name)
                                {
                                    Board.Children.Remove(ellipse);
                                    boardPieces.Remove(tp);
                                    break;
                                }
                            }
                        }
                        cp.TranslateX += c - e.TotalManipulation.Translation.X;
                        cp.TranslateY += r - e.TotalManipulation.Translation.Y;
                        (boardPieces.Where(piece => piece.Name.Equals(this.ellipse.GetValue(NameProperty))).ToList<Pieces>()).ForEach(pi => pi.Position = pos);
                        this.turn = !this.turn;
                    }
                    else
                    {
                        cp.TranslateX -= e.TotalManipulation.Translation.X;
                        cp.TranslateY -= e.TotalManipulation.Translation.Y;
                    }
                }
                #endregion

                if (PlayMove.IsCrow(pos, p.Color))
                {
                    (boardPieces.Where(piece => piece.Name.Equals(this.ellipse.GetValue(NameProperty))).ToList<Pieces>()).ForEach(pi => pi.IsQueen = true);
                    if (p.Color == ColorPeace.black)
                    {
                        this.ellipse.Style = bgBlackQueen as Style;
                    }
                    else
                    {
                        this.ellipse.Style = bgWhiteQueen as Style;
                    }
                }
            }
                
            this.txt.Text = string.Format("Blancas {0} vs Negras {1}",
                boardPieces.Where(p => p.Color == ColorPeace.white).ToList().Count.ToString(),
                boardPieces.Where(p => p.Color == ColorPeace.black).ToList().Count.ToString());
        }
        
    }
}