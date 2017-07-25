using ExpressBase.Common;
using ExpressBase.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Data;

namespace ExpressBase.Security
{
    public class Permission : RBACBase
    {
        private int _id;
        private int _object_id;
        private int _operation_id;

        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public int Object_id
        {
            get { return _object_id; }
            private set { _object_id = value; }
        }

        public int Operation_id
        {
            get { return _operation_id; }
            private set { _operation_id = value; }
        }

        public override bool Equals(object obj)
        {
            Permission p = obj as Permission;
            return p.Id == this.Id;
        }

        public Permission(int id, int object_id, int operation_id)
        {
            this.Id = id;
            this.Object_id = object_id;
            this.Operation_id = operation_id;
        }

        public static PermissionCollection Create(int object_id, int[] operation_ids)
        {
            string[] sa = new string[operation_ids.Length];

            PermissionCollection pc = new PermissionCollection();

            foreach (int operation_id in operation_ids)
            {

                // sa[i++] = string.Format("({0}, {1})", object_id, operation_id);
                var dt = df.ObjectsDB.DoQuery(string.Format(df.ObjectsDB.INSERT_EB_PERMISSIONS, object_id, operation_id));
                pc.Add(new Permission(Convert.ToInt32(dt.Rows[0][0]), object_id, operation_id));
            }

            return pc;

        }
        public static void Delet(int permissionid)
        {
            string sql = string.Format(df.ObjectsDB.DELETE_PER_FROM_EB_PERMISSIONS, permissionid);
            sql += string.Format(df.ObjectsDB.DELETE_PER_FROM_EB_PERMISSION2ROLE, permissionid);
            df.ObjectsDB.DoQuery(sql);
        }
    }

    public class PermissionCollection : List<Permission>
    {
        new public void Add(Permission p)                                      //to add  new permissions into permission collection
        {
            if (!this.Contains(p))
                base.Add(p);
            else
                throw new PermissionAlreadyFoundInCollectionException();
        }

        new public void Remove(Permission p)                                   //to delete  a permission from permission collection
        {
            if (this.Contains(p))
                base.Remove(p);
            else
                throw new PermissionNotFoundInCollectionException();
        }

        internal string GetInsert_Permission2Role_Sql(int role_id)             // grant  permissions to a role
        {
            string[] sa = new string[this.Count];
            int i = 0;
            foreach (Permission p in this)
            {
                sa[i++] = string.Format("({0}, {1})", p.Id, role_id);
            }

            string _in_str = string.Join(", ", sa);
            return ((!string.IsNullOrEmpty(_in_str)) ? "INSERT INTO eb_permission2role (permission_id, role_id) VALUES " + _in_str + ";" : string.Empty);
        }

        internal string GetUpdate_Permission2Role_Sql(int role_id)              //to revoke  permissions of a role
        {
            int[] sa = new int[this.Count];
            int i = 0;
            foreach (Permission p in this)
                sa[i++] = p.Id;

            string _in_str = string.Join<int>(", ", sa);

            return ((!string.IsNullOrEmpty(_in_str)) ? string.Format("UPDATE eb_permission2role SET eb_del=TRUE WHERE role_id={0} AND permission_id IN ({1});", role_id, _in_str) : string.Empty);
        }

        private class PermissionAlreadyFoundInCollectionException : Exception
        {
            public PermissionAlreadyFoundInCollectionException()
            {
            }

            public PermissionAlreadyFoundInCollectionException(string message) : base(message)
            {
            }

            public PermissionAlreadyFoundInCollectionException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        private class PermissionNotFoundInCollectionException : Exception
        {
            public PermissionNotFoundInCollectionException()
            {
            }

            public PermissionNotFoundInCollectionException(string message) : base(message)
            {
            }

            public PermissionNotFoundInCollectionException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }



}
