using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace PetaPocoSample {
	public class UpdateSample {

		private Database GetDatabase() {
			//return new Database("DataSource=\"test.sdf\"; Password=\"chrissiespassword\"", "System.Data.SqlServerCe.4.0");
			return new Database("Data Source=localhost;Initial Catalog=SampleStuff;Integrated Security=SSPI;", "System.Data.SqlClient");
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

		public void AddSelectAndUpdate() {
			using (var db = GetDatabase()) {
				//SQL: INSERT INTO [Person] ([LastName],[FirstName]) VALUES (@0,@1); SELECT SCOPE_IDENTITY() AS NewID;
				db.Insert("Person","Id", true, new Person() { LastName = "lastname1", FirstName = "firstname1" });

				//SQL: SELECT * FROM Person WHERE Id=@0
				var person = db.First<Person>("SELECT * FROM Person WHERE Id=@0", 1);
				person.FirstName = "NewFirstName";

				//SQL: UPDATE [Person] SET [LastName] = @0, [FirstName] = @1 WHERE [Id] = @2
				db.Update("Person", "Id", person);
			}
		}

		public void AddSelectAndUpdateDecorated() {
			using (var db = GetDatabase()) {
				//SQL: INSERT INTO [Person] ([LastName],[FirstName]) VALUES (@0,@1); SELECT SCOPE_IDENTITY() AS NewID;
				db.Insert(new DecoratedPerson() { LastName = "lastname1", FirstName = "firstname1" });

				//SQL: SELECT [Person].[Id], [Person].[LastName], [Person].[FirstName] FROM [Person] WHERE Id=@0
				var person = db.First<DecoratedPerson>("WHERE Id=@0", 1);
				person.FirstName = "NewFirstName";

				//SQL: UPDATE [Person] SET [LastName] = @0, [FirstName] = @1 WHERE [Id] = @2
				db.Update(person);
			}
		}

		public void AddSelectAndUpdateASpecificField() {
			using (var db = GetDatabase()) {
				//SQL: INSERT INTO [Person] ([LastName],[FirstName]) VALUES (@0,@1); SELECT SCOPE_IDENTITY() AS NewID;
				db.Insert("Person", "Id", true, new Person() { LastName = "lastname1", FirstName = "firstname1" });

				//SQL: SELECT * FROM Person WHERE Id=@0
				var person = db.First<Person>("SELECT * FROM Person WHERE Id=@0", 1);
				person.FirstName = "NewFirstName";

				//SQL: UPDATE [Person] SET [FirstName] = @0 WHERE [Id] = @1
				db.Update("Person", "Id", person, new string [] { "FirstName" });
			}
		}

		public void UpdateWithoutAPOCO() {
			using (var db = GetDatabase()) {
				//SQL: INSERT INTO [Person] ([LastName],[FirstName]) VALUES (@0,@1); SELECT SCOPE_IDENTITY() AS NewID;
				db.Insert("Person", "Id", true, new Person() { LastName = "lastname1", FirstName = "firstname1" });

				//SQL: UPDATE [Person] SET [FirstName] = @0 WHERE [Id] = @1
				db.Update("Person", "Id", new { Id=1, FirstName = "NewFirstName" });
			}
		}

		public void UpdateWithAStatement() {
			using (var db = GetDatabase()) {
				//SQL: INSERT INTO [Person] ([LastName],[FirstName]) VALUES (@0,@1); SELECT SCOPE_IDENTITY() AS NewID;
				db.Insert("Person", "Id", true, new Person() { LastName = "lastname1", FirstName = "firstname1" });

				//SQL: UPDATE [Person] SET FirstName=@0 WHERE Id=@1
				db.Update<DecoratedPerson>("SET FirstName=@0 WHERE Id=@1", "NewLastName", 1);
			}
		}

		public void InsertAndUpdateWithSave() {
			using (var db = GetDatabase()) {
				DecoratedPerson person = new DecoratedPerson() { LastName = "lastname1", FirstName = "firstname1" };

				//SQL: INSERT INTO [Person] ([LastName],[FirstName]) VALUES (@0,@1); SELECT SCOPE_IDENTITY() AS NewID;
				db.Save(person);	// person.Id is now set

				//SQL: SELECT * FROM Person WHERE Id=@0
				person.FirstName = "NewFirstName";

				//SQL: UPDATE [Person] SET [LastName] = @0, [FirstName] = @1 WHERE [Id] = @2
				db.Save(person);
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
		public class DecoratedPerson {
			public int Id { get; set; }
			public string LastName { get; set; }
			public string FirstName { get; set; }

			public override string ToString() {
				return String.Format("{0}: {1}, {2}", Id, LastName, FirstName);
			}
		}

	}
}
