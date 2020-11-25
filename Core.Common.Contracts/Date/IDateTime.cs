using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Contracts.Date
{
    public interface IDateTime
    {
        DateTime UtcNow { get;}
    }
}
