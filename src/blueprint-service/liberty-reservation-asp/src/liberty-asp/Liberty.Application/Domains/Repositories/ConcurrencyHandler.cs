using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberty.Application.Domains.Repositories
{
    public interface IConcurrencyHandler<TEntity>
    {
        void SetRowVersion(TEntity entity, byte[] version);

        bool IsDbUpdateConcurrencyException(Exception ex);
    }
}
