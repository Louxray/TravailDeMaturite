﻿using System;
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

            this.CreateAttack(1, 50, 20, 20, 15.0, "./images/attack/feu0.bmp");
        }

        public void MovePlayer(List<Decor> decors, Form1 whichForm)
        {
            double indic = 0;  //indicateur droite/gauche  -1 = gauche, 1 = droite

            if (Keyboard.IsKeyDown(Key.Space) == true) //test pour sauter
            {
                this.Jump(decors);
            }

            if (Keyboard.IsKeyDown(Key.W) == true)   //test pour se baiser
            {
                foreach (Decor whichDecor in decors)
                {
                    if ((whichDecor.Bounds.IntersectsWith(this.Bounds)) && (whichDecor.interactive))
                    {
                        this.Interact();
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

            if ((Control.MouseButtons == MouseButtons.Left) && (this.listAttacks[0].timeRemainingCD == 0))
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

            this.MoveEntity(indic, decors);    //transmet la direction et la vitesse pour bouger le joueur
        }

        /*
        public override bool TestMort(int formHeight)
        {
            return this.health[1] <= 0; //meurt si plus de pv
        }
        */

        private void Interact()
        {            
        }
    }
}
