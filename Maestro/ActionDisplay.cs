﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Threading;

namespace Maestro
{
    class ActionDisplay
    {
        const int DISPLAYMARGIN = 1001;

        private Canvas StpScr { get; set; }
        private List<Step> stepList { get; set; }
        public double columnSpace {get;set;}
        public double rowSpace {get;set;}
        private int currentStep = 0;
        private Judge judge= new Judge();


        private ImageBrush lefthand, righthand, leftfoot, rightfoot;
        private TextBox score;
        public int currentScore { get; set; }

        public Difficulty selectedDifficulty { get; set; }

        public int leftHandPos, rightHandPos, leftFootPos, rightFootPos;

        public ActionDisplay()
        {

        }

        public ActionDisplay(Canvas gameScreen)
        {
            this.StpScr = gameScreen;
            columnSpace = gameScreen.Width / 3.0;
            rowSpace = gameScreen.Height / 3.0;
            //Divide the screen into 3 parts
            //Step.gameScr = gameScreen;

            currentScore = 0;

            lefthand = new ImageBrush();
            righthand = new ImageBrush();
            leftfoot = new ImageBrush();
            rightfoot = new ImageBrush();

            lefthand.ImageSource = new BitmapImage(new Uri("images\\icon_lefthand.jpg", UriKind.Relative));
            righthand.ImageSource = new BitmapImage(new Uri("images\\icon_righthand.jpg", UriKind.Relative));
            leftfoot.ImageSource = new BitmapImage(new Uri("images\\icon_leftfoot.jpg", UriKind.Relative));
            rightfoot.ImageSource = new BitmapImage(new Uri("images\\icon_rightfoot.jpg", UriKind.Relative));

            leftHandPos = 0;
            rightHandPos = 0;
            leftFootPos = 0;
            rightFootPos = 0;
        }

        public void loadSteps(List<Step> stepList)
        {
            this.stepList = stepList;
        }

        public void drawWithLabel(TextBox t)
        {
            TextBox border = new TextBox();
            border.Background = null;
            border.BorderBrush = null;
            border.TextAlignment = t.TextAlignment;
            border.FontFamily = t.FontFamily;
            border.Width = t.Width;
            border.Text = t.Text;
            border.RenderTransform = t.RenderTransform;

            border.FontSize = t.FontSize * 1.02;
            border.Foreground = new RadialGradientBrush(Colors.White, Colors.LightGray);
            
            StpScr.Children.Add(border);
            StpScr.Children.Add(t);
        }

        public void prepareCanvas()
        {
            //Clear the canvas
            StpScr.Children.Clear();

            //Margins
            double marginX = columnSpace * 0.33;
            double marginY = rowSpace * 0.27;

            //For each case
            for (int i = 0; i < 9; i++)
            {
                if (i != 4)
                {

                    Ellipse circle = new Ellipse();
                    circle.Width = rowSpace * 0.5;
                    circle.Height = rowSpace * 0.5;
                    circle.StrokeThickness = 10;

                    circle.Stroke = new LinearGradientBrush(Colors.Snow, Colors.SkyBlue, 90);
                    circle.RenderTransform = new TranslateTransform(columnSpace * (i % 3) + marginX, rowSpace * (i / 3) + marginY);

                    //Add it to the canvas
                    StpScr.Children.Add(circle);
                }
            }

            // current score display
            score = new TextBox();

            score.Background = null;
            score.BorderBrush = null;
            score.TextAlignment = System.Windows.TextAlignment.Center;
            score.Width = columnSpace * 0.8;

            score.FontSize = 36;
            score.FontFamily = new FontFamily("MV Boli");
            score.Foreground = Brushes.Snow;

            score.RenderTransform = new TranslateTransform(columnSpace * 2.2, 0);
            score.Text = "Score : " + currentScore;

            StpScr.Children.Add(score);
            //drawWithLabel(score);
        }

