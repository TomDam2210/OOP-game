using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public abstract class Razred:Sprite
    {
        protected int brzina;

        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }

        public Razred(string putanja, int x, int y)
            :base(putanja,x,y)
        {
            
        }

        public override int X
        {
            get { return base.X; }
            set
            {
                if(value<GameOptions.LeftEdge)
                {
                    base.X = GameOptions.LeftEdge;
                }
                else if(value > GameOptions.RightEdge - this.Width)
                {
                    base.X = GameOptions.RightEdge - this.Width;
                }

                else
                {
                    base.X = value;
                }
            }
        }

        public override int Y
        {
            get { return base.Y; }
            set
            {
                if(value > GameOptions.DownEdge-this.Heigth)
                {
                    base.Y = GameOptions.DownEdge - this.Heigth;
                }
                else if(value < GameOptions.UpEdge)
                {
                    base.Y = GameOptions.UpEdge;
                }
                else
                {
                    base.Y = value;
                }
            }
        }
    }

    public class Aktivnosti:Razred
    {
        Random r = new Random();

        public Aktivnosti(string putanja, int x, int y)
            :base(putanja, x, y)
        {
            this.Brzina = r.Next(3,10);
            this.Width = 50;
            this.Heigth = 50;
        }
    }

    public class Likovi:Sprite
    {
        private int koncentracija;
        private int brzina;
        

        public int Koncentracija
        {
            get { return koncentracija; }
            set 
            {
                if (value < 0)
                {
                    koncentracija = 0;
                }
                else if(value>100)
                {
                    koncentracija = 100;
                }
                else
                {
                    koncentracija = value;
                }
            }
        }

        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }

        

        public Likovi(string putanja, int x, int y)
            :base(putanja,x,y)
        {            
            this.Brzina = 8;
        }

        public override int X
        {
            get { return base.X; }
            set
            {
                if (value < GameOptions.LeftEdge)
                {
                    base.X = GameOptions.LeftEdge;
                }
                else if (value > GameOptions.RightEdge - this.Width)
                {
                    base.X = GameOptions.RightEdge - this.Width;
                }

                else
                {
                    base.X = value;
                }
            }
        }


    }

    public class Student:Likovi
    {
        private int otkucaji;
        private int predmet;
        public int Otkucaji
        {
            get { return otkucaji; }
            set 
            {
                if(value<0)
                {
                    otkucaji = 0;
                }
                else
                {
                    otkucaji = value;
                }
            }
        }
        public int Predmet
        {
            get { return predmet; }
            set { predmet = value; }
        }
        public Student(string putanja, int x, int y)
            :base(putanja,x,y)
        {
            this.Koncentracija = 0;
            this.Otkucaji = 0;
            this.Predmet = 0;
        }

    }

    public class Cuvar:Likovi
    {
        public Cuvar(string putanja, int x, int y)
            :base(putanja, x, y)
        {
            this.Brzina = 30;
        }
    }

    public class Granica:Likovi
    {
        public Granica(string putanja, int x, int y)
            :base(putanja,x,y)
        {
            this.Width = 700;
            this.Heigth = 5;
        }
    }
}
