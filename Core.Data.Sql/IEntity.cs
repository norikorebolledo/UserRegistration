using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Sql
{
    public interface IEntity
    {

    }
    public interface IEntity<TId> : IEntity
    {
        TId Id { get; set; }

    }
}
