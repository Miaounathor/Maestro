﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;

namespace Maestro
{
    class DisplayEngine
    {
        //Current gamescreen
        Canvas GameScreen { get; set; }

        public double columnSpace { get; set; }
        public double rowSpace { get; set; }

        //Grid
        private Rectangle[] grid;

        //Textbox
        private TextBox[] texts;

        //Current displayed screen
        public Screen CurrentScreen { get; private set; }

        //Active for command
        private bool active;
        private static Timer count;

        //Current difficulty
        public Difficulty selectedDifficulty;

        //Profiles list
        public List<Profile> profileList;

        //Song list
        public List<String> songList;

        public int currentScore;//

        private ImageBrush main, profile, leaderboards, selectsong, game, score;

        //Constructor
        public DisplayEngine(Canvas mainScreen)
        {
            GameScreen = mainScreen;
            CurrentScreen = Screen.Main;
            grid = new Rectangle[9];

            texts = new TextBox[4];

            //Divide the screen into 3 parts
            columnSpace = GameScreen.Width / 3.0;
            rowSpace = GameScreen.Height / 3.0;

            currentScore = 0;

            active = true;
            count = new Timer(1200);
            count.Elapsed += delegate
            {

                active = true;

            };


            main = new ImageBrush();
            main.ImageSource = new BitmapImage(
                    new Uri("images\\screen_main.png", UriKind.Relative));

            profile = new ImageBrush();
            profile.ImageSource = new BitmapImage(
                    new Uri("images\\screen_profile.png", UriKind.Relative));

            leaderboards = new ImageBrush();
            leaderboards.ImageSource = new BitmapImage(
                    new Uri("images\\screen_leaderboards.png", UriKind.Relative));

            selectsong = new ImageBrush();
            selectsong.ImageSource = new BitmapImage(
                    new Uri("images\\screen_selectsong.png", UriKind.Relative));

            game = new ImageBrush();
            game.ImageSource = new BitmapImage(
                    new Uri("images\\screen_game.png", UriKind.Relative));

            score = new ImageBrush();
            score.ImageSource = new BitmapImage(
                    new Uri("images\\screen_score.png", UriKind.Relative));

        }

        //!Calculate grid position
        public void GenerateGrid()
        {
            int x = 0, y = 0;

            //Put the grid
            for (int c = 0; c < 9; c++)
            {
                grid[c] = new Rectangle();
                grid[c].Width = columnSpace;
                grid[c].Height = rowSpace;

                grid[c].RenderTransform = new TranslateTransform(x * columnSpace, y * rowSpace);
                grid[c].Opacity = 0.0;

                GameScreen.Children.Add(grid[c]);

                x++;

                x = x % 3;

                if (x == 0)
                {
                    y++;
                    y = y % 3;
                }
            }

            for (int c = 0; c < 4; c++)
            {
                texts[c] = new TextBox();

                texts[c].Background = null;
                texts[c].BorderBrush = null;
                texts[c].Foreground = new RadialGradientBrush(Colors.Red, Colors.DarkRed);

                texts[c].TextAlignment = System.Windows.TextAlignment.Center;
                texts[c].Width = columnSpace;
            }


        }


        public TextBox drawWithLabel(TextBox t)
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

            GameScreen.Children.Add(border);
            GameScreen.Children.Add(t);

            return border;
        }

        public Screen clap(int hand, int leftFoot, int rightFoot)
        {
            updateScreen(hand, hand, leftFoot, rightFoot);
            return CurrentScreen;
        }

        public int CompareProfileScore(Profile p1, Profile p2)
        {
            return p2.highScore.CompareTo(p1.highScore);
        }


