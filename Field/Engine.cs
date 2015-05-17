﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using TeamWork.Field;
using System.Media;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Security.AccessControl;

namespace TeamWork
{
    public class Engine
    {
        public static Random rnd = new Random();
        //public static event MoveHandler Move;
        public Thread musicThread;

        //public static void OnEventMove(MoveArgs moveArgs)
        //{
        //    var handler = Move;
        //    if (Move != null)
        //        Move(null, moveArgs);
        //}

        public static Player player = new Player();

        public const int WindowWidth = 80; //Window Width constant to be accesed from everywhere
        public const int WindowHeight = 32; //Window height constant to be accesed from everywhere

        //TODO: Implement Engine Class!
        public Engine()
        {
            this.Start();
        }
        public void Start()
        {
            //musicThread = new Thread(Engine.LoadMusic);
            //musicThread.Start();

            while (player.Lives != 0)
            {
                GameIntro();
                Console.Clear();
                player.Print();
                //Drawing.DrawField();
                Interface.Table();
                Interface.UIDescription();

                while (player.Lives != 0)
                {

                    if (Console.KeyAvailable)
                    {
                        this.TakeInput(Console.ReadKey(true));
                        while (Console.KeyAvailable)
                        {
                            Console.ReadKey(true); // Seems to clear the buffer of keys
                        }
                    }

                    UpdateAndRender();
                    if (player.Lives < 1)
                    {
                        End();
                        break;
                    }

                    Thread.Sleep(100);
                }
                if (player.Lives < 1)
                {
                    End();
                    break;
                }
                this.End();
                break;
            }
        }

        private void UpdateAndRender()
        {
            DrawAndMoveMeteor();
            MoveAndPrintBullets();
            GenerateMeteorit();
        }

        private void End()
        {
            Drawing.GameOver();
            Thread.Sleep(2500);
            Console.Clear();
            Drawing.Credits();
        }

        private void GameIntro()
        {
            Drawing.WelcomeScreen();
            Thread.Sleep(3500);
            Console.Clear();
            Drawing.GameName();
            Thread.Sleep(2500);
            Console.Clear();
            Drawing.LetsPlay();
            Thread.Sleep(2500);
            Console.Clear();
            Drawing.UserName();
            this.TakeName();
        }

        private void TakeInput(ConsoleKeyInfo keyPressed)
        {
            switch (keyPressed.Key)
            {
                case ConsoleKey.W: player.MoveUp();
                    break;
                case ConsoleKey.S: player.MoveDown();
                    break;
                case ConsoleKey.A: player.MoveLeft();
                    break;
                case ConsoleKey.D: player.MoveRight();
                    break;
                // Create a new bullet object
                case ConsoleKey.Spacebar: _bullets.Add(new GameObject(new Point2D(player.Point.X + 22, player.Point.Y + 1)));
                    break;
            }
        }

        #region Player Bullets

        private List<GameObject> _bullets = new List<GameObject>(); // Stores all bullets fired
        /// <summary>
        /// Print and move the bullets
        /// </summary>
        private void MoveAndPrintBullets()
        {
            List<GameObject> newBullets = new List<GameObject>(); //Stores the new coordinates of the bullets

            for (int i = 0; i < _bullets.Count; i++) // Cycle through all bullets and change their position
            {
                Drawing.ClearAtPosition(_bullets[i].Point); // Clear bullet at its current position
                if (_bullets[i].Point.X + _bullets[i].Speed + 2 >= Engine.WindowWidth)
                {
                    // If the bullet exceeds sceen size, dont add it to new Bullets list
                }
                else
                {
                    _bullets[i].Point.X += _bullets[i].Speed + 1;
                    Drawing.DrawAt(_bullets[i].Point, ".", ConsoleColor.Cyan); // Print the bullets at their new position;
                    newBullets.Add((_bullets[i]));
                }
            }
            _bullets = newBullets; // Overwrite global bullets list, with newBullets list
        }

        #endregion

        #region Object Generator
        private List<GameObject> _meteorits = new List<GameObject>();
        private int counter = 0; // Just a counter
        public int chance = 24; // Chance variable 1 per 25 loops spawn a meteor
        private void GenerateMeteorit()
        {
            if (counter % chance == 0)
            {
                _meteorits.Add(new GameObject(new Point2D(WindowWidth - 3, rnd.Next(5, WindowHeight - 5))));
                counter++;
            }
            else
            {
                counter++;
            }

        }

