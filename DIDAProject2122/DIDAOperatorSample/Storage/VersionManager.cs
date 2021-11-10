using System.Collections.Generic;
using System.Linq;

namespace Storage
{
    class VersionManager
    {
        private Dictionary<int, DIDARecord> versions;

        public VersionManager(string id, string value)
        {
            var version = new DIDAVersion { ReplicaId = -1, VersionNumber = 1 };
            var record = new DIDARecord { Id = id, Val = value, Version = version };
            versions = new Dictionary<int, DIDARecord>
            {
                [1] = record
            };
        }

        public DIDARecord GetHighestRecord()
        {
            var highestVersion = versions.Keys.Max();
            return versions[highestVersion];
        }

        public DIDARecord GetRecord(int versionNumber)
        {
            return versions[versionNumber];
        }

        private DIDAVersion IncrementVersion(DIDAVersion oldVersion)
        {
            var id = oldVersion.ReplicaId;
            var newVersionNum = oldVersion.VersionNumber + 1;
            return new DIDAVersion { ReplicaId = id, VersionNumber = newVersionNum };
        }

        public DIDARecord UpdateRecord(string newVal)
        {
            var oldRecord = GetHighestRecord();
            var id = oldRecord.Id;
            var highestVersion = oldRecord.Version.VersionNumber;
            var newVersion = IncrementVersion(oldRecord.Version);
            var newRecord = new DIDARecord { Id = id, Version = newVersion, Val = newVal };
            versions[highestVersion] = newRecord;
            return newRecord;
        }

        public override string ToString()
        {
            string res = "";
            foreach(KeyValuePair<int, DIDARecord> item in versions)
            {
                res += item.Value + "\n";
            }
            res = res.Trim();
            return res;
        }
    }
}
