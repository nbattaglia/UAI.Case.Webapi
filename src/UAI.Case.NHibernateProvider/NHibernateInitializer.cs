using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Dialect;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using UAI.Case.NHibernateProvider;

namespace UAI.Case.NHibernateProvider
{
   
        public class NHibernateInitializer
        {
            public static Configuration Initialize(String connectionString)
            {
                var configuration = new Configuration();


            configuration
                .Proxy(p => p.ProxyFactoryFactory<DefaultProxyFactoryFactory>())
                .DataBaseIntegration(db =>
                {
                    db.ConnectionString = connectionString;
                    db.Dialect <MySQL55InnoDBDialect>();
                    db.BatchSize = 500;
                   // db.LogFormattedSql = true;
                    //db.LogSqlInConsole = true;
                    
                    //db.AutoCommentSql = true;
                })
                .CurrentSessionContext<WebSessionContext>();
                
                var mapper = new ConventionModelMapper();
            
                    mapper.WithConventions(configuration);
               

           
          
            return configuration;
            }

            public static void UpdateSchema(Configuration configuration, ISessionFactory sessionFactory)
            {
                using (ISession session = sessionFactory.OpenSession())
                {
                    //configuration.CreateIndexesForForeignKeys(); //ya mysql lo hace por defecto
                    new SchemaUpdate(configuration).Execute(true, true);
                }
            }
        }
    }

