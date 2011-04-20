using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;


namespace KinectAPI
{
    public class NumPad
    {

        public void MoveRight(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            e.Handled = true;

            switch (e.KeyCode)
            {
                case Keys.NumPad6:
                    break;
                case Keys.NumPad4:
                    break;
                case Keys.NumPad8:
                    break;
                case Keys.NumPad2:
                    break;
                case Keys.Enter:
                    break;
                default:
                    break;

            }

        }
    }
}