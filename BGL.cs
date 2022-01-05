using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            Semestar1();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */


        /* Initialization */
        Cuvar mater;
        Student tomo;
        Granica granica;
        Aktivnosti whatsapp;
        Aktivnosti facebook;
        Aktivnosti youtube;
        Aktivnosti netflix;
        Aktivnosti instagram;
        Aktivnosti piva;
        Likovi kuhaca;

        
        private void Semestar1()
        {
            //1. setup stage
            SetStageTitle("Učionica");
            setBackgroundColor(Color.SpringGreen);            
            //setBackgroundPicture("backgrounds\\green.jpg");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //2. add sprites
            //student
            tomo = new Student("sprites\\ucenik.png", (GameOptions.RightEdge / 2) - 40, GameOptions.DownEdge - 70);
            tomo.SetSize(30);
            Game.AddSprite(tomo);
            //granica
            granica = new Granica("sprites\\crta.png", (GameOptions.RightEdge / 2) - 420, GameOptions.DownEdge - 100);
            Game.AddSprite(granica);
            //cuvar
            mater = new Cuvar("sprites\\mater.png", (GameOptions.RightEdge / 2) - 40, GameOptions.DownEdge - 165);
            mater.SetSize(30);
            Game.AddSprite(mater);
            //wap
            whatsapp = new Aktivnosti("sprites\\wap.png", 0, 0);
            whatsapp.SetVisible(false);
            whatsapp.SetSize(60);
            Game.AddSprite(whatsapp);
            //insta
            instagram = new Aktivnosti("sprites\\insta.png", 0, 0);
            instagram.SetVisible(false);
            instagram.SetSize(60);
            Game.AddSprite(instagram);
            //face
            facebook = new Aktivnosti("sprites\\fb.jpg", 0, 0);
            facebook.SetVisible(false);
            facebook.SetSize(60);
            Game.AddSprite(facebook);
            //netflix
            netflix = new Aktivnosti("sprites\\netflix.png", 0, 0);
            netflix.SetVisible(false);
            netflix.SetSize(60);
            Game.AddSprite(netflix);
            //yt
            youtube = new Aktivnosti("sprites\\youtube.png", 0, 0);
            youtube.SetVisible(false);
            youtube.SetSize(60);
            Game.AddSprite(youtube);
            //piva
            piva = new Aktivnosti("sprites\\piva.jpg", 0, 0);
            piva.SetVisible(false);
            piva.SetSize(60);
            Game.AddSprite(piva);
            //kuhaca
            kuhaca = new Likovi("sprites\\kuhaca.jpg", 0, 0);
            //kuhaca.RotationStyle = "AllAround";
            kuhaca.SetVisible(false);
            kuhaca.SetSize(50);
            Game.AddSprite(piva);

            //3. scripts that start
            Game.StartScript(Kretanje);
            Game.StartScript(Aktivnosti1);
            Game.StartScript(Obrana);

            Semestar += Semestar2;
            Kraj += KrajIgre;
            
        }

        public delegate void SemestarDel(bool s);
        public static event SemestarDel Semestar;
        public delegate void Semestar_drugi(bool sem);
        public static event Semestar_drugi Semestar_dr;
        public delegate void PobjedaDel(bool p);
        public static event PobjedaDel Pobjeda;
        public delegate void KrajDel(bool k);
        public static event KrajDel Kraj;



        public void Semestar2(bool semestar)
        {
            START = false;
            if(semestar==true)
            {
                if (MessageBox.Show("Prva godina je gotova! Prelazim na drugu...\nNe daj im ni blizu!","", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {

                    START = true;
                    Semestar2();
                }
            }
        }


        public void Semestar3(bool sem)
        {
            START = false;
            if (sem == true)
            {
                if (MessageBox.Show("Prelazim na zadnju godinu, izdrži još malo!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {

                    START = true;
                    Semestar3();
                }
            }
        }

        public void PobjedaStudent(bool pobjeda)
        {
            START = false;
            if(pobjeda == true)
            {
                instagram.SetVisible(false);
                whatsapp.SetVisible(false);
                youtube.SetVisible(false);
                facebook.SetVisible(false);
                netflix.SetVisible(false);
                piva.SetVisible(false);
                mater.SetVisible(false);
                tomo.SetVisible(false);
                granica.SetVisible(false);
                ISPIS = "";
                if(MessageBox.Show("Završio sam uspješno preddiplomski studij! Hvala vam!","DIPLOMA",MessageBoxButtons.OK,MessageBoxIcon.Information)==DialogResult.OK)
                {
                    
                    Application.Exit();
                }
                Wait(2);
            }
            
        }

        public void KrajIgre(bool poraz)
        {
            START = false;
            if (poraz == true)
            {
                instagram.SetVisible(false);
                whatsapp.SetVisible(false);
                youtube.SetVisible(false);
                facebook.SetVisible(false);
                netflix.SetVisible(false);
                piva.SetVisible(false);
                mater.SetVisible(false);
                tomo.SetVisible(false);
                granica.SetVisible(false);
                ISPIS = "";
                if (MessageBox.Show("Nažalost, niste uspjeli. Platite 8000kn i pokušajte ponovno!\nZa ponovni pokušaj pritisnite Retry.\nAko želite izaći pritisnite Cancel.", "SEZONA", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
                {

                    Application.Restart();
                    
                }
                else
                {
                    Application.Exit();
                }
                Wait(2);
            }

        }



        private void Semestar2()
        {
            Game.StartScript(Kretanje);
            Game.StartScript(Aktivnosti2);
            Game.StartScript(Obrana);
            Semestar_dr += Semestar3;
        }

        private void Semestar3()
        {
            Game.StartScript(Kretanje);
            Game.StartScript(Aktivnosti3);
            Game.StartScript(Obrana);
            Pobjeda += PobjedaStudent;
        }

       
        

        /* Scripts */

        private int Kretanje()
        {
            while (START) //ili neki drugi uvjet
            {
                if(sensing.KeyPressed(Keys.Left))
                {
                    mater.SetDirection(270);
                    mater.MoveSteps(mater.Brzina);
                }
                else if(sensing.KeyPressed(Keys.Right))
                {
                    mater.SetDirection(90);
                    mater.MoveSteps(mater.Brzina);
                }
                
                Wait(0.05);
            }
            return 0;
        }

        private int Aktivnosti1()
        {
            Random r = new Random();
            instagram.X = r.Next(0, GameOptions.RightEdge - instagram.Width);
            instagram.Y = 0;
            instagram.SetVisible(true);
            whatsapp.X = r.Next(0, GameOptions.RightEdge - whatsapp.Width);
            whatsapp.Y = 0;
            whatsapp.SetVisible(true);
            
            while (START)
            {
                instagram.Y += instagram.Brzina;
                whatsapp.Y += whatsapp.Brzina;                
                Wait(0.1);

                if (instagram.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    instagram.X = r.Next(0, GameOptions.RightEdge - instagram.Width);
                    instagram.Y = 0;
                    if(tomo.Koncentracija==0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (whatsapp.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    whatsapp.X = r.Next(0, GameOptions.RightEdge - whatsapp.Width);
                    whatsapp.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }                
                if (tomo.Predmet==8)
                {                   
                    Semestar.Invoke(true);
                    Wait(3);
                    break;
                }
                if(tomo.Predmet<0)
                {
                    Kraj.Invoke(true);
                    break;
                }
                
                
            }
            return 0;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            tomo.Otkucaji++;
            ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            if (tomo.Otkucaji == 5)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 10)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 15)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 20)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 25)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 30)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 35)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 40)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                
            }
            else if (tomo.Otkucaji == 45)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 50)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 55)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 60)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 65)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 70)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 75)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 80)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;

            }
            else if (tomo.Otkucaji == 85)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 90)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 95)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 100)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 105)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 110)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 115)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
            }
            else if (tomo.Otkucaji == 120)
            {
                tomo.Predmet++;
                ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                
            }
        }

        public int Aktivnosti2()
        {
            Random r = new Random();
            instagram.X = r.Next(0, GameOptions.RightEdge - instagram.Width);
            instagram.Y = 0;
            instagram.SetVisible(true);
            whatsapp.X = r.Next(0, GameOptions.RightEdge - whatsapp.Width);
            whatsapp.Y = 0;
            whatsapp.SetVisible(true);
            facebook.X = r.Next(0, GameOptions.RightEdge - facebook.Width);
            facebook.Y = 0;
            facebook.SetVisible(true);
            youtube.X = r.Next(0, GameOptions.RightEdge - youtube.Width);
            youtube.Y = 0;
            youtube.SetVisible(true);
            
            while (START)
            {
                instagram.Y += instagram.Brzina;
                whatsapp.Y += whatsapp.Brzina;
                facebook.Y += facebook.Brzina;
                youtube.Y += youtube.Brzina;                
                Wait(0.1);

                if (instagram.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    instagram.X = r.Next(0, GameOptions.RightEdge - instagram.Width);
                    instagram.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (whatsapp.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    whatsapp.X = r.Next(0, GameOptions.RightEdge - whatsapp.Width);
                    whatsapp.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (youtube.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    youtube.X = r.Next(0, GameOptions.RightEdge - youtube.Width);
                    youtube.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (facebook.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    facebook.X = r.Next(0, GameOptions.RightEdge - facebook.Width);
                    facebook.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }              
                if (tomo.Predmet==16)
                {
                    Semestar_dr.Invoke(true);
                    Wait(3);
                    break;
                }
                if (tomo.Predmet < 0)
                {
                    Kraj.Invoke(true);
                    break;
                }



            }            
            return 0;
        }

        public int Aktivnosti3()
        {
            Random r = new Random();
            instagram.X = r.Next(0, GameOptions.RightEdge - instagram.Width);
            instagram.Y = 0;
            instagram.SetVisible(true);
            whatsapp.X = r.Next(0, GameOptions.RightEdge - whatsapp.Width);
            whatsapp.Y = 0;
            whatsapp.SetVisible(true);
            facebook.X = r.Next(0, GameOptions.RightEdge - facebook.Width);
            facebook.Y = 0;
            facebook.SetVisible(true);
            youtube.X = r.Next(0, GameOptions.RightEdge - youtube.Width);
            youtube.Y = 0;
            youtube.SetVisible(true);
            piva.X = r.Next(0, GameOptions.RightEdge - piva.Width);
            piva.Y = 0;
            piva.SetVisible(true);
            netflix.X = r.Next(0, GameOptions.RightEdge - netflix.Width);
            netflix.Y = 0;
            netflix.SetVisible(true);
            while (START)
            {
                instagram.Y += instagram.Brzina;
                whatsapp.Y += whatsapp.Brzina;
                facebook.Y += facebook.Brzina;
                youtube.Y += youtube.Brzina;
                piva.Y += piva.Brzina;
                netflix.Y += netflix.Brzina;
                Wait(0.1);

                if (instagram.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    instagram.X = r.Next(0, GameOptions.RightEdge - instagram.Width);
                    instagram.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (whatsapp.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    whatsapp.X = r.Next(0, GameOptions.RightEdge - whatsapp.Width);
                    whatsapp.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (youtube.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    youtube.X = r.Next(0, GameOptions.RightEdge - youtube.Width);
                    youtube.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (facebook.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    facebook.X = r.Next(0, GameOptions.RightEdge - facebook.Width);
                    facebook.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (piva.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    piva.X = r.Next(0, GameOptions.RightEdge - piva.Width);
                    piva.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if (netflix.TouchingSprite(granica))
                {
                    tomo.Koncentracija -= 20;
                    tomo.Otkucaji -= 2;
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    netflix.X = r.Next(0, GameOptions.RightEdge - netflix.Width);
                    netflix.Y = 0;
                    if (tomo.Koncentracija == 0)
                    {
                        tomo.Predmet -= 1;
                    }
                }
                if(tomo.Predmet==24)
                {
                    Pobjeda.Invoke(true);
                    break;
                }
                if (tomo.Predmet < 0)
                {
                    Kraj.Invoke(true);
                    break;
                }


            }
            return 0;
        }

        private int Obrana()
        {
            Random r = new Random();
            while (START)
            {
                if (instagram.TouchingSprite(mater))
                {
                    tomo.Koncentracija += 20;                    
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    instagram.Y = 0;
                    instagram.X = r.Next(0, GameOptions.RightEdge - instagram.Width);                    
                }
                else if (whatsapp.TouchingSprite(mater))
                {
                    tomo.Koncentracija += 20;                    
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    whatsapp.Y = 0;
                    whatsapp.X = r.Next(0, GameOptions.RightEdge - whatsapp.Width);                    
                }
                else if (youtube.TouchingSprite(mater))
                {
                    tomo.Koncentracija += 20;                    
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    youtube.Y = 0;
                    youtube.X = r.Next(0, GameOptions.RightEdge - youtube.Width);                    
                }
                else if (facebook.TouchingSprite(mater))
                {
                    tomo.Koncentracija += 20;                    
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    facebook.Y = 0;
                    facebook.X = r.Next(0, GameOptions.RightEdge - facebook.Width);                    
                }
                else if (piva.TouchingSprite(mater))
                {
                    tomo.Koncentracija += 20;                    
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    piva.Y = 0;
                    piva.X = r.Next(0, GameOptions.RightEdge - piva.Width);                    
                }
                else if(netflix.TouchingSprite(mater))
                {
                    tomo.Koncentracija += 20;                   
                    ISPIS = "Koncentracija: " + tomo.Koncentracija + "\nVrijeme: " + tomo.Otkucaji.ToString() + "\nPredmet: " + tomo.Predmet;
                    netflix.Y = 0;
                    netflix.X = r.Next(0, GameOptions.RightEdge - netflix.Width);                    
                }
                

            }
            return 0;
        }

        



        /* ------------ GAME CODE END ------------ */


    }
}
