using SixtyNames.Commands;
using System;

namespace SixtyNames
{
    /// <summary>
    /// Класс - событийный слой
    /// </summary>
    internal sealed class EventAggregator
    {
        public delegate void EventArgs();

        #region Events
        /// <summary>
        /// События
        /// </summary>

        public event Action<ICommand> OnCommandToModel;
        public event Action<ICommand> OnCommandToViewControl;

        #endregion

        #region Fields

        private static EventAggregator _eventAggregator;
        private static object _syncRoot = new object();

        #endregion

        #region Methods
        /// <summary>
        /// Инстанциирование
        /// </summary>
        /// <returns></returns>
        public static EventAggregator GetInstance()
        {
            lock (_syncRoot)
            {
                if (_eventAggregator == null)
                {
                    _eventAggregator = new EventAggregator();
                }
                return _eventAggregator;
            }
        }
        /// <summary>
        /// Посылка команд в модель
        /// </summary>
        /// <param name="command"></param>
        public void SendCommandToModel(ICommand command)
        {
            OnCommandToModel?.Invoke(command);    
        }
        /// <summary>
        /// Посылка команд в представление 
        /// </summary>
        /// <param name="command"></param>
        public void SendCommandToViewControl(ICommand command)
        {
            OnCommandToViewControl?.Invoke(command);
        }

        #endregion

    }
}
