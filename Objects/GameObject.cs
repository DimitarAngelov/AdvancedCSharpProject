﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using TeamWork.Field;

namespace TeamWork.Objects
{
    public class GameObject : Entity
    {
        public enum ObjectType
        {
            Bullet,
            Normal,
            Small,
            Silver,
            Gold,
            Lenghty,
            Quadcopter
        }

        private ObjectType objectType;
        public int life;
        public GameObject()
        {
            base.Speed = 1;

        }

        public GameObject(Point2D point)
            : base(point)
        {
            base.Speed = 1;
            base.Point = point;
            objectType = 0;
        }

        public GameObject(Point2D point, int type)
            : base(point)
        {
            base.Speed = 1;
            base.Point = point;
            objectType = (ObjectType)type;
        }

        public GameObject(int type)
        {
            base.Speed = 1;
            objectType = (ObjectType)type;
            switch (objectType)
            {
                case ObjectType.Normal:
                    life = 2;
                    base.Point = new Point2D(Engine.WindowWidth - 2, Engine.rnd.Next(6, Engine.WindowHeight - 3));
                    break;
                case ObjectType.Small:
                    life = 1;
                    base.Point = new Point2D(Engine.WindowWidth - 1, Engine.rnd.Next(4, Engine.WindowHeight - 2));
                    break;
                case ObjectType.Silver:
                    life = 4;
                    base.Point = new Point2D(Engine.WindowWidth - 3, Engine.rnd.Next(3, Engine.WindowHeight - 4));
                    break;
                case ObjectType.Gold:
                    life = 3;
                    base.Point = new Point2D(Engine.WindowWidth - 3, Engine.rnd.Next(3, Engine.WindowHeight - 4));
                    break;
                case ObjectType.Lenghty:
                    life = 3;
                    base.Point = new Point2D(Engine.WindowWidth - 3, Engine.rnd.Next(4, Engine.WindowHeight - 3));
                    break;
                case ObjectType.Quadcopter:
                    life = 7;
                    base.Point = new Point2D(Engine.WindowWidth - 2, Engine.rnd.Next(6, Engine.WindowHeight - 4));
                    break;
            }
        }

        public bool toBeDeleted;
        private bool Moveable = true;
        public override string ToString()
        {
            return string.Format("Object type:{0}, X:{1}, Y:{2},Moveable:{3}",objectType,Point.X,Point.Y,Moveable);
        }

        /// <summary>
        /// Print GameObject based on its type
        /// </summary>
        private int Frames = 1;
        private Point2D diagonalInc = new Point2D(1,1);
        private Point2D diagonalDec = new Point2D(-1, 1);
        private Point2D upRight;
        private Point2D upLeft;
        private Point2D downLeft;
        private Point2D downRight;
        private int projectileCounter = 1;
        private int projectileChance = Engine.rnd.Next(20, 50);
        public bool GotHit = false;
        public void PrintObject()
        {
            switch (objectType)
            {
                case ObjectType.Bullet:
                    Printing.DrawAt(this.Point, '-', ConsoleColor.DarkCyan);
                    break;
                case ObjectType.Normal:
                    if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point, "/\\", ConsoleColor.Red);
                        Printing.DrawAt(this.Point.X, Point.Y + 1, "\\/", ConsoleColor.Red);
                    }
                    else
                    {
                        upLeft = this.Point - diagonalInc * Frames;
                        upRight = this.Point - diagonalDec * Frames;
                        downRight = this.Point + diagonalInc * Frames;
                        downLeft = this.Point + diagonalDec * Frames;
                        char[] c = { '/', '\\', '\\', '/' };
                        PrintAndClearExplosion(false, c, ConsoleColor.Red);
                    }
                    
