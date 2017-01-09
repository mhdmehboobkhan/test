using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp.Models
{
    public class user
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string pass { get; set; }

    }

    public class Customer
    {
        public Customer()
        {
            CustomerAddress = new Address();
        }

        public Guid CustomerGuid { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }
        public Address CustomerAddress { get; set; }

    }

    public class Address
    {
        public string _address_firstname { get; set; }
        public string _address_lastname { get; set; }
        public string _address_Company { get; set; }
        public string _address_country_id { get; set; }
        //as province
        public string _address_city { get; set; }
        //Address
        public string _address_street { get; set; }

        public string _address_postcode { get; set; }

        public string _address_telephone { get; set; }
        public string _address_fax { get; set; }
        
    }

}