﻿using ExpressBase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressBase.Data
{
    public class DatabaseFactory
    {
        private IEbConf _config;

        private IDatabase _InfraDB = null;
        private IDatabase _InfraDB_RO = null;
        private IDatabase _ObjectsDB = null;
        private IDatabase _DataDatabase = null;
        private IDatabase _LogsDatabase = null;
        private IDatabase _FilesDatabase = null;
        private IDatabase _ObjectsDB_RO = null;
        private IDatabase _DataDatabase_RO = null;
        private IDatabase _LogsDatabase_RO = null;
        private IDatabase _FilesDatabase_RO = null;

        public IDatabase InfraDB { get { return _InfraDB; } }

        public IDatabase InfraDB_RO { get { return _InfraDB_RO; } }

        public IDatabase ObjectsDB { get { return _ObjectsDB; } }

        public IDatabase DataDB { get { return _DataDatabase; } }

        public IDatabase LogsDB { get { return _LogsDatabase; } }

        public IDatabase FilesDB { get { return _FilesDatabase; } }

        public IDatabase ObjectsDBRO { get { return _ObjectsDB_RO; } }

        public IDatabase DataDBRO { get { return _DataDatabase_RO; } }

        public IDatabase LogsDBRO { get { return _LogsDatabase_RO; } }

        public IDatabase FilesDBRO { get { return _FilesDatabase_RO; } }

        public DatabaseFactory(IEbConf config)
        {
            _config = config;
            InitDatabases();
        }

        private void InitDatabases()
        {
            foreach (EbDatabaseConfiguration dbconf in _config.DatabaseConfigurations.Values)
            {
                var databaseType = dbconf.DatabaseVendor;

                switch (databaseType)
                {
                    case DatabaseVendors.PGSQL:
                        if (dbconf.EbDatabaseType == EbDatabaseTypes.EbINFRA)
                            _InfraDB = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbINFRA_RO)
                            _InfraDB_RO = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbOBJECTS)
                            _ObjectsDB = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbDATA)
                            _DataDatabase = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbLOGS)
                            _LogsDatabase = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbFILES)
                            _FilesDatabase = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbOBJECTS_RO)
                            _ObjectsDB_RO = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbDATA_RO)
                            _DataDatabase_RO = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbLOGS_RO)
                            _LogsDatabase_RO = new PGSQLDatabase(dbconf);
                        else if (dbconf.EbDatabaseType == EbDatabaseTypes.EbFILES_RO)
                            _FilesDatabase_RO = new PGSQLDatabase(dbconf);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}

