using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UAI.Case.Domain;
using UAI.Case.Domain.Common;
using UAI.Case.Domain.Roles;


namespace UAI.Case.NHibernateProvider
{

    

    public static class Conventions
    {


        public static void WithConventions(this ConventionModelMapper mapper, Configuration configuration)
        {


            mapper.Class<Mail>(map => map.Property(p => p.Body, m => m.Type(NHibernateUtil.StringClob)));


            mapper.Class<ChatMessage>(map => map.Property(p => p.ReadedBy, m => m.Type(NHibernateUtil.StringClob)));
            //TODO: hacer que funcione el mapeo para EntityWithTypedId como base, si no no tiene mucho sentido tener 2 clase base

            Type baseEntityType = typeof(Entity);

            mapper.IsEntity((type, declared) => IsEntity(type));


            mapper.IsRootEntity((type, declared) => baseEntityType.Equals(type.BaseType));

            mapper.BeforeMapClass += (modelInspector, type, classCustomizer) =>
            {
                //classCustomizer.Id(c => c.Column("Id"));
                classCustomizer.Id(c => c.Generator(Generators.GuidComb));
                //classCustomizer.Table(Inflector.Net.Inflector.Pluralize(type.Name.ToString()).n);
                


                //agregar el versionado para concrrencia
                //classCustomizer.Version( , m => m.Generated(VersionGeneration.Always));
            };
            
            mapper.BeforeMapJoinedSubclass += (mi, t, map) =>
            {
                map.Key(km => km.Column("Id"));
                
                //map.Table(t.Name.ToLowerInvariant());
            };

            mapper.BeforeMapProperty += (mi, propertyPath, map) =>
            {
                if (typeof(decimal).Equals(propertyPath.LocalMember.GetPropertyOrFieldType()))
                    map.Type(NHibernateUtil.Currency);

              

                

                if (typeof(byte[]).Equals(propertyPath.LocalMember.GetPropertyOrFieldType()))
                {
                    map.Type(NHibernateUtil.BinaryBlob);
                    map.Length(102400); //int.maxvalue //TODO: ya con esto me la especifica como image, ver de cambiar el sql-type
                }
            };
            

            mapper.AfterMapProperty += (modelInspector, member, propertyCustomizer) =>
            {
                //TODO: si es un enum nulleable chequear por el underlygingenerictype, sino .IsEnum da false

                var propertyType = member.LocalMember.GetPropertyOrFieldType();
                if (propertyType.IsEnum == false) return;
                var nhEnumType = typeof(EnumStringType<>).MakeGenericType(new[] { propertyType });
                var typeInstance = Activator.CreateInstance(nhEnumType) as IType;
                propertyCustomizer.Type(typeInstance);
                propertyCustomizer.Index(member.LocalMember.Name);
                propertyCustomizer.NotNullable(true);
                

            };

            mapper.BeforeMapManyToOne += (modelInspector, propertyPath, map) =>
            {
                map.Column("Id_" + propertyPath.LocalMember.Name);
                map.Cascade(Cascade.None);  //lo manejo a manopla
            };

            
            mapper.BeforeMapBag += (modelInspector, propertyPath, map) =>
            {
                map.Key(keyMapper => keyMapper.Column("Id_" + propertyPath.GetContainerEntity(modelInspector).Name));
                map.Cascade(Cascade.All);
                
            };

                //mapper.BeforeMapManyToOne += (modelInspector, propertyPath, map) =>
                //{

                //    map.Column(propertyPath.LocalMember.GetPropertyOrFieldType().Name +
                //        "Fk");
                //    map.Cascade(Cascade.Persist);
                //};

            AddConventionOverrides(mapper);

            var entities = Assembly.Load("UAI.Case.Domain").GetExportedTypes().Where(t => typeof(Entity).IsAssignableFrom(t) && !typeof(Entity).Equals(t)).ToList();

            HbmMapping mapping = mapper.CompileMappingFor(entities);
            configuration.AddDeserializedMapping(mapping, "UAI.Case.Domain");

           

        }

        /// <summary>
        /// Determine if type implements IEntityWithTypedId<>
        /// </summary>
        public static bool IsEntity(Type type)
        {
            return typeof(Entity).IsAssignableFrom(type) && typeof(Entity) != type && !type.IsInterface ;
        }

        /// <summary>
        /// Looks through this assembly for any IOverride classes.  If found, it creates an instance
        /// of each and invokes the Override(mapper) method, accordingly.
        /// </summary>
        private static void AddConventionOverrides(ConventionModelMapper mapper)
        {
            Type overrideType = typeof(IConventionOverride);

            IList<Type> types = Assembly.Load("UAI.Case.NHibernateProvider").GetTypes()
                .Where(t => overrideType.IsAssignableFrom(t) && t != typeof(IConventionOverride))
                .ToList();

            types.ForEach(t =>
            {
                IConventionOverride conventionOverride = Activator.CreateInstance(t) as IConventionOverride;
                conventionOverride.Override(mapper);
            });
        }

        //TODO: quedaria mejor este metodo en NhibernateInitializer, pero esa clase no es estatica
        public static void CreateIndexesForForeignKeys(this Configuration configuration)
        {
            PropertyInfo tableMappingsProperty = typeof(Configuration).GetProperty("TableMappings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            configuration.BuildMappings();
            var tables = (ICollection<Table>)tableMappingsProperty.GetValue(configuration, null);
            foreach (var table in tables)
            {
                foreach (var foreignKey in table.ForeignKeyIterator)
                {
                    var idx = new Index();
                    idx.AddColumns(foreignKey.ColumnIterator);
                    idx.Name = "IDX_" + idx.ColumnIterator.First().Name + "_" + foreignKey.Name.Substring(2);
                    idx.Table = table;
                    table.AddIndex(idx);
                }
            }
        }

    }
}
