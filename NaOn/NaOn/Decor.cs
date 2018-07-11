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
        public Decor(int coordX, int coordY, int typeOfDecorGiven)
        {
            this.tag = "decor";
            this.Image = Image.FromFile("./images/decor/sol/ecole.bmp");    //charge le skin de test
            this.Location = new Point(coordX * this.Width, coordY * this.Height);   //positionne aux coordonnes voulues
            this.typeOfDecor = typeOfDecorGiven;
        }
        
        public int typeOfDecor;  //0 = dur, 1= gravity, 2 = nothing
    }
}
