using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UAI.Case.NHibernateProvider
{
    public interface IConventionOverride
    {
        void Override(ModelMapper mapper);
    }
}
