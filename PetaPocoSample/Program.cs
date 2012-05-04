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

			//var sample = new SingleTableSample();
			//sample.CreateTable();
			//sample.CreatePerson();
			//sample.CreateDecoratedPerson();
			////sample.SelectRecords();
			////sample.SelectDecoratedRecords();
			//sample.SelectSingleRecord();
			//sample.SelectSingleDecoratedRecord();

			//var sample = new TwoTableSample();
			//sample.CreateTables();
			////sample.QuerySeperately();
			//sample.QueryMultiStyle();

			var sample = new UpdateSample();
			sample.AddSelectAndUpdate();
			sample.AddSelectAndUpdateDecorated();
			sample.AddSelectAndUpdateASpecificField();
			sample.UpdateWithoutAPOCO();
			sample.UpdateWithAStatement();
			sample.InsertAndUpdateWithSave();

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

		
	}


}
