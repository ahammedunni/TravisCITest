using ExpressBase.Common;
using ExpressBase.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Data;

namespace ExpressBase.Security
{
    public class Role : RBACBase
    {
        private int _id;
        private string _name;
        public PermissionCollection PermissionCollection { get; private set; }
        public PermissionCollection _addedPermissions { get; set; }
        public PermissionCollection _removedPermissions { get; set; }

        public RoleCollection RoleCollection { get; private set; }
        private RoleCollection _addedRoles { get; set; }
        private RoleCollection _removedRoles { get; set; }

        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        public void Rename(string newname)
        {
            this.Name = newname;
        }

        public Role(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.PermissionCollection = new PermissionCollection();
            this._addedPermissions = new PermissionCollection();
            this._removedPermissions = new PermissionCollection();
            this.RoleCollection = new RoleCollection();
            this._addedRoles = new RoleCollection();
            this._removedRoles = new RoleCollection();
        }

        ~Role()
        {
            this.PermissionCollection.Clear();
            this.PermissionCollection = null;

            this._addedPermissions.Clear();
            this._addedPermissions = null;

            this._removedPermissions.Clear();
            this._removedPermissions = null;
        }

        public static Role Create(string rolename)
        {
            var dt = df.ObjectsDB.DoQuery(string.Format(df.ObjectsDB.INSERT_EB_ROLES, rolename));
            return new Role(Convert.ToInt32(dt.Rows[0][0]), rolename);
        }

        public void Save()
        {
            string sql = string.Format(df.ObjectsDB.RENAME_ROLE_IN_EB_ROLES, this.Name, this.Id);     //re-naming
            sql += _removedPermissions.GetUpdate_Permission2Role_Sql(this.Id);                                       //removing
            sql += _addedPermissions.GetInsert_Permission2Role_Sql(this.Id);                                         //adding
            df.ObjectsDB.DoQuery(sql);
        }

        public void SaveRoles()
        {
            string sql = _addedRoles.GetInsert_Role2Role_sql(this.Id);
            sql += _removedRoles.GetUpdate_Role2Role_sql(this.Id);
            df.ObjectsDB.DoQuery(sql);
        }

        public void LoadRole()
        {
            // Loading role collection(role2role)
            string sql = string.Format(df.ObjectsDB.LOAD_ROLE_FROM_EB_ROLES, this.Id);
            var dt = df.ObjectsDB.DoQuery(sql);

            // this.PermissionCollection=
            foreach (EbDataRow dr in dt.Rows)
            {
                int rid = Convert.ToInt32(dr[0]);
                string rname = dr[1].ToString();
                Role r = new Role(rid, rname);
                this.RoleCollection.Add(r);
            }
            // loading permissions

            sql = string.Format(df.ObjectsDB.LOAD_PERMISSION_FROM_EB_PERMISSION, this.Id);
            dt = df.ObjectsDB.DoQuery(sql);
            foreach (EbDataRow dr in dt.Rows)
            {
                int p_id = Convert.ToInt32(dr[0]);
                int o_id = Convert.ToInt32(dr[1]);
                int op_id = Convert.ToInt32(dr[2]);

                Permission p = new Permission(p_id, o_id, op_id);
                this.PermissionCollection.Add(p);
            }
            // loading roleCollection( role2user)
        }

        public void AddPermissions(PermissionCollection pcol)                                                        // assigning permissions to a role
        {
            foreach (Permission p in pcol)
            {
                this.PermissionCollection.Add(p);
                this._addedPermissions.Add(p);
            }
        }

        public void RemovePermissions(PermissionCollection pcol)
        {
            foreach (Permission p in pcol)
            {
                this.PermissionCollection.Remove(p);
                this._removedPermissions.Add(p);
            }
        }
        //------------for adding into roleCollection

        public void AddRoles(RoleCollection rcol)
        {
            foreach (Role r in rcol)
            {
                this.RoleCollection.Add(r);
                this._addedRoles.Add(r);
            }
        }
        //------------for removing from roleCollection

        public void RemoveRoles(RoleCollection rcol)
        {
            foreach (Role r in rcol)
            {
                this.RoleCollection.Remove(r);
                this._removedRoles.Add(r);
            }
        }
        //public override int GetHashCode()
        //{
        //    return this._id;
        //}

