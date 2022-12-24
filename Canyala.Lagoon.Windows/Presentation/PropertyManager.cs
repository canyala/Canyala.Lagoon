//
// Copyright (c) 2013 Canyala Innovation AB
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Command;

namespace Canyala.Lagoon.Presentation
{
    public class PropertyManager : DynamicObject
    {
        private readonly IViewModel _viewModel;
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        internal PropertyManager(IViewModel viewModel)
        { _viewModel = viewModel; }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!_properties.ContainsKey(binder.Name))
            {
                _properties.Add(binder.Name, value);
                return true;
            }

            if (_properties[binder.Name].Equals(value) == false)
            {
                _properties[binder.Name] = value;
                _viewModel.NotifyChanged(binder.Name);
                PropagateNotifyChanged(_viewModel, binder.Name);
            }

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_properties.TryGetValue(binder.Name, out result))
                return true;

            return false;
        }

        private static void PropagateNotifyChanged(IViewModel viewModel, string propertyPath)
        {
            var modelType = viewModel.GetType();

            foreach (var property in modelType.GetProperties())
            {
                foreach (var dependencyAttribute in property.GetCustomAttributes<DependsOnAttribute>())
                {
                    if (dependencyAttribute.HasDependencyOn(propertyPath))
                    {
                        if (property.PropertyType.IsAssignableFrom(typeof(DelegateCommand)))
                        {
                            var command = property.GetValue(viewModel, null) as DelegateCommand;
                            if (command != null) command.RaiseCanExecute();
                        }
                        else
                            viewModel.NotifyChanged(property.Name);
                    }

                    if (viewModel.Parent != null)
                        viewModel
                            .Parent
                            .GetType()
                            .GetProperties()
                            .Where(p => p.PropertyType == modelType)
                            .Do(parentProperty => PropagateNotifyChanged(viewModel.Parent, "{0}.{1}".Args(parentProperty.Name, propertyPath)));
                }
            }
        }
    }
}
