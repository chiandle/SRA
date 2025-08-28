using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SVPUtil.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SRA.Models
{
    public class StruttOrg
    {
        [Key]
        public int ProgressivoStruttura { get; set; }
        public string Nome { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public string LivelloGerarchico { get; set; }
        public string CodiceStruttura { get; set; }
        public int? ProgressivoStrutturaPadre { get; set; }
        public string DescrizioneEstesa { get; set; }
        public string DescrizioneBreve { get; set; }
        public string CodiceFiscaleStruttura { get; set; }
        public string ProgressivoSede { get; set; }

        public List<StruttOrg> CaricaStrutture(CSAdbContext csaContext)
        {
            var sqlQry = @"select uomef.Progressivo as ""ProgressivoStruttura"",
uomef.descr as ""Nome"",
to_date('01/01/2020', 'DD/MM/YYYY') as ""DataInizio"",
to_date('01/01/2022', 'DD/MM/YYYY') as ""DataFine"",
uomef.descr_tipo as ""LivelloGerarchico"",
uomef.uo as ""Codicestruttura"",
uopadre.Progressivo as ""ProgressivoStrutturaPadre"",
uomef.descr as ""DescrizioneEstesa"",
uomef.breve as ""DescrizioneBreve"",
'04400441004' as ""CodiceFiscaleStruttura"",
null as ""ProgressivoSede""
from
(select ROW_NUMBER()
   OVER(ORDER BY uo) + 1 as Progressivo, uo.uo, uo.uo_padre, uo.tipo, uo.descr_tipo, uo.descr, uo.breve
from SIARU_UNIROMA3_PROD.vd_vista_org uo
where data_fin > sysdate

union
select 1, '170117' as UO, NULL AS UO_PADRE, 'UNI' as TIPO, 'Università' as DESCR_TIPO,
'Università Roma Tre' as DESCR, 'Università' as BREVE from dual
) uomef
left outer join(select ROW_NUMBER()
   OVER (ORDER BY uo)+1 as Progressivo, uo.uo, uo.uo_padre, uo.tipo, uo.descr_tipo, uo.descr, uo.breve
from SIARU_UNIROMA3_PROD.vd_vista_org uo
where data_fin > sysdate
union
select 1, '170117' as UO, NULL AS UO_PADRE, 'UNI' as TIPO, 'Università' as DESCR_TIPO,
'Università Roma Tre' as DESCR, 'Università' as BREVE from dual) uopadre on uomef.uo_padre = uopadre.uo
where 1 = 1
and uomef.descr_tipo <> 'Sede fisica'";

            List<StruttOrg> listaStrutture = new List<StruttOrg>();
            listaStrutture = csaContext.DBSetStrutture.FromSqlRaw(sqlQry).ToList();
            return listaStrutture;
        }
    }
}
