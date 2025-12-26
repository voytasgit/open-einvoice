using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERechnung.Core.Dtos
{
    public class ImportResult
    {
        public InvoiceInputDto Invoice { get; set; }
        public List<string> Warnings { get; set; } = new();
    }

}
