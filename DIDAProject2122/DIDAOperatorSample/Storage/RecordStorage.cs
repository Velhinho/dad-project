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
            records["a"] = new DIDARecord { Id = "a", Val = "1"};
            records["b"] = new DIDARecord { Id = "b", Val = "2" };
            records["c"] = new DIDARecord { Id = "c", Val = "3" };
        }

        public DIDARecord GetRecord(string id)
        {
            return records[id];
        }

        public DIDARecord GetRecord(string id, DIDAVersion version)
        {
            return records[id];
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
            var record = GetRecord(id);
            var newRecord = UpdateRecord(record, newValue);
            records[id] = newRecord;
            return newRecord.Version;
        }
    }
}
