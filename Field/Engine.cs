﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Media;
using System.Windows.Media;
using System.IO;
using System.Text;
using TeamWork.Objects;
using System.Linq;

namespace TeamWork.Field
{
    public class Engine
    {
        public static Random rnd = new Random();
        public bool drawMenu = false;
        public Thread musicThread;
        public Thread EffectsThread;

        public const int WindowWidth = 80; //Window Width constant to be accesed from everywhere
        public const int WindowHeight = 32; //Window height constant to be accesed from everywhere

        public Engine()
        {
            this.Start();
        }
        public void Start()
        {
            Menu.StartMenu();
            musicThread = new Thread(Engine.LoadMusic);
            musicThread.Start();
            EffectsThread = new Thread(SoundEffects);
            EffectsThread.Start();
            Menu.EntryStoryLine();
            Printing.EnterName();
            TakeName();
            Thread.Sleep(1000);
            while (true)
            {
                Console.Clear();
                Printing.Player.Print();
                Interface.Table();
                Interface.UIDescription();

                while (Printing.Player.Lives > 0)
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
                    Thread.Sleep(80);
                }
                Console.Clear();
                SetHighscore();
                Printing.GameOver();
                ResetGame();
            }
        }

        public static bool BossActive = false;
        public static Boss boss = new Boss(0);
        private void UpdateAndRender()
        {
            if (Printing.Player.Level == 2 && BossActive == false)
            {
                BossActive = true;
                
                if (boss.bossLife <= 0)
                {
                    boss = new Boss(0);
                }
            }
            ProjectileMoveAndPrint();
            ProjectileCollisionCheck();
            if (BossActive)
            {
                DrawAndMoveMeteor();
                boss.BossAI();
                foreach (var bullets in _bullets)
                {
                    if (boss.BossHit(bullets.Point))
                    {
                        bullets.Point.X += 100;
                    }
                }
            }
            else
            {
                DrawAndMoveMeteor();
                GenerateMeteorit();
            }
        }

        private void ResetGame()
        {
            Printing.Player.Level = 1;
            Printing.Player.Score = 0;
            Printing.Player.Lives = 3;
            Printing.Player.Point = Printing.PlayerPoint;
            BossActive = false;
            boss = new Boss(0);
            _bullets.Clear();
            _meteorits.Clear();
        }

        /// <summary>
        /// Player move handler
        /// </summary>
        /// <param name="keyPressed"></param>
        private void TakeInput(ConsoleKeyInfo keyPressed)
        {
            switch (keyPressed.Key)
            {
                case ConsoleKey.W: Printing.Player.MoveUp();
                    break;
                case ConsoleKey.S: Printing.Player.MoveDown();
                    break;
                case ConsoleKey.A: Printing.Player.MoveLeft();
                    break;
                case ConsoleKey.D: Printing.Player.MoveRight();
                    break;
                // Create a new bullet object
                case ConsoleKey.Spacebar:
                    _bullets.Add(new GameObject(new Point2D(Printing.Player.Point.X + 20, Printing.Player.Point.Y + 1),0));
                    playEffect = true;
                    break;
            }
        }

        #region Projectiles

        public static List<GameObject> _objectProjectiles = new List<GameObject>();
        private List<GameObject> _bullets = new List<GameObject>(); // Stores all bullets fired
        private void ProjectileMoveAndPrint()
        {
            List<GameObject> newProjectiles = new List<GameObject>();
            List<GameObject> newBullets = new List<GameObject>(); //Stores the new coordinates of the bullets
            for (int i = 0; i < _objectProjectiles.Count; i++)
            {
                if (_objectProjectiles[i].Point.X >= 0)
                {
                    _objectProjectiles[i].ClearObject();
                }

                if (_objectProjectiles[i].Point.X - _objectProjectiles[i].Speed - 2 <= 0)
                {
                    // If the Projectile exceeds sceen size, dont add it to new Projectiles list
                }
                else
                {
                    _objectProjectiles[i].Point.X -= _objectProjectiles[i].Speed + 2;
                    _objectProjectiles[i].PrintObject();
                    newProjectiles.Add((_objectProjectiles[i]));
                }
            }

            Printing.DrawAt(Printing.Player.Point.X + 20, Printing.Player.Point.Y + 1, '=', ConsoleColor.DarkCyan); // Fire effect lol
            
            for (int i = 0; i < _bullets.Count; i++) // Cycle through all bullets and change their position
            {
                if (_bullets[i].Point.X <= Engine.WindowWidth)
                {
                    _bullets[i].ClearObject();
                }
                // Clear bullet at its current position
                if (_bullets[i].Point.X + _bullets[i].Speed + 1 >= Engine.WindowWidth)
                {
                    // If the bullet exceeds sceen size, dont add it to new Bullets list
                }
                else
                {
                    _bullets[i].Point.X += _bullets[i].Speed + 1;
                    _bullets[i].PrintObject(); // Print the bullets at their new position;
                    newBullets.Add((_bullets[i]));
                }
            }
            _objectProjectiles = newProjectiles;
            _bullets = newBullets; // Overwrite global bullets list, with newBullets list
        }
       