        public void displayStep(int currentTime)
        {
            Step curStep = stepList.ElementAt(currentStep);
            //score.UpdateLayout();

            score.Text = "Score : " + currentScore;
            score.UpdateLayout();

            double col, row;
            if (currentTime >= curStep.timing - 2000 && currentTime < curStep.timing + 0)
            {
                double marginX = columnSpace * 0.33;
                double marginY = rowSpace * 0.27;
                Color fill;

                col = columnSpace * (curStep.area % 3) + marginX;
                row = rowSpace * (curStep.area / 3) + marginY;

                Ellipse circle = new Ellipse();
                circle.Width = rowSpace * 0.5;
                circle.Height = rowSpace * 0.5;
                circle.StrokeThickness = 10;


                //Text for actiontype
                TextBlock block = new TextBlock();
                block.Width = rowSpace * 0.5;
                block.Background = null;
                block.TextAlignment = System.Windows.TextAlignment.Center;

                block.FontSize = 30;
                block.FontFamily = new FontFamily("Jokerman");
                block.Foreground = Brushes.Snow;


                if (curStep.action == ActionType.TouchHandLeft)
                {
                    circle.Stroke = new LinearGradientBrush(Colors.Orange, Colors.BlueViolet, 90);
                    //circle.Fill = new RadialGradientBrush(Colors.Orange, Colors.BlueViolet);
                    fill = Colors.Red;
                    block.Text = "Left";
                    circle.Fill = lefthand;
                }
                else if (curStep.action == ActionType.TouchHandRight)
                {
                    circle.Stroke = new LinearGradientBrush(Colors.BlueViolet, Colors.Orange, 90);
                    //circle.Fill = new RadialGradientBrush(Colors.BlueViolet, Colors.Orange);
                    fill = Colors.OrangeRed;
                    block.Text = "Right";
                    circle.Fill = righthand;
                }
                else if (curStep.action == ActionType.TouchFeetLeft)
                {
                    circle.Stroke = new LinearGradientBrush(Colors.Violet, Colors.LightBlue, 90);
                    //circle.Fill = new RadialGradientBrush((Colors.Violet, Colors.LightBlue);
                    fill = Colors.Blue;
                    block.Text = "Left";
                    circle.Fill = leftfoot;
                }
                else
                {
                    circle.Stroke = new LinearGradientBrush(Colors.LightBlue, Colors.Violet, 90);
                    //circle.Fill = new RadialGradientBrush((Colors.LightBlue, Colors.Violet);
                    fill = Colors.BlueViolet;
                    block.Text = "Right";
                    circle.Fill = rightfoot;
                }


                Console.WriteLine("Correct condition");
                Thread t = new Thread(new ThreadStart(
                    delegate()
                    {
                        circle.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                            delegate()
                            {
                                Step theCurStp = curStep;
                                Console.WriteLine("circle is made");
                                Console.WriteLine("circle is about to appear");

                                ColorAnimation anime = new ColorAnimation(Colors.White, fill, TimeSpan.FromSeconds(2));
                                SolidColorBrush myBrush = new SolidColorBrush();
                                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, anime);

                                //circle.Fill = myBrush;
                                double midX = columnSpace + marginX;
                                double midY = rowSpace + marginY;

                                DoubleAnimation animeX = new DoubleAnimation(midX, col, TimeSpan.FromSeconds(2));
                                DoubleAnimation animeY = new DoubleAnimation(midY, row, TimeSpan.FromSeconds(2));

                                TranslateTransform trans = new TranslateTransform();
                                trans.BeginAnimation(TranslateTransform.XProperty, animeX);
                                trans.BeginAnimation(TranslateTransform.YProperty, animeY);
                                circle.RenderTransform = trans;



                                DoubleAnimation animeY2 = new DoubleAnimation(midY + 0.15 * rowSpace, row + 0.15 * rowSpace, TimeSpan.FromSeconds(2));
                                TranslateTransform trans2 = new TranslateTransform();
                                trans2.BeginAnimation(TranslateTransform.XProperty, animeX);
                                trans2.BeginAnimation(TranslateTransform.YProperty, animeY2);
                                block.RenderTransform = trans2;//


                                StpScr.Children.Add(circle);
                                //StpScr.Children.Add(block);//

                                //Console.WriteLine("circle appeared");
                                System.Timers.Timer timer = new System.Timers.Timer(2200);
                                timer.Elapsed += delegate
                                {
                                    circle.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(
                                        delegate()
                                        {
                                            StpScr.Children.Remove(circle);
                                            StpScr.Children.Remove(block);//

                                            TextBox popper = new TextBox();

                                            popper.Background = null;
                                            popper.BorderBrush = null;
                                            popper.TextAlignment = System.Windows.TextAlignment.Center;
                                            popper.Width = columnSpace;

                                            popper.FontSize = 40;
                                            popper.FontFamily = new FontFamily("Jokerman");
                                            popper.Foreground = new RadialGradientBrush(Colors.Red, Colors.DarkRed);

                                            popper.RenderTransform = new TranslateTransform(col - 0.3 * columnSpace, row + 0.1 * rowSpace);
                                            popper.Text = "Great!!";

                                            //drawWithLabel(popper);
                                            StpScr.Children.Add(popper);



                                            //sleep
                                            System.Timers.Timer timer2 = new System.Timers.Timer(1000);
                                            timer2.Elapsed += delegate
                                            {
                                                circle.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                    new Action(
                                                        delegate()
                                                        {
                                                            StpScr.Children.Remove(popper);
                                                        }
                                                )
                                            );
                                            };
                                            timer2.Start();
                                            
                                            //exit the thread
                                        }
                                    )
                                );
                                };
                                timer.Disposed += delegate
                                {
                                    StpScr.Children.Remove(circle);
                                    //text box pops up


                                };
                                timer.Start();
                                
                                //timer2.Start();
                                
                                bool touched = false;
                                
                                if (theCurStp.done)
                                {
                                    touched = true;
                                    timer.Dispose();
                                }
                               
                            }
                        ));
                    }
                ));
                t.Start();
                if (currentStep < stepList.Count()-1)
                    currentStep++;
            }
        }
    }
}
