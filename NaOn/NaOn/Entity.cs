using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaOn
{
    class Entity : Item
    {
        protected Timer life;

        protected Entity()
        {
            this.ResetFallSpeed();   //cree l entite sans chute
            this.injured = false;
            this.immunity = 0;
            this.life = new Timer();
            this.life.Interval = 50;
            this.life.Tick += this.live;
        }
        
        protected bool injured;

        protected int immunity;

        protected double direction = 0;

        //initialisation des caracteres d une entite
        public int[] health = new int[2];   //creation d un tableau pour la vie actuelle/max
        //vie[0] = vie max
        //vie[1] = vie actuelle

        protected int[] mana = new int[2];   //creation d un tableau pour la mana actuelle/max
        //mana[0] = mana max
        //mana[1] = mana actuelle
        
        //variables pour la chute d une entite
        private const double acceleration = 0.2;    //acceleration = g
        private double fallSpeed;   //vitesse de base lors d une chute
        protected double moveSpeed; //vitesse de l entite

        public List<Attack> listAttacks = new List<Attack>();

        public virtual void Wound(int typeOfDamage, int damage) //0 = normal, 1 = feu, 2 = eau, 3 = terre, 4 = vent, 5 = electricite
        {
            this.health[1] -= damage;
        }

        public void Recover()
        {
            if (this.injured)
            {
                this.immunity -= 1;
            }
            if (this.immunity == 0)
            {
                this.injured = false;
            }
        }

        protected void CreateAttack(int typeOfDamageGiven, int damageGiven, int coutGiven, int cooldownGiven, double speedGiven, string pathOfImageGiven)
        {
            this.listAttacks.Add(new Attack(typeOfDamageGiven, damageGiven, coutGiven, cooldownGiven, speedGiven, pathOfImageGiven));
            this.listAttacks[this.listAttacks.Count-1].DesactivateAttack();
        }

        public void ResetFallSpeed()
        {
            this.fallSpeed = 0.0;   //reset
        }

        protected void MoveEntity(double X)
        {
            if (X != 0)
            {
                this.direction = X;
            }
            this.Location = new Point(this.Left + (int)(X * this.moveSpeed), this.Top); //permet de deplacer le perso de X, qui est combien la quantite de deplacement de l entite
        }

        protected void Jump(List<Decor> decors)
        {
            if ((!this.CollisionToit(decors)) && (this.CollisionSol(decors)))
            {
                this.fallSpeed = -7.0;  //vitesse d un saut 
                this.Fall(); //fait tomber
            }
        }

        private void GravityEffect()
        {
            if (this.fallSpeed < 12)
            {
                this.fallSpeed = this.fallSpeed + acceleration;   //prise de vitesse lors de la chute
            }
        }

        public void Gravity(List<Decor> decors)
        {
            if (!this.CollisionSol(decors))
            {
                if ((this.CollisionToit(decors)) && (!this.CollisionMur(decors, 0)))
                {
                    this.fallSpeed = 2;  //si touche le plafond le perso retombe avec une vitesse initîale de 2pixels/0.001s
                }
                this.Fall();
            }
            else
            {
                this.ResetFallSpeed();   //si touche le sol alors il tombe plus
            }
        }

        private void Fall()
        {
            this.GravityEffect(); //fait prendre de la vitesse a une entite en chute ou en saut
            this.Location = new Point(this.Left, (int)(this.Top + this.fallSpeed));  //fait tomber
        }
                
        protected bool CollisionSol(List<Decor> decors)
        {
            foreach (Decor whichObject in decors)
            {
                //test de collision avec le decor
                if ((whichObject.whichDecor < 2)  //verifie que l objet est touchable
                    && ((this.Bottom + 7 > whichObject.Top) //test de collision
                    && (this.Bottom - 7 < whichObject.Top)
                    && (this.Left < whichObject.Right)
                    && (this.Right > whichObject.Left)))
                {
                    if (Math.Abs(this.Bottom - whichObject.Top) < 7)
                    {
                        this.Location = new Point(this.Left, whichObject.Top - this.Height);    //colle le perso au sol s il ne le touche pas tout a fait
                    }
                    return true;
                }
            }
            return false;
        }

        private bool CollisionToit(List<Decor> decors)
        {
            foreach (Decor whichObject in decors)
            {
                //test de collision avec le decor
                if ((whichObject.whichDecor == 0)  //verifie que l objet est touchable
                    && ((this.Top + 5 > whichObject.Bottom) //test de collision
                    && (this.Top - 5 < whichObject.Bottom)
                    && (this.Left + 1 < whichObject.Right)
                    && (this.Right - 1 > whichObject.Left)))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool CollisionMur(List<Decor> decors, double direction)
        {
            foreach (Decor whichObject in decors)
            {
                //test de collision avec le decor
                if ((whichObject.whichDecor == 0)  //verifie que l objet est touchable
                    && (((direction < 0) //test de collision selon le sens du perso
                    && ((this.Left + 2 > whichObject.Right)
                    && (this.Left - 5 < whichObject.Right)
                    && ((this.Top < whichObject.Bottom)
                    && (this.Bottom > whichObject.Top))))
                    || ((direction > 0)
                    && ((this.Right + 5 > whichObject.Left)
                    && (this.Right - 2 < whichObject.Left)
                    && ((this.Top < whichObject.Bottom)
                    && (this.Bottom > whichObject.Top))))))
                {
                    return true;
                }
            }
            return false;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////ULTRA IMPORTANT             this.Bounds.IntersectsWith(whichObject.Bounds)
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool TestTombe(int formHeight)
        {
            if (this.Location.Y > formHeight + 10)  //test si le perso est tombe
            {
                return true;
            }
            return false;
        }

        public virtual bool TestMort(int formHeight)
        {
            if ((this.health[1] <= 0) || (TestTombe(formHeight)))   //test si le perso est mort
            {
                this.DesactivateEntity();
                return true;
            }
            return false;
        }

        public virtual void ActivateEntity()
        {
            this.Enabled = true;
            this.life.Start();
        }

        public virtual void DesactivateEntity()
        {
            this.Enabled = false;
            this.life.Stop();
        }

        private void live(Object sender, EventArgs e)
        {
            this.Recover();

            foreach (Attack whichAttack in this.listAttacks)
            {
                if (whichAttack.Enabled)
                {
                    whichAttack.MoveToTarget();
                }
            }
        }
    }
}
