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
			p.CreateDatabase();
			p.CreateTable();
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
				db.Execute("CREATE TABLE Person (LastName nvarchar (40) NOT NULL, FirstName nvarchar (40))");
			}
		}


	}
}
