using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace KineticControl
{
    abstract class Model
    {
        private String fixtureAddress;
        private Colors currentColor;


/*
 * Getters and Setter for the current  color 
 */

        public Colors getCurrentColor()
        {
            return currentColor;
        }

        public void setCurrentColor(Colors currentColor)
        {
            this.currentColor = currentColor;
        }
/*
 * Getters and setters for the current fixture address
 */

        public String getFixtureAddress()
        {
            return fixtureAddress;
        }

        public void setFixtureAddress(String fixtureAddress)
        {
            this.fixtureAddress = fixtureAddress;
        }

    }
}
