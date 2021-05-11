using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupAPI.Authorization
{ 
    public enum OperationType
    {
        Create,
        Read,
        Update,
        Delete
    }
    public class ResourceOperationRequirement : IAuthorizationRequirement
    {
        public ResourceOperationRequirement(OperationType operationType)
        {
            OperationType = operationType;
        }
        public OperationType  OperationType { get; }
    }
}
