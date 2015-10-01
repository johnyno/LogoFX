﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using LogoFX.Client.Mvvm.Model.Contracts;

namespace LogoFX.Client.Mvvm.Model
{
    /// <summary>
    /// Represents collection of models, supporting collection change notifications
    /// </summary>
    /// <typeparam name="TItem">Type of model</typeparam>
    public class ModelsCollection<TItem> : ModelsCollectionBase, IModelsCollection<TItem>
    {
        private readonly ObservableCollection<TItem> _items = new ObservableCollection<TItem>();
        private ObservableCollection<TItem> Items
        {
            get { return _items; }
        }

        IEnumerable<TItem> IReadModelsCollection<TItem>.Items
        {
            get { return Items; }
        }

        public override int ItemsCount { get { return Items.Count; } }
        public override bool HasItems { get { return ItemsCount > 0; } }
        public void Add(TItem item)
        {
            Items.Add(item);
            SafeRaiseHasItemsChanged();
        }

        public void Remove(TItem item)
        {
            Items.Remove(item);
            SafeRaiseHasItemsChanged();
        }

        public void Update(IEnumerable<TItem> items)
        {
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
            SafeRaiseHasItemsChanged();
        }

        public void Clear()
        {
            Items.Clear();
            SafeRaiseHasItemsChanged();
        }
    }
}
