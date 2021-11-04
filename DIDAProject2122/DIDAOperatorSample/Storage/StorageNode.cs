﻿using System;
using System.Collections.Generic;

namespace Storage
{
    class StorageNode
    {
        private readonly RecordStorage recordStorage;
        private readonly DIDAVersion emptyVersion;
        private readonly DIDARecord emptyRecord;
        private readonly int replicaId;

        public DIDAVersion EmptyVersion => emptyVersion;

        public DIDARecord EmptyRecord => emptyRecord;

        public StorageNode(int replicaId)
        {
            emptyVersion = new DIDAVersion { ReplicaId = -1, VersionNumber = -1 };
            emptyRecord = new DIDARecord { Id = "", Val = "", Version = emptyVersion };
            this.recordStorage = new RecordStorage();
            this.replicaId = replicaId;
        }

        public DIDARecord read(string id, DIDAVersion version)
        {
            lock (recordStorage)
            {
                if (version.Equals(emptyVersion))
                {
                    return recordStorage.GetRecord(id);
                }
                else
                {
                    return recordStorage.GetRecord(id, version);
                }
            }
        }

        public DIDAVersion write(string id, string val)
        {
            lock(recordStorage)
            {
                return recordStorage.WriteRecord(id, val);
            }
        }

        public DIDAVersion updateIfValueIs(string id, string oldvalue, string newvalue)
        {
            lock(recordStorage)
            {
                return recordStorage.UpdateIfValue(id, oldvalue, newvalue);
            }
        }
    }
}
