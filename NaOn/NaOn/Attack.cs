﻿using System;
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
        public bool useable { get; private set; }
        public int cout { get; private set; }
        public int speed { get; private set; }
        public string pathOfImage { get; private set; }
        private Point target;

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
            this.useable = true;
        }

        public void Aim(Point targetGiven)
        {
            this.target = targetGiven;
        }

        public void MoveToTarget()
        {
            this.Location = new Point(
        }

        public void ActivateAttack(Entity who, Point aim)
        {
            this.target = aim;
            if ((who.Location.Y + (who.Width / 2.0) - aim.Y) >= 0)
            {
                if ((who.Location.X + (who.Width / 2.0) - aim.X) <= 0)
                {
                    this.Location = new Point(
                        (int)(who.Location.X + (who.Width - this.Width) / 2.0 + Math.Cos(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))) * 20.0),
                        (int)(who.Location.Y + (who.Width - this.Width) / 2.0 - Math.Sin(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))) * 20.0));
                }
                else
                {
                    this.Location = new Point(
                        (int)(who.Location.X + (who.Width - this.Width) / 2.0 - Math.Cos(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))) * 20.0),
                        (int)(who.Location.Y + (who.Width - this.Width) / 2.0 - Math.Sin(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))) * 20.0));
                }
            }
            else
            {
                if ((who.Location.X + (who.Width / 2.0) - aim.X) <= 0)
                {
                    this.Location = new Point(
                        (int)(who.Location.X + (who.Width - this.Width) / 2.0 + Math.Cos(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))) * 20.0),
                        (int)(who.Location.Y + (who.Width - this.Width) / 2.0 + Math.Sin(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))) * 20.0));
                }
                else
                {
                    this.Location = new Point(
                        (int)(who.Location.X + (who.Width - this.Width) / 2.0 - Math.Cos(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))) * 20.0),
                        (int)(who.Location.Y + (who.Width - this.Width) / 2.0 + Math.Sin(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))) * 20.0));
                }
            }
            this.Visible = true;
            this.Enabled = true;
            //this.useable = false;
        }

        public void DesactivateAttack()
        {
            this.Location = new Point(-200, -200);
            this.Visible = false;
            this.Enabled = false;
            this.useable = true;
        }
    }
}