        public void changeScreen(Screen screen, int shift)
        {

            GameScreen.Children.Clear();
            CurrentScreen = screen;
            active = false;
            count.Start();


            if (screen == Screen.Profile)
            {
                for (int c = 0; c < 4; c++)
                {
                    texts[c].FontFamily = new FontFamily("Jokerman");
                }

                texts[0].Width = columnSpace;
                texts[0].FontSize = 90;
                texts[1].FontSize = 60;
                texts[2].FontSize = 60;
                texts[3].FontSize = 48;

                texts[0].RenderTransform = new TranslateTransform(columnSpace, 150);
                texts[1].RenderTransform = new TranslateTransform(0, 130);
                texts[2].RenderTransform = new TranslateTransform(columnSpace * 2, 130);
                texts[3].RenderTransform = new TranslateTransform(columnSpace, 110);

                if (shift == 0)
                {
                    //nothing
                }
                else if (shift == 1)
                {
                    Profile p = profileList.ElementAt(0);
                    profileList.Remove(p);
                    profileList.Add(p);
                }
                else if (shift == 2)
                {
                    Profile p = profileList.ElementAt(profileList.Count - 1);
                    profileList.Remove(p);
                    profileList.Insert(0, p);
                }

                texts[0].Text = profileList.ElementAt(0).name;
                texts[1].Text = profileList.ElementAt(1).name;
                texts[2].Text = profileList.ElementAt(2).name;
                texts[3].Text = "Player";

                for (int c = 0; c < 4; c++)
                    drawWithLabel(texts[c]);
            }
            else if (screen == Screen.SelectSong)
            {
                for (int c = 0; c < 4; c++)
                {
                    texts[c].FontFamily = new FontFamily("Jokerman");
                }

                texts[0].Width = columnSpace * 2;
                texts[0].FontSize = 90;
                texts[1].FontSize = 60;
                texts[2].FontSize = 60;
                texts[3].FontSize = 48;

                texts[0].RenderTransform = new TranslateTransform(columnSpace * 0.5, 150);
                texts[1].RenderTransform = new TranslateTransform(0, 130);
                texts[2].RenderTransform = new TranslateTransform(columnSpace * 2, 130);
                texts[3].RenderTransform = new TranslateTransform(columnSpace, 110);

                if (shift == 0 || shift == 3)
                {

                }
                else if (shift == 1)
                {
                    String s = songList.ElementAt(0);
                    songList.Remove(s);
                    songList.Add(s);


                }
                else if (shift == 2)
                {
                    String s = songList.ElementAt(songList.Count - 1);
                    songList.Remove(s);
                    songList.Insert(0, s);

                }

                if (shift == 3)
                {
                    if (selectedDifficulty == Difficulty.Easy)
                    {
                        selectedDifficulty = Difficulty.Medium;
                        texts[3].Text = "Medium";
                    }
                    else if (selectedDifficulty == Difficulty.Medium)
                    {
                        selectedDifficulty = Difficulty.Hard;
                        texts[3].Text = "Hard";
                    }
                    else if (selectedDifficulty == Difficulty.Hard)
                    {
                        selectedDifficulty = Difficulty.Easy;
                        texts[3].Text = "Easy";
                    }
                }
                else
                {
                    texts[3].Text = "Easy";
                }

                texts[0].Text = songList.ElementAt(0);
                texts[1].Text = songList.ElementAt(1);
                texts[2].Text = songList.ElementAt(2);

                for (int c = 0; c < 4; c++)
                    drawWithLabel(texts[c]);
            }
            else if (screen == Screen.Leaderboards)
            {
                for (int c = 0; c < 4; c++)
                {
                    texts[c].FontFamily = new FontFamily("MV Boli");
                    texts[c].FontSize = 54;
                }

                texts[0].Width = columnSpace;
                texts[0].RenderTransform = new TranslateTransform(0, 240);
                texts[1].RenderTransform = new TranslateTransform(columnSpace, 240);
                texts[2].RenderTransform = new TranslateTransform(columnSpace * 2, 240);

                //profileList.Sort();
                List<Profile> sortedList = profileList;
                sortedList.Sort(CompareProfileScore);

                texts[0].Text = "1 " + sortedList.ElementAt(0).name + "\n2 " + sortedList.ElementAt(1).name + "\n3 " + sortedList.ElementAt(2).name;
                texts[1].Text = sortedList.ElementAt(0).title +"\n" + sortedList.ElementAt(1).title + "\n" + sortedList.ElementAt(2).title;
                texts[2].Text = sortedList.ElementAt(0).highScore + "\n" + sortedList.ElementAt(1).highScore + "\n" + sortedList.ElementAt(2).highScore;
                texts[3].Text = "";

                for (int c = 0; c < 4; c++)
                    drawWithLabel(texts[c]);
            }
            else if (screen == Screen.Score)
            {
                for (int c = 0; c < 4; c++)
                {
                    texts[c].FontFamily = new FontFamily("MV Boli");
                    texts[c].FontSize = 60;
                }
                texts[0].Width = columnSpace;
                texts[0].FontFamily = new FontFamily("Jokerman");
                texts[0].FontSize = 216;

                texts[0].RenderTransform = new TranslateTransform(60, 180);
                texts[1].RenderTransform = new TranslateTransform(columnSpace, 240);
                texts[2].RenderTransform = new TranslateTransform(2 * columnSpace, 240);

                //int score = getTotalScore();
                int score = currentScore;

                if (score >= 4000)
                    texts[0].Text = "A";
                else if (score >= 2000)
                    texts[0].Text = "B";
                else if (score >= 1000)
                    texts[0].Text = "C";
                else
                    texts[0].Text = "F";

                //texts[1].Text = "Minho\nSong A";
                texts[1].Text = profileList.ElementAt(0).name +"\n" + songList.ElementAt(0);
                texts[2].Text = "Score\n" + score;

                for (int c = 0; c < 4; c++)
                    drawWithLabel(texts[c]);
            }
            else
            {
                //nothing
            }
        }

