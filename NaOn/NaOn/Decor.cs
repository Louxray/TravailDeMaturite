using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NaOn
{
    class Decor : Object
    {
        public int whichDecor; //0 = mur, 1 = platform, 2 = porte
        public int typeOfDecor { get; private set; }    //0 = dur, 1= gravity, 2 = nothing

        public Decor(int coordX, int coordY, int whichDecorGiven, int typeOfDecorGiven)
        {
            this.tag = "decor";
            switch (whichDecorGiven)
            {
                case 0:
                    this.Image = Image.FromFile("./images/decor/zombie/zone/0.bmp");    //charge le skin de test
                    this.Location = new Point(coordX * this.Width, coordY * this.Height);   //positionne aux coordonnes voulues
                    break;
                case 1:
                    this.Image = Image.FromFile("./images/decor/zombie/platform/0.bmp");    //charge le skin de test
                    this.Location = new Point(coordX * (this.Width + 135) + 60, (coordY + 1) * 120 + 60);   //positionne aux coordonnes voulues
                    break;
            }
            this.typeOfDecor = typeOfDecorGiven;
            whichDecor = whichDecorGiven;
        }
    }
}
