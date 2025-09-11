using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OpenSilverShowcase.Datas.Models
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public static ObservableCollection<Contact> People { get; } = new ObservableCollection<Contact>
        {
            new Contact { FirstName = "John",   LastName = "Smith",   State = "California",   City = "Los Angeles",   Email = "john.smith@email.com",   Phone = "310-555-1234", Address = "123 Sunset Blvd" },
            new Contact { FirstName = "Emily",  LastName = "Johnson", State = "California",   City = "San Francisco", Email = "emily.johnson@email.com",Phone = "415-555-2345", Address = "456 Market Street" },
            new Contact { FirstName = "Michael",LastName = "Brown",   State = "New York",     City = "New York City", Email = "michael.brown@email.com",Phone = "212-555-3456", Address = "789 Broadway Ave" },
            new Contact { FirstName = "Sophia", LastName = "Davis",   State = "New York",     City = "Buffalo",       Email = "sophia.davis@email.com", Phone = "716-555-4567", Address = "101 Main Street" },
            new Contact { FirstName = "James",  LastName = "Miller",  State = "Illinois",     City = "Chicago",       Email = "james.miller@email.com", Phone = "312-555-5678", Address = "202 Michigan Ave" },

            new Contact { FirstName = "Olivia", LastName = "Wilson",  State = "Texas",        City = "Houston",       Email = "olivia.wilson@email.com",Phone = "713-555-6789", Address = "303 Bayou Road" },
            new Contact { FirstName = "William",LastName = "Moore",   State = "Texas",        City = "Dallas",        Email = "william.moore@email.com",Phone = "214-555-7890", Address = "404 Elm Street" },
            new Contact { FirstName = "Ava",    LastName = "Taylor",  State = "Florida",      City = "Miami",         Email = "ava.taylor@email.com",   Phone = "305-555-8901", Address = "505 Ocean Drive" },
            new Contact { FirstName = "Ethan",  LastName = "Anderson",State = "Florida",      City = "Orlando",       Email = "ethan.anderson@email.com",Phone = "407-555-9012",Address = "606 Orange Ave" },

            new Contact { FirstName = "Isabella",LastName = "Thomas", State = "Washington",   City = "Seattle",       Email = "isabella.thomas@email.com",Phone = "206-555-0123",Address = "707 Pine Street" },
            new Contact { FirstName = "Daniel", LastName = "Jackson", State = "Washington",   City = "Spokane",       Email = "daniel.jackson@email.com",Phone = "509-555-1234",Address = "808 Riverside Dr" },
            new Contact { FirstName = "Mia",    LastName = "White",   State = "Massachusetts",City = "Boston",        Email = "mia.white@email.com",   Phone = "617-555-2345", Address = "909 Beacon Street" },

            new Contact { FirstName = "Alexander",LastName = "Harris",State = "Colorado",     City = "Denver",        Email = "alex.harris@email.com", Phone = "303-555-3456", Address = "111 Colfax Ave" },
            new Contact { FirstName = "Charlotte",LastName = "Martin",State = "Nevada",       City = "Las Vegas",     Email = "charlotte.martin@email.com",Phone = "702-555-4567",Address = "222 Fremont Street" },
            new Contact { FirstName = "Benjamin", LastName = "Thompson",State="Arizona",      City = "Phoenix",       Email = "benjamin.thompson@email.com",Phone="602-555-5678",Address="333 Camelback Rd" },

            new Contact { FirstName = "Amelia", LastName = "Garcia",  State = "Georgia",      City = "Atlanta",       Email = "amelia.garcia@email.com",Phone = "404-555-6789", Address = "444 Peachtree St" },
            new Contact { FirstName = "Lucas",  LastName = "Martinez",State = "New Jersey",   City = "Newark",        Email = "lucas.martinez@email.com",Phone = "973-555-7890", Address = "555 Broad Street" },

            new Contact { FirstName = "Harper", LastName = "Robinson",State = "Oregon",       City = "Portland",      Email = "harper.robinson@email.com",Phone = "503-555-8901",Address = "666 Burnside St" },
            new Contact { FirstName = "Elijah", LastName = "Clark",   State = "Minnesota",    City = "Minneapolis",   Email = "elijah.clark@email.com", Phone = "612-555-9012", Address = "777 Nicollet Mall" },
            new Contact { FirstName = "Abigail",LastName = "Rodriguez",State="Pennsylvania",  City = "Philadelphia",  Email = "abigail.rodriguez@email.com",Phone="215-555-0123",Address="888 Market Street" }
        };


        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{FullName} ({City}, {State})";
        }
    }
}
