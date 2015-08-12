﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogoFX.Client.Mvvm.Model
{
    partial class TypeInformationProvider
    {
        private static readonly Dictionary<Type,PropertyInfo[]> InnerDictionary = new Dictionary<Type, PropertyInfo[]>(); 
        private static readonly object SyncObject = new object();

        internal static PropertyInfo[] GetStorableProperties(Type type)
        {
            lock (SyncObject)
            {
                if (InnerDictionary.ContainsKey(type) == false)
                {
                    InnerDictionary.Add(type, GetStorablePropertiesImpl(type));
                }    
            }
            
            return InnerDictionary[type];
        }

        private static PropertyInfo[] GetStorablePropertiesImpl(Type type)
        {
            var storableProperties = new HashSet<PropertyInfo>();
            var modelProperties = GetStorableCandidates(type).ToArray();
            foreach (var modelProperty in modelProperties)
            {
                storableProperties.Add(modelProperty);
            }
            var declaredInterfaces = type.GetInterfaces();
            var explicitProperties = declaredInterfaces.SelectMany(GetStorableCandidates);
            foreach (var explicitProperty in explicitProperties)
            {
                storableProperties.Add(explicitProperty);
            }
            return storableProperties.ToArray();
        }

        private static IEnumerable<PropertyInfo> GetStorableCandidates(Type modelType)
        {
            return modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }
    }
}
