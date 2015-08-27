﻿using System;
using LogoFX.Client.Mvvm.Model.Contracts;

namespace LogoFX.Client.Mvvm.Model
{
    public abstract class ModelsCollectionBase : IInfoModelsCollection
    {
        public abstract int ItemsCount { get; }
        public abstract bool HasItems { get; }
        public event EventHandler HasItemsChanged;

        protected void SafeRaiseHasItemsChanged()
        {
            if (HasItemsChanged != null)
            {
                HasItemsChanged(this, new EventArgs());
            }
        }
    }
}