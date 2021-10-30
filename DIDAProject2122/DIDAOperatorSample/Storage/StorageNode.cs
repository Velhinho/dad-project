using System;
using System.Collections.Generic;

namespace Storage
{
    class StorageNode
    {
        private readonly RecordStorage recordStorage;

        public StorageNode()
        {
            this.recordStorage = new RecordStorage();
        }

        public DIDARecord read(string id, DIDAVersion version)
        {
            lock (recordStorage)
            {
                if (version.Equals(null))
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
            throw new NotImplementedException();
        }
    }
}
