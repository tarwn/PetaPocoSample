using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace PetaPocoSample {
	public class TwoTableSample {

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

		public void QuerySeperately() {
			// already called CreateDatabase()
			// already called CreateTables()
			using (var db = GetDatabase()) {
				db.Insert(new Address() { Street = "street1", HouseNumber = "1" });
				db.Insert(new Person() { LastName = "lastname1", FirstName = "firstname1", AddressId = 1 });
				db.Insert(new Person() { LastName = "lastname1", FirstName = "firstname2", AddressId = 1 });

				var results = db.Query<Person>("WHERE LastName=@0", "lastname1");
				foreach (var person in results) {
					Console.WriteLine("Person: {0} {1} {2}", person.Id, person.LastName, person.FirstName);
					var address = db.Single<Address>("Where Id=@0", person.AddressId);
					Console.WriteLine("Address: {0} {1}", address.Street, address.HouseNumber);
				}

				int count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Person WHERE LastName=@0", "lastname1");
				Console.WriteLine("Count: " + count.ToString());
			}
		}

		public void QueryMultiStyle() { 
			// already called CreateDatabase()
			// already called CreateTables()
			using (var db = GetDatabase()) {
				db.Insert(new Address() { Street = "street1", HouseNumber = "1" });
				db.Insert(new Person() { LastName = "lastname1", FirstName = "firstname1", AddressId = 1 });
				db.Insert(new Person() { LastName = "lastname1", FirstName = "firstname2", AddressId = 1 });

				var results = db.Query<Person, Address>(@"SELECT Person.*, Address.* 
														  FROM Person 
															INNER JOIN Address ON Person.AddressId = Address.Id 
														  WHERE Person.lastname=@0", "lastname1");
				foreach (var person in results) {
					Console.WriteLine("Person: {0} {1}", person.LastName, person.FirstName);
					Console.WriteLine("Address: {0} {1}", person.Address.Street, person.Address.HouseNumber);
				}
			}
		}

		[TableName("Person")]
		[PrimaryKey("Id", autoIncrement = true)]
		public class Person {
			public int Id { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }
			public int AddressId { get; set; }

			[Ignore] public Address Address { get; set; }

			public override string ToString() {
				return String.Format("{0}: {1}, {2}", Id, LastName, FirstName);
			}
		}

		[TableName("Address")]
		[PrimaryKey("Id", autoIncrement = true)]
		public class Address {
			public int Id { get; set; }
			public string Street { get; set; }
			public string HouseNumber { get; set; }
		}
	}
}
