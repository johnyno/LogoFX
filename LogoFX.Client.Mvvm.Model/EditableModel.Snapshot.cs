﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using LogoFX.Client.Mvvm.Model.Contracts;
using LogoFX.Core;

namespace LogoFX.Client.Mvvm.Model
{
    public partial class EditableModel<T>
    {
        protected sealed class Snapshot
        {
            private readonly IList<ValidationResult> _validationErrors;

            private readonly IDictionary<PropertyInfo, object> _state = new Dictionary<PropertyInfo, object>();

            private readonly IDictionary<PropertyInfo, IList<object>> _listsState = new Dictionary<PropertyInfo, IList<object>>();

            private readonly bool _isDirty;

            public Snapshot(EditableModel<T> model)
            {
                var storableProperties = TypeInformationProvider.GetStorableProperties(model.GetType());                
                foreach (PropertyInfo propertyInfo in storableProperties)
                {
                    if (propertyInfo.IsDefined(typeof(EditableListAttribute), true) &&
                        typeof(IList).IsAssignableFrom(propertyInfo.GetValue(model, null).GetType()))
                    {
                        _listsState.Add(new KeyValuePair<PropertyInfo, IList<object>>(propertyInfo,
                            new List<object>(((IList)propertyInfo.GetValue(model, null)).OfType<object>())));
                    }
                    else if (propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.GetSetMethod() != null)
                    {
                        _state.Add(new KeyValuePair<PropertyInfo, object>(propertyInfo,
                                                                          propertyInfo.GetValue(model, null)));
                    }
                }
                _validationErrors = model.ValidationErrors;
                _isDirty = model.IsDirty;
            }

            public bool IsDirty
            {
                get { return _isDirty; }
            }

            public static Snapshot Take(EditableModel<T> model)
            {
                return new Snapshot(model);
            }

            public void Restore(EditableModel<T> model)
            {
                foreach (KeyValuePair<PropertyInfo, object> result in _state)
                {
                    if (result.Key.GetCustomAttributes(typeof(EditableSingleAttribute), true).Any() && result.Value is ICloneable<object>)
                    {
                        result.Key.SetValue(model, (result.Value as ICloneable<object>).Clone(), null);
                    }
                    else
                    {
                        result.Key.SetValue(model, result.Value, null);
                    }
                }

                foreach (KeyValuePair<PropertyInfo, IList<object>> result in _listsState)
                {
                    IList il = (IList)result.Key.GetValue(model, null);
                    //TODO:optimize this
                    il.Clear();
                    if (((EditableListAttribute)result.Key.GetCustomAttributes(typeof(EditableListAttribute), true)[0]).CloneItems)
                        result.Value.ForEach(a => il.Add(a is ICloneable<object> ? ((ICloneable<object>)a).Clone() : a));
                    else
                        result.Value.ForEach(a => il.Add(a));                    
                }
                model.ValidationErrors = _validationErrors;
#if SILVERLIGHT
                model.NotifyOfErrorsChanged(new DataErrorsChangedEventArgs(null));
#else
                model.NotifyOfPropertyChange(() => model.Error);
#endif
                model.OwnDirty = IsDirty;
            }
        }
    }
}
