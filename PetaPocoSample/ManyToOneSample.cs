using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace PetaPocoSample {
	public class ManyToOneSample {

		private Database GetDatabase() {
			return new Database("DataSource=\"test.sdf\"; Password=\"chrissiespassword\"", "System.Data.SqlServerCe.4.0");
			//return new Database("Data Source=localhost;Initial Catalog=SampleStuff;Integrated Security=SSPI;", "System.Data.SqlClient");
		}

		public void CreateTables() {
			using (var db = new Database("DataSource=\"test.sdf\"; Password=\"chrissiespassword\"", "System.Data.SqlServerCe.4.0")) {
				db.Execute("CREATE TABLE Person (Id int IDENTITY(1,1) PRIMARY KEY, LastName nvarchar (40) NOT NULL, FirstName nvarchar (40), AddressId int NOT NULL);");
				db.Execute("CREATE TABLE Address (Id int IDENTITY(1,1) PRIMARY KEY, Street nvarchar (40) NOT NULL, HouseNumber nvarchar (10));");
			}
		}

		public void InsertTwoPersonsAndAnAddress() {
			using (var db = GetDatabase()) {
				db.Insert("Address", "Id", true, new { Street = "street1", HouseNumber = "1" });
				db.Insert("Person", "Id", true, new { LastName = "lastname1", FirstName = "firstname1", AddressId = 1 });
				db.Insert("Person", "Id", true, new { LastName = "lastname1", FirstName = "firstname2", AddressId = 1 });
			}
		}
		
		public void SelectUsingDecoratedClasses() {
			using (var db = GetDatabase()) {
				var results = db.Query<DecoratedPerson, DecoratedAddress>(@"SELECT Person.*, Address.* 
														  FROM Person 
															INNER JOIN Address ON Person.AddressId = Address.Id 
														  WHERE Person.lastname=@0", "lastname1");
				foreach (var person in results) {
					Console.WriteLine("Person: {0} {1}", person.LastName, person.FirstName);
					Console.WriteLine("Address: {0} {1}", person.Address.Street, person.Address.HouseNumber);
				}
			}
		}

		public void SelectUsingMappingAndPOCO() {
			using (var db = GetDatabase()) {
				var results = db.Query<Person, Address, Person>(
										(p, a) => { p.Address = a; return p; },
										@"SELECT Person.*, Address.* 
											FROM Person 
											INNER JOIN Address ON Person.AddressId = Address.Id 
											WHERE Person.lastname=@0", "lastname1");
				
				foreach (var person in results) {
					Console.WriteLine("Person: {0} {1}", person.LastName, person.FirstName);
					Console.WriteLine("Address: {0} {1}", person.Address.Street, person.Address.HouseNumber);
				}
			}
		}

		public void SelectOneToMany() {
			using (var db = GetDatabase()) {
				var results = db.Query<AddressWithPeople, Person, AddressWithPeople>(
										new AddressToPersonRelator().MapIt,
										@"SELECT Address.*, Person.*
											FROM Person 
											INNER JOIN Address ON Person.AddressId = Address.Id 
											WHERE Address.Id=@0", 1);

				foreach (var address in results) {
					Console.WriteLine("Address: {0} {1}", address.Street, address.HouseNumber);
					foreach(var person in address.Persons)
						Console.WriteLine("\tPerson: {0} {1}", person.LastName, person.FirstName);
				}
			}
		}

		public void SelectReportPOCO() {
			using (var db = GetDatabase()) {
				var results = db.Query<ReportPerson>(
										@"SELECT Person.*, Address.Street, Address.HouseNumber 
											FROM Person 
											INNER JOIN Address ON Person.AddressId = Address.Id 
											WHERE Person.lastname=@0", "lastname1");

				foreach (var person in results) {
					Console.WriteLine("Person: {0} {1}", person.LastName, person.FirstName);
					Console.WriteLine("Address: {0} {1}", person.Street, person.HouseNumber);
				}
			}
		}

		public void SelectWithDynamics() {
			using (var db = GetDatabase()) {
				var results = db.Query<dynamic>(
										@"SELECT Person.*, Address.Street, Address.HouseNumber 
											FROM Person 
											INNER JOIN Address ON Person.AddressId = Address.Id 
											WHERE Person.lastname=@0", "lastname1");

				foreach (var person in results) {
					Console.WriteLine("Person: {0} {1}", person.LastName, person.FirstName);
					Console.WriteLine("Address: {0} {1}", person.Street, person.HouseNumber);
				}
			}
		}

		public class Person {
			public int Id { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }
			public int AddressId { get; set; }
			public Address Address { get; set; }
		}

		public class Address {
			public int Id { get; set; }
			public string Street { get; set; }
			public string HouseNumber { get; set; }
		}

		public class AddressWithPeople : Address { 
			public List<Person> Persons { get; set; }
		}

		public class AddressToPersonRelator {
			public AddressWithPeople current;
			
			public AddressWithPeople MapIt(AddressWithPeople a, Person p) {
				if (a == null)
					return current;

				if (a != null && current != null && a.Id == current.Id) {
					current.Persons.Add(p);
					return null;
				}

				var prev = current;
				current = a;
				current.Persons = new List<Person>() { p };

				return prev;
			}
		}

		[TableName("Person")]
		[PrimaryKey("Id", autoIncrement = true)]
		public class DecoratedPerson {
			public int Id { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }
			public int AddressId { get; set; }

			[Ignore]
			public DecoratedAddress Address { get; set; }

			public override string ToString() {
				return String.Format("{0}: {1}, {2}", Id, LastName, FirstName);
			}
		}

		[TableName("Address")]
		[PrimaryKey("Id", autoIncrement = true)]
		public class DecoratedAddress {
			public int Id { get; set; }
			public string Street { get; set; }
			public string HouseNumber { get; set; }
		}

		public class ReportPerson {
			public int Id { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }
			public string Street { get; set; }
			public string HouseNumber { get; set; }
		}
	}
}
