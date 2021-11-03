using System.Collections.Generic;
using System.Linq;

namespace Storage
{
    class VersionManager
    {
        private readonly List<DIDARecord> versions;

        public VersionManager(string id, string value)
        {
            var version = new DIDAVersion { ReplicaId = -1, VersionNumber = 0 };
            var record = new DIDARecord { Id = id, Val = value, Version = version };
            versions = new List<DIDARecord>
            {
                record
            };
        }

        public DIDARecord GetHighestRecord()
        {
            var highestVersion = versions.Select(record => record.Version.VersionNumber).Max();
            return versions.Where(record => record.Version.VersionNumber == highestVersion)
                .FirstOrDefault();
        }

        public DIDARecord GetRecord(int versionNumber)
        {
            var record = versions.Where((record) => record.Version.VersionNumber == versionNumber)
                .FirstOrDefault();
            return record;
        }

        private DIDAVersion UpdateVersion(DIDAVersion oldVersion)
        {
            var id = oldVersion.ReplicaId;
            var newVersionNum = oldVersion.VersionNumber + 1;
            return new DIDAVersion { ReplicaId = id, VersionNumber = newVersionNum };
        }

        public DIDARecord UpdateRecord(string newVal)
        {
            var oldRecord = GetHighestRecord();
            var id = oldRecord.Id;
            var newVersion = UpdateVersion(oldRecord.Version);
            var newRecord = new DIDARecord { Id = id, Version = newVersion, Val = newVal };
            versions.Append(newRecord);
            return newRecord;
        }
    }
}
