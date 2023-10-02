using Npgsql;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SixtyNames.Commands;
using System;
using System.Collections.Generic;
using System.IO;

namespace SixtyNames
{
    internal sealed class ProgramModel
    {
        #region Constants

        const string help = "help";
        const string getContractsAmount = "get";
        const string contractsByCounterparty = "2";
        const string getEmailsByContract = "3";
        const string updateContractsStatusForElderly = "4";
        const string getPersonsInMoscow = "5";

        #endregion


        #region Constructor
        private ProgramModel()
        {
            _eventAggregator = EventAggregator.GetInstance();
            
            _eventAggregator.OnCommandToModel += RunCommand;

            _connection = new NpgsqlConnection();
        }

       
        #endregion

        #region Fields

        private static ProgramModel _programModel;

        private static object _synchroot = new object();

        private readonly EventAggregator _eventAggregator;

        private readonly NpgsqlConnection _connection;

        #endregion

        #region Methods

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
                    using (var cmd = new NpgsqlCommand("SELECT count(*)FROM contracts where date >= '2023-01-01'::date;)", _connection))
                    {
                        var reader = cmd.ExecuteReader();

                        command.Response += (decimal)reader.GetFieldValue<decimal>(0);
                    }
                }
                else if (command is GetContractsByCounterparty)
                {
                    var result = new Dictionary<string, decimal>();

                    using (var cmd = new NpgsqlCommand("SELECT lp.CompanyName, SUM(c.Amount) FROM public.Contract c JOIN public.LegalPerson lp ON c.LegalPersonId = lp.Id WHERE lp.Country = 'Россия' GROUP BY lp.CompanyName", _connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string companyName = reader.GetString(0);
                            decimal amount = reader.GetDecimal(1);
                            result.Add(companyName, amount);
                        }
                    }
                }
                else if (command is GetEmailsByContract)
                {
                    var result = new List<string>();
                    using (var cmd = new NpgsqlCommand("SELECT p.Email FROM public.Contract c JOIN public.Person p ON c.PersonId = p.Id WHERE c.Amount > :amount AND c.SigningDate >= CURRENT_DATE - :days", _connection))
                    {
                        cmd.Parameters.AddWithValue("amount", amount);
                        cmd.Parameters.AddWithValue("days", days);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string email = reader.GetString(0);
                                result.Add(email);
                            }
                        }
                    }
                }
                else if(command is UpdateContractsStatusForElderly)
                {
                    using (var cmdSelect = new NpgsqlCommand("SELECT c.Id FROM public.Contract c JOIN public.Person p ON c.PersonId = p.Id WHERE c.Status = 'Действующий' AND p.Age >= 60", _connection))
                    using (var reader = cmdSelect.ExecuteReader())
                    {
                        int numRecordsUpdated = 0;
                        while (reader.Read())
                        {
                            int contractId = reader.GetInt32(0);
                            using (var cmdUpdate = new NpgsqlCommand("UPDATE public.Contract SET Status = 'Расторгнут' WHERE Id = :id", _connection))
                            {
                                cmdUpdate.Parameters.AddWithValue("id", contractId);
                                numRecordsUpdated += cmdUpdate.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else if(command is GetPersonsInMoscow)
                {
                    var result = new List<Person>();
                    using (var cmd = new NpgsqlCommand("SELECT p.Id, p.FirstName, p.LastName, p.MiddleName, p.Gender, p.Age, p.Workplace, p.Country, p.City, p.Address, p.Email, p.PhoneNumber, p.Birthday FROM public.Contract c JOIN public.Person p ON c.PersonId = p.Id JOIN public.LegalPerson lp ON c.LegalPersonId = lp.Id WHERE lp.City = 'Москва' AND c.Status = 'Действующий'", _connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var person = new Person
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                MiddleName = reader.GetString(3),
                                Gender = reader.GetString(4),
                                Age = reader.GetInt32(5),
                                Workplace = reader.GetString(6),
                                Country = reader.GetString(7),
                                City = reader.GetString(8),
                                Address = reader.GetString(9),
                                Email = reader.GetString(10),
                                PhoneNumber = reader.GetString(11),
                                Birthday = reader.GetDateTime(12)
                            };
                            result.Add(person);
                        }
                    }       
                }

                else if (command is WriteXmlReport)
                {
                   // Создание Excel-файла(требуется установить пакет EPPlus через NuGet)
                        using (var package = new ExcelPackage())
                    {
                        // Создание листа
                        var worksheet = package.Workbook.Worksheets.Add("Лист1");

                        // Заголовки столбцов отчета
                        worksheet.Cells[1, 1].Value = "Фамилия";
                        worksheet.Cells[1, 2].Value = "Имя";
                        worksheet.Cells[1, 3].Value = "Отчество";
                        worksheet.Cells[1, 4].Value = "E-mail";
                        worksheet.Cells[1, 5].Value = "Моб. телефон";
                        worksheet.Cells[1, 6].Value = "Дата рождения";

                        // Заполнение данными
                        int row = 2;
                        foreach (Person person in persons)
                        {
                            worksheet.Cells[row, 1].Value = person.LastName;
                            worksheet.Cells[row, 2].Value = person.FirstName;
                            worksheet.Cells[row, 3].Value = person.MiddleName;
                            worksheet.Cells[row, 4].Value = person.Email;
                            worksheet.Cells[row, 5].Value = person.PhoneNumber;
                            worksheet.Cells[row, 6].Value = person.Birthday.Date;
                            row++;
                        }

                        // Сохранение файла
                        var file = new FileInfo("report.xlsx");
                        package.SaveAs(file);
                    }
                }
            }
            catch (Exception ex) 
            {
                command.Response += ex.Message;
            }

            _eventAggregator.SendCommandToViewControl(command);
        }

        #endregion
    }
}
