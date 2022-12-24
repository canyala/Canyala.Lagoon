/*
 
  MIT License

  Copyright (c) 2022 Canyala Innovation

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Canyala.Lagoon.Reflection {
    /// <summary>
    /// Provides a simple api for accessing fields, properties, indexers and method calls dynamically on
    /// any object instance or static class.
    /// </summary>
    /// <remarks>
    /// The purpose of the Dynacutor class is to make it much simpler to read and set the values referenced
    /// by various instances in an object graph. This is accomplished using the reflection api provided,
    /// but simplifies the process by giving support for the normal c#-syntax used in ordinary code to
    /// access a value or reference.
    /// 
    /// For example, in c# you could see code such as "person.Lastname.Substring(2)" which access the substring
    /// method on the string value referenced as the lastname of a Person instance. To handcode this
    /// using reflection directly is tedious. With the Dynacutor you simply provide the instance acting as root
    /// of the access and a string representing the chain of references, like this:
    /// 
    /// Dynacutor dyn = new Dynacutor();
    /// string value = dyn.GetValueOf(person, "Lastname.Substring(2)");
    /// 
    /// Dynacutor also supports settingvalues in the same fashon:
    /// 
    /// dyn.SetValueOf(person, "Child["Jonas"].Age", 13);
    /// 
    /// This expression sets the age property of the Child of a person. Note the reference to the indexer on the Child
    /// instance.
    /// 
    /// You can also access static properties or methods by setting null as instance:
    /// DateTime value = dyn.GetValueOf(null, "System.DateTime.Now");
    /// 
    /// You have to use the fullname of the class.
    /// 
    /// The Dynacutor contains a name/Value table where you can put values needed as parameters to methods. You then 
    /// access these values using a syntax where you prepend "@@" to the name of the value:
    /// 
    /// Dynacutor dyn = new Dynacutor();
    /// dyn.SetEnv("Foo", 42);
    /// dyn.SetEnv("Bar", 100);
    /// int value = (int)dyn.GetValueOf(null, "System.Math.Max(@@Foo, @@Bar)");
    /// 
    /// Here value == 100
    /// 
    /// Currently the following limitations exist:
    /// - Only string or Int32 indexers are supported
    /// - Currently there is no support for special characters in strings (i.e. "\r\n" etc.)
    /// </remarks>
    public class Dynacutor {
        /// <summary>
        /// Environment table containing value bindings to be used with dynamic
        /// value access.
        /// </summary>
        /// <remarks>
        /// Use 
        /// - SetEnv() method to add bindings to the environment
        /// - GetEnv() mthod to fetch value of a binding
        /// - RemoveEnv() to remove a binding
        /// - EnvProperties to get an enumeration of all bindings currently installed
        /// 
        /// To incur the value of a binding in a dynamic expression use the name of the binding
        /// prepended with the "@@" string:
        /// example: 
        /// Dynacutor dyn = new Dynacutor();
        /// dyn.SetEnv("Foo", 42);
        /// dyn.SetEnv("Bar", 100);
        /// int value = (int)dyn.GetValueOf(null, "System.Math.Max(@@Foo, @@Bar)");
        /// </remarks>
        private Dictionary<string, object> env = new Dictionary<string, object>();
        /// <summary>
        /// Reference to mscorlib assembly (needed to get Type of a class for static methods and properties).
        /// </summary>
        private Assembly mscorlib = Assembly.ReflectionOnlyLoad("mscorlib");

        /// <summary>
        /// Add or change a binding in the environment.
        /// </summary>
        /// <param name="propertyName">the name of the binding (without @@!)</param>
        /// <param name="value">the  value of the binding (any object)</param>
        public void SetEnv(string propertyName, object value) {
            if (env.ContainsKey(propertyName))
                env[propertyName] = value;
            else
                env.Add(propertyName, value);
        }
        /// <summary>
        /// Removes a binding from the environment.
        /// </summary>
        /// <param name="propertyName">The name of the binding to remove (without @@)</param>
        public void RemoveEnv(string propertyName) {
            if (env.ContainsKey(propertyName))
                env.Remove(propertyName);
        }
        /// <summary>
        /// Enumerates all currently installed binding names.
        /// </summary>
        public IEnumerable<string> EnvProperties {
            get { return env.Keys; }
        }
        /// <summary>
        /// Returns The value bound to a name in the environment.
        /// </summary>
        /// <param name="PropertName">The name of the binding (without @@)</param>
        /// <returns>the value bound to this name</returns>
        public object GetEnv(string PropertName) {
            return env[PropertName];
        }

        public bool HasMember(object instance, string memberName, ClassTypes type)
        {
            var metaData = GetInstanceMembersOf(instance).SingleOrDefault(m => m.Type == type && m.Name == memberName);
            return metaData != null;
        }

        /// <summary>
        /// Evaluates a C# reference chain expression on an instance and returns
        /// the value of the reference.
        /// </summary>
        /// <param name="instance">The root object on which to evaluate (use null to evaluate static methods/properties)</param>
        /// <param name="expression">The expression to evaluate to get a value</param>
        /// <returns>the value of the expression</returns>
        /// <remarks>
        /// Read the class doc for an example...
        /// </remarks>
        public object GetValueOf(object instance, string expression) {
            if (instance != null)
                return GetValue(instance, SplitExpression(expression));
            else {
                List<string> exprList = SplitExpression(expression);
                string typeName = TypenamePart(expression);
                string expr = expression.Substring(typeName.Length + 1);
                return GetStaticValueOf(typeName, expr);
            }
        }
        /// <summary>
        /// Evaluates a C# static reference chain expression on a type name and returns
        /// the value of the reference.
        /// </summary>
        /// <param name="typeName">The fullname of the type</param>
        /// <param name="expression">static expression chain</param>
        /// <returns>the value of the expression</returns>
        /// <remarks>
        /// Read the class doc for an example...
        /// </remarks>
        public object GetStaticValueOf(string typeName, string expression) {
            Type t = TypeOf(null, typeName);
            List<string> exprList = SplitExpression(expression);
            string firstExpr = exprList[0];
            exprList.Remove(firstExpr);
            object instance = GetStaticValue(t, firstExpr);
            return GetValue(instance, exprList);
        }
        /// <summary>
        /// Sets a new value by dynamically dereferenceing a C# expression on an object.
        /// </summary>
        /// <param name="instance">The root object</param>
        /// <param name="expression">The C# reference chaing expression</param>
        /// <param name="value">The new value</param>
        public void SetValueOf(object instance, string expression, object value) {
            SetValue(instance, SplitExpression(expression), value);
        }

        /// <summary>
        /// Retrieves a list of all instance members (events, fields, properties and methods) defined
        /// by the Type of an instance.
        /// </summary>
        /// <param name="instance">the instance object to list members for</param>
        /// <returns>list of MetaInfo objects each representing one member</returns>
        public IEnumerable<MetaInfo> GetInstanceMembersOf(object instance) {
            return GetMembersOf(instance, BindingFlags.Instance | BindingFlags.Public);
        }
        /// <summary>
        /// Retrieves a list of all static members (events, fields, properties and methods) defined
        /// by the Type of an instance.
        /// </summary>
        /// <param name="instance">the instance object to list members for</param>
        /// <returns>list of MetaInfo objects each representing one static member</returns>
        public IEnumerable<MetaInfo> GetStaticMembersOf(object instance) {
            return GetMembersOf(instance, BindingFlags.Static | BindingFlags.Public);
        }
        /// <summary>
        /// Retrieves a list of all static members (events, fields, properties and methods) defined
        /// by a Type.
        /// </summary>
        /// <param name="t">the Type to list static members for</param>
        /// <returns>list of MetaInfo objects each representing one static member</returns>
        public IEnumerable<MetaInfo> GetStaticMembersOf(Type t) {
            return GetMembersOf(t, BindingFlags.Static | BindingFlags.Public);
        }
        /// <summary>
        /// Retrieves a list of all members (events, fields, properties and methods) defined
        /// by the Type of an instance that match a set of BindingFlags.
        /// </summary>
        /// <param name="instance">the instance object to list members for</param>
        /// <param name="flags">BindingFlags to govern how members are searched</param>
        /// <returns>list of MetaInfo objects each representing one member</returns>
        public IEnumerable<MetaInfo> GetMembersOf(object instance, BindingFlags flags) {
            Type t = instance.GetType();
            return GetMembersOf(t, flags);
        }
        /// <summary>
        /// Retrieves a list of all members (events, fields, properties and methods) defined
        /// by a Type that match a set of BindingFlags.
        /// </summary>
        /// <param name="t">The type to list members for</param>
        /// <param name="flags">BindingFlags to govern how members are searched</param>
        /// <returns>list of MetaInfo objects each representing one member</returns>
        public IEnumerable<MetaInfo> GetMembersOf(Type t, BindingFlags flags) {
            #region Convert member type to ClassTypes enum value
            Func<MemberInfo, ClassTypes> FindClassType =
                mi => {
                    switch (mi.MemberType) {
                        case MemberTypes.Event:
                            return ClassTypes.Event;
                        case MemberTypes.Field:
                            return ClassTypes.Field;
                        case MemberTypes.Method:
                            return ClassTypes.Method;
                        case MemberTypes.Property:
                            return ClassTypes.Property;
                        default:
                            throw new ArgumentException(string.Format("Cannot map MemberTypes value {0} to a ClassTypes value!", mi.MemberType));
                    }
                };
            #endregion
            #region Get Method Arity
            Func<MemberInfo, int> GetArity =
                mi => {
                    MethodInfo method = mi as MethodInfo;
                    if (method != null)
                        return method.GetParameters().Count();
                    else
                        return 0;
                };
            #endregion

            var result = from mi in t.GetMembers(flags)
                         where mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field || 
                                mi.MemberType == MemberTypes.Event || mi.MemberType == MemberTypes.Method
                         select new MetaInfo { Name = mi.Name, Arity = GetArity(mi), Type = FindClassType(mi), Info = mi};

            return result;
        }
        /// <summary>
        /// Creates a new instance of a type given by its full type name, by calling the
        /// public constructor of that type that match a list of argument instances.
        /// </summary>
        /// <param name="typeName">the fullname of the type to create (as by Type.FullName)</param>
        /// <param name="args">array of arguments in correct order</param>
        /// <returns>The instance</returns>
        public object CreateInstanceOf(string typeName, params object[] args) {
            Type t = TypeOf(null, typeName);
            return CreateInstanceOf(t, args);
        }
        /// <summary>
        /// Creates a new instance of a type given by its Type instance, by calling the
        /// public constructor of that type that match a list of argument instances.
        /// </summary>
        /// <param name="t">Type to create instance of</param>
        /// <param name="args">array of arguments in correct order</param>
        /// <returns>The Instance</returns>
        public object CreateInstanceOf(Type t, params object[] args) {
            Assembly asm = t.Assembly;
            var bindings = BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            return asm.CreateInstance(t.FullName, false, bindings, null, args, null, null);
        }

        /// <summary>
        /// Returns the Type for the value of an C# reference chain expression.
        /// </summary>
        /// <param name="instance">The root instance (or null of expression is a static type expression)</param>
        /// <param name="expression">The C# reference chain expression</param>
        /// <returns>The Type instance for the value of the expression</returns>
        public Type TypeOf(object instance, string expression) {
            if (instance != null) {
                object value = GetValueOf(instance, expression);
                if (value != null)
                    return value.GetType();
                else
                    return null;
            } else {
                Type t = mscorlib.GetType(expression);
                return t;
            }
        }
        /// <summary>
        /// Creates a new instance of a primitive .NET type.
        /// </summary>
        /// <param name="t">The Type instance of the type to create</param>
        /// <param name="value">a string value to be parsed into a value of Type</param>
        /// <returns>an object refere3ncing a value of type Type</returns>
        public object Instantiate(Type t, string value) {
            if (string.IsNullOrEmpty(value))
                return null;

            if (t == typeof(String))
                return value;
            if (t == typeof(Double))
                return Double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            if (t == typeof(Int32))
                return Int32.Parse(value);
            if (t == typeof(DateTime))
                return DateTime.Parse(value);

            if (t == typeof(Int64))
                return Int64.Parse(value);
            if (t == typeof(Int16))
                return Int16.Parse(value);

            if (t == typeof(Decimal))
                return Decimal.Parse(value);
            if (t == typeof(Single))
                return Single.Parse(value);

            if (t == typeof(UInt16))
                return UInt16.Parse(value);
            if (t == typeof(UInt32))
                return UInt32.Parse(value);
            if (t == typeof(UInt64))
                return UInt64.Parse(value);

            if (t == typeof(Char))
                return value[0];

            throw new ArgumentException(string.Format("Unsupported type argument {0}", t));
        }
        /// <summary>
        /// Attempts to create a new primitive value based only on its string representation.
        /// </summary>
        /// <param name="item">C# literal value to instantiate from</param>
        /// <returns>The value represented by the literal</returns>
        /// <remarks>
        /// This method currently supports only primitive types and the null value:
        /// - String
        /// - Char
        /// - DateTime
        /// - bool
        /// - Byte
        /// - Integers of different lenghts
        /// - Floats of different lengths
        /// </remarks>
        public object InstantiateLiteral(string item) {
            if (item == "null")
                return null;

            if (item.ToLower() == "true" || item.ToLower() == "false")
                return Boolean.Parse(item);

            if (item.StartsWith("\"") && item.EndsWith("\""))
                return item.Substring(1, item.Length - 2);

            if (item.Length == 3 && item.StartsWith("'") && item.EndsWith("'"))
                return item[1];

            if (item.StartsWith("(sbyte)") || item.StartsWith("(SByte)"))
                return SByte.Parse(item.Substring(7));

            if (item.StartsWith("(short)") || item.StartsWith("(Int16)"))
                return Int16.Parse(item.Substring(7));

            if (item.StartsWith("(int)"))
                return Int32.Parse(item.Substring(5));
            if (item.StartsWith("(Int32)"))
                return Int32.Parse(item.Substring(7));

            if (item.EndsWith("L"))
                return long.Parse(item.Substring(0, item.Length - 1));
            if (item.StartsWith("(long)"))
                return long.Parse(item.Substring(6));
            if (item.StartsWith("(Int64)"))
                return long.Parse(item.Substring(7));

            if (item.StartsWith("(byte)"))
                return byte.Parse(item.Substring(6));
            if (item.StartsWith("(Byte)"))
                return byte.Parse(item.Substring(6));

            if (item.StartsWith("(ushort)"))
                return ushort.Parse(item.Substring(8));
            if (item.StartsWith("(UInt16)"))
                return ushort.Parse(item.Substring(8));

            if (item.EndsWith("U"))
                return uint.Parse(item.Substring(0, item.Length - 1));
            if (item.StartsWith("(uint)"))
                return uint.Parse(item.Substring(6));
            if (item.StartsWith("(UInt32)"))
                return UInt32.Parse(item.Substring(8));

            if (item.EndsWith("UL"))
                return ulong.Parse(item.Substring(0, item.Length - 2));
            if (item.StartsWith("(ulong)"))
                return ulong.Parse(item.Substring(7));
            if (item.StartsWith("(UInt64)"))
                return UInt64.Parse(item.Substring(8));


            if (item.EndsWith("F"))
                return float.Parse(item.Substring(0, item.Length - 1));
            if (item.StartsWith("(float)"))
                return ulong.Parse(item.Substring(7));
            if (item.StartsWith("(Single)"))
                return UInt64.Parse(item.Substring(8));

            if (item.EndsWith("D"))
                return float.Parse(item.Substring(0, item.Length - 1));
            if (item.StartsWith("(double)"))
                return ulong.Parse(item.Substring(8));
            if (item.StartsWith("(Double)"))
                return UInt64.Parse(item.Substring(8));


            if (item.EndsWith("M"))
                return decimal.Parse(item.Substring(0, item.Length - 1));
            if (item.StartsWith("(decimal)"))
                return ulong.Parse(item.Substring(9));
            if (item.StartsWith("(Decimal)"))
                return UInt64.Parse(item.Substring(9));

            return int.Parse(item);
        }

        #region Private implementation methods
        private List<string> SplitExpression(string expression) {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(expression)) {
                string[] fields = expression.Split(new char[] { '.' });
                foreach (string item in fields) {
                    if (item == string.Empty)
                        throw new ArgumentException(string.Format("The expression is malformed!: {0}", expression));

                    if (!(item.Contains("[") || item.Contains("]"))) {
                        result.Add(item);
                    } else {
                        string[] split = item.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                        if (split.Length > 0) {
                            int startIndex = 0;
                            if (!split[0].StartsWith("[")) {
                                result.Add(split[0]);
                                startIndex = 1;
                            }
                            for (int i = startIndex; i < split.Length; i++) {
                                result.Add("[" + split[i]);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private string TypenamePart(string expression) {
            string[] tokens = expression.Split('.');
            if (tokens.Length > 0) {
                string result = "";
                for (int i = 0; i < tokens.Length; i++) {
                    string name = (result.Length > 0 ? result + "." + tokens[i] : tokens[i]);
                    if (IsTypename(name))
                        return name;
                    else
                        result = name;
                }

                throw new ArgumentException(string.Format("Expression {0} is a namespace or class name with NO static value part!", expression));
            }

            throw new ArgumentException("Expression is empty!");
        }

        private object GetValue(object instance, List<string> expressions) {
            if (instance == null || expressions.Count == 0)
                return instance;

            string expr = expressions[0];
            expressions.Remove(expr);
            object subInstance = GetValueFor(instance, expr);
            return GetValue(subInstance, expressions);
        }

        private object GetStaticValue(Type t, string expr) {
            object val = null;
            if (IsMethod(expr)) {
                val = GetMethodValue(null, expr, t);

            } else if (IsIndexer(expr)) {
                val = GetIndexerValue(null, expr, t);

            } else {
                val = GetFieldOrPropertyValue(null, expr, t);
            }

            if (val != null)
                return val;

            throw new ArgumentException(string.Format("No static property '{0}' found on type {1}", expr, t));
        }

        private void SetValue(object instance, List<string> expressions, object value) {
            if (expressions.Count >= 1) {
                var allButLast = expressions.Take(expressions.Count - 1).ToList();
                object instanceToSet = GetValue(instance, allButLast);
                string expr = expressions.Last();
                SetValue(instanceToSet, expr, value);

            } else
                throw new ArgumentException("Can't set value from empty expression!");
        }

        private void SetValue(object instance, string expr, object value) {
            Type t = instance.GetType();
            if (IsMethod(expr)) {
                throw new ArgumentException("Attempt to set a method to a value!");

            } else if (IsIndexer(expr)) {
                if (t.IsArray) {
                    string iArg = GetIndexerArg(expr);
                    object[] args = ConvertToIndexerIndexType(iArg);
                    Array ar = (Array)instance;
                    ar.SetValue(value, args.Select(x => (int)x).ToArray());
                    return;

                } else {
                    MemberInfo[] info = t.GetDefaultMembers();
                    string iArg = GetIndexerArg(expr);
                    object[] args = ConvertToIndexerIndexType(iArg);

                    if (info.Length == 1) {
                        PropertyInfo pinfo = (PropertyInfo)info[0];
                        pinfo.SetValue(instance, value, args);

                    } else if (info.Length > 1) {
                        foreach (MemberInfo mi in info) {
                            try {
                                PropertyInfo pinfo = (PropertyInfo)mi;
                                pinfo.SetValue(instance, value, args);
                                return;
                            } catch { }
                        }
                        throw new ArgumentException(string.Format("found no indexer with index parmater type String or Int32 on type {0}", t));
                    }
                }
                throw new ArgumentException(string.Format("found no indexer on type {0}", t));

            } else {
                PropertyInfo pinfo = t.GetProperty(expr);
                if (pinfo != null) {
                    pinfo.SetValue(instance, value, null);
                    return;
                }

                FieldInfo finfo = t.GetField(expr);
                if (finfo != null) {
                    finfo.SetValue(instance, value);
                    return;
                }
            }
            throw new ArgumentException(string.Format("No property '{0}' found on type {1}", expr, t));
        }

        private object GetValueFor(object instance, string expr) {
            object val = null;
            Type t = instance != null ? instance.GetType() : null;
            if (IsMethod(expr)) {
                val = GetMethodValue(instance, expr, t);

            } else if (IsIndexer(expr)) {
                val = GetIndexerValue(instance, expr, t);

            } else {
                val = GetFieldOrPropertyValue(instance, expr, t);

            }

            return val;
        }

        private object GetMethodValue(object instance, string expr, Type t) {
            List<string> methodParameters = new List<string>();
            string methodName = GetMethodAndParameters(expr, methodParameters);

            var actualParameters = GetParameterValues(methodParameters);
            var actualParamsTypes = actualParameters.Select(x => x.GetType());

            MethodInfo minfo = t.GetMethod(methodName, actualParamsTypes.ToArray());
            if (minfo != null)
                return minfo.Invoke(instance, actualParameters.ToArray());
            else {
                var methods = t.GetMethods().Where(x => x.Name == methodName).ToList();
                if (methods.Count == 0)
                    throw new ArgumentException(string.Format("No method found with name {0} on type {1}", methodName, t));
                else {
                    StringBuilder builder = new StringBuilder("No matching method found for" + expr).AppendLine();
                    foreach (var item in methods) {
                        builder.AppendFormat("{0} {1}(", item.ReflectedType, item.Name);
                        ParameterInfo[] parameters = item.GetParameters();
                        for (int i = 0; i < parameters.Length; i++) {
                            builder.Append(parameters[0].ParameterType);
                            if (i < parameters.Length - 1)
                                builder.Append(", ");
                        }
                        builder.Append(")").AppendLine();
                    }
                    throw new ArgumentException(builder.ToString());
                }
            }
        }

        private object GetIndexerValue(object instance, string expr, Type t) {
            if (t.IsArray) {
                string iArg = GetIndexerArg(expr);
                object[] args = ConvertToIndexerIndexType(iArg);
                Array ar = (Array)instance;
                return ar.GetValue(args.Select(x => (int)x).ToArray());

            } else {
                MemberInfo[] info = t.GetDefaultMembers();
                string iArg = GetIndexerArg(expr);
                object[] args = ConvertToIndexerIndexType(iArg);

                if (info.Length == 1) {
                    PropertyInfo pinfo = (PropertyInfo)info[0];
                    return pinfo.GetValue(instance, args);

                } else if (info.Length > 1) {
                    foreach (MemberInfo mi in info) {
                        try {
                            PropertyInfo pinfo = (PropertyInfo)mi;
                            return pinfo.GetValue(instance, args);
                        } catch { }
                    }
                    throw new ArgumentException(string.Format("found no indexer with index parmater type String or Int32 on type {0}", t));
                }
            }

            throw new ArgumentException(string.Format("found no indexer on type {0}", t));
        }

        private object GetFieldOrPropertyValue(object instance, string expr, Type t) {
            PropertyInfo pinfo = t.GetProperty(expr);
            if (pinfo != null)
                return pinfo.GetValue(instance, null);

            FieldInfo finfo = t.GetField(expr);
            if (finfo != null)
                return finfo.GetValue(instance);

            throw new ArgumentException(string.Format("No property or field '{0}' found on type {1}", expr, t));
        }

        private string GetMethodAndParameters(string expr, List<string> methodParameters) {
            int startOfParameters = expr.IndexOf("(");
            string method = expr.Substring(0, startOfParameters);
            string parameterSequence = expr.Substring(startOfParameters + 1, expr.Length - startOfParameters - 2);

            if (parameterSequence.Length > 0) {
                string[] parameters = parameterSequence.Split(',');
                foreach (string parameter in parameters) {
                    if (string.IsNullOrEmpty(parameter))
                        throw new ArgumentException(string.Format("Malformed method parameter list encountered for '{0}'", expr));

                    methodParameters.Add(parameter.Trim());
                }
            }
            return method;
        }

        private IEnumerable<object> GetParameterValues(List<string> methodParameters) {
            List<object> result = new List<object>();

            foreach (string item in methodParameters) {
                object val = item.StartsWith("@@") ?
                    GetEnv(item.Substring(2)) :
                    InstantiateLiteral(item);
                result.Add(val);
            }

            return result;
        }

        private object[] ConvertToIndexerIndexType(string indexArgument) {
            string[] tokens = indexArgument.Split(',');
            object[] result = new object[tokens.Length];

            for (int i = 0; i < tokens.Length; i++) {
                string arg = tokens[i].Trim();
                if (arg.StartsWith("\"") && arg.EndsWith("\""))
                    result[i] = arg.Substring(1, arg.Length - 2);
                else
                    result[i] = Instantiate(typeof(Int32), arg);
            }
            return result;
        }

        private bool IsFieldOrProperty(string expr) {
            return !(IsIndexer(expr) || IsMethod(expr));
        }

        private bool IsMethod(string expr) {
            return expr.Contains("(") && expr.EndsWith(")");
        }

        private bool IsIndexer(string expr) {
            return expr.StartsWith("[") && expr.EndsWith("]");
        }

        private bool IsTypename(string name) {
            return mscorlib.GetType(name) != null;
        }

        private string GetIndexerArg(string indexExpr) {
            return indexExpr.Substring(1, indexExpr.Length - 2).Trim();
        }
        #endregion
    }

    public class MetaInfo {
        public string Name { get; internal set; }
        public int Arity { get; internal set; }
        public ClassTypes Type { get; internal set; }
        public MemberInfo Info { get; internal set; }
    }
    
    public enum ClassTypes { Event, Field, Method, Property }
  
    
}
