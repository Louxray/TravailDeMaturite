using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaOn
{
    class Entity : Object
    {
        protected Entity()
        {
            ResetFallSpeed();   //cree l entite sans chute
            injured = false;
            immunity = 0;
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

        protected int level;    //level pour savoir quelles competences avoir
        
        //variables pour la chute d une entite
        private const double acceleration = 0.2;    //acceleration = g
        private double fallSpeed;   //vitesse de base lors d une chute
        protected double moveSpeed; //vitesse de l entite

        public List<Attack> listAttacks = new List<Attack>();

        public void Wound(int typeOfDamage, int damage)
        {
            if (!injured)
            {
                this.health[1] -= damage;
                this.injured = true;
                immunity = 50;
            }
        }

        public void Recover()
        {
            if (injured)
            {
                immunity -= 1;
            }
            if (immunity == 0)
            {
                injured = false;
            }
        }

        protected void CreateAttack(int typeOfDamageGiven, int damageGiven, int coutGiven, int cooldownGiven, double speedGiven, string pathOfImageGiven)
        {
            this.listAttacks.Add(new Attack(typeOfDamageGiven, damageGiven, coutGiven, cooldownGiven, speedGiven, pathOfImageGiven));
            this.listAttacks[listAttacks.Count-1].DesactivateAttack();
        }

        private void ResetFallSpeed()
        {
            this.fallSpeed = 0.0;   //reset
        }

        protected void MoveEntity(double X, List<Decor> decors)
        {
            if (!CollisionMur(decors, X))
            {
                if (X != 0)
                {
                    this.direction = X;
                }
                this.Location = new Point(this.Left + (int)(X * moveSpeed), this.Top); //permet de deplacer le perso de X, qui est combien la quantite de deplacement de l entite 
            }
        }

        protected void Jump(List<Decor> decors)
        {
            if ((!CollisionToit(decors)) && (CollisionSol(decors)))
            {
                this.fallSpeed = -8.0;  //vitesse d un saut 
                Fall(); //fait tomber
            }
        }

        private void GravityEffect()
        {
            this.fallSpeed = fallSpeed + acceleration;   //prise de vitesse lors de la chute
        }

        public void Gravity(List<Decor> decors)
        {
            if (!CollisionSol(decors))
            {
                if ((CollisionToit(decors)) && (!CollisionMur(decors, 0)))
                {
                    this.fallSpeed = 2;  //si touche le plafond le perso retombe avec une vitesse initîale de 2pixels/0.001s
                }
                Fall();
            }
            else
            {
                ResetFallSpeed();   //si touche le sol alors il tombe plus
            }
        }

        private void Fall()
        {
            GravityEffect(); //fait prendre de la vitesse a une entite en chute ou en saut
            this.Location = new Point(this.Left, (int)(this.Top + fallSpeed));  //fait tomber
        }
                
        protected bool CollisionSol(List<Decor> decors)
        {
            foreach (Decor whichObject in decors)
            {
                //test de collision avec le decor
                if (/*(whichObject.whichDecor == 0)  //verifie que l objet est touchable
                    && */((this.Bottom + 7 > whichObject.Top) //test de collision
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

        private bool CollisionMur(List<Decor> decors, double direction)
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

        public /*virtual*/ bool TestMort(int formHeight)
        {
            return this.health[1] <= 0 || TestTombe(formHeight) ;   //test si le perso est mort
        }        
    }
}
