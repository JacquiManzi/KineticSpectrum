using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace KineticControl
{
    abstract class Model
    {
        private IList<Fixture> _fixtures;

        public Model()
        {
            _fixtures = new List<Fixture>(50);
            for(int i=0; i<50 ;i++)
            {
                _fixtures.Add(new Fixture(i));
            }
        }
    }
}
