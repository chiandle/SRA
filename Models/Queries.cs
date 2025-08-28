using System;
using System.Collections.Generic;
using System.Text;

namespace SVPUtil.Data
{
    public class Queries
    {

        public const string qryElettoratoAttivoPAC = @"select   p.CodiceFiscale, p.Matricola, p.Nome, p.Cognome,
p.DataNascita, rtrim(p.ComuneNascita) as ComuneNascita, p.email, p.Aff_org as Cod_Afferenza, p.Afferenza, p.Ruolo_cod as Ruolo, null as GerarchiaLivello,
null as Votazione, d.mail as IdentificativoEligo, p.CellulareEsteso as cellulare, null as Incoerenza, p.AccountDominio + '@os.uniroma3.it' as AccountName, null as DataAggiornamento, null as TipoCaricamento
from PAC_V_ANAG_SERVIZIO p
inner join ASI_dominio_new d on d.cod_fisc = p.CodiceFiscale
where 1=1
and p.Ruolo_cod in ('PO', 'PA', 'PF', 'PD', 'RU','RD','D8','ND', 'CL', 'LC', 'NM', 'DR') ";

        public const string qryElettoratoAttivoPACSTUD = @"select  s.CodiceFiscale, null as 'Matricola', upper(s.Nome) as 'Nome', upper(s.Cognome) as 'Cognome', s.DataNascita,
null as 'ComuneNascita', lower(s.username + '@stud.uniroma3.it')  as Email, cast(s.CDS_ID as varchar(50)) as Cod_Afferenza, s.CDS as Afferenza, 'STUD' as Ruolo, null as GerarchiaLivello,
null as Votazione, lower(s.username + '@stud.uniroma3.it') as IdentificativoEligo, 
null as Cellulare, null as Incoerenza, lower(s.username + '@stud.uniroma3.it') as AccountName, null as DataAggiornamento, null as TipoCaricamento
from PAC_STUDENTI s
where s.CodiceFiscale = '";

        public const string qryElettoratoAttivoGOMP = @"select s.CodiceFiscale , null as matricola,
s.Nome, s.Cognome, s.DataNascita, ltrim(rtrim(s.ComuneNascita)) as ComuneNascita, 
null as email, 
null as Cod_Afferenza, null as Afferenza, null as Ruolo, 4 as GerarchiaLivello, null as IdentificativoEligo,
null as Cellulare, null as Incoerenza, null as AccountName
from segreterie_studenti s
where s.codicefiscale = '";

        public string qryElettoratoAttivoCSA(string dataindizione, string datavoto, string ruoli, string afferenza = "", bool usaopenquery = false)
        {
            string qry;
            qry = @"select tab.CodiceFiscale, tab.Matricola, tab.Nome, tab.Cognome, tab.DataNascita, tab.ComuneNascita, tab.eMail,
tab.Afferenza as Cod_Afferenza, trim(tab.funzione) as Afferenza,  
tab.Ruolo as Ruolo, tab.Attivita as Attività, tab.ds_attivita as DescrizioneAttività,
case tab.ruolo 
when 'PO' then 1
when 'PA' then 1
when 'RU' then 1
when 'RD' then 2
when 'PF' then 3
when 'D8' then 4
when 'ND' then 4
when 'CL' then 4
when 'LC' then 4
when 'NM' then 4

else null
end as GerarchiaLivello
from 
(
select uo.descr as Funzione, sge.matricola as Matricola, upper(sge.cognome) as Cognome, Upper(sge.nome) as Nome, va.sesso as Sesso , 
va.data_nasc as DataNascita, va.cd_nazionalita as Nazionalita, va.naz_nasc as NazioneNascita, NULL as ProvinciaNascita,  va.istat_nasc as ComuneNascita,
va.cod_fisc as CodiceFiscale, va.e_mail as eMail,  sge.inquadr as Inquadramento, sge.aff_org as Afferenza, sge.SEDE_DESCR as Sede, 
sge.SEDE_INDIRIZZO as IndirizzoSede, sge.ruolo as Ruolo, sge.attivita , sge.ds_attivita ,
rank() over(partition by sge.matricola order by sge.data_in desc) rango
            from SIARU_UNIROMA3_PROD.sge_intervalli_plus sge
            inner join SIARU_UNIROMA3_PROD.vd_anagrafica va on sge.id_ab = va.id_ab
            inner join SIARU_UNIROMA3_PROD.vd_vista_org uo on sge.aff_org = uo.uo
            where 1 = 1
            and sge.data_in <= to_date('" + dataindizione + "', 'DD/MM/YYYY') " +
                "and sge.data_fin >= to_date('" + dataindizione + "', 'DD/MM/YYYY') " +
                "and sge.data_rap_fin > to_date('" + dataindizione + "', 'DD/MM/YYYY') " +
                "and sge.ruolo in (" + ruoli + ") " +
                "and not (sge.data_fin > to_date('16/11/2022', 'DD/MM/YYYY') and sge.attivita = '1095') " +
                "and uo.data_in < to_date('" + dataindizione + "', 'DD/MM/YYYY') " +
                "and uo.data_fin >= to_date('" + dataindizione + "', 'DD/MM/YYYY') " +
                //"and sge.ATTIVITA not in ('1095', '0180', '0074', '0331') " +
                @"and not exists (select cod_fisc from SIARU_UNIROMA3_PROD.sge_intervalli_plus sged
            where 1 = 1
            and sged.matricola = sge.matricola and sged.Ruolo = sge.ruolo 
            and sged.attivita in ('0022', '0030', '0004', '0608', '1066')
            and sged.data_in <= to_date('" + datavoto + "', 'DD/MM/YYYY') " +
            "and sged.data_fin >= to_date('" + datavoto + "', 'DD/MM/YYYY')) ) tab " +
            "where 1=1 and tab.rango = 1";
            if (!String.IsNullOrEmpty(afferenza))
            {
                qry += " and trim(tab.funzione) in (" + afferenza + ")";

            }
            if (usaopenquery)
            {
                qry = qry.Replace("'", "''");
                qry = "select * from openquery(UGOV, '" + qry + "')";
            }
            return qry;
        }
    }
}
