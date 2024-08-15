using System.Configuration;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NHibernateModels;
using NHibernateSql.FrameworkExtensions;

namespace NHibernateSql
{
    public class NHibernateManagerFactory
    {
        private ISessionFactory contextFactory = Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(ConfigurationManager.AppSettings.Get("ConnectionString")))
            .Mappings(m =>
                m.AutoMappings
                    .Add(AutoMap.AssemblyOf<Client>().UseOverridesFromAssemblyOf<UserMappingOverride>()))
            .ExposeConfiguration(cfg => new SchemaExport(cfg)
                .Create(false, false))
            .BuildSessionFactory();
        public NHibernateManager Create()
        {
            return new NHibernateManager(contextFactory);
        }
    }
}
