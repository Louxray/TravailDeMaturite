using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NaOn
{
    class Item : PictureBox
    {
        public string tag { get; protected set; } = null;    //tag pour reconnaitre l objet
        public bool interactive { get; protected set; } = false;

        protected Item()
        {
            this.Anchor = (AnchorStyles.Left | AnchorStyles.Top);   //referentiel pour positionner les images
            this.Location = new Point();    //positionne l objet a 0,0 pour commencer
            this.SizeMode = PictureBoxSizeMode.AutoSize;    //autosize
            this.Visible = true;    //rend visible
        }
    }
}
