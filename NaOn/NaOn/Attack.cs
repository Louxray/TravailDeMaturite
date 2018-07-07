using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NaOn
{
    class Attack : Object
    {
        //type des degats pour faiblesse/resistances
        public int typeOfDamage { get; private set; }   //0 = normal, 1 = feu, 2 = eau, 3 = terre, 4 = vent, 5 = electricite
        public int damage { get; private set; }
        public int cout { get; private set; }
        public int speed { get; private set; }
        public string pathOfImage { get; private set; }

        public Attack(int typeOfDamageGiven, int damageGiven, int coutGiven, int speedGiven, string pathOfImageGiven)
        {
            this.tag = "attack";
            this.typeOfDamage = typeOfDamageGiven;
            this.damage = damageGiven;
            this.cout = coutGiven;
            this.speed = speedGiven;
            this.pathOfImage = pathOfImageGiven;
            this.Image = Image.FromFile(pathOfImage);
            this.Location = new Point();
        }
    }
}
