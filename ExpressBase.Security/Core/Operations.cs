using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressBase.Common;
using System.Data;
using ExpressBase.Data;

namespace ExpressBase.Security
{
    public class Operation : RBACBase
    {
        private int _id;
        private string _name;

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

        public Operation(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public static Operation Create(string operationname)
        {
            var dt = df.ObjectsDB.DoQuery(string.Format(df.ObjectsDB.INSERT_EB_OPERATIONS, operationname));
            return new Operation(Convert.ToInt32(dt.Rows[0][0]), operationname);
        }

        public static void Delet(int operation_id)
        {
            string sql = string.Format(df.ObjectsDB.DELETE_OP_FROM_EB_OPERATIONS, operation_id);
            sql += string.Format(df.ObjectsDB.DELETE_OP_FROM_EB_PERMISSIONS, operation_id);
            sql += string.Format(df.ObjectsDB.DELETE_OP_FROM_EB_PERMISSION2ROLE, operation_id);
            df.ObjectsDB.DoQuery(sql);
        }
    }

    public class OperationCollection : List<Operation>
    {


    }
}