        //Update the screen
        public void updateScreen(int leftHand, int rightHand, int leftFoot, int rightFoot)
        {
            /*
            grid[leftHand].Opacity = 0.7;
            grid[leftHand].Fill = Brushes.Green;
            grid[rightHand].Opacity = 0.7;
            grid[rightHand].Fill = Brushes.Gold;
             * */

            if (active)
            {

                switch (CurrentScreen)
                {
                    case Screen.Main:
                        if (leftHand == 4)
                        {
                            //go to profile
                            changeScreen(Screen.Profile, 0);
                        }
                        break;
                    case Screen.Profile:
                        if (leftHand == 3)
                        {
                            //move profile left
                            changeScreen(Screen.Profile, 1);
                        }
                        if (leftHand == 4)
                        {
                            changeScreen(Screen.Leaderboards, 0);
                        }
                        if (rightHand == 5)
                        {
                            //move profile right
                            changeScreen(Screen.Profile, 2);
                        }
                        if (leftHand == 7)
                        {
                            changeScreen(Screen.SelectSong, 0);
                        }
                        if (leftHand == 8)
                        {
                            changeScreen(Screen.Main, 0);
                        }
                        break;
                    case Screen.Leaderboards:
                        if (leftHand == 8)
                        {
                            changeScreen(Screen.Profile, 0);
                        }
                        break;
                    case Screen.SelectSong:
                        if (leftHand == 3)
                        {
                            //move song left
                            changeScreen(Screen.SelectSong, 1);
                        }
                        if (leftHand == 4)
                        {
                            //change difficulty
                            changeScreen(Screen.SelectSong, 3);
                        }
                        if (rightHand == 5)
                        {
                            //move song right
                            changeScreen(Screen.SelectSong, 2);
                        }
                        if (leftHand == 7)
                        {
                            changeScreen(Screen.Game, 0);
                        }
                        if (leftHand == 8)
                        {
                            changeScreen(Screen.Profile, 0);
                        }
                        break;
                    case Screen.Score:
                        if (leftHand == 8)
                        {
                            changeScreen(Screen.SelectSong, 0);
                        }
                        break;
                    default:
                        break;
                }

            }

            Display();
        }

        //Update the display
        public void Display()
        {
            if (CurrentScreen == Screen.Main)
            {
                GameScreen.Background = main;
            }
            else if (CurrentScreen == Screen.Profile)
            {
                GameScreen.Background = profile;
            }
            else if (CurrentScreen == Screen.Leaderboards)
            {
                GameScreen.Background = leaderboards;
            }
            else if (CurrentScreen == Screen.Game)
            {
                GameScreen.Background = game;
            }
            else if (CurrentScreen == Screen.Score)
            {
                GameScreen.Background = score;
            }
            else if (CurrentScreen == Screen.SelectSong)
            {
                GameScreen.Background = selectsong;
            }
            else
            {
                //this isn't good
            }
        }


        //Game over
        public void endOfGameDisplay()
        {
            changeScreen(Screen.Score, 0);
        }

    }
}