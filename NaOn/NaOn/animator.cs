using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NaOn
{
    class Animator 
    {
        List<Image> animations;
        public Animator(string chemin,int nb_image)
        {
            animations = new List<Image>();
            for(int i = 0; i < nb_image; i++)
            {
                animations.Add(Image.FromFile(""));
            }            
        }
    }
}
