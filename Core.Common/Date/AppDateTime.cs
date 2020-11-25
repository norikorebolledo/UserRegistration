using Core.Common.Contracts.Date;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Date
{
    public class AppDateTime : IDateTime
    {
        public DateTime UtcNow { get => DateTime.UtcNow; }
    }
}
