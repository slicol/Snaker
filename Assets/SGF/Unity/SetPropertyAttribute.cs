// Copyright (c) 2014 Luminary LLC
// Licensed under The MIT License (See LICENSE for full text)
using UnityEngine;
using System.Collections;

namespace SGF.Unity
{
    public class SetPropertyAttribute : PropertyAttribute
    {
        public string Name { get; private set; }
        public bool IsDirty { get; set; }

        public SetPropertyAttribute(string name)
        {
            this.Name = name;
        }
    }

    public class RangePropertyAttribute : PropertyAttribute
    {
        public string Name { get; private set; }

        public float Min { get; private set; }
        public float Max { get; private set; }

        public bool IsDirty { get; set; }

        public RangePropertyAttribute(string name,float min,float max)
        {
            this.Name = name;
            this.Min = min;
            this.Max = max;
        }
    }
}