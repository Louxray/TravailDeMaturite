using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NaOn
{
    class Heros : Entity
    {
        private int travelling = 0;
        public Point positionStart { get; private set; }
        public Point position { get; private set; }
        Timer test = new Timer();   //test du labyrinthe
        int i;  //test du labyrinthe
        int j;  //test du labyrinthe

        protected int level;    //level pour savoir quelles competences avoir

        public Heros()
        {            
            //initialisation des donnes de base d un heros
            this.healthMax = 1000;   //vie max = 100
            this.health = this.healthMax;    //vie actuelle = vie max
            this.manaMax = 100; //mana max = 100
            this.mana = this.manaMax;    //mana actuelle = mana max
            
            this.moveSpeed = 3; //vitesse du heros ([v] = pixel/0.01sec)
            this.Image = Image.FromFile("./images/heros/5.gif"); //charge l image d attente du heros
            this.tag = "player";

            this.CreateAttack(1, 100, 20, 15, 15.0, "./images/attack/feu0/0.bmp");

            Random rdm = new Random();
            this.position = new Point(rdm.Next(0, Form1.MAZE_WIDTH), rdm.Next(0, Form1.MAZE_HEIGHT));
            this.positionStart = this.position;

            this.test = new Timer();
            this.test.Interval = 5000;

            //permet de verifier que le labyrinthe est bien un labyrinthe
            /*
            this.test.Tick += this.test_Tick;
            this.test.Enabled = false;
            i = 0;
            j = 0;
            position = new Point(i, j);
            */
        }

        private void test_Tick(object sender, EventArgs e)
        {
            if (i == 4)
            {
                i = 0;
                j += 1;
            }
            if (j == 4)
            {
                j = 0;
            }
            position = new Point(i, j);
            i += 1;
        }

        public override void Wound(int typeOfDamage, int damage) //0 = normal, 1 = feu, 2 = eau, 3 = terre, 4 = vent, 5 = electricite
        {
            if (!this.injured)
            {
                this.health -= damage;
                this.injured = true;
                this.immunity = 12;
            }
        }

        public void MovePlayer(List<Decor> decors, Form1 whichForm)
        {
            double indic = 0;  //indicateur droite/gauche  -1 = gauche, 1 = droite

            if (travelling > 0)
            {
                travelling -= 1;
            }

            if (Keyboard.IsKeyDown(Key.Space) == true) //test pour sauter
            {
                this.Jump(decors);
            }

            if (Keyboard.IsKeyDown(Key.W) == true)   //test pour se baiser
            {
                foreach (Decor whichDecor in decors)
                {
                    if ((whichDecor.Bounds.IntersectsWith(this.Bounds)) && (whichDecor.interactive)
                        && (whichForm.Controls.Contains(whichDecor)))
                    {
                        switch (whichDecor.whichDecor)
                        {
                            case 2:
                                if (travelling == 0)
                                {
                                    this.OpenDoor(whichDecor.typeOfDecor);
                                    travelling = 50;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (Keyboard.IsKeyDown(Key.A) == true)   //test pour aller a gauche
            {
                indic -= 1; //vers la gauche
            }

            if (Keyboard.IsKeyDown(Key.S) == true)   //test pour se baiser
            {
                //player.Bow();
            }

            if (Keyboard.IsKeyDown(Key.D) == true)  //test pour aller a droite
            {
                indic += 1;    //vers la droite
            }

            if ((Control.MouseButtons == MouseButtons.Left) && (this.listAttacks[0].timeRemainingCD == 0) && (!this.listAttacks[0].Enabled))
            {
                if ((whichForm.PointToClient(System.Windows.Forms.Cursor.Position).X > -1)
                    && (whichForm.PointToClient(System.Windows.Forms.Cursor.Position).X < whichForm.ClientSize.Width)
                    && (whichForm.PointToClient(System.Windows.Forms.Cursor.Position).Y > -1)
                    && (whichForm.PointToClient(System.Windows.Forms.Cursor.Position).Y < whichForm.ClientSize.Height))
                {
                    this.listAttacks[0].ActivateAttack(this, whichForm.PointToClient(System.Windows.Forms.Cursor.Position));
                }
            }

            if ((Keyboard.IsKeyDown(Key.LeftShift) == true))
            {
                indic *= 1.7; //permet de courir
            }

            if (!this.CollisionMur(decors, indic))
            {
                this.MoveEntity(indic);    //transmet la direction et la vitesse pour bouger le joueur
            }
        }
                
        public override bool TestMort(int formHeight)
        {
            return this.health <= 0; //meurt si plus de pv
        }        

        private void OpenDoor(int whichDoor)
        {
            switch (whichDoor)
            {
                case 0:
                    this.position = new Point(this.position.X, this.position.Y - 1);
                    break;
                case 1:
                    this.position = new Point(this.position.X + 1, this.position.Y);
                    break;
                case 2:
                    this.position = new Point(this.position.X, this.position.Y + 1);
                    break;
                case 3:
                    this.position = new Point(this.position.X - 1, this.position.Y);
                    break;
            }
        }
    }
}
