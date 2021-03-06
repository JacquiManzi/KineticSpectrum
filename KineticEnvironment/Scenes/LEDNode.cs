﻿using System;
using System.Windows.Media;
using KineticControl;
using System.Windows.Media.Media3D;

namespace RevKitt.KS.KineticEnvironment.Scenes
{
    public class LEDNode
    {
        private readonly LightAddress _address;
        private          Color        _color;
        private          Vector3D     _position;
        private          bool         _active;

        private static readonly LightSystem _lightSystem = LightSystemProvider.LightSystem;

        public LEDNode(LightAddress address, Vector3D position)
        {
            if(address == null)
                throw new ArgumentException("LightAddres cannot be null on LEDNode");
            _address = address;
            _active = false;
            Position = position;
        }

        public LightAddress Address
        {
            get { return _address; }
        }

        public Vector3D Position { 
        get { return _position; }
            set
            {
                if(value == null)
                    throw new ArgumentException("Cannot set position to null on LEDNode");
                _position = value;
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if(value == null)
                    throw new ArgumentException("Cannot set Color to null on LEDNode");
                if (!_address.IsUnknown && !_address.IsVirtual && _active)
                    _lightSystem[_address] = value;
                _color = value;
            }
        }

        public bool IsActive
        {
            get { return _active; }
            set
            {
                _active = value;
                if (value)
                {
                    Color = _color;
                }
            }
        }
    }
}
