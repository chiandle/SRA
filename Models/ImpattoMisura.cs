using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SRA.Models
{
    public class ImpattoMisura :InfoModifica
    {
        public Guid IDMisura { get; set; }
        public Guid IDRischio { get; set; }
        public Guid IDApplicazione { get; set; }
        public string Motivazione { get; set; }

    }
        
}
