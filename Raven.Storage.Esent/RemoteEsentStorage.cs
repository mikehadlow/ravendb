﻿using System;
using Microsoft.Isam.Esent.Interop;
using Raven.Database.Plugins;
using Raven.Database.Storage;
using Raven.Storage.Esent.StorageActions;

namespace Raven.Storage.Esent
{
    public class RemoteEsentStorage : IRemoteStorage
    {
        private readonly JET_INSTANCE instance;
        private readonly TableColumnsCache tableColumnsCache;
        private string database;

        public RemoteEsentStorage(RemoteEsentStorageState state)
        {
            instance = state.Instance;
            database = state.Database;
            tableColumnsCache = new TableColumnsCache();
            tableColumnsCache.InitColumDictionaries(instance, database);
        }

        public void Batch(Action<IStorageActionsAccessor> action)
        {
            using (var pht = new DocumentStorageActions(instance, database, tableColumnsCache, new AbstractDocumentCodec[0]))
            {
                action(new StorageActionsAccessor(pht));
            }
        }
    }
}