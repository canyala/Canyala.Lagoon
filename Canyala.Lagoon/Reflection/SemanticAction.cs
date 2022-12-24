//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using Canyala.Lagoon.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Canyala.Lagoon.Reflection
{
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

        public static T SemanticFunc<T>(this object target, object query)
        {
            var handlerType = target is Type ? (Type)target : target.GetType();
            var requestType = query.GetType();

            var method = handlerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                .Where(item => item.GetParameters().Length == 1 &&
                               item.GetParameters().Any(parameter => parameter.ParameterType == requestType) &&
                               item.ReturnType == typeof(T))
                .SingleOrDefault();

            if (method == null)
                throw new InvalidOperationException(requestType.Name);

            return (T)method.Invoke(target, Seq.Array(query));
        }
    }
}
