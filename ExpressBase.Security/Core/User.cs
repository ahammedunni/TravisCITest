using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ExpressBase.Common;
using ExpressBase.Data;
using Npgsql;
using System.Data.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using ServiceStack.Auth;

namespace ExpressBase.Security
{
    [DataContract]
    public class User : UserAuth
    {
        [DataMember(Order = 1)]
        public string ImageStr { get; set; }

        [DataMember(Order = 12)]
        public RoleCollection _roleCollection;
        [DataMember(Order = 13)]
        public RoleCollection added_rolecollection = new RoleCollection();
        [DataMember(Order = 14)]
        public RoleCollection removed_rolecollection = new RoleCollection();
        [DataMember(Order = 15)]
        public PermissionCollection _permissionCollection;
        [DataMember(Order = 16)]
        public int loginattempts;

        public PermissionCollection PermissionCollection
        {
            get
            {
                if (_permissionCollection == null)
                    _permissionCollection = new PermissionCollection();
                return _permissionCollection;
            }

            private set
            {
                _permissionCollection = value;
            }
        }

        public RoleCollection RoleCollection
        {
            get
            {
                if (_roleCollection == null)
                    _roleCollection = new RoleCollection();
                return _roleCollection;
            }

            private set
            {
                _roleCollection = value;
            }
        }

        //for protobuf - do not remove
        public User() { }

        public User(int id, string uname,string fname)
        {
            this.Email = uname;
            this.Id = id;
        }

        public User(int id, string uname, string fname,string profileimg)
        {
            this.Email = uname;
            this.Id = id;
            this.ImageStr = profileimg;
        }

        public User(int id, string uname, RoleCollection rcol, PermissionCollection pcol)
        {
            this.Id = id;
            this.Email = uname;
            this.RoleCollection = rcol;
            this.PermissionCollection = pcol;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns>bool</returns>
        public static bool IsValidmail(string emailaddress)
        {
            //  return true;
            return (Regex.IsMatch(emailaddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase));
        }

        public static bool Isvalidpassword(string password)
        {
            var input = password;

            var hasPassword = new Regex(@"^(?=.*[0-9])^(?=.*[a-zA-Z])^(?=.*[!#$%&'()*+,-./:;<=>?@[\]^_`{|}~]).{8,}");
            var isValidated = hasPassword.IsMatch(input);
            return isValidated;
        }

        public static string MD5Hash(string text)
        {
            var md5 = MD5.Create();

            //compute hash from the bytes of text
            byte[] result = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public void AddRoles(RoleCollection rcol)
        {
            foreach (Role r in rcol)
            {
                this.RoleCollection.Add(r);
                this.added_rolecollection.Add(r);
            }
        }

        public void removeRoles(RoleCollection rc)
        {
            foreach (Role r in rc)
            {
                this.RoleCollection.Remove(r);
                this.removed_rolecollection.Add(r);
            }
        }

        public bool CheckPermission(Permission p)
        {
            return (this.PermissionCollection.Contains(p)) ? true : false;
        }

        public static User GetInfraUser(IDatabase db, string uname, string pass)
        {
            User _user = null;

            string sql = "UPDATE eb_tenants SET loginattempts = loginattempts + 1 WHERE cname = @cname AND password = @password;";
                 sql +="SELECT id, firstname, profileimg,loginattempts FROM eb_tenants WHERE cname = @cname AND password = @password AND isverified = TRUE";

            DbParameter[] parameters = {
                db.GetNewParameter("cname", System.Data.DbType.String, uname),
                db.GetNewParameter("password", System.Data.DbType.String, pass)
            };

            EbDataTable dt = db.DoQuery(sql, parameters);

            if (dt.Rows.Count != 0)
            {
                _user = new User(Convert.ToInt32(dt.Rows[0][0]), uname, dt.Rows[0][1].ToString(), dt.Rows[0][2].ToString());
                _user.loginattempts = Convert.ToInt32(dt.Rows[0][3]);
            }        

            return _user;
        }

        public static User GetInfraUserViaSocial(IDatabase db, string uname, string socialId)
        {
            User _user = null;

            string sql = "SELECT id, firstname, prolink FROM eb_tenants WHERE cname = @cname AND socialid = @socialid";

            DbParameter[] parameters = {
                db.GetNewParameter("cname", System.Data.DbType.String, uname),
                db.GetNewParameter("socialid", System.Data.DbType.String, socialId)
            };

            EbDataTable dt = db.DoQuery(sql, parameters);

            if (dt.Rows.Count != 0)
                _user = new User(Convert.ToInt32(dt.Rows[0][0]), uname, dt.Rows[0][1].ToString(), dt.Rows[0][2].ToString());

            return _user;
        }

        public static User GetInfraVerifiedUser(IDatabase db, string uname, string u_token)
        {
            User _user = null;
            string sql = "UPDATE eb_tenants SET isverified = TRUE WHERE cname = @cname AND u_token = @u_token;";
               sql += "SELECT id, firstname,profileimg FROM eb_tenants WHERE cname = @cname AND u_token = @u_token";

            DbParameter[] parameters = {
                db.GetNewParameter("cname", System.Data.DbType.String, uname),
                db.GetNewParameter("u_token", System.Data.DbType.String, u_token)
            };

            EbDataTable dt = db.DoQuery(sql, parameters);

            if (dt.Rows.Count != 0)
                _user = new User(Convert.ToInt32(dt.Rows[0][0]), uname, dt.Rows[0][1].ToString(), dt.Rows[0][2].ToString());

            return _user;
        }

        public static User GetDetails(DatabaseFactory df, string uname, string pass)
        {
           // string str = MD5Hash(pass + uname);

            string sql = string.Format("SELECT id, email,firstname,profileimg FROM eb_users WHERE email = @uname AND pwd = @pass;");
            sql += string.Format(df.ObjectsDB.LOAD_USERALLPERMISSION_FROM_EB_PERMISSIONS);
            sql += string.Format(df.ObjectsDB.LOAD_USERALLROLES_FROM_EB_ROLES);

            DbParameter[] parameters = { df.ObjectsDB.GetNewParameter("@uname", DbType.String, uname), df.ObjectsDB.GetNewParameter("@pass", DbType.String, pass) };
            EbDataSet ds = df.ObjectsDB.DoQueries(sql, parameters);

            User _user = null;

            if (ds.Tables[0].Rows.Count == 1)
            {
                _user = new User(Convert.ToInt32(ds.Tables[0].Rows[0][0]), ds.Tables[0].Rows[0][1].ToString(),ds.Tables[0].Rows[0][2].ToString(),ds.Tables[0].Rows[0][3].ToString());

                foreach (EbDataRow dr in ds.Tables[1].Rows)
                    _user.PermissionCollection.Add(new Permission(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), Convert.ToInt32(dr[2])));

                foreach (EbDataRow dr in ds.Tables[2].Rows)
                    _user.RoleCollection.Add(new Role(Convert.ToInt32(dr[0]), dr[1].ToString()));
            }

            return _user;
        }
    }
}

