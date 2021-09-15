using System.Collections.Generic;

namespace ClientApi.Models
{
    public abstract class Customer
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Cvr { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public class ReadCustomer : Customer
    {
        public int Id { get; set; }
        public IEnumerable<int> OrderIds { get; set; }
    }
    
    public class CreateCustomer : Customer
    {

    }
}
