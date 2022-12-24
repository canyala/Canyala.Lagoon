//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Canyala.Lagoon.Reflection
{
    /// <summary>
    /// SemanticProperty provides a type representing properties with a semantic meaning.
    /// 
    /// The assumption is that properties of the same type and name have
    /// the same semantic meaning even if they are part of different classes.
    /// This assumption is used as a base for implementing copy between different
    /// classes, thereby bridging the gap between the statically typed and 
    /// dynamically typed world's and in a sense provide a 
    /// typesafe 'duck copy' for the static world.
    /// </summary>
    public class SemanticProperty
    {
        public Type Type { get; private set; }
        public string Name { get; private set; }

        private SemanticProperty(PropertyInfo propertyInfo)
        {
            Type = propertyInfo.PropertyType;
            Name = propertyInfo.Name;
        }

        public SemanticProperty(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        internal static SemanticProperty FromPropertyInfo(PropertyInfo propertyInfo)
        {
            return new SemanticProperty(propertyInfo);
        }

        public override bool Equals(object instance)
        {
            var target = instance as SemanticProperty;

            if (target == null)
                return false;

            if (Type != target.Type)
                return false;

            return Name.Equals(target.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Type.GetHashCode();
        }
    }
}
