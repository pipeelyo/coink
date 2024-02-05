namespace coink.Models
{
    public class Client
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? phone { get; set; }
        public int? id_country { get; set; }
        public int? id_department { get; set; }
        public int? id_municipality { get; set; }
        public string? addres { get; set; }

        public string? country_name { get; set; }
        public string? department_name { get; set; }
        public string? municipality_name { get; set; }
    }
}
