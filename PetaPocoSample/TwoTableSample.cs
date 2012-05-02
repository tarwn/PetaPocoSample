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
				db.Execute("CREATE TABLE Person (Id int IDENTITY(1,1) PRIMARY KEY, LastName nvarchar (40) NOT NULL, FirstName nvarchar (40), AddressId int NOT NULL));");
				db.Execute("CREATE TABLE Address (Id int IDENTITY(1,1) PRIMARY KEY, Street nvarchar (40) NOT NULL, HouseNumber nvarchar (10));");
			}
		}

		[TableName("Person")]
		[PrimaryKey("Id", autoIncrement = true)]
		public class Person {
			public int Id { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }
			public int AddressId { get; set; }
			public Address Address { get; set; }

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
