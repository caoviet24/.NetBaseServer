using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace Domain.Enums
{
    public enum Action
    {
        CREATE,
        UPDATE,
        DELETE,
        DELETE_SOFT,
        RESTORE,
        GET,
        GET_ALL,
        GET_BY_ID,
    }
}