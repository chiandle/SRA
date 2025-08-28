using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Text;

namespace SVPUtil.Data
{
    public class Costanti
    {
        public  string qryDipendentiCSA {get; set;}
        public string qryStrutture { get; set;}
        public string qryDipendentiMEF { get; set;}
        public string qryStruttureMEF { get; set;}
        public string qryDipendentiETL { get; set;}

		public string qryDipendentiValutazione { get; set;}
        public string qryDocentiETL { get; set;}

        public string qryDipendentiPAC { get; set;}
        public string qryDocentiPAC { get; set; }

        public string qryStudentiL104 { get; set;}
        public string qryStudentiL170 { get; set; }

        public Dictionary<string, string> Ruoli { get; set;}

		public Costanti()
		{
			Ruoli= new Dictionary<string, string>();
			Ruoli.Add("PO", "Professore Ordinario");
            Ruoli.Add("PA", "Professore Associato");
            Ruoli.Add("RU", "Ricercatore universitario");
            Ruoli.Add("RD", "Ricercatore tempo det. Legge 240/2010");
            Ruoli.Add("D8", "Dirigente");
            Ruoli.Add("DC", "Dirigente a contratto");
            Ruoli.Add("ND", "Non docente");
            Ruoli.Add("CL", "Collaboratore Esperto Linguistico");
            Ruoli.Add("LC", "Collaboratore Esperto Linguistico");
        }
        public Costanti(string dataanalisi)
        {
            #region qryDipendentiCSA
            qryDipendentiCSA = @"select 
tab.Afferenza as ""CodiceFunzioneAziendale"", tab.funzione as ""FunzioneAziendale"", tab.Matricola as ""CodiceHR"", tab.Nome, tab.Cognome,
decode(funz.DescrizioneFunzione,null, 'N', 'S') as Responsabile, 
da.DataAssunzione as ""DataInizioCollaborazione"", tab.Ruolo as ""RuoloContrattuale"", funz.DescrizioneFunzione as ""Ruolo"", tab.Inquadramento as Livello, 
case SUBSTR(tab.Inquadramento,1,1) 
when '0' then 1
when 'E' then 2
when 'D' then 3
when 'Z' then 3
when 'C' then 4
when 'B' then 5
else null
end as GerarchiaLivello,
tab.DataNascita, tab.CodiceFiscale, tab.eMail
from 
(
select uo.descr as Funzione, sge.matricola as Matricola, upper(sge.cognome) as Cognome, Upper(sge.nome) as Nome, va.sesso as Sesso , 
va.data_nasc as DataNascita, va.cd_nazionalita as Nazionalita, va.naz_nasc as NazioneNascita, NULL as ProvinciaNascita,  va.istat_nasc as ComuneNascita,
va.cod_fisc as CodiceFiscale, va.e_mail as eMail, null as DataAssunzione, sge.inquadr as Inquadramento, sge.aff_org as Afferenza, sge.SEDE_DESCR as Sede, 
sge.SEDE_INDIRIZZO as IndirizzoSede, sge.ruolo as Ruolo, 
rank() over(partition by sge.matricola order by sge.data_in desc) rango
            from SIARU_UNIROMA3_PROD.sge_intervalli_plus sge
            inner join SIARU_UNIROMA3_PROD.vd_anagrafica va on sge.id_ab = va.id_ab
            inner join SIARU_UNIROMA3_PROD.vd_vista_org uo on sge.aff_org = uo.uo
            where 1 = 1
            and sge.data_in <= to_date('"
            + dataanalisi + "', 'DD/MM/YYYY') and sge.data_rap_fin > to_date('"
            + dataanalisi + "', 'DD/MM/YYYY') and sge.data_fin >= to_date('"
            + dataanalisi + @"', 'DD/MM/YYYY') and sge.ruolo in ('ND', 'D8', 'DC', 'CL', 'LS', 'LC', 'NM')
            and uo.data_fin > to_date('"
            + dataanalisi + @"', 'DD/MM/YYYY')
) tab, (select Matricola, Funzione, DescrizioneFunzione from 
(
select c.matricola, c.funzione, c.in_vigore_giur, fi.descr DescrizioneFunzione, row_number() over(partition by c.matricola order by c.in_vigore_giur desc) rango
from SIARU_UNIROMA3_PROD.CARRIERE c
inner join SIARU_UNIROMA3_PROD.FUNZIONI Fi ON c.funzione = fi.funzione
where 1=1
and c.evento = 32
and c.stato_carriere in ('E', 'U')
and c.termine > to_date('"
+ dataanalisi + @"', 'DD/MM/YYYY')
and (fi.descr like 'RESPONSABILE%'  or lower(fi.descr) like 'segretari%' or fi.descr = 'FUNZIONI SPECIALISTICHE AD INTERIM')
) f
where f.rango =1 ) funz,
(select ra.matricola, to_char(min(ra.data_rap_ini), 'DD/MM/YYYY') as DataAssunzione
from SIARU_UNIROMA3_PROD.V_IE_RU_PERS_RAPPORTI ra
where 1=1
and ra.ruolo in ('ND', 'D8', 'DC','CL', 'LS', 'LC', 'NM')
group by ra.matricola
) da
where 1=1
and tab.rango = 1
and tab.matricola = funz.matricola(+)
and tab.matricola = da.matricola(+)
and tab.afferenza in (
select uo from (
select uo, uo_padre, livello,
regexp_substr(path, '[^/]+', 1, 1) as DirezioneGenerale,
regexp_substr(path, '[^/]+', 1, 2) as Direzione,
regexp_substr(path, '[^/]+', 1, 3) as Area,
regexp_substr(path, '[^/]+', 1, 4) as Divisione,
regexp_substr(path, '[^/]+', 1, 5) as Ufficio
from
(
select uo, uo_padre,
       RPAD('.', (level-1), '.') || uo AS tree,
       level as livello,
       CONNECT_BY_ROOT uo AS root_id,
       LTRIM(SYS_CONNECT_BY_PATH(Descr, '->'), '-') AS path,
       CONNECT_BY_ISLEAF AS leaf
FROM  (
select uo, uo_padre, descr from siaru_uniroma3_prod.vista_org_attiva) tab1
START WITH uo_padre =170117
CONNECT BY uo_padre = PRIOR uo
--ORDER SIBLINGS BY uo
) tp)

)";
            #endregion

            #region qryStrutture
            qryStrutture = @"select * from (
select uo as uo_id, uo_padre, Nome, data_in, data_fin, livello,
regexp_substr(path, '[^/]+', 1, 1) as Livello1,
regexp_substr(path, '[^/]+', 1, 2) as Livello2,
regexp_substr(path, '[^/]+', 1, 3) as Livello3,
regexp_substr(path, '[^/]+', 1, 4) as Livello4,
regexp_substr(path, '[^/]+', 1, 5) as Livello5
from
(
select uo, uo_padre, nome, data_in, data_fin,
       RPAD('.', (level-1), '.') || uo AS tree,
       level as livello,
       CONNECT_BY_ROOT uo AS root_id,
       LTRIM(SYS_CONNECT_BY_PATH(replace(Descr,'/','-')  , '/'), '-') AS path,
       CONNECT_BY_ISLEAF AS leaf
FROM  (
select uo, uo_padre, data_in, data_fin, descr as nome, descr from siaru_uniroma3_prod.vd_vista_org
where 1=1
and data_in < to_date('"+ dataanalisi + "', 'DD/MM/YYYY') and data_fin > to_date('" + dataanalisi + @"', 'DD/MM/YYYY')) tab1
START WITH uo_padre =170117
CONNECT BY uo_padre = PRIOR uo
--ORDER SIBLINGS BY uo
) tp
where 1=1
)
";
            #endregion

            #region QueryDipendentiETL
            qryDipendentiETL = @"select  '" + dataanalisi + "' as DataCampionamento," +
@"tab.Afferenza as Afferenza, tab.funzione as DescrizioneAfferenza, tab.Matricola as Matricola, tab.Nome, tab.Cognome, tab.Sesso, 
decode(funz.DescrizioneFunzione,null, 'N', 'S') as Responsabile, 
da.DataAssunzione as DataInizioCollaborazione, tab.Ruolo as RuoloContrattuale, funz.DescrizioneFunzione as Ruolo, tab.Attivita, tab.DescrizioneAttivita, tab.tipo_part_time as TipoImpegno, 
tab.perc as PercentualePartTime, tab.Inquadramento as Inquadramento, tab.inquadramento as Livello,
case SUBSTR(tab.Inquadramento,1,1) 
when '0' then 1
when 'E' then 2
when 'D' then 3
when 'F' then 3
when 'Z' then 4
when 'C' then 5
when 'O' then 6
when 'B' then 6
else null
end as GerarchiaLivello,
tab.DataNascita, tab.LuogoNascita, tab.CodiceFiscale, tab.eMail, null as AreaScientificoDisciplinare
from 
(
select uo.descr as Funzione, sge.matricola as Matricola, upper(sge.cognome) as Cognome, Upper(sge.nome) as Nome, va.sesso as Sesso , 
to_char(va.data_nasc, 'DD/MM/YYYY') as DataNascita, va.cd_nazionalita as Nazionalita, va.naz_nasc as NazioneNascita, NULL as ProvinciaNascita,  comune.descr as LuogoNascita,
va.cod_fisc as CodiceFiscale, va.e_mail as eMail, null as DataAssunzione, sge.inquadr as Inquadramento, sge.aff_org as Afferenza, sge.SEDE_DESCR as Sede, 
sge.SEDE_INDIRIZZO as IndirizzoSede, sge.ruolo as Ruolo, sge.attivita as Attivita, sge.ds_attivita as DescrizioneAttivita,  sge.tipo_part_time, sge.perc,
rank() over(partition by sge.matricola order by sge.data_in desc) rango
            from SIARU_UNIROMA3_PROD.sge_intervalli_plus sge
            inner join SIARU_UNIROMA3_PROD.vd_anagrafica va on sge.id_ab = va.id_ab
            inner join SIARU_UNIROMA3_PROD.vd_vista_org uo on sge.aff_org = uo.uo
            inner join SIARU_UNIROMA3_PROD.Comune comune on va.istat_nasc = comune.istat
            where 1 = 1
            and sge.data_in <= to_date('" + dataanalisi + @"', 'DD/MM/YYYY') and sge.data_rap_fin > to_date('" + dataanalisi +
            @"', 'DD/MM/YYYY') and sge.data_fin >= to_date('" + dataanalisi + 
            @"', 'DD/MM/YYYY') and sge.ruolo in ('ND', 'D8', 'DC', 'CL', 'LS', 'LC', 'NM')
            and uo.data_fin > to_date('" + dataanalisi +
            @"', 'DD/MM/YYYY') and uo.data_in <= to_date('" + dataanalisi + @"', 'DD/MM/YYYY')
) tab, (select Matricola, Funzione, DescrizioneFunzione from 
(
select c.matricola, c.funzione, c.in_vigore_giur, fi.descr DescrizioneFunzione, row_number() over(partition by c.matricola order by c.in_vigore_giur desc) rango
from SIARU_UNIROMA3_PROD.CARRIERE c
inner join SIARU_UNIROMA3_PROD.FUNZIONI Fi ON c.funzione = fi.funzione
where 1=1
and c.evento = 32
and c.stato_carriere in ('E', 'U')
and c.termine > to_date('" + dataanalisi + @"', 'DD/MM/YYYY')
and (fi.descr like 'RESPONSABILE%'  or lower(fi.descr) like 'segretari%' or fi.descr = 'FUNZIONI SPECIALISTICHE AD INTERIM')
) f
where f.rango =1 ) funz,
(select ra.matricola, ra.ruolo, to_char(min(ra.data_rap_ini), 'DD/MM/YYYY') as DataAssunzione
from SIARU_UNIROMA3_PROD.V_IE_RU_PERS_RAPPORTI ra
where 1=1
and ra.ruolo in ('ND', 'D8', 'DC','CL', 'LS', 'LC', 'NM')
group by ra.matricola, ra.ruolo
) da
where 1=1
and tab.rango = 1
and tab.matricola = funz.matricola(+)
and tab.matricola = da.matricola(+)
and tab.ruolo = da.ruolo(+)
and tab.afferenza in (
select uo from (
select uo, uo_padre, livello,
regexp_substr(path, '[^/]+', 1, 1) as DirezioneGenerale,
regexp_substr(path, '[^/]+', 1, 2) as Direzione,
regexp_substr(path, '[^/]+', 1, 3) as Area,
regexp_substr(path, '[^/]+', 1, 4) as Divisione,
regexp_substr(path, '[^/]+', 1, 5) as Ufficio
from
(
select uo, uo_padre,
       RPAD('.', (level-1), '.') || uo AS tree,
       level as livello,
       CONNECT_BY_ROOT uo AS root_id,
       LTRIM(SYS_CONNECT_BY_PATH(Descr, '|'), '-') AS path,
       CONNECT_BY_ISLEAF AS leaf
FROM  (
select uo, uo_padre, descr from siaru_uniroma3_prod.vista_org_attiva) tab1
START WITH uo_padre =170117
CONNECT BY uo_padre = PRIOR uo
--ORDER SIBLINGS BY uo
) tp)
)
";
			#endregion

			#region QueryDipendentiValutazione
			qryDipendentiValutazione = @"select tab.Matricola as Matricola,  tab.Cognome || ' ' || tab.Nome as Cogn_nome, tab.CodiceFiscale as Cod_fisc, tab.Profilo,  tab.funzione as Aff_org

from 
(
select uo.descr as Funzione, sge.matricola as Matricola, upper(sge.cognome) as Cognome, Upper(sge.nome) as Nome, va.sesso as Sesso , 
to_char(va.data_nasc, 'DD/MM/YYYY') as DataNascita, va.cd_nazionalita as Nazionalita, va.naz_nasc as NazioneNascita, NULL as ProvinciaNascita,  comune.descr as LuogoNascita,
va.cod_fisc as CodiceFiscale, va.e_mail as eMail, null as DataAssunzione, sge.inquadr as Inquadramento, sge.aff_org as Afferenza, sge.SEDE_DESCR as Sede, 
sge.SEDE_INDIRIZZO as IndirizzoSede, sge.ruolo as Ruolo, sge.attivita as Attivita, sge.ds_attivita as DescrizioneAttivita,  sge.tipo_part_time, sge.perc,
rank() over(partition by sge.matricola order by sge.data_in desc) rango, sge.profilo_descr as Profilo
            from SIARU_UNIROMA3_PROD.sge_intervalli_plus sge
            inner join SIARU_UNIROMA3_PROD.vd_anagrafica va on sge.id_ab = va.id_ab
            inner join SIARU_UNIROMA3_PROD.vd_vista_org uo on sge.aff_org = uo.uo
            inner join SIARU_UNIROMA3_PROD.Comune comune on va.istat_nasc = comune.istat
            where 1 = 1
            and sge.data_in <= to_date('" + dataanalisi + "', 'DD/MM/YYYY') and sge.data_rap_fin > to_date('" + dataanalisi + "', 'DD/MM/YYYY') and sge.data_fin >= to_date('" + dataanalisi + @"', 'DD/MM/YYYY') and sge.ruolo in ('ND', 'D8', 'DC', 'CL', 'LS', 'LC', 'NM')
            and uo.data_fin > to_date('" + dataanalisi + @"', 'DD/MM/YYYY')
) tab, (select Matricola, Funzione, DescrizioneFunzione from 
(
select c.matricola, c.funzione, c.in_vigore_giur, fi.descr DescrizioneFunzione, row_number() over(partition by c.matricola order by c.in_vigore_giur desc) rango
from SIARU_UNIROMA3_PROD.CARRIERE c
inner join SIARU_UNIROMA3_PROD.FUNZIONI Fi ON c.funzione = fi.funzione
where 1=1
and c.evento = 32
and c.stato_carriere in ('E', 'U')
and c.termine > to_date('" + dataanalisi + @"', 'DD/MM/YYYY')
and (fi.descr like 'RESPONSABILE%'  or lower(fi.descr) like 'segretari%' or fi.descr = 'FUNZIONI SPECIALISTICHE AD INTERIM')
) f
where f.rango =1 ) funz,
(select ra.matricola, to_char(min(ra.data_rap_ini), 'DD/MM/YYYY') as DataAssunzione
from SIARU_UNIROMA3_PROD.V_IE_RU_PERS_RAPPORTI ra
where 1=1
and ra.ruolo in ('ND', 'D8', 'DC','CL', 'LS', 'LC', 'NM')
group by ra.matricola
) da
where 1=1
and tab.rango = 1
and tab.matricola = funz.matricola(+)
and tab.matricola = da.matricola(+)";
            #endregion

            #region qryDocentiETL
            qryDocentiETL = @"select  '" + dataanalisi + @"' as DataCampionamento,
tab.Afferenza as Afferenza, trim(tab.funzione) as DescrizioneAfferenza, tab.Matricola as Matricola, tab.Nome, tab.Cognome, tab.Sesso,
decode(funz.DescrizioneFunzione,null, 'N', 'S') as Responsabile, 
da.DataAssunzione as DataInizioCollaborazione, tab.Ruolo as RuoloContrattuale, funz.DescrizioneFunzione as Ruolo, tab.attivita as Attivita, tab.DescrizioneAttivita,  tab.tipo_impegno as TipoImpegno, null as PercentualePartTime,
tab.Inquadramento, tab.inquadramento as Livello,
case SUBSTR(tab.Ruolo,1,2) 
when 'PO' then 1
when 'PA' then 2
when 'RU' then 3
when 'RD' then 4
when 'PF' then 5
else null
end as GerarchiaLivello,
tab.DataNascita, tab.LuogoNascita, tab.CodiceFiscale, tab.eMail, null as AreaScientificoDisciplinare
from 
(
select uo.descr as Funzione, sge.matricola as Matricola, upper(sge.cognome) as Cognome, Upper(sge.nome) as Nome, va.sesso as Sesso , 
to_char(va.data_nasc, 'DD/MM/YYYY') as DataNascita, va.cd_nazionalita as Nazionalita, va.naz_nasc as NazioneNascita, NULL as ProvinciaNascita,  comune.descr as LuogoNascita,
va.cod_fisc as CodiceFiscale, va.e_mail as eMail, null as DataAssunzione, sge.inquadr as Inquadramento, sge.aff_org as Afferenza, sge.SEDE_DESCR as Sede, 
sge.SEDE_INDIRIZZO as IndirizzoSede, sge.ruolo as Ruolo, sge.tipo_impegno, sge.attivita as Attivita, sge.ds_attivita as DescrizioneAttivita,
rank() over(partition by sge.matricola order by sge.data_in desc) rango
            from SIARU_UNIROMA3_PROD.sge_intervalli_plus sge
            inner join SIARU_UNIROMA3_PROD.vd_anagrafica va on sge.id_ab = va.id_ab
            inner join SIARU_UNIROMA3_PROD.vd_vista_org uo on sge.aff_org = uo.uo
            inner join SIARU_UNIROMA3_PROD.Comune comune on va.istat_nasc = comune.istat
            where 1 = 1
            and sge.data_in <= to_date('" + dataanalisi + @"', 'DD/MM/YYYY') and sge.data_rap_fin > to_date('" + dataanalisi + @"', 'DD/MM/YYYY') 
            and sge.data_fin >= to_date('" + dataanalisi + @"', 'DD/MM/YYYY') and sge.ruolo in ('PO', 'PA', 'RU','RD', 'PF')
            and uo.data_fin > to_date('" + dataanalisi + @"', 'DD/MM/YYYY')
) tab, (select Matricola, Funzione, DescrizioneFunzione from 
(
select c.matricola, c.funzione, c.in_vigore_giur, fi.descr DescrizioneFunzione, row_number() over(partition by c.matricola order by c.in_vigore_giur desc) rango
from SIARU_UNIROMA3_PROD.CARRIERE c
inner join SIARU_UNIROMA3_PROD.FUNZIONI Fi ON c.funzione = fi.funzione
where 1=1
and c.evento = 32
and c.stato_carriere in ('E', 'U')
and c.termine > to_date('" + dataanalisi + @"', 'DD/MM/YYYY')) f
where f.rango =1 ) funz,
(select ra.matricola, ra.ruolo, to_char(min(ra.data_rap_ini), 'DD/MM/YYYY') as DataAssunzione
from SIARU_UNIROMA3_PROD.V_IE_RU_PERS_RAPPORTI ra
where 1=1
and ra.ruolo in ('PO', 'PA', 'RU','RD', 'PF')
group by ra.matricola, ra.ruolo
) da
where 1=1
and tab.rango = 1
and tab.matricola = funz.matricola(+)
and tab.matricola = da.matricola(+)
and tab.ruolo = da.ruolo
and tab.afferenza in (
select uo from (
select uo, uo_padre, livello,
regexp_substr(path, '[^/]+', 1, 1) as DirezioneGenerale,
regexp_substr(path, '[^/]+', 1, 2) as Direzione,
regexp_substr(path, '[^/]+', 1, 3) as Area,
regexp_substr(path, '[^/]+', 1, 4) as Divisione,
regexp_substr(path, '[^/]+', 1, 5) as Ufficio
from
(
select uo, uo_padre,
       RPAD('.', (level-1), '.') || uo AS tree,
       level as livello,
       CONNECT_BY_ROOT uo AS root_id,
       LTRIM(SYS_CONNECT_BY_PATH(Descr, '|'), '-') AS path,
       CONNECT_BY_ISLEAF AS leaf
FROM  (
select uo, uo_padre, descr from siaru_uniroma3_prod.vista_org_attiva) tab1
START WITH uo_padre =170117
CONNECT BY uo_padre = PRIOR uo
--ORDER SIBLINGS BY uo
) tp)

)
";
            #endregion

            #region qryDipendentiPAC
            qryDipendentiPAC = @"select s.Nome, s.Cognome, s.Ruolo_cod as Ruolo, s.Inquadramento as Livello, format(s.DataNascita, 'dd/MM/yyyy') as DataNascita,
s.ComuneNascita as LuogoNascita, s.CodiceFiscale, s.Email as eMail, d.cn + '@os.uniroma3.it' as NomeAccount, s.CellulareEsteso, s.SSD
from PAC_V_ANAG_SERVIZIO s
inner join ASI_dominio_new d on d.cod_fisc = s.CodiceFiscale
where 1=1
and Ruolo_cod in ('ND', 'D8', 'DC','CL', 'LS', 'LC', 'NM')
and DataFineRapp > convert(datetime, '" + dataanalisi + "',103)";
            #endregion

            #region qryDocentiPAC
            qryDocentiPAC = @"select s.Nome, s.Cognome, s.Ruolo_cod as Ruolo, s.Inquadramento as Livello, format(s.DataNascita, 'dd/MM/yyyy') as DataNascita,
s.ComuneNascita as LuogoNascita, s.CodiceFiscale, s.Email as eMail, d.cn + '@os.uniroma3.it' as NomeAccount, s.CellulareEsteso,SSD
from PAC_V_ANAG_SERVIZIO s
inner join ASI_dominio_new d on d.cod_fisc = s.CodiceFiscale
where 1=1
and Ruolo_cod in ('PO', 'PA', 'RU','RD', 'PF')
and DataFineRapp > convert(datetime, '" + dataanalisi + "',103)";
            #endregion
        }
        public void qryGestioneSD(string dataanalisi)
        {
            #region qryStudentiL170
            qryStudentiL170 = @"select distinct
	s.nome,
	s.Cognome,
	s.CodiceFiscale,
	se.InnerXml.value('(EventoBase/DataInizioValidita)[1]', 'nvarchar(max)') as DataInizioValidita,
	se.InnerXml.value('(EventoBase/DescrizioneTipologia)[1]', 'nvarchar(max)') as DescrizioneEvento,
	se.InnerXml.value('(EventoBase/AnnoAccademico/Denominazione)[1]', 'nvarchar(max)') as AnnoAccademico
into #disabilita
from Segreterie_Studenti s, Segreterie_Eventi se
where se.EventOwner  = s.UID
and se.ClassName like '%DichiarazioneDsa%'
and se.EventDeleted = 0
and not exists (
				select 1
				from Segreterie_ExtTable_Annullamenti ea WITH(NOLOCK)
				where ea.EventDeleted = 0
				and ea.LinkedEvent = se.UID)
-- Ultima Iscrizione
and se.uid = (select max(se2.uid)
									from Segreterie_Eventi se2 WITH(NOLOCK)
									where 
										se2.EventOwner = se.EventOwner
										and se2.ClassName like '%DichiarazioneDsa%'
										and se2.EventDeleted  = 0
										and not exists (
									       select 1
											from Segreterie_ExtTable_Annullamenti ea WITH(NOLOCK)
											where ea.EventDeleted = 0
											and ea.LinkedEvent = se2.UID)
						)
order by s.cognome
----------------------------------------------
select email.Owner, email.Recapito as email_ateneo
into #email
from Segreterie_Eventi_RecapitiData email
where email.TipoRecapito = 'Email'
	and email.Recapito not like '%STUD.UNIROMA3.IT'
-----------------------------------------------------
select email.Owner, email.Recapito as email_ateneo
into #email_ateneo
from Segreterie_Eventi_RecapitiData email
where email.TipoRecapito = 'Email'
	and email.Recapito like '%STUD.UNIROMA3.IT'
-----------------------------------------------------
select tel.Owner, max(tel.Recapito) as tel1
into #telefono1
from Segreterie_Eventi_RecapitiData tel
where tel.TipoRecapito = 'Telefono'
group by tel.Owner
-----------------------------------------------------
select tel.Owner, min(tel.Recapito) as tel1
into #telefono2
from Segreterie_Eventi_RecapitiData tel
where tel.TipoRecapito = 'Telefono'
group by tel.Owner
-----------------------------------------------------
SELECT distinct
	  o.AnnoAccademico as Coorte
	  ,o.Facolta as Dipartimento
	  ,o.Tipologia
	  ,o.ClasseDiLaurea_1
	  ,o.ClasseDiLaurea_2
	  ,o.CodiceFamiglia 
	  ,o.Denominazione
	  ,imm.Matricola
	  ,s.Cognome
      ,s.Nome
      ,s.CodiceFiscale
      ,s.Genere as Sesso
	  ,d.AnnoAccademico as AnnoAccademicoDichiarazione
-- DISABILITA'
	  ,CONVERT(VARCHAR(10),d.DataInizioValidita,112) as DataInizioValiditaDichiarazione
	  ,d.DescrizioneEvento as Dichiarazione
	  ,se.AA_AnnoInizio as AnnoIscrizione
      ,iscr.AnnoCorso
	  ,o.Tipologia
	  ,iscr.AnnoCorso as Iscrizione_anno_corso
	  ,iscr.TipoIscrizione as Iscrizione_tipo
	  -- COLONNA CON PERFEZIONAMENTO
	  ,(select max(vi.Perfezionato)
			from View_Iscritti vi
			where vi.IscrizioneCarrieraUid = imm.CarrieraUID and vi.Perfezionato = 1
		) as IscrizionePerfezionata,
	-- Presenza eventi di fine carriera
		(select max(se.ClassName + ' - ' + convert (varchar, se.DataEvento, 105) )
			from  
				Segreterie_Eventi se WITH(NOLOCK), 
				Segreterie_Eventi_DidatticaContexts did WITH(NOLOCK)
			where 
				se.UID = did.UID 
				and did.FineCarriera = 1
				and se.EventDeleted  = 0
				and did.Carriera = iscr.CarrieraUid
				-- Controllo eventi annullati 
				and not exists (
				select 1
					from Segreterie_ExtTable_Annullamenti ea WITH(NOLOCK)
					where ea.EventDeleted = 0
					and ea.LinkedEvent = se.UID)
			) as EventoFineCarriera
-- CONTATTI
	  ,(select max (email.email_ateneo)
			from #email_ateneo email
				where email.owner = s.uid) as Email_ateneo
	  ,(select max (email.email_ateneo)
			from #email email
				where email.owner = s.uid) as Email
	  ,(select max (tel1.tel1)
			from #telefono1 tel1
				where tel1.owner = s.uid) as Telefono1
	  ,(select max (tel2.tel1)
			from #telefono2 tel2
				where tel2.owner = s.uid) as Telefono2
	  ,(
	  select distinct	(vi.ResidenzaIndirizzo +' '+ vi.ResidenzaCivico +' '+ vi.ResidenzaCap  +' '+vi.ResidenzaComuneProvinciaSigla +' ' +vi.ResidenzaComuneDenominazione )
	  from View_Iscritti_extended vi WITH(NOLOCK)
	  where vi.CodiceFiscale = s.codiceFiscale
	  ) as ResidenzaIndirizzo

  FROM 
	  Segreterie_Eventi se WITH(NOLOCK), 
	  Segreterie_Studenti s WITH(NOLOCK),
	  Segreterie_ExtTable_Iscrizioni iscr WITH(NOLOCK),
	  Segreterie_ExtTable_Immatricolazioni imm WITH(NOLOCK),
	  Ordinamenti o WITH(NOLOCK),
	  #disabilita d

  where 
        s.UID = se.EventOwner
		and s.CodiceFiscale = d.CodiceFiscale 
		and se.UID = iscr.UID 
		and o.Facolta <> 'ERASMUS'
-- condizione per filtrare i soli eventi di Iscrizione
		and iscr.ManifestoUid = o.UID
		and iscr.CarrieraUid = imm.CarrieraUID
-- Controllo eventi annullati 
        and se.EventDeleted  = 0
        and not exists (
        select 1
        from Segreterie_ExtTable_Annullamenti ea
        where ea.EventDeleted = 0
        and ea.LinkedEvent = se.UID)
-- Ultima Iscrizione
		and se.AA_AnnoInizio = (select max(se2.AA_AnnoInizio)
									from Segreterie_Eventi se2 WITH(NOLOCK), Segreterie_ExtTable_Iscrizioni iscr2 WITH(NOLOCK)
									where se2.UID = iscr2.UID and iscr2.CarrieraUid  = iscr.CarrieraUid
									and se2.EventDeleted  = 0
									and not exists (
									       select 1
											from Segreterie_ExtTable_Annullamenti ea WITH(NOLOCK)
											where ea.EventDeleted = 0
											and ea.LinkedEvent = se2.UID)
		)

-- Clausole per report
		and (o.Tipologia = 'CorsoDiStudio')
		and  se.AA_AnnoInizio = '" + dataanalisi + @"'
		order by  s.Cognome

drop table #email_ateneo
drop table #email
drop table #disabilita
drop table #telefono1
drop table #telefono2
";
            #endregion
            #region qryStudentiL104
            qryStudentiL104 = @"select distinct
	s.nome,
	s.Cognome,
	s.CodiceFiscale,
	se.InnerXml.value('(EventoBase/DataInizioValidita)[1]', 'nvarchar(max)') as DataInizioValidita,
	--se.InnerXml.value('(EventoBase/DescrizioneEvento)[1]', 'nvarchar(max)') as DescrizioneEvento,
	se.InnerXml.value('(EventoBase/AnnoAccademico/Denominazione)[1]', 'nvarchar(max)') as AnnoAccademico,
	se.InnerXml.value('(EventoBase/Tipo)[1]', 'nvarchar(max)') as Tipo,
	se.InnerXml.value('(EventoBase/Permanente)[1]', 'nvarchar(max)') as Permanente,
	se.InnerXml.value('(EventoBase/Grado)[1]', 'nvarchar(max)') as Grado
into #disabilita
from Segreterie_Studenti s, Segreterie_Eventi se
where se.EventOwner  = s.UID
and se.ClassName like '%Handycap%'
and se.EventDeleted = 0
and not exists (
				select 1
				from Segreterie_ExtTable_Annullamenti ea WITH(NOLOCK)
				where ea.EventDeleted = 0
				and ea.LinkedEvent = se.UID)
-- Ultima Iscrizione
and se.uid = (select max(se2.uid)
									from Segreterie_Eventi se2 WITH(NOLOCK)
									where 
										se2.EventOwner = se.EventOwner
										and se2.ClassName like '%Handycap%'
										and se2.EventDeleted  = 0
										and not exists (
									       select 1
											from Segreterie_ExtTable_Annullamenti ea WITH(NOLOCK)
											where ea.EventDeleted = 0
											and ea.LinkedEvent = se2.UID)
						)
order by s.cognome
----------------------------------------------
select email.Owner, email.Recapito as email_ateneo
into #email
from Segreterie_Eventi_RecapitiData email
where email.TipoRecapito = 'Email'
	and email.Recapito not like '%STUD.UNIROMA3.IT'
-----------------------------------------------------
select email.Owner, email.Recapito as email_ateneo
into #email_ateneo
from Segreterie_Eventi_RecapitiData email
where email.TipoRecapito = 'Email'
	and email.Recapito like '%STUD.UNIROMA3.IT'
-----------------------------------------------------
select tel.Owner, max(tel.Recapito) as tel1
into #telefono1
from Segreterie_Eventi_RecapitiData tel
where tel.TipoRecapito = 'Telefono'
group by tel.Owner
-----------------------------------------------------
select tel.Owner, min(tel.Recapito) as tel1
into #telefono2
from Segreterie_Eventi_RecapitiData tel
where tel.TipoRecapito = 'Telefono'
group by tel.Owner
-----------------------------------------------------
SELECT distinct
	  o.AnnoAccademico as Coorte
	  ,o.Facolta as Dipartimento
	  ,o.Tipologia
	  ,o.ClasseDiLaurea_1
	  ,o.ClasseDiLaurea_2
	  ,o.CodiceFamiglia 
	  ,o.Denominazione
	  ,imm.Matricola
	  ,s.Cognome
      ,s.Nome
      ,s.CodiceFiscale
      ,s.Genere as Sesso
	  ,d.AnnoAccademico as AnnoAccademicoDichiarazione
-- DISABILITA'
	  ,CONVERT(VARCHAR(10),d.DataInizioValidita,112) as DataInizioValiditaDichiarazione
	  ,d.tipo as Disabilità_Tipologia
	  ,d.Permanente as Disabiltà_Permanente
	  ,d.Grado as Disabilità_Grado
	  --,d.DescrizioneEvento as Tipologia
	  ,se.AA_AnnoInizio as AnnoIscrizione
      ,iscr.AnnoCorso as Iscrizione_anno_corso
	  ,iscr.TipoIscrizione as Iscrizione_tipo
	  -- COLONNA CON PERFEZIONAMENTO
	  ,(select max(vi.Perfezionato)
			from View_Iscritti vi
			where vi.IscrizioneCarrieraUid = imm.CarrieraUID and vi.Perfezionato = 1
		) as IscrizionePerfezionata,
	-- Presenza eventi di fine carriera
		(select max(se.ClassName + ' - ' + convert (varchar, se.DataEvento, 105) )
			from  
				Segreterie_Eventi se WITH(NOLOCK), 
				Segreterie_Eventi_DidatticaContexts did WITH(NOLOCK)
			where 
				se.UID = did.UID 
				and did.FineCarriera = 1
				and se.EventDeleted  = 0
				and did.Carriera = iscr.CarrieraUid
				-- Controllo eventi annullati 
				and not exists (
				select 1
					from Segreterie_ExtTable_Annullamenti ea WITH(NOLOCK)
					where ea.EventDeleted = 0
					and ea.LinkedEvent = se.UID)
			) as EventoFineCarriera
-- CONTATTI
	  ,(select max (email.email_ateneo)
			from #email_ateneo email
				where email.owner = s.uid) as Email_ateneo
	  ,(select max (email.email_ateneo)
			from #email email
				where email.owner = s.uid) as Email
	  ,(select max (tel1.tel1)
			from #telefono1 tel1
				where tel1.owner = s.uid) as Telefono1
	  ,(select max (tel2.tel1)
			from #telefono2 tel2
				where tel2.owner = s.uid) as Telefono2
	  ,(
	  select distinct	(vi.ResidenzaIndirizzo +' '+ vi.ResidenzaCivico +' '+ vi.ResidenzaCap  +' '+vi.ResidenzaComuneProvinciaSigla +' ' +vi.ResidenzaComuneDenominazione )
	  from View_Iscritti_extended vi WITH(NOLOCK)
	  where vi.CodiceFiscale = s.codiceFiscale
	  ) as ResidenzaIndirizzo

  FROM 
	  Segreterie_Eventi se WITH(NOLOCK), 
	  Segreterie_Studenti s WITH(NOLOCK),
	  Segreterie_ExtTable_Iscrizioni iscr WITH(NOLOCK),
	  Segreterie_ExtTable_Immatricolazioni imm WITH(NOLOCK),
	  Ordinamenti o WITH(NOLOCK),
	  #disabilita d

  where 
        s.UID = se.EventOwner
		and s.CodiceFiscale = d.CodiceFiscale 
		and se.UID = iscr.UID 
		and o.Facolta <> 'ERASMUS'
-- condizione per filtrare i soli eventi di Iscrizione
		and iscr.ManifestoUid = o.UID
		and iscr.CarrieraUid = imm.CarrieraUID
-- Controllo eventi annullati 
        and se.EventDeleted  = 0
        and not exists (
        select 1
        from Segreterie_ExtTable_Annullamenti ea
        where ea.EventDeleted = 0
        and ea.LinkedEvent = se.UID)
-- Ultima Iscrizione
		and se.AA_AnnoInizio = (select max(se2.AA_AnnoInizio)
									from Segreterie_Eventi se2 WITH(NOLOCK), Segreterie_ExtTable_Iscrizioni iscr2 WITH(NOLOCK)
									where se2.UID = iscr2.UID and iscr2.CarrieraUid  = iscr.CarrieraUid
									and se2.EventDeleted  = 0
									and not exists (
									       select 1
											from Segreterie_ExtTable_Annullamenti ea WITH(NOLOCK)
											where ea.EventDeleted = 0
											and ea.LinkedEvent = se2.UID)
		)

-- Clausole per report
		and (o.Tipologia = 'CorsoDiStudio')
		and  se.AA_AnnoInizio = '" + dataanalisi + @"'
		order by  s.Cognome

drop table #email_ateneo
drop table #email
drop table #disabilita
drop table #telefono1
drop table #telefono2
";
            #endregion
        }
    }

   
}
