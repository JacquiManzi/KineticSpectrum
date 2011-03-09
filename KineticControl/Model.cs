using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace KineticControl
{
    abstract class Model
    {
        private String fixtureAddress;
        private Color currentColor;


/*
 * Getters and Setter for the current  color 
 */

        public Color CurrentColor 
        { 
            get { return currentColor; } 
            set { currentColor = value; }
        }

        public String FixtureAddres
        {
            get { return fixtureAddress;  }
            set { fixtureAddress = value; }
        }


    }
}
