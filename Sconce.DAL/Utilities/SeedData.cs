using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Utilities
{
    public class SeedData : ISeedData
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeedData(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        //public async Task DataSeedingAsync()
        //{
            
        //}

        public async Task IdentityDataSeedingAsync()
        {
            var defaultPassword = _configuration["SeedSettings:DefaultPassword"] ?? "Temp@123"; // fallback just in case

            foreach (var roleName in new[] { "Super Admin", "Admin", "Student", "Instructor", "Parent" })
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            if (!await _userManager.Users.AnyAsync())
            {
                var user1 = new ApplicationUser()
                {
                    Email = "anas.melhem@gmail.com",
                    FullName = "Anas Melhem",
                    UserName = "anasmelhem",
                    PhoneNumber = "0598765432",
                    Country = "Palestine",
                    City = "Tulkarm",
                    Street = "Yafa Street",
                    EmailConfirmed = true
                };
                var user2 = new ApplicationUser()
                {
                    Email = "layla.saabna@gmail.com",
                    FullName = "Layla Sa'abna",
                    UserName = "laylasaabna",
                    PhoneNumber = "0598765432",
                    Country = "Palestine",
                    City = "Jenin",
                    Street = "Fahma Main Street",
                    EmailConfirmed = true
                };
                //var user3 = new ApplicationUser()
                //{
                //    Email = "afnanalaa49@gmail.com",
                //    FullName = "Afnan Abo-Asal",
                //    UserName = "implutogal",
                //    PhoneNumber = "0598765432",
                //    Country = "Palestine",
                //    City = "Anabta",
                //    Street = "Nablus-Tulkarm Street",
                //    EmailConfirmed = true
                //};
                //var user4 = new ApplicationUser()
                //{
                //    Email = "baraa.aboasal@gmail.com",
                //    FullName = "Bara'a Abo-Asal",
                //    UserName = "baraaaboasal",
                //    PhoneNumber = "0598765432",
                //    Country = "Palestine",
                //    City = "Ramallah",
                //    Street = "Irsal Street",
                //    EmailConfirmed = true
                //};
                //var user5 = new ApplicationUser()
                //{
                //    Email = "bayan.aboasal@gmail.com",
                //    FullName = "Bayan Abo-Asal",
                //    UserName = "bayanaboasal",
                //    PhoneNumber = "0598765432",
                //    Country = "Palestine",
                //    City = "Anabta",
                //    Street = "Nablus-Tulkarm Street",
                //    EmailConfirmed = true
                //};

                await _userManager.CreateAsync(user1, defaultPassword);
                await _userManager.CreateAsync(user2, defaultPassword);
                //await _userManager.CreateAsync(user3, defaultPassword);
                //await _userManager.CreateAsync(user4, defaultPassword);
                //await _userManager.CreateAsync(user5, defaultPassword);

                await _userManager.AddToRoleAsync(user1, "Admin");
                await _userManager.AddToRoleAsync(user2, "Super Admin");
                //await _userManager.AddToRoleAsync(user3, "Student");
                //await _userManager.AddToRoleAsync(user4, "Instructor");
                //await _userManager.AddToRoleAsync(user5, "Parent");
            }
        }
    }
}
