using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CSharpFinal.Models{
    public class User : DbContext{
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public double Wallet { get; set; }
        public string Password { get; set; }
        public List<Product> Selling { get; set; }
        public List<Bidder> Bidder_On { get; set; }
        public User()
        {
            Selling = new List<Product>();
            Bidder_On = new List<Bidder>();
        }
    }
}