using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HandGesture
{
    public partial class info : Form
    {
        public info()
        {
            InitializeComponent();
            pictureBox_icon.BackgroundImage = Image.FromFile("손바닥.jpg");        
            label_developers.Text = "12091443 김용래\n12111631 장윤희\n12111663 진정은";
       
        }
    }
}
