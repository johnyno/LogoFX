﻿using System.Collections.Generic;

namespace LogoFX.Client.Mvvm.Model.Contracts
{
    public interface IWriteModelsCollection<TItem>
    {
        void Add(TItem item);
        void Remove(TItem item);
        void Update(IEnumerable<TItem> items);
        void Clear();
    }
}