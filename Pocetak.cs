using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    public partial class Pocetak : Form
    {
        public Pocetak()
        {
            InitializeComponent();
        }

        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        

        private void Pocetak_Load(object sender, EventArgs e)
        {
            SetStageTitle("Početak");
            setBackgroundPicture("backgrounds\\ucionica.jpg");
            setPictureLayout("stretch");
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            string upute = "Cilj igre je štiti studenta od aktivnosti koje ga ometaju dok uči." +
                "\nDok vrijeme prolazi, student polaže predmete i pokušava se skoncentrirati." +
                "\nAko aktivnost dotakne granicu sa studentom, njegova koncentracija pada i vrijeme sporije prolazi." +
                "\nKreći se što brže, drži aktivnosti podalje od studenta i pomozi mu da završi preddiplomski studij!" +
                "\nKRETANJE: strelice lijevo i desno";
            if (MessageBox.Show(upute, "UPUTE", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
            {
                this.Hide();
                BGL forma1 = new BGL();
                forma1.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        
    }
}