        #endregion
        
        #region Object Generator

        /// <summary>
        /// Generate meteorObjects
        /// </summary>
        private List<GameObject> _meteorits = new List<GameObject>();
        private int counter = 0; // Just a counter
        public int chance = 40; // Chance variable 1 per # loops spawn a meteor
        private void GenerateMeteorit()
        {
            if (counter % chance == 0)
            {
                _meteorits.Add(new GameObject(rnd.Next(1, 7)));
                counter++;
            }
            else
            {
                counter++;
            }
        }

        /// <summary>
        /// Print and move meteorites
        /// </summary>
        private void DrawAndMoveMeteor()
        {
            List<GameObject> newMeteorits = new List<GameObject>();
            if (counter % 1 == 0)
            {
                for (int i = 0; i < _meteorits.Count; i++)
                {
                    _meteorits[i].ClearObject();
                    if (_meteorits[i].Point.X - _meteorits[i].Speed <= 1)
                    {
                        // If the meteorit exceeds sceen size, dont add it to new meteorit list
                    }
                    else
                    {
                        // Collision handling
                        if (BulletCollision(_meteorits[i]) || ShipCollision(_meteorits[i])) // Bullet and ship collision check
                        {
                           
                            if (--_meteorits[i].life == 0)
                            {
                                _meteorits[i].ClearObject();
                                playMeteorEffect = true;
                                _meteorits[i].GotHit = true;
                            }
                            
                            newMeteorits.Add((_meteorits[i]));
                            
                        }
                        else
                        {
                            _meteorits[i].MoveObject();
                            if (!_meteorits[i].toBeDeleted)
                            {
                                _meteorits[i].PrintObject();
                                newMeteorits.Add((_meteorits[i]));
                            }
                        }
                    }
                }
                _meteorits = newMeteorits;

            }
        }
        #endregion

