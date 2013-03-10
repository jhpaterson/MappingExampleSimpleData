using System;
using System.Collections.Generic;
using Simple.Data;

namespace MappingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // note trace listener in app.config configured to log SQL to file trace.log

            var db = Database.OpenNamedConnection("CompanyConnection");

            #region DYANMIC OBJECTS

            // insert
            var newDynamic = db.Employees.Insert(name: "Jenson", username: "jenson", phonenumber: "4735", addressID: 2);    

            // SimpleRecord
            var emp1 = db.Employees.FindByName("Fernando");
            Console.WriteLine("SIMPLERECORD: Id: {0}, Name: {1}, Phone: {2}", 
                emp1.employeeID, emp1.name, emp1.phonenumber);

            // update
            emp1.phonenumber = "1111";   
            db.Employees.Update(emp1);

            // SimpleQuery and Select (with join)
            // this is a natural join which works because FK is defined in db
            // can do explicit join if join field is not FK
            var emps1 = db.Employees.FindAll(db.Employees.AddressID == 2)
                .select(
                    db.Employees.name, 
                    db.Employees.phonenumber,                               // note column in db is nchar(10) so filed has fixed length
                    db.Employees.Addresses.propertyname
                );
            Console.WriteLine("SIMPLEQUERY, JOIN");
            foreach (var em in emps1)
            {
                Console.WriteLine("Name: {0}, Phone: {1}, Address.propertyname: {2}", 
                    em.name, em.phonenumber, em.propertyname);
            }

            // SimpleQuery and With to get related object
            // note table name is Addresses even though an employee will only have one address
            var emps2 = db.Employees.FindAll(db.Employees.AddressID == 2)
                .WithAddresses();                     
            Console.WriteLine("SIMPLEQUERY, WITH");
            foreach (var em in emps2)
            {
                Console.WriteLine("Name: {0}, Phone: {1}, Addresses.propertyname: {2}",
                    em.name, em.phonenumber, em.Addresses.propertyname);
            }

            // delete
            db.Employees.Delete(name: "Jenson");

            #endregion

            #region POCOs

            // insert POCO
            var newEmp = new Employee{Name ="Checo", Username ="checo", PhoneNumber = "1846", AddressID = 2 };
            db.Employees.Insert(newEmp);

            // implicit cast to POCO
            Employee emp2 = db.Employees.FindByName("Checo");
            Console.WriteLine("SINGLE POCO: Id: {0}, Name: {1}", emp2.EmployeeID, emp2.Name);

            // update POCO
            emp2.Name = "Sergio";
            db.Employees.Update(emp2);

            // implicit cast to POCO collection 
            List<Employee> emps3 = db.Employees.
                FindAll(db.Employees.AddressID == 2);

            Console.WriteLine("POCO COLLECTION");
            foreach (var em in emps3)
            {
                Console.WriteLine("Id: {0}, Name: {1}", em.EmployeeID, em.Name);
            }

            // explicit cast to POCO collection with eager loading
            // variable em in loop is POCO Employee type with associated Address object
            // note naviagation property in Employee has to be called Addresses to match related
            // table name even though it is a reference to a single Address object
            var emps4 = db.Employees.FindAll(db.Employees.AddressID == 2)
                                       .WithAddresses()  
                                       .Cast<Employee>();

            Console.WriteLine("POCO COLLECTION EAGER LOAD");
            foreach (var em in emps4)
            {
                Console.WriteLine("Id: {0}, Name: {1}, Addresses.propertyname: {2}", 
                    em.EmployeeID, em.Name, em.Addresses.PropertyName);
            }

            #endregion

            Console.ReadLine();

        }
    }
}
