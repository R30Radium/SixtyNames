using SixtyNames.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames
{
    /// <summary>
    /// Класс, реализующий представление 
    /// </summary>
    internal sealed class ProgramViewControl
    {
        #region Constructors
        /// <summary>
        /// Коструктор ProgramViewControl(
        /// </summary>
        private ProgramViewControl()
        {
            _eventAggreagator = EventAggregator.GetInstance();
            _eventAggreagator.OnCommandToViewControl += ResponseForModel;

            _isWorking = true;
        }

        #endregion

        #region Fields
        /// <summary>
        /// Поля ProgramControl
        /// </summary>
        private static ProgramViewControl _programControll;
        private static object _synchroot = new object();
        private readonly EventAggregator _eventAggreagator;

        private bool _isWorking;

        #endregion

        #region Methods
        /// <summary>
        /// Инстанциирование ProgramViewControl
        /// </summary>
        /// <returns></returns>
        public static ProgramViewControl GetInstance()
        {
            lock ( _synchroot)
            {
                if ( _programControll == null )
                {
                    _programControll = new ProgramViewControl();                
                }

                return _programControll;
            }
        }
        /// <summary>
        /// Жизненный цикл ProgramViewControl
        /// </summary>
        public void Run() 
        {
            while (_isWorking)
            {
                Console.WriteLine($"введите help для отображения списка команд");

                string command = Console.ReadLine();

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine("Некорректный ввод.");
                }
                else if (command == "exit")
                {
                    _isWorking = false;

                    Console.WriteLine("Программа завершена.");
                }
                else
                {
                    switch (command)
                    {
                        case "help":
                            _eventAggreagator.SendCommandToModel(new HelpCommand(command));
                            break;
                        case ProgramModel.getContractsAmount:
                            Console.WriteLine("Сумма договоров за текущий год: ");

                            _eventAggreagator.SendCommandToModel(new GetTotalContractAmount(command));
                             break;

                        case ProgramModel.getContractsByCounterparty:
                            Console.WriteLine("Сумма заключенных договоров по контрагентам: ");

                            _eventAggreagator.SendCommandToModel(new GetContractsByCounterparty(command));
                            break;

                        case ProgramModel.getEmailsByContract:
                            Console.WriteLine("Список e-mail: ");

                            _eventAggreagator.SendCommandToModel(new GetEmailsByContract(command));
                            break;

                        case ProgramModel.getUpdateContractsStatusForElderly:
                            Console.WriteLine("Статус изменён: ");

                            _eventAggreagator.SendCommandToModel(new GetUpdateContractsStatusForElderly(command));
                            break;

                        case ProgramModel.getPersonsInMoscow:
                            Console.WriteLine("Отчёт по Москвичам: ");

                            _eventAggreagator.SendCommandToModel(new GetPersonsInMoscow(command));
                            break;

                    }
                }
            }

        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Вывод результата команды в консоль
        /// </summary>
        /// <param name="command">Команда</param>
        private void ResponseForModel (ICommand command)
        {
            Console.WriteLine(command.Response);
        }

        #endregion

    }
}
