using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using System.Windows.Forms;

namespace NaOn
{
    class Attack : Item
    {
        //type des degats pour faiblesse/resistances
        public int typeOfDamage { get; private set; }   //0 = normal, 1 = feu, 2 = eau, 3 = terre, 4 = vent, 5 = electricite
        public int damage { get; private set; }
        public int timeRemainingCD;
        public int cooldown { get; private set; }
        public int cout { get; private set; }
        public double speed { get; private set; }
        private string pathOfImage;
        private Point target;
        private System.Windows.Point direction; //0 = X, 1 = Y

        public Attack(int typeOfDamageGiven, int damageGiven, int coutGiven, int cooldownGiven, double speedGiven, string pathOfImageGiven)
        {
            this.tag = "attack";
            this.typeOfDamage = typeOfDamageGiven;
            this.damage = damageGiven;
            this.cout = coutGiven;
            this.speed = speedGiven;
            this.pathOfImage = pathOfImageGiven;
            this.Image = Image.FromFile(pathOfImage);
            this.Location = new Point();
            this.direction = new System.Windows.Point(0.0,0.0);
            this.cooldown = cooldownGiven;
            this.timeRemainingCD = 0;

            Bitmap bmp = new Bitmap(this.Image);
            bmp.MakeTransparent();
            this.Image = bmp;
        }

        public void Aim(Point targetGiven)
        {
            this.target = targetGiven;
        }

        public void MoveToTarget()
        {
            this.Left = (int)Math.Round(this.Location.X + (direction.X * this.speed));
            this.Top  = (int)Math.Round(this.Location.Y + (direction.Y * this.speed));
        }

        public void ActivateAttack(Entity who, Point aim)
        {
            this.target = aim;
            if ((who.Location.Y + (who.Width / 2.0) - aim.Y) >= 0)
            {
                if ((who.Location.X + (who.Width / 2.0) - aim.X) <= 0)
                {
                    this.direction.X = (Math.Cos(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y - (this.Width / 2.0)) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X - (this.Width / 2.0)))));
                    this.direction.Y = (-Math.Sin(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y - (this.Width / 2.0)) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X - (this.Width / 2.0)))));
                }
                else
                {
                    this.direction.X = (-Math.Cos(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y - (this.Width / 2.0)) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X - (this.Width / 2.0)))));
                    this.direction.Y = (-Math.Sin(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y - (this.Width / 2.0)) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X - (this.Width / 2.0)))));
                }                
            }
            else
            {
                if ((who.Location.X + (who.Width / 2.0) - aim.X) <= 0)
                {
                    this.direction.X = (Math.Cos(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y - (this.Width / 2.0)) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X - (this.Width / 2.0)))));
                    this.direction.Y = (Math.Sin(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X))));
                }
                else
                {
                    this.direction.X = (-Math.Cos(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y - (this.Width / 2.0)) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X - (this.Width / 2.0)))));
                    this.direction.Y = (Math.Sin(Math.Atan(Math.Abs(who.Location.Y + (who.Width / 2.0) - aim.Y - (this.Width / 2.0)) / Math.Abs(who.Location.X + (who.Width / 2.0) - aim.X - (this.Width / 2.0)))));
                }
            }
            this.Location = new Point(
                (int)Math.Round(who.Location.X + (who.Width - this.Width) / 2.0 + direction.X * 20.0),
                (int)Math.Round(who.Location.Y + (who.Width - this.Width) / 2.0 + direction.Y * 20.0));
            this.Visible = true;
            this.Enabled = true;
            this.timeRemainingCD = this.cooldown;
        }        

        public void DesactivateAttack()
        {
            this.direction.X = 0;
            this.direction.Y = 0;
            this.Location = new Point(-200, -200);
            this.Visible = false;
            this.Enabled = false;
        }

        public void LowerCD()
        {
            this.timeRemainingCD -= 1;
        }
    }
}
