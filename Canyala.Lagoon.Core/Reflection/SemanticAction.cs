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

using Canyala.Lagoon.Core.Functional;

namespace Canyala.Lagoon.Core.Reflection;

static public class SemanticMethodExtensions
{
    public static void SemanticAction(this object target, object command)
    {
        var handlerType = target is Type ? (Type)target : target.GetType();
        var requestType = command.GetType();

        var methods = handlerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

        var actionMethods = methods
            .Where(item => item.GetParameters().Length == 1 &&
                           item.GetParameters().Any(parameter => parameter.ParameterType == requestType) &&
                           item.ReturnType == typeof(void));

        if (!actionMethods.Any())
            throw new InvalidOperationException(requestType.Name);

        var args = Seq.Array(command);
        foreach (var method in actionMethods)
            method.Invoke(target, args);
    }

    public static T? SemanticFunc<T>(this object target, object query)
    {
        var handlerType = target is Type type ? type : target.GetType();
        var requestType = query.GetType();

        var method = handlerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
            .Where(item => item.GetParameters().Length == 1 &&
                           item.GetParameters().Any(parameter => parameter.ParameterType == requestType) &&
                           item.ReturnType == typeof(T))
            .SingleOrDefault();

        if (method == null)
            throw new InvalidOperationException(requestType.Name);

        return (T?) method.Invoke(target, Seq.Array(query));
    }
}
