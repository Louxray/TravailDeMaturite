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
        public Heros()
        {
            //initialisation des donnes de base d un heros
            this.health[0] = 100;   //vie max = 100
            this.health[1] = this.health[0];    //vie actuelle = vie max
            this.mana[0] = 100; //mana max = 100
            this.mana[1] = this.mana[0];    //mana actuelle = mana max
            
            this.moveSpeed = 3; //vitesse du heros ([v] = pixel/0.01sec)
            this.Image = Image.FromFile("./images/heros/5.gif"); //charge l image d attente du heros
            this.tag = "player";

            this.CreateAttack(1, 45, 20, 30, 6.0, "./images/attack/feu0.bmp");
        }

        public void MovePlayer(List<Decor> decors, Form whichForm)
        {
            double indic = 0;  //indicateur droite/gauche  -1 = gauche, 1 = droite

            if (Keyboard.IsKeyDown(Key.Space) == true) //test pour sauter
            {
                this.Jump(decors);
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

            if ((Control.MouseButtons == MouseButtons.Left) && (this.listAttacks[0].timeRemainingCD == 0))
            {
                this.listAttacks[0].ActivateAttack(this, whichForm.PointToClient(System.Windows.Forms.Cursor.Position));
            }

            if ((Keyboard.IsKeyDown(Key.LeftShift) == true) && (this.CollisionSol(decors)))
            {
                indic *= 1.7; //permet de courir
            }

            this.MoveEntity(indic, decors);    //transmet la direction et la vitesse pour bouger le joueur
        }

        /*
        public override bool TestMort(int formHeight)
        {
            return this.health[1] <= 0; //meurt si plus de pv
        }
        */
    }
}
