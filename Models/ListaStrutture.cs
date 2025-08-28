using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;


namespace SRA.Models
{
    public class ListaStrutture
    {
        public List<UnitàOrganizzativa> Lista { get; set; }


        public void Sposta(string uo, string uo_id_padre)
        {
            int index;
            index = Lista.IndexOf(Lista.Find(d => d.UO_ID == uo));
            
            if(index == -1)
            {
                return;
            }
            
            var UO_Padre = Lista.Where(d => d.UO_ID == uo_id_padre).FirstOrDefault();
            Lista[index].UO_Padre = uo_id_padre;
            Lista[index].Livello = UO_Padre.Livello + 1;
            switch (Lista[index].Livello)
            {
                case 2:
                    Lista[index].Livello1 = UO_Padre.Livello1;
                    Lista[index].Livello2 = Lista[index].Nome;
                    break;
                case 3:
                    Lista[index].Livello1 = UO_Padre.Livello1;
                    Lista[index].Livello2 = UO_Padre.Livello2;
                    Lista[index].Livello3 = Lista[index].Nome;
                    break;
                case 4:
                    Lista[index].Livello1 = UO_Padre.Livello1;
                    Lista[index].Livello2 = UO_Padre.Livello2;
                    Lista[index].Livello3 = UO_Padre.Livello3;
                    Lista[index].Livello4 = Lista[index].Nome;
                    break;
                case 5:
                    Lista[index].Livello1 = UO_Padre.Livello1;
                    Lista[index].Livello2 = UO_Padre.Livello2;
                    Lista[index].Livello3 = UO_Padre.Livello3;
                    Lista[index].Livello4 = UO_Padre.Livello4;
                    Lista[index].Livello5 = Lista[index].Nome;
                    break;
            }
            
            var listafigli = Lista.Where(d => d.UO_Padre == uo).ToList();
            foreach (var item in listafigli)
            {
                this.Sposta(item.UO_ID, uo);
            }


        }
    }
}
