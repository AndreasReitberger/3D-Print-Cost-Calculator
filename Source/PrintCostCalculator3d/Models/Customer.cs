using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models
{
    public class Customer
    {
        public string CustomerId
        { get; set; }
        public string Firstname
        { get; set; }
        public string Lastname
        { get; set; }

        public Customer() { }

        public Customer(string CustomerId, string Firstname, string Lastname)
        {
            this.CustomerId = CustomerId;
            this.Firstname = Firstname;
            this.Lastname = Lastname;
        }
    }
}
