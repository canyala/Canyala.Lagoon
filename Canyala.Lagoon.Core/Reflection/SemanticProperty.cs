//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation (Martin Fredriksson)
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//------------------------------------------------------------------------------- 

using System.Reflection;

namespace Canyala.Lagoon.Reflection;

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

    public override bool Equals(object? instance)
    {
        if (instance is not SemanticProperty target)
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
