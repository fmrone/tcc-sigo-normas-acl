using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tcc.Sigo.Normas.Acl.Models;

namespace Tcc.Sigo.Normas.Acl.Repository
{
    public interface INormaRepository
    {
        Task<IEnumerable<NormaSegurancaQAModel>> ListarNormasSegurancaQA();
        Task<bool> PersistirNorma(NormaMessageModel normaMessageModel);
    }
}