        public override bool Equals(object obj)
        {
            Role r = obj as Role;
            return r.Id == this.Id;
        }
        public static void Delet(int roleid)
        {
            string sql = string.Format(df.ObjectsDB.DELETE_ROLE_FROM_EB_ROLES, roleid);
            sql += string.Format(df.ObjectsDB.DELETE_ROLE_FROM_EB_ROLE2ROLE, roleid);
            sql += string.Format(df.ObjectsDB.DELETE_ROLE_FROM_EB_ROLE2USER, roleid);
            sql += string.Format(df.ObjectsDB.DELETE_ROLE_FROM_EB_PERMISSION2ROLE, roleid);
            df.ObjectsDB.DoQuery(sql);
        }
    }

    public class RoleCollection : List<Role>
    {
        new public void Add(Role r)
        {
            if (!this.Contains(r))
            {
                try
                {
                    base.Add(r);
                }
                catch
                {

                }

            }

            else
                throw new RoleAlreadyFoundInRoleCollectonException();
        }

        new public void Remove(Role r)
        {
            if (this.Contains(r))
            {
                base.Remove(r);
            }
            else
                throw new RoleNotFoundInRoleCollectionException();
        }

        internal string GetInsert_Role2Role_sql(int role_id)
        {
            string[] sa = new string[this.Count];
            int i = 0;
            foreach (Role r in this)
            {
                sa[i++] = string.Format("({0}, {1})", role_id, r.Id);
            }
            string _instr = string.Join(", ", sa);
            return ((!string.IsNullOrEmpty(_instr)) ? "INSERT INTO eb_role2role (role1_id,role2_id) VALUES " + _instr + ";" : string.Empty);
        }

        internal string GetUpdate_Role2Role_sql(int role_id)
        {
            int[] sa = new int[this.Count];
            int i = 0;
            foreach (Role r in this)
            {
                sa[i++] = r.Id;
            }
            string _instr = string.Join<int>(", ", sa);
            return (!string.IsNullOrEmpty(_instr) ? (string.Format("UPDATE eb_role2role SET eb_del = true WHERE role1_id = {0} AND role2_id IN ({1});  ", role_id, _instr)) : string.Empty);
        }

        internal string GetInsert_User2role(int user_id)
        {
            string[] sa = new string[this.Count];
            int i = 0;
            foreach (Role r in this)
            {
                sa[i++] = string.Format("({0},{1})", r.Id, user_id);
            }
            string _instr = string.Join(", ", sa);
            return (!string.IsNullOrEmpty(_instr) ? (string.Format(" INSERT INTO eb_role2user ( role_id, user_id ) VALUES" + _instr + ";")) : string.Empty);
        }

        internal string GetUpdate_user2role_sql(int user_id)
        {
            int[] ia = new int[this.Count];
            int i = 0;
            foreach (Role r in this)
            {
                ia[i++] = r.Id;
            }
            string _instr = string.Join<int>(", ", ia);
            return (!string.IsNullOrEmpty(_instr) ? (string.Format("UPDATE eb_role2user SET eb_del = true WHERE role_id = {0} AND user_id = {1} ;", _instr, user_id)) : string.Empty);
        }

        public bool HasSystemRole()
        {
            foreach (var Role in this)
            {
              if(HasRole(Role.Id))
                {
                    return true;
                }
            }
            return false;

        }

        public bool HasRole(int role_id)
        {
            var Enumvalues = Enum.GetValues(typeof(SystemRoles));
            foreach (var value in Enumvalues)
            {
                if (role_id == (int)value)
                {
                    return true;
                }
            }
            return false;
        }
    }

    internal class RoleNotFoundInRoleCollectionException : Exception
    {
        public RoleNotFoundInRoleCollectionException()
        {
        }

        public RoleNotFoundInRoleCollectionException(string message) : base(message)
        {
        }

        public RoleNotFoundInRoleCollectionException(string message, Exception innerException) : base(message, innerException)
        {
        }


    }


    internal class RoleAlreadyFoundInRoleCollectonException : Exception
    {
        public RoleAlreadyFoundInRoleCollectonException()
        {
        }

        public RoleAlreadyFoundInRoleCollectonException(string message) : base(message)
        {
        }

        public RoleAlreadyFoundInRoleCollectonException(string message, Exception innerException) : base(message, innerException)
        {
        }


    }
}