        #region Collision Handling Methods
        /// <summary>
        /// Bullet collision check
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>If any bullet hits a meteorite</returns>
        private bool BulletCollision(GameObject obj)
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (obj.Collided(_bullets[i].Point))
                {
                    Printing.ClearAtPosition(_bullets[i].Point);
                    _bullets.RemoveAt(i);
                    
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Ship collision handling
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>If ship was struck by meteorite</returns>
        private bool ShipCollision(GameObject obj)
        {
            Point2D point = Printing.Player.Point;
            if (obj.Collided(point.X + 21, point.Y) || obj.Collided(point.X + 21, point.Y + 1) || // Front collision
                obj.Collided(point.X + 18, point.Y) || obj.Collided(point.X + 15, point.Y) || // Top collision
                obj.Collided(point.X + 11, point.Y) || obj.Collided(point.X + 6, point.Y) ||  // Top collision
                obj.Collided(point.X + 18, point.Y + 1) || obj.Collided(point.X + 15, point.Y + 1) ||// Bottom collision
                obj.Collided(point.X + 11, point.Y + 1) || obj.Collided(point.X + 6, point.Y + 1) || // Bottom collision
                obj.Collided(point.X + 3, point.Y - 1) || obj.Collided(point.X + 3, point.Y + 1)) // Tail collision
            {
                Printing.Player.DecreaseLives();

                Interface.Table();
                Interface.UIDescription();
                return true;
            }
            return false;
        }

        private void ProjectileCollisionCheck()
        {
            var hits =
                _objectProjectiles.Select((x, i) => new { Value = x, Index = i })
                    .Where(x => Printing.Player.ShipCollided(x.Value.Point)).ToList();

            foreach (var hit in hits)
            {
                hit.Value.ClearObject();
                _objectProjectiles.RemoveAt(hit.Index);
                Printing.Player.Lives--;
                Interface.Table();
                Interface.UIDescription();

            }
        }
        #endregion

        #region Highscore and Score Methods
        //Checks if the oldHighScore and the CurrentHighScore are different, and sets the higher value as the new HighScore
        //Also adds all scores to the Scores.txt file
        private static void SetHighscore()
        {
            string highscore = string.Format("Player {0}, Highscore {1}, Time Achieved: {2} / {3} / {4}",
                Printing.Player.Name, Printing.Player.Score, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);

            string[] oldText = File.ReadAllText("Resources/Highscore.txt").Split();

            string oldHighScore = oldText[3].Remove(oldText[3].Length - 1);
            int oldHighScoreToInt = int.Parse(oldHighScore);

            if (oldHighScoreToInt < Printing.Player.Score)
                File.WriteAllText("Resources/Highscore.txt", highscore);

            string currentScores = File.ReadAllText("Resources/Scores.txt");
            highscore = string.Format("Player {0}, Score {1}, Time Achieved: {2} / {3} / {4}",
                Printing.Player.Name, Printing.Player.Score, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
            currentScores += "#" + highscore + @"
";
            File.WriteAllText("Scores.txt", currentScores);
        }
        public static void PrintHighscore()
        {
            string currentHighscore = File.ReadAllText("Resources/Highscore.txt");
            Printing.DrawAt(new Point2D(15, 14), "Current Highscore: ", ConsoleColor.Green);
            Printing.DrawAt(new Point2D(15, 15), currentHighscore, ConsoleColor.Green);
            Printing.DrawAt(new Point2D(15, 17), "Last Achieved Scores: ", ConsoleColor.Green);

            string[] currentScores = File.ReadAllLines("Resources/Scores.txt");
            int y = 15;
            int counter = 0;
            for (int i = currentScores.Length - 1; i >= currentScores.Length - 10; i--)
            {
                y++;
                counter++;
                Printing.DrawAt(new Point2D(15, y), counter + " " + currentScores[i], ConsoleColor.Green);
            }
        }
        #endregion

        #region Music
        private static bool playMeteorEffect;
        private static bool playEffect;
        private static void LoadMusic()
        {
            var sound = new SoundPlayer();
            sound.SoundLocation = "Resources/STARS.wav";
            sound.PlayLooping();

        }

        private void SoundEffects()
        {

            MediaPlayer soundFX = new MediaPlayer();
            MediaPlayer soundFX2 = new MediaPlayer();

            while (true)
            {
                if (playMeteorEffect)
                {
                    soundFX.Open(new Uri("Resources/meteor.wav", UriKind.Relative));

                    soundFX.Volume = 60;
                    soundFX.Play();
                    playMeteorEffect = false;
                }
                if (playEffect)
                {
                    soundFX2.Open(new Uri("Resources/laser.wav", UriKind.Relative));
                    soundFX2.Volume = 400;
                    soundFX2.Play();
                    playEffect = false;
                }
                Thread.Sleep(5);
            }
            
        }

        #endregion

        private void TakeName()
        {
            Console.WriteLine();
            Console.Write("\n\t\t\t\t Name:");
            string name = Console.ReadLine();
            if (String.IsNullOrEmpty(name) || name.Length >= 10)
            {
                Console.WriteLine("\t\t\t    Please enter your name! Name must also be less than/or 10 symbols");
                Thread.Sleep(2000);
                Console.Clear();
                //Printing.UserName();
                TakeName();
            }
            else
            {
                Printing.Player.setName(name);
                Console.Clear();               
            }
        }

        /// <summary>
        /// Initialize Console size;
        /// </summary>
        public static void InitConsole()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;
            Console.WindowWidth = WindowWidth;
            Console.BufferWidth = WindowWidth;
            Console.WindowHeight = WindowHeight;
            Console.BufferHeight = WindowHeight;
        }
    }
}