        private void DrawAndMoveMeteor()
        {
            List<GameObject> newMeteorits = new List<GameObject>();
            if (counter % 1 == 0)
            {

                for (int i = 0; i < _meteorits.Count; i++)
                {

                    Drawing.ClearAtPosition(_meteorits[i].Point); // Clear meteorit at its current position
                    Drawing.ClearAtPosition(_meteorits[i].Point.X, _meteorits[i].Point.Y + 1);
                    Drawing.ClearAtPosition(_meteorits[i].Point.X, _meteorits[i].Point.Y - 1);

                    Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y);
                    Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y + 1);
                    Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y - 1);

                    Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y);
                    Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y + 1);
                    Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y - 1);
                    if (_meteorits[i].Point.X - _meteorits[i].Speed <= 1)
                    {
                        // If the meteorit exceeds sceen size, dont add it to new meteorit list
                    }
                    else
                    {
                        if (BulletCollision(_meteorits[i].Point) ||
                            BulletCollision(new Point2D(_meteorits[i].Point.X, _meteorits[i].Point.Y)) ||
                            BulletCollision(new Point2D(_meteorits[i].Point.X, _meteorits[i].Point.Y + 1)) ||
                            BulletCollision(new Point2D(_meteorits[i].Point.X, _meteorits[i].Point.Y - 1)) ||

                            BulletCollision(new Point2D(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y)) ||
                            BulletCollision(new Point2D(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y + 1)) ||
                            BulletCollision(new Point2D(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y - 1)) ||

                            BulletCollision(new Point2D(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y)) ||
                            BulletCollision(new Point2D(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y + 1)) ||
                            BulletCollision(new Point2D(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y - 1)))
                        {
                            Drawing.ClearAtPosition(_meteorits[i].Point);
                            Drawing.ClearAtPosition(_meteorits[i].Point.X, _meteorits[i].Point.Y + 1);
                            Drawing.ClearAtPosition(_meteorits[i].Point.X, _meteorits[i].Point.Y - 1);

                            Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y);
                            Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y + 1);
                            Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y - 1);

                            Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y);
                            Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y + 1);
                            Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y - 1);


                        }
                        else
                        {
                            if (MeteoriteCollision(_meteorits[i].Point) ||
                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X, _meteorits[i].Point.Y)) ||
                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X, _meteorits[i].Point.Y + 1)) ||
                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X, _meteorits[i].Point.Y - 1)) ||

                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y)) ||
                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y + 1)) ||
                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y - 1)) ||

                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y)) ||
                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y + 1)) ||
                            MeteoriteCollision(new Point2D(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y - 1)))
                            {
                                Drawing.ClearAtPosition(_meteorits[i].Point);
                                Drawing.ClearAtPosition(_meteorits[i].Point.X, _meteorits[i].Point.Y + 1);
                                Drawing.ClearAtPosition(_meteorits[i].Point.X, _meteorits[i].Point.Y - 1);

                                Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y);
                                Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y + 1);
                                Drawing.ClearAtPosition(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y - 1);

                                Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y);
                                Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y + 1);
                                Drawing.ClearAtPosition(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y - 1);
                            }
                            else
                            {
                                _meteorits[i].Point.X -= _meteorits[i].Speed;
                                Drawing.DrawAt(_meteorits[i].Point.X, _meteorits[i].Point.Y, '|', ConsoleColor.Green);
                                Drawing.DrawAt(_meteorits[i].Point.X, _meteorits[i].Point.Y + 1, '|', ConsoleColor.Green);
                                Drawing.DrawAt(_meteorits[i].Point.X, _meteorits[i].Point.Y - 1, '|', ConsoleColor.Green);

                                Drawing.DrawAt(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y, '|', ConsoleColor.Yellow);
                                Drawing.DrawAt(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y + 1, '|', ConsoleColor.Yellow);
                                Drawing.DrawAt(_meteorits[i].Point.X + 1, _meteorits[i].Point.Y - 1, '|', ConsoleColor.Yellow);

                                Drawing.DrawAt(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y, '|', ConsoleColor.Red);
                                Drawing.DrawAt(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y + 1, '|', ConsoleColor.Red);
                                Drawing.DrawAt(_meteorits[i].Point.X + 2, _meteorits[i].Point.Y - 1, '|', ConsoleColor.Red);
                                newMeteorits.Add((_meteorits[i]));
                            }
                        }

                    }
                }
                _meteorits = newMeteorits;

            }
        }
        private bool BulletCollision(Point2D point)
        {
            if (_bullets.Any(bullet => point == bullet.Point))
            {
                Point2D currentBulletPoint = _bullets.FirstOrDefault(bullet => point == bullet.Point).Point;
                _bullets.Remove(_bullets.FirstOrDefault(bullet => point == bullet.Point));
                Drawing.ClearAtPosition(currentBulletPoint.X, currentBulletPoint.Y);
                Drawing.Player.IncreasePoints();
                Interface.Table();
                Interface.UIDescription();
                return true;
            }
            return false;
        }
        private bool MeteoriteCollision(Point2D point)
        {
            if (player.Point.X + 22 == point.X && (player.Point.Y == point.Y || player.Point.Y == point.Y - 1 || player.Point.Y == point.Y + 1))
            {
                Drawing.Player.DecreaseLives();
                Interface.Table();
                Interface.UIDescription();
                return true;
            }
            return false;
        }
        #endregion

        /*public static void LoadMusic()
        {
            var sound = new System.Media.SoundPlayer();
            sound.SoundLocation = "STARS.wav";
            sound.PlaySync();
        }*/

        private void TakeName()
        {
            Console.WriteLine();
            Console.Write("\n\t\t\t\tName:");
            string name = Console.ReadLine();
            if (String.IsNullOrEmpty(name))
            {
                Console.WriteLine("\t\t\t    Please entry your name");
                Thread.Sleep(2000);
                Console.Clear();
                Drawing.UserName();
                TakeName();
            }
            else
            {
                Drawing.Player.setName(name);
                Console.Clear();
                //musicThread.Interrupt();
            }
        }

        /// <summary>
        /// Initialize Console size;
        /// </summary>
        public static void InitConsole()
        {
            Console.CursorVisible = false;
            Console.WindowWidth = WindowWidth;
            Console.BufferWidth = WindowWidth;
            Console.WindowHeight = WindowHeight;
            Console.BufferHeight = WindowHeight;
        }
    }
}
