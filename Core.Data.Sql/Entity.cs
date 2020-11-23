using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Sql
{
    public class Entity<TType> : IEntity<TType>
    {
        public TType Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
