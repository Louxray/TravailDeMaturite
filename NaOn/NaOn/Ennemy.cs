using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NaOn
{
    class Ennemy : Entity
    {
        private string race;
        private int typeOfEnnemy;   //0 = distance, 1 = melee
        private int typeOfDamage;   //type de l ennemi
        private Timer artifialIntelligence;
        private bool actived = false;
        private Point aim;
        private int whatShot = 0;

        public Ennemy(string raceGiven, int typeOfEnnemyGiven, int typeOfDamageGiven)
        {
            this.tag = "ennemy";
            this.race = raceGiven;
            this.typeOfEnnemy = typeOfEnnemyGiven;
            this.typeOfDamage = typeOfDamageGiven;
            this.artifialIntelligence = new Timer();
            this.artifialIntelligence.Tick += this.artifialIntelligence_Tick;

            if (typeOfEnnemy == 0)
            {
                switch (typeOfDamage)
                {
                    case 1:
                        this.Image = Image.FromFile("./images/ennemies/elementalFeu/1.bmp");
                        this.CreateAttack(1, 10, 20, 15, 13.0, "./images/attack/feu0/0.bmp");
                        this.artifialIntelligence.Interval = 2000;
                        break;
                    case 2:
                        this.Image = Image.FromFile("./images/ennemies/elementalEau/1.bmp");
                        this.CreateAttack(1, 20, 20, 15, 10.0, "./images/attack/eau0/0.bmp");
                        this.artifialIntelligence.Interval = 2500;
                        break;
                    case 3:
                        this.Image = Image.FromFile("./images/ennemies/elementalTerre/1.bmp");
                        this.CreateAttack(1, 40, 20, 15, 7.0, "./images/attack/terre0/0.bmp");
                        this.artifialIntelligence.Interval = 3000;
                        break;
                }
            }
            if (typeOfEnnemy == 1)
            {
                this.Image = Image.FromFile("./images/ennemies/" + race + "/5.gif");
            }
            if (typeOfEnnemy == 2)
            {
                this.moveSpeed = 6; //vitesse du heros ([v] = pixel/0.01sec)
                this.Image = Image.FromFile("./images/ennemies/boss/0.bmp");
                for (int i = 0; i < 8; i++)
                {
                    this.CreateAttack(1, 10, 20, 15, 12.0, "./images/attack/normal0/0.bmp");
                }
                this.artifialIntelligence.Interval = 200;
            }
            
            Bitmap bmp = new Bitmap(this.Image);
            bmp.MakeTransparent();
            this.Image = bmp;

            this.Location = new Point(300, 0);

            //initialisation des donnes de base
            this.health[0] = 100;   //vie max = 100
            this.health[1] = this.health[0];    //vie actuelle = vie max
            this.mana[0] = 100; //mana max = 100
            this.mana[1] = this.mana[0];    //mana actuelle = mana max
        }

        public void WhereIsTheHero(Point herosPosition)
        {
            aim = herosPosition;
        }

        public override void ActivateEntity()
        {
            this.Enabled = true;
            this.life.Start();
            this.artifialIntelligence.Start();
        }

        public override void DesactivateEntity()
        {
             this.Enabled = false;
            this.life.Stop();
            this.artifialIntelligence.Stop();
            foreach (Attack whichAttack in listAttacks)
            {
                whichAttack.DesactivateAttack();
                whichAttack.timeRemainingCD = 0;
            }
        }

        private void artifialIntelligence_Tick(Object sender, EventArgs e)  //selon le type d'ennemi, attaque d une certaine maniere
        {
            if (actived)
            {
                if ((typeOfEnnemy == 0) && (this.listAttacks[0].timeRemainingCD == 0))
                {
                    this.listAttacks[0].ActivateAttack(this, aim);
                }
                if (typeOfEnnemy == 1)
                {

                }
                if (typeOfEnnemy == 2)
                {
                    if (this.Left + this.Width / 2 - aim.X > -60)
                    {
                        this.MoveEntity(-1);
                    }
                    if (this.Left + this.Width / 2 - aim.X < -60)
                    {
                        this.MoveEntity(1);
                    }

                    whatShot += 1;
                    if (whatShot == 8)
                    {
                        whatShot = 0;
                    }
                    this.listAttacks[whatShot].ActivateAttack(this, new Point(this.Location.X + (int)Math.Round(Math.Cos(Math.PI / 4 * whatShot) * 20), this.Location.Y + (int)Math.Round(Math.Sin(Math.PI / 4 * whatShot) * 20)));
                }
            }
            actived = true; //active l ennemi apres un premier tour (un peu de repis pour notre joueur)
        }
    }
}
