using Npgsql;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SixtyNames.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace SixtyNames
{
	/// <summary>
	/// Класс, реализующий модель 
	/// </summary>
	internal sealed class ProgramModel
	{
		#region Constants
		/// <summary>
		/// Константы, используемые в жизненном цикле программы
		/// </summary>
		public const string help = "help";
		public const string getContractsAmount = "1";
		public const string getContractsByCounterparty = "2";
		public const string getEmailsByContract = "3";
		public const string getUpdateContractsStatusForElderly = "4";
		public const string getPersonsInMoscow = "5";

		#endregion

		#region Constructor
		/// <summary>
		/// Конструктор ProgramModel
		/// </summary>
		private ProgramModel()
		{
			_eventAggregator = EventAggregator.GetInstance();
			
			_eventAggregator.OnCommandToModel += RunCommand;

			_connection = new NpgsqlConnection();
		}

	   
		#endregion

		#region Fields
		/// <summary>
		/// Поля ProgramControl
		/// </summary>
		private static ProgramModel _programModel;

		private static object _synchroot = new object();

		private readonly EventAggregator _eventAggregator;

		private readonly NpgsqlConnection _connection;

		#endregion

		#region Methods
		/// <summary>
		/// Инстанциирование ProgramModel
		/// </summary>
		/// <returns></returns>
		public static ProgramModel GetInstance()
		{
			lock (_synchroot)
			{
				if (_programModel == null)
				{
					_programModel = new ProgramModel();
				}

				return _programModel;
			}
		}
		/// <summary>
		/// Исполнение команд
		/// </summary>
		/// <param name="command"></param>
		public void RunCommand(ICommand command)
		{
			try
			{
				_connection.ConnectionString = "Server=localhost;Port=6000;User Id=postgres;Password=123;Database=local_db;";
				_connection.Open();

				if (command is HelpCommand)
				{
					command.Response +=
					"1. Вывести сумму всех заключенных договоров за текущий год: \r\n" +
					"2. Вывести сумму заключенных договоров по каждому контрагенту из России: \r\n" +
					"3. Вывести список e-mail уполномоченных лиц, заключивших договора за последние 30 дней, на сумму больше 40000: \r\n" +
					"4. Изменить статус договора на \"Расторгнут\" для физических лиц, у которых есть действующий договор, и возраст которых старше 60 лет включительно: \r\n" +
					"5. Создать отчет : \r\n";

				}
				else if (command is GetTotalContractAmount)
				{
					var cmd = new NpgsqlCommand("select sum(amount) from contract where status = 'closed' and signingdate is not null", _connection);
					var res = cmd.ExecuteScalar();
					command.Response += res;
				}
				else if (command is GetContractsByCounterparty)
				{
					using (var cmd = new NpgsqlCommand("SELECT legalperson.companyname , SUM(contract.amount) " +
													   "FROM contract JOIN legalperson ON contract.legalpersonid = legalperson.id " +
													   "WHERE legalperson.country = 'Russia' GROUP BY legalperson.companyname ", _connection))
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							string companyName = reader.GetString(0);
							decimal amount = reader.GetDecimal(1);
							command.Response += $"{companyName} - {amount}\n";
						}
					}
				}
				else if (command is GetEmailsByContract)
				{
					using (var cmd = new NpgsqlCommand("SELECT person.email from public.contract JOIN public.person on contract.personid = person.id " +
													   "WHERE contract.amount > 40000 AND (current_date - contract.signingdate) < 30", _connection))
					{
						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								command.Response += reader.GetString(0) + "\n";
							} 
						}
					}
				}
				else if(command is GetUpdateContractsStatusForElderly)
				{
					using (var cmdUpdate = new NpgsqlCommand("update contract as c set status = 'closed' " +
															 "from person as p where p.id = c.personid " +
															 "and c.status = 'work' and p.age >= 60", _connection))
					{
						cmdUpdate.ExecuteNonQuery();
						command.Response += "Успешно выполнено!";
					}
				}
				else if (command is GetUpdateContractsStatusForElderly)
				{
					using (var cmdSelect = new NpgsqlCommand("SELECT contract.id from public.contract JOIN public.person ON contract.personid = person.id " +
										 "WHERE contract.status = 'work' AND person.age >= 60", _connection))
					using (var reader = cmdSelect.ExecuteReader())
					{
						int numRecordsUpdated = 0;
						while (reader.Read())
						{
							int contractId = reader.GetInt32(0);
							using (var cmdUpdate = new NpgsqlCommand("UPDATE public.contract SET status = 'closed' WHERE Id = :id", _connection))
							{
								cmdUpdate.Parameters.AddWithValue("id", contractId);
								numRecordsUpdated += cmdUpdate.ExecuteNonQuery();
							}
						}
					}
				}
				else if (command is GetPersonsInMoscow)
				{
					var persons = new List<Person>();
					using (var cmd = new NpgsqlCommand("SELECT person.firstname, person.lastname, person.middlename, person.email, person.phonenumber, person.birthday " +
													   "FROM person JOIN contract ON person.id = contract.personid " +
													   "JOIN legalperson ON person.legalperson = legalperson.companyname " +
													   "WHERE contract.status = 'work' AND legalperson.city = 'Moscow' ", _connection))
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{                        
							persons.Add(new Person()
							{
								FirstName = reader.GetString(0),
								LastName = reader.GetString(1),
								MiddleName = reader.GetString(2),
								Email = reader.GetString(3),
								PhoneNumber = reader.GetString(4),
								Birthday = reader.GetDateTime(5)
							});
						}
					}
					if (persons.Count == 0)
					{
						command.Response += "У вас нет данных для записи в отчет!";
					}
					else
					{
						using (ExcelHelper helper = new ExcelHelper())
						{
							var resOpen = helper.Open(Path.Combine(Environment.CurrentDirectory, "Report.xls"));
							if (!resOpen.isSuccess)
							{
								command.Exception = resOpen.exception;
							}

							for (var i = 0; i < persons.Count; i++)
							{
								helper.Set("A", i + 1, persons[i].FirstName);
								helper.Set("B", i + 1, persons[i].LastName);
								helper.Set("C", i + 1, persons[i].MiddleName);
								helper.Set("D", i + 1, persons[i].Email);
								helper.Set("E", i + 1, persons[i].PhoneNumber);
								helper.Set("F", i + 1, persons[i].Birthday.ToString("dd.MM.yyyy"));
							}

							helper.Save();

							command.Response += "Отчёт успешно сформирован.";
						}
					}
				}
			}
			catch (Exception ex) 
			{
				command.Response += ex.Message;

				command.Exception = ex;
			}
			finally
			{
				_connection.Close();
			}

			_eventAggregator.SendCommandToViewControl(command);
		}

		#endregion
	}
}
