﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Storage
{
    class RecordStorage //FIXME only supports one version of each record
    {
        private readonly Dictionary<string, VersionManager> records;
        private readonly DIDAVersion emptyVersion;
        private readonly DIDARecord emptyRecord;

        internal Dictionary<string, VersionManager> Records => records;

        public DIDAVersion EmptyVersion => emptyVersion;

        public DIDARecord EmptyRecord => emptyRecord;

        public RecordStorage()
        {
            records = new Dictionary<string, VersionManager>();
            emptyVersion = new DIDAVersion { ReplicaId = -1, VersionNumber = -1 };
            emptyRecord = new DIDARecord { Id = "", Val = "", Version = emptyVersion };
        }

        public DIDARecord GetRecord(string id)
        {
            try
            {
                return records[id].GetHighestRecord();
            } 
            catch(KeyNotFoundException e)
            {
                return EmptyRecord;
            }
        }

        public DIDARecord GetRecord(string id, DIDAVersion version)
        {
            try
            {
                return Records[id].GetRecord(version.VersionNumber);
            }
            catch(KeyNotFoundException e)
            {
                return EmptyRecord;
            }
        }

        public DIDAVersion WriteRecord(string id, string newValue)
        {
            try
            {
                return records[id].UpdateRecord(newValue).Version;
            }
            catch(KeyNotFoundException e)
            {
                records[id] = new VersionManager(id, newValue);
                return records[id].GetHighestRecord().Version;
            }
        }
    }
}
