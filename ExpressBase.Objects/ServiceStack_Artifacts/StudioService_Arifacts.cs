﻿using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [Route("/ebo")]
    [Route("/ebo/{Id}")]
    public class EbObjectRequest : IReturn<EbObjectResponse>, IEbSSRequest
    {
        public int Id { get; set; } // (Id > 0) Fetch all version without bytea / Fetch latest version with bytea 

        public int VersionId { get; set; } // (VersionId > 0 and VersionId != Int32.MaxValue) Fetch particular version with Bytea

        public int EbObjectType { get; set; } // Get All latest of this Object Type without Bytea

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class EbObjectResponse :IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
  
    [Route("/ebo", "POST")]
    public class EbObjectSaveOrCommitRequest : IReturn<EbObjectSaveOrCommitResponse>, IEbSSRequest
    {      
        public bool IsSave { get; set; } // If (IsSave == true) Save else Commit
        
        public int Id { get; set; } // (Id == 0) First Commit else Subsequent Commit
      
        public int EbObjectType { get; set; }
       
        public string Name { get; set; }
        
        public string Description { get; set; }
     
        public byte[] Bytea { get; set; }
       
        public ObjectLifeCycleStatus Status { get; set; }
      
        public string ChangeLog { get; set; }
       
        public string Token { get; set; }
      
        public string TenantAccountId { get; set; }
       
        public int UserId { get; set; }
    }

    [DataContract]
    public class EbObjectSaveOrCommitResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class EbObjectWrapper
    {
        [DataMember(Order = 1)]
        public int Id { get; set; } 

        [DataMember(Order = 2)]
        public EbObjectType EbObjectType { get; set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 5)]
        public byte[] Bytea { get; set; }

        [DataMember(Order = 6)]
        public ObjectLifeCycleStatus Status { get; set; }

        [DataMember(Order = 7)]
        public int VersionNumber { get; set; }

        [DataMember(Order = 8)]
        public string ChangeLog { get; set; }

        [DataMember(Order = 9)]
        public string CommitUname { get; set; }

        [DataMember(Order = 10)]
        public DateTime CommitTs { get; set; }

        public EbObjectWrapper() { }
    }
}
