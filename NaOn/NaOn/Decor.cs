using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NaOn
{
    class Decor : Item
    {
        public int whichDecor; //0 = mur, 1 = platform, 2 = porte
        public int typeOfDecor { get; private set; }    //depend du fichier a ouvrir
        private bool ForeGround = false;
        Timer keepForeGround = new Timer();

        public Decor(int coordX, int coordY, int whichDecorGiven, int typeOfDecorGiven)
        {
            this.tag = "decor";
            switch (whichDecorGiven)
            {
                case 0:
                    this.Image = Image.FromFile("./images/decor/zone/0.bmp");    //charge le skin de test
                    this.Location = new Point(coordX * this.Width, coordY * this.Height);   //positionne aux coordonnes voulues
                    break;
                case 1:
                    this.Image = Image.FromFile("./images/decor/platform/0.bmp");    //charge le skin de test
                    this.Location = new Point(coordX * (this.Width + 135) + 60, (coordY + 1) * 120 + 70);   //positionne aux coordonnes voulues
                    break;
                case 2:
                    this.interactive = true;
                    switch (typeOfDecorGiven)
                    {
                        case 0:
                            this.Image = Image.FromFile("./images/decor/door/0.bmp");    //charge le skin de test
                            this.Location = new Point(500, (coordY + 1) * 120 + 70 - this.Height);   //positionne aux coordonnes voulues
                            break;
                        case 1:
                            this.Image = Image.FromFile("./images/decor/door/1.bmp");    //charge le skin de test
                            this.Location = new Point(780 - this.Width, (coordY + 1) * 120 + 70 - this.Height);   //positionne aux coordonnes voulues
                            break;
                        case 2:
                            this.Image = Image.FromFile("./images/decor/door/2.bmp");    //charge le skin de test
                            this.Location = new Point(284, (coordY + 1) * 120 + 70 - this.Height);   //positionne aux coordonnes voulues
                            this.ForeGround = true;
                            break;
                        case 3:
                            this.Image = Image.FromFile("./images/decor/door/1.bmp");    //charge le skin de test
                            this.Location = new Point(60, (coordY + 1) * 120 + 70 - this.Height);   //positionne aux coordonnes voulues
                            break;
                    }
                    break;
            }
            this.typeOfDecor = typeOfDecorGiven;
            this.whichDecor = whichDecorGiven;
            this.keepForeGround.Tick += this.keepForeGround_Tick;
            this.keepForeGround.Interval = 10;
            this.keepForeGround.Enabled = true;
        }

        private void keepForeGround_Tick(Object sender, EventArgs e)
        {
            if (ForeGround)
            {
                this.BringToFront();
            }
        }
    }
}
