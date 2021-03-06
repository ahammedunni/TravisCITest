﻿using ServiceStack;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public interface IEbSSRequest
    {
        string Token { get; set; }

        string TenantAccountId { get; set; }

        int UserId { get; set; }
    }

    public interface IEbSSResponse
    {
        string Token { get; set; }

        ResponseStatus ResponseStatus { get; set; } //Exception gets serialized here
    }
}
