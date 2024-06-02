using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERTC_Client.Helper
{
    public class Product
    {
        public int Id { get; set; }
        public string nom_produit { get; set; }
        public string type_produit { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int entreprise_id { get; set; }
    }

}