                    break;
                case ObjectType.Small:
                   if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point, "<>");
                    }
                    else
                    {
                        upRight = new Point2D(this.Point.X + Frames+1,this.Point.Y);
                        upLeft = new Point2D(this.Point.X - Frames, this.Point.Y);
                        downRight = new Point2D(this.Point.X, this.Point.Y + Frames);
                        downLeft = new Point2D(this.Point.X, this.Point.Y - Frames);
                        char[] c = {'<', '>', '/', '\\'};
                        PrintAndClearExplosion(false,c,ConsoleColor.Gray);
                    }
                    break;
                case ObjectType.Silver:
                    if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point.X, this.Point.Y - 1, "\\ /", ConsoleColor.Gray);
                        Printing.DrawAt(this.Point, " X ", ConsoleColor.Gray);
                        Printing.DrawAt(this.Point.X, this.Point.Y + 1, "/ \\", ConsoleColor.Gray);
                    }
                    else
                    {
                        upLeft = this.Point - diagonalInc * Frames;
                        upRight = this.Point - diagonalDec * Frames;
                        downRight = this.Point + diagonalInc * Frames;
                        downLeft = this.Point + diagonalDec * Frames;
                        Printing.DrawAt(this.Point,'x',ConsoleColor.Gray);
                        char[] c = { '\\', '/', '/', '\\' };
                        PrintAndClearExplosion(false,c,ConsoleColor.Gray);
                    }
                    break;
                case ObjectType.Gold:
                    if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point.X, this.Point.Y - 1, " \u25B2", ConsoleColor.Yellow);
                        Printing.DrawAt(this.Point, "\u25C4\u25A0\u25BA", ConsoleColor.Yellow);
                        Printing.DrawAt(this.Point.X, this.Point.Y + 1, " \u25BC", ConsoleColor.Yellow);
                    }
                    else
                    {
                        upLeft = this.Point - diagonalInc * Frames;
                        upRight = this.Point - diagonalDec * Frames;
                        downRight = this.Point + diagonalInc * Frames;
                        downLeft = this.Point + diagonalDec * Frames;
                        char[] c = {'\u25B2', '\u25BA', '\u25C4', '\u25BC'};
                        PrintAndClearExplosion(false, c, ConsoleColor.Yellow);
                    }
                    break; 
                case ObjectType.Lenghty:
                    if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point, "{\u25A0\u25A0\u25BA");
                    }
                    else
                    {
                        upLeft = this.Point - diagonalInc * Frames;
                        upRight = this.Point - diagonalDec * Frames;
                        downRight = this.Point + diagonalInc * Frames;
                        downLeft = this.Point + diagonalDec * Frames;
                        char[] c = { '{', '\u25BA', '\u25A0', '\u25A0' };
                        PrintAndClearExplosion(false, c);
                    }
                    break;
                case ObjectType.Quadcopter:
                    
                    if (!this.GotHit)
                    {
                        if (projectileCounter % projectileChance == 0)
                        {
                            Engine._objectProjectiles.Add(new GameObject(new Point2D(this.Point.X - 1,this.Point.Y),0));
                            projectileCounter++;
                        }
                        else
                        {
                            projectileCounter++;
                        }

                        #region Quadcopter Entry animation

                        if (this.Point.X + 2 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point, @"<[");
                        }
                        else if (this.Point.X + 3 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X, Point.Y - 1, @"_");
                            Printing.DrawAt(this.Point, @"<[_");
                        }
                        else if (this.Point.X + 4 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\");
                            Printing.DrawAt(this.Point, @"<[__");
                        }
                        else if (this.Point.X + 5 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_");
                            Printing.DrawAt(this.Point, @"<[__\");
                        }
                        else if (this.Point.X + 6 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__ ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_\");
                            Printing.DrawAt(this.Point, @"<[__\_");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"o");
                        }
                        else if (this.Point.X + 7 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__  ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_\_");
                            Printing.DrawAt(this.Point, @"<[__\_\");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"oo");
                        }
                        else if (this.Point.X + 8 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__   ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_\__");
                            Printing.DrawAt(this.Point, @"<[__\_\_");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"oo'");
                        }
                        else if (this.Point.X + 9 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__    ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_\___");
                            Printing.DrawAt(this.Point, @"<[__\_\_-");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"oo'");
                        }
                        else if (this.Point.X + 10 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__     ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_\____");
                            Printing.DrawAt(this.Point, @"<[__\_\_--");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"oo'");
                        }
                        else if (this.Point.X + 11 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__      ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_\_____");
                            Printing.DrawAt(this.Point, @"<[__\_\_---");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"oo'");
                        }
                        else if (this.Point.X + 12 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__       ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_\_____/");
                            Printing.DrawAt(this.Point, @"<[__\_\_----");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"oo'");
                        }
                        else if (this.Point.X + 13 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"__       _");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"_\_\_____/_");
                            Printing.DrawAt(this.Point, @"<[__\_\_-----");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"oo'");
                        }
                        else
                        {
                            Printing.DrawAt(this.Point.X, Point.Y - 2, @"   __       __");
                            Printing.DrawAt(this.Point.X, Point.Y - 1, @"  _\_\_____/_|");
                            Printing.DrawAt(this.Point, @"<[__\_\_-----<");
                            Printing.DrawAt(this.Point.X, Point.Y + 1, @"     oo'");
                        } 
                        #endregion
                    }
                    else
                    {
                        upLeft = this.Point - diagonalInc * Frames;
                        upRight = this.Point - diagonalDec * Frames;
                        downRight = this.Point + diagonalInc * Frames;
                        downLeft = this.Point + diagonalDec * Frames;
                        PrintAndClearExplosion(false);
                    }
                    break;
    //  __       __
    //| __\_\_____/_|
    //<[___\_\_-----<
    //|   oo'
            }
        }

       
        /// <summary>
        /// Clear GameObject based on its type
        /// </summary>
        public void ClearObject()
        {
            switch (objectType)
            {
                case ObjectType.Bullet:
                    Printing.DrawAt(this.Point.X, this.Point.Y, ' ');
                    break;
                case ObjectType.Normal:
                   #region Normal object clearing and breaking effect
		            if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point, "  ");
                        Printing.DrawAt(this.Point.X, Point.Y + 1, "  ");
                    }
                    else
                    {
                        upRight = this.Point - diagonalDec * Frames;
                        upLeft = this.Point + diagonalDec * Frames;
                        downLeft = this.Point - diagonalInc * Frames;
                        downRight = this.Point + diagonalInc * Frames;
                        Moveable = false;
                        PrintAndClearExplosion(true);                        
                        if (Frames == 5)
                        {
                            this.toBeDeleted = true;
                            Printing.Player.IncreasePoints(2);

                            Interface.Table();
                            Interface.UIDescription();
                        }
                        Frames++;
                    }  
	                #endregion
                    break;
                case ObjectType.Small:
                    #region Small object clearing and breaking effect
		            if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point, "   ");
                    }
                    else
                    {
                        upRight = new Point2D(this.Point.X + Frames + 1, this.Point.Y);
                        upLeft = new Point2D(this.Point.X - Frames, this.Point.Y);
                        downRight = new Point2D(this.Point.X, this.Point.Y + Frames);
                        downLeft = new Point2D(this.Point.X, this.Point.Y - Frames);
                        Moveable = false;
                        PrintAndClearExplosion(true);
                        if (Frames == 5)
                        {
                            toBeDeleted = true;
                            Printing.Player.IncreasePoints(1);

                            Interface.Table();
                            Interface.UIDescription();
                        }
                        Frames++;
                    }  
	                #endregion
                    break;
                case ObjectType.Silver:
                    #region Silver object clearing and breaking effect
                    if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point.X, this.Point.Y - 1, "   ");
                        Printing.DrawAt(this.Point, "   ");
                        Printing.DrawAt(this.Point.X, this.Point.Y + 1, "   ");
                    }
                    else
                    {
                        upRight = this.Point - diagonalDec * Frames;
                        upLeft = this.Point + diagonalDec * Frames;
                        downRight = this.Point - diagonalInc * Frames;
                        downLeft = this.Point + diagonalInc * Frames;
                        Printing.ClearAtPosition(this.Point);
                        Moveable = false;
                        PrintAndClearExplosion(true);
                        if (Frames == 5)
                        {
                            this.toBeDeleted = true;
                            Printing.Player.IncreasePoints(5);

                            Interface.Table();
                            Interface.UIDescription();
                        }
                        Frames++;
                    } 
	                #endregion
                    break;
                case ObjectType.Gold:
                    #region Gold object clearing and breaking effect math
		            if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point.X, this.Point.Y - 1, "  ");
                        Printing.DrawAt(this.Point, "   ");
                        Printing.DrawAt(this.Point.X, this.Point.Y + 1, "  ");
                    }
                    else
                    {
                        upRight = this.Point - diagonalDec * Frames;
                        upLeft = this.Point + diagonalDec * Frames;
                        downRight = this.Point - diagonalInc * Frames;
                        downLeft = this.Point + diagonalInc * Frames;
                        PrintAndClearExplosion(true);
                        if (Frames == 5)
                        {
                            this.toBeDeleted = true;
                            Printing.Player.IncreasePoints(4);

                            Interface.Table();
                            Interface.UIDescription();
                        }
                        Frames++;
                    } 
	                #endregion
                    break;
                case ObjectType.Lenghty:
                    #region Lenghty object clear and breaking effect math
		            if (!this.GotHit)
                    {
                        Printing.DrawAt(this.Point, "    ");
                    }
                    else
                    {
                        upRight = this.Point - diagonalDec * Frames;
                        upLeft = this.Point + diagonalDec * Frames;
                        downLeft = this.Point - diagonalInc * Frames;
                        downRight = this.Point + diagonalInc * Frames;
                        Moveable = false;
                        PrintAndClearExplosion(true);
                        if (Frames == 5)
                        {
                            this.toBeDeleted = true;
                            Printing.Player.IncreasePoints(3);

                            Interface.Table();
                            Interface.UIDescription();
                        }
                        Frames++;
                    }   
	                #endregion
                    break;
                case ObjectType.Quadcopter:
                    #region Quadcopter object clear and breaking effect
		            if (!GotHit)
                    {
                        if (this.Point.X + 2 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point, @"  ");
                        }
                        else if (this.Point.X + 3 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X, Point.Y - 1, @" ");
                            Printing.DrawAt(this.Point, @"   ");
                        }
                        else if (this.Point.X + 4 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"  ");
                            Printing.DrawAt(this.Point, @"    ");
                        }
                        else if (this.Point.X + 5 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"  ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"   ");
                            Printing.DrawAt(this.Point, @"     ");
                        }
                        else if (this.Point.X + 6 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"   ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"    ");
                            Printing.DrawAt(this.Point, @"      ");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @" ");
                        }
                        else if (this.Point.X + 7 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"    ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"     ");
                            Printing.DrawAt(this.Point, @"       ");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"  ");
                        }
                        else if (this.Point.X + 8 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"     ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"      ");
                            Printing.DrawAt(this.Point, @"        ");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"   ");
                        }
                        else if (this.Point.X + 9 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"      ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"       ");
                            Printing.DrawAt(this.Point, @"         ");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"   ");
                        }
                        else if (this.Point.X + 10 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"       ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"        ");
                            Printing.DrawAt(this.Point, @"          ");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"   ");
                        }
                        else if (this.Point.X + 11 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"        ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"         ");
                            Printing.DrawAt(this.Point, @"           ");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"   ");
                        }
                        else if (this.Point.X + 12 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"         ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"          ");
                            Printing.DrawAt(this.Point, @"            ");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"   ");
                        }
                        else if (this.Point.X + 13 >= Engine.WindowWidth)
                        {
                            Printing.DrawAt(this.Point.X + 3, Point.Y - 2, @"          ");
                            Printing.DrawAt(this.Point.X + 2, Point.Y - 1, @"           ");
                            Printing.DrawAt(this.Point, @"             ");
                            Printing.DrawAt(this.Point.X + 5, Point.Y + 1, @"   ");
                        }
                        else
                        {
                            Printing.DrawAt(this.Point.X, Point.Y - 2, @"              ");
                            Printing.DrawAt(this.Point.X, Point.Y - 1, @"              ");
                            Printing.DrawAt(this.Point, @"              ");
                            Printing.DrawAt(this.Point.X, Point.Y + 1, @"        ");
                        } 
                    }
                    else
                    {
                        upLeft = this.Point - diagonalInc * Frames;
                        upRight = this.Point - diagonalDec * Frames;
                        downRight = this.Point + diagonalInc * Frames;
                        downLeft = this.Point + diagonalDec * Frames;
                        PrintAndClearExplosion(true);
                        if (Frames == 5)
                        {
                            this.toBeDeleted = true;
                            Printing.Player.IncreasePoints(10);

                            Interface.Table();
                            Interface.UIDescription();
                        }
                        Frames++;
                    } 
	                #endregion
                    break;

            }
        }

        public void MoveObject()
        {
            if (Moveable)
            {
                this.Point.X -= Speed;
            }
        }

        private void PrintAndClearExplosion(bool clear, char[] c = null,ConsoleColor clr = ConsoleColor.White)
        {
            
            if (c == null && !clear)
            {
                c = new[] { '*', '*', '*', '*' };
            }
            else if (clear)
            {
                c = new[] { ' ', ' ', ' ', ' ' };
            }
            if ((upLeft.X > 1 && upLeft.X < 79) && (upLeft.Y > 1 && upLeft.Y < 30))
            {
                Printing.DrawAt(upLeft, c[0], clr);
            }
            if ((upRight.X > 1 && upRight.X < 79) && (upRight.Y > 1 && upRight.Y < 30))
            {
                Printing.DrawAt(upRight, c[1], clr);
            }
            if ((downLeft.X > 1 && downLeft.X < 79) && (downLeft.Y > 1 && downLeft.Y < 30))
            {
                Printing.DrawAt(downLeft, c[2], clr);
            }
            if ((downRight.X > 1 && downRight.X < 79) && (downRight.Y > 1 && downRight.Y < 30))
            {
                Printing.DrawAt(downRight, c[3], clr);
            }
            
        }
        /// <summary>
        /// Collision check
        /// </summary>
        /// <param name="x">X to check with</param>
        /// <param name="y">Y to check with</param>
        /// <returns>If there is a collision</returns>
        public bool Collided(int x, int y)
        {
            if (GotHit)
            {
                return false;
            }
            switch (objectType)
            {
                case ObjectType.Normal:
                    /*
                     * (.)AB
                     * (.)CD
                     */
                    if ((x == this.Point.X && (y == this.Point.Y || y == this.Point.Y + 1)) || // A / C
                        (x == this.Point.X + 1 &&
                        (y == this.Point.Y || y == this.Point.Y + 1)) || // B / D
                        (x == this.Point.X - 1 &&
                        (y == this.Point.Y || y == this.Point.Y + 1)))// ..
                        return true;
                    return false;
                case ObjectType.Small:
                    /*
                     * (.)AB              
                     */
                    if ((x == this.Point.X && y == this.Point.Y) || // A
                        (x == this.Point.X + 1 && y == this.Point.Y) || // B
                        (x == this.Point.X - 1 && y == this.Point.Y)) // .
                        return true;
                    return false;
                case ObjectType.Silver:
                    /*  
                     * CFI
                     * ADG
                     * BEH
                     */
                    if ((x == this.Point.X && y == this.Point.Y) || //A
                        (x == this.Point.X &&
                        (y == this.Point.Y + 1 || y == this.Point.Y - 1)) || // B / C
                        (x == this.Point.X + 1 &&
                        (y == this.Point.Y || y == this.Point.Y + 1 || y == this.Point.Y - 1)) || // D / E / F
                        (x == this.Point.X + 2 &&
                        (y == this.Point.Y || y == this.Point.Y + 1 || y == this.Point.Y - 1))) // G / H / I
                        return true;
                    return false;
                case ObjectType.Gold:
                    /*  
                     * CF(.)
                     * ADG
                     * BE(.)
                     */
                    if ((x == this.Point.X &&
                         (y == this.Point.Y || y == this.Point.Y + 1 || y == this.Point.Y - 1)) || // A / B / C
                        (x == this.Point.X + 1 &&
                         (y == this.Point.Y || y == this.Point.Y + 1 || y == this.Point.Y - 1)) || // D / E / F
                        (x == this.Point.X + 2 &&
                         (y == this.Point.Y || y == this.Point.Y + 1 || y == this.Point.Y - 1))) // G / . / .
                    {
                        return true;
                    }
                    else 
                    { 
                        return false;
                    }
                    ;
                case ObjectType.Lenghty:
                    /*
                     * ABCD
                     */
                    if ((x == this.Point.X && y == this.Point.Y) || // A
                        (y == this.Point.Y &&
                        (x == this.Point.X || x == this.Point.X + 1 || // B / C / D
                        x == this.Point.X + 2 || x == this.Point.X + 3)))
                        return true;
                    return false;
                case ObjectType.Quadcopter:
                   if (
                        (x == this.Point.X && y == this.Point.Y) 
                        ||
                        (y == this.Point.Y && (x == this.Point.X || x == this.Point.X + 1 || x == this.Point.X + 2 || x == this.Point.X + 3))
                        ||
                        (y == this.Point.Y + 1 && (x == this.Point.X || x == this.Point.X + 1 || x == this.Point.X + 2 || x == this.Point.X + 3))
                        ||
                        (y == this.Point.Y - 1 && (x == this.Point.X || x == this.Point.X + 1 || x == this.Point.X + 2 || x == this.Point.X + 3))
                        ||
                        (y == this.Point.Y - 2 && (x == this.Point.X+3 || x == this.Point.X + 4 || x == this.Point.X + 5))
                       )
                        return true;
                    return false;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Collision check with Point2D
        /// </summary>
        /// <param name="point">Point2D To check with</param>
        /// <returns>If there is a collision</returns>
        public bool Collided(Point2D point)
        {
            return Collided(point.X, point.Y);
        }


    }
}
