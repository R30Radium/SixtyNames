using SixtyNames.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames
{
    internal sealed class ProgramViewControl
    {
        #region Constructors

        private ProgramViewControl()
        {
            _eventAggreagator = EventAggregator.GetInstance();
            _eventAggreagator.OnCommandToViewControl += ResponseForModel;

            _isWorking = true;
        }

        #endregion

        #region Fields

        private static ProgramViewControl _programControll;
        private static object _synchroot = new object();
        private readonly EventAggregator _eventAggreagator;

        private bool _isWorking;

        #endregion

        #region Methods

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
                }
                else
                {
                    switch (command)
                    {
                        case "help":
                            _eventAggreagator.SendCommandToModel(new HelpCommand(command));
                            break;
                        case "1":
                            _eventAggreagator.SendCommandToModel(new GetTotalContractAmount(command));
                             break;


                        //case getContractsAmount:
                        //    Console.WriteLine("Сумма договоров за текущий год: ");
                        //    break;    
                        
                        //case contractsByCounterparty:
                        //    Console.WriteLine("Сумма заключенных договоров: ");
                        //    break;
                        
                        //case getEmailsByContract:
                        //    Console.WriteLine("Список e-mail: ");
                        //    break;

                        //case updateContractsStatusForElderly:
                        //    Console.WriteLine("Статус изменён: ");
                        //    break;

                        //case getPersonsInMoscow:
                        //    Console.WriteLine("Отчёт по Москвичам: ");
                        //    break;

                        //case exit:
                        //     break;

                        //default:
                        
                        

                    }
                }
            }

        }
        #endregion

        #region Callbacks

        private void ResponseForModel (ICommand command)
        {
            Console.WriteLine(command.Response);
        }

        #endregion

    }
}
