using AutoMapper;

using NHibernate;
using StructureMap;
using System;
using UAI.Case.Domain.Academico;
using UAI.Case.Domain.CASE;
using UAI.Case.Domain.Common;
using UAI.Case.Domain.Proyectos;
using UAI.Case.Domain.Roles;
using UAI.Case.Dto;
using UAI.Case.NHibernateProvider;
using UAI.Case.Repositories;
using UAI.Case.Security;
using Microsoft.AspNetCore.Http;

namespace UAI.Case.Boot
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Booter
    {

        public static void RunAutoMapperConfig()
        {
            Mapper.CreateMap<Docente, Docente>();
            Mapper.CreateMap<Curso, Curso>();
            Mapper.CreateMap<Todo, Todo>();
            Mapper.CreateMap<Nota, Nota>();
            Mapper.CreateMap<Materia, Materia>();
            Mapper.CreateMap<Alumno, Alumno>();
            Mapper.CreateMap<Curso, CursoDTO>();
            Mapper.CreateMap<Proyecto, ProyectoDTO>();
            Mapper.CreateMap<Usuario, Usuario>();
            Mapper.CreateMap<Grupo, Grupo>();
            Mapper.CreateMap<Mail, Mail>();
            Mapper.CreateMap<ContenidoMateria, ContenidoMateria>();
            Mapper.CreateMap<Contenido, Contenido>();
            Mapper.CreateMap<Unidad, Unidad >();
            Mapper.CreateMap<Archivo, Archivo>();
            Mapper.CreateMap<Clase, Clase>();
            
            Mapper.CreateMap<UnidadClase,UnidadClase>();
            Mapper.CreateMap<Evaluacion, Evaluacion>();
            Mapper.CreateMap<Elemento, Elemento>();
            Mapper.CreateMap<Evaluacion, EvaluacionDTO>();
            Mapper.CreateMap<AlumnoCursoGrupo, AlumnoCursoGrupo>();
            

        }

        

        public static Container Run(string cn)
        {
            var configuration = NHibernateInitializer.Initialize(cn);
            var container = new Container(c =>
            {


            c.For<ISessionFactory>().Singleton().Use(() => configuration.BuildSessionFactory());
            c.For<IDbContext>().Use<DbContext>();
            c.For(typeof(IRepository<>)).Use(typeof(Repository<>));
            c.For<IAuthDataExtractor>().Use<AuthDataExtractor>();
            c.For<IHttpContextAccessor>().Use<HttpContextAccessor>();
            c.For<IAuthenticatedData>().Use(_ => _.GetInstance<IAuthDataExtractor>().GetCurrentData());
            c.For(typeof(ITodoRepository)).Use(typeof(TodoRepository));
            c.AddRegistry(new AppServiceRegistry());
            c.AddRegistry(new RepositoryRegistry());

          

            });
            NHibernateInitializer.UpdateSchema(configuration, container.GetInstance<ISessionFactory>());

            RunAutoMapperConfig();
            return container;
        }

    }
}


