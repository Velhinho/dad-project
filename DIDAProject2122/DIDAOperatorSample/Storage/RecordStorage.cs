using System;
using System.Collections.Generic;
using System.Text;

namespace Storage
{
    class RecordStorage //FIXME only supports one version of each record
    {
        private Dictionary<string, DIDARecord> records;

        public RecordStorage()
        {
            records = new Dictionary<string, DIDARecord>();
            var version = new DIDAVersion { ReplicaId = 1, VersionNumber = 1 };
            records["a"] = new DIDARecord { Id = "a", Val = "1", Version = version };
            records["b"] = new DIDARecord { Id = "b", Val = "2", Version = version };
            records["c"] = new DIDARecord { Id = "c", Val = "3" , Version = version };
            records["Epic"]  = new DIDARecord { Id = "Epic", Val = "42", Version = version };
        }

        public DIDARecord GetRecord(string id)
        {
            return records[id];
        }

        public DIDARecord GetRecord(string id, DIDAVersion version)
        {
            try
            {
                return records[id];
            }
            catch
            {
                Console.WriteLine("Invalid Id");
                throw new KeyNotFoundException(id);
            }
        }

        private DIDAVersion UpdateVersion(DIDAVersion oldVersion)
        {
            var id = oldVersion.ReplicaId;
            var newVersionNum = oldVersion.VersionNumber + 1;
            return new DIDAVersion { ReplicaId = id, VersionNumber = newVersionNum };
        }

        private DIDARecord UpdateRecord(DIDARecord oldRecord, string newVal)
        {
            var id = oldRecord.Id;
            var newVersion = UpdateVersion(oldRecord.Version);
            return new DIDARecord { Id = id, Version = newVersion, Val = newVal };
        }

        public DIDAVersion WriteRecord(string id, string newValue)
        {
            DIDARecord record = null;
            try
            {
                record = GetRecord(id); 
            }
            catch(KeyNotFoundException e)
            {
                record = new DIDARecord { Id = id, Val = newValue, Version = new DIDAVersion { ReplicaId = 1, VersionNumber = 1 } };
            }
            var newRecord = UpdateRecord(record, newValue);
            records[id] = newRecord;
            Console.Write("Wrote a new record!");
            return newRecord.Version;
        }
    }
}
