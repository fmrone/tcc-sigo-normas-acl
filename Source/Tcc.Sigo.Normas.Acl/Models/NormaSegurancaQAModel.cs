using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tcc.Sigo.Normas.Acl.Models
{
    public class NormaSegurancaQAModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public byte Area { get; set; }
        public bool Status { get; set; }

        //utilizado nas integrações
        public DateTime? IntegradoEm { get; set; }
        public string Integracao { get; set; }
    }
}
