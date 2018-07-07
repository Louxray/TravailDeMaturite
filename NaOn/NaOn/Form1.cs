using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace NaOn
{
    public partial class Form1 : Form
    {
        int test = 0;
        Heros player = new Heros(); //creation personnage

        List<Entity> goodOnes = new List<Entity>();
        List<Ennemy> ennemis = new List<Ennemy>();
        List<Decor> decors = new List<Decor>();
        List<Entity> everybody = new List<Entity>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //DoubleBuffered = true;
            

            //ajoute les persos ici
            goodOnes.Add(player);
            Ennemy lol = new Ennemy("zombie");
            ennemis.Add(lol);


            //repetorie toutes les entites
            for (int i = 0; i < goodOnes.Count; i++)
            {
                everybody.Add(goodOnes[i]);
            }

            for (int i = 0; i < ennemis.Count; i++)
            {
                everybody.Add(ennemis[i]);
            }
            
            for (int i = 0; i < everybody.Count; i++)
            {
                this.Controls.Add(everybody[i]);    //ajoute tout le monde aux controles
                foreach(Attack whatAdd in everybody[i].listAttacks)
                {
                    this.Controls.Add(whatAdd);
                }
            }

            //parametres de base de la fenetre
            this.ClientSize = new Size((int)(Screen.PrimaryScreen.Bounds.Width / 1.5), (int)(Screen.PrimaryScreen.Bounds.Height / 2));
            this.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 6), (int)(Screen.PrimaryScreen.Bounds.Height / 4));   //position de la fenetre sur l ecran
            this.MaximumSize = this.ClientSize; //bloque la taille max de la fenetre
            this.MinimumSize = this.ClientSize; //bloque la taille min de la fenetre
            this.MaximizeBox = false;   //empeche le plein ecran

            //creation zones de decor
            for (int i = 0; i < 12; i++)
            {
                decors.Add(new Decor((i - 2), (i < 5) ? (this.ClientSize.Height) : (this.ClientSize.Height - 100), 0));
                this.Controls.Add(decors[i]);
            }
            player.Location = new Point(player.Left, decors[0].Top - player.Height);
        }   

        private void MoveEntities(object sender, EventArgs e)
        {
            player.MovePlayer(decors);    //demarre le test des touches
            CollisionAttack();
            label1.Text = player.health[1].ToString() + "/" + player.health[0].ToString();
            for (int i = 0; i < everybody.Count; i++)
            {
                everybody[i].Gravity(decors);
                if (everybody[i].TestMort(this.ClientSize.Height))
                {
                    everybody.Remove(everybody[i]);
                    //  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //
                    //  IL FAUT ENCORE CHANGER LE FAIT QUE SI LES GENTILS TOMBENT ILS NE MEURENT PAS
                    //
                    //  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
            }
        }

        protected void CollisionAttack()
        {
            foreach (Entity whichAlly in goodOnes)
            {
                foreach (Ennemy whichEnnemi in ennemis)
                {
                    foreach (Attack whichAttack in whichEnnemi.listAttacks)
                    {
                        if ((whichAttack.Enabled) && (whichAttack.Bounds.IntersectsWith(whichAlly.Bounds)))
                        {
                            whichAlly.Wound(whichAttack.typeOfDamage, whichAttack.damage);
                        }
                    }
                    foreach (Attack whichAttack in whichAlly.listAttacks)
                    {
                        if ((whichAttack.Enabled) && (whichAttack.Bounds.IntersectsWith(whichEnnemi.Bounds)))
                        {
                            whichEnnemi.Wound(whichAttack.typeOfDamage, whichAttack.damage);
                        }
                    }
                }
            }
        }

        /*
        private bool TestExist(Entity who)
        {
            return who != null;
        }
        */
    }
}