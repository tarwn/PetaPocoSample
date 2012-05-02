using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace PetaPocoSample {
	public class SingleTableSample {

		private Database GetDatabase() {
			return new Database("DataSource=\"test.sdf\"; Password=\"chrissiespassword\"", "System.Data.SqlServerCe.4.0");
			//return new Database("Data Source=localhost;Initial Catalog=SampleStuff;Integrated Security=SSPI;", "System.Data.SqlClient");
		}

		public void CreateTable() {
			using (var db = new Database("DataSource=\"test.sdf\"; Password=\"chrissiespassword\"", "System.Data.SqlServerCe.4.0")) {
				db.Execute("CREATE TABLE Person (Id int IDENTITY(1,1) PRIMARY KEY, LastName nvarchar (40) NOT NULL, FirstName nvarchar (40));");
			}
		}

		public void OutputPersonTable(string name) {
			Console.WriteLine("\n" + name);
			using (var db = GetDatabase()) {
				var persons = db.Query<Person>("SELECT * FROM Person;");
				Console.WriteLine(db.LastSQL);
				Console.WriteLine(String.Join("\n", persons.Select(p => p.ToString())));
			}
		}

		public void CreatePerson() {
			using (var db = GetDatabase()) {
				db.Insert("Person", "Id", true, new Person() { LastName = "lastname1", FirstName = "firstname1" });
			}

			OutputPersonTable("CreatePerson");
		}

		public void CreateDecoratedPerson() {
			using (var db = GetDatabase()) {
				db.Insert(new DecoratedPerson() { LastName = "lastname3", FirstName = "firstname3" });
			}

			OutputPersonTable("CreateDecoratedPerson");
		}

		public void SelectRecords() {
			using (var db = GetDatabase()) {
				var results = db.Query<Person>("SELECT * FROM Person WHERE lastname=@0", "lastname1");
				Console.WriteLine(String.Join("\n", results.Select(p => p.ToString())));
			}
		}

		public void SelectDecoratedRecords() {
			using (var db = GetDatabase()) {
				var results = db.Query<DecoratedPerson>("WHERE lastname=@0", "lastname1");
				Console.WriteLine(String.Join("\n", results.Select(p => p.ToString())));
			}
		}

		public void SelectSingleRecord() {
			using (var db = GetDatabase()) {
				var result = db.Single<Person>("SELECT * FROM Person WHERE lastname=@0", "lastname1");
				Console.WriteLine(String.Format("{0}: {1}", result.GetType(), result));
			}
		}

		public void SelectSingleDecoratedRecord() {
			using (var db = GetDatabase()) {
				var result = db.Single<DecoratedPerson>("WHERE lastname=@0", "lastname1");
				Console.WriteLine(String.Format("{0}: {1}", result.GetType(), result));
			}
		}

		public void SelectOtherIndividualDecoratedRecords() {
			using (var db = GetDatabase()) {
				// T
				var result = db.First<DecoratedPerson>("SELECT * FROM Person WHERE lastname=@0", "lastname1");
				// List<T>
				var results = db.SkipTake<DecoratedPerson>(1, 1, "SELECT * FROM Person WHERE lastname=@0", "lastname1");
				//IEnumerable<T>
				var results2 = db.Query<DecoratedPerson>("SELECT * FROM Person WHERE lastname=@0", "lastname1");
				//List<T>
				var results3 = db.Fetch<DecoratedPerson>("SELECT * FROM Person WHERE lastname=@0", "lastname1");
				//Page<T> - page #2 and page size of 1
				var results4 = db.Page<DecoratedPerson>(2, 1, "SELECT * FROM Person WHERE lastname=@0", "lastname1");
			}
		}

		public class Person {
			public int Id { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }

			public override string ToString() {
				return String.Format("{0}: {1}, {2}", Id, LastName, FirstName);
			}
		}

		[TableName("Person")]
		[PrimaryKey("Id", autoIncrement = true)]
		public class DecoratedPerson : Person { }
	}
}
