using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.IO;
using PetaPoco;

namespace PetaPocoSample {
	class Program {
		static void Main(string[] args) {
			var p = new Program();
			//p.CreateDatabase();
			//p.CreateTable();
			p.CreatePerson();
			p.CreatePerson2();
			p.CreateDecoratedPerson();
			Console.Read();
			p.SelectRecords();
			Console.Read();
			p.SelectDecoratedRecords();

			Console.WriteLine("\nPress enter to continue");
			Console.Read();
		}

		private SqlCeEngine CreateDatabase() {
			if (File.Exists("test.sdf")) File.Delete("test.sdf");

			string connectionString = "DataSource=\"test.sdf\"; Password=\"chrissiespassword\"";
			var en = new SqlCeEngine(connectionString);
			en.CreateDatabase();
			return en;
		}

		private void CreateTable() {
			using (var db = new Database("DataSource=\"test.sdf\"; Password=\"chrissiespassword\"", "System.Data.SqlServerCe.4.0")) {
				db.Execute("CREATE TABLE Person (LastName nvarchar (40) NOT NULL, FirstName nvarchar (40));");
			}
		}

		private Database GetDatabase() {
//			return new Database("DataSource=\"test.sdf\"; Password=\"chrissiespassword\"", "System.Data.SqlServerCe.4.0");
			return new Database("Data Source=localhost;Initial Catalog=SampleStuff;Integrated Security=SSPI;", "System.Data.SqlClient");
		}

		private void OutputPersonTable(string name) {
			Console.WriteLine("\n" + name);
			using (var db = GetDatabase()) {
				var persons = db.Query<Person>("SELECT * FROM Person;");
				Console.WriteLine(db.LastSQL);
				Console.WriteLine(String.Join("\n", persons.Select(p => p.ToString())));
			}
		}

		private void CreatePerson() {
			using (var db = GetDatabase()) {
				db.Insert("Person", null, new Person() { LastName = "lastname1", FirstName = "firstname1" });
			}

			OutputPersonTable("CreatePerson");
		}

		private void CreatePerson2() {
			using (var db = GetDatabase()) {
				db.Insert(new Person() { LastName = "lastname2", FirstName = "firstname2" });
			}

			OutputPersonTable("CreatePerson2");
		}

		private void CreateDecoratedPerson() {
			using (var db = GetDatabase()) {
				db.Insert(new DecoratedPerson() { LastName = "lastname3", FirstName = "firstname3" });
			}

			OutputPersonTable("CreateDecoratedPerson");
		}

		private void SelectRecords() {
			using (var db = GetDatabase()) {
				var results = db.Query<Person>("SELECT * FROM Person WHERE lastname=@0", "lastname1");
				Console.WriteLine(String.Join("\n", results.Select(p => p.ToString())));
			}
		}

		private void SelectDecoratedRecords() {
			using (var db = GetDatabase()) {
				var results = db.Query<DecoratedPerson>("WHERE lastname=@0", "lastname1");
				Console.WriteLine(String.Join("\n", results.Select(p => p.ToString())));
			}
		}
	}

	public class Person {
		public string LastName { get; set; }
		public string FirstName { get; set; }

		public override string ToString() {
			return String.Format("{0}, {1}", LastName, FirstName);
		}
	}

	[PetaPoco.TableName("Person")]
	public class DecoratedPerson : Person {	}
}
