using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using SharedContext.Dao;
using XrmBedrock.SharedContext;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace SharedContext.Dao;

public partial class DataverseAccessObject : IDataverseAccessObject
{
    public T Producer<T>(T? entity, Action<T> modifier)
        where T : Entity, new()
    {
        entity = NewIfNull(entity);
        modifier(entity);
        entity.Id = Create(entity);
        return entity;
    }

    public T Producer<T>(T? entity)
        where T : Entity, new()
    {
        entity = NewIfNull(entity);
        entity.Id = Create(entity);
        return entity;
    }

    public T Constructor<T>(T? entity, Action<T> modifier)
        where T : Entity, new()
    {
        entity = NewIfNull(entity);
        modifier(entity);
        return entity;
    }

    private static T NewIfNull<T>(T? entity)
        where T : Entity, new()
    {
        if (entity == null)
        {
            return new T();
        }

        return entity;
    }
}