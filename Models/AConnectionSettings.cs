using System;
using Elasticsearch.Net;
using Nest;

namespace ElasticSearch_Sample.Models
{
    public class AConnectionSettings : ConnectionSettings
    {
        public AConnectionSettings(IConnectionPool pool,IConnection connection):base(pool,connection)
        {
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        protected override void DisposeManagedResources()
        {
            this.IsDisposed = true;
            base.DisposeManagedResources();
        }
    }

    public class AConnectionPool : SingleNodeConnectionPool
    {
        public AConnectionPool(Uri uri,DateTimeProvider dateTimeProvider=null):base(uri,dateTimeProvider)
        {

        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        protected override void DisposeManagedResources()
        {
            this.IsDisposed = true;
            base.DisposeManagedResources();
        }
    }

    public class AConnection:InMemoryConnection
    {
        public bool IsDisposed
        {
            get;
            private set;
        }

        protected override void DisposeManagedResources()
        {
            this.IsDisposed = true;
            base.DisposeManagedResources();
        }
    }
}
