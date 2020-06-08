﻿using Abp.Dependency;
using GraphQL;
using GraphQL.Types;
using Hinnova.Queries.Container;

namespace Hinnova.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IDependencyResolver resolver) :
            base(resolver)
        {
            Query = resolver.Resolve<QueryContainer>();
        }
    }
}