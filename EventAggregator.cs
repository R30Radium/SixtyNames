using SixtyNames.Commands;
using System;

namespace SixtyNames
{
    internal sealed class EventAggregator
    {
        public delegate void EventArgs();

        #region Constructor

        public EventAggregator()
        {
            
        }

        #endregion

        #region Events


        public event Action<ICommand> OnCommandToModel;
        public event Action<ICommand> OnCommandToViewControl;



        #endregion

        #region Fields

        private static EventAggregator _eventAggregator;
        private static object _syncRoot = new object();

        #endregion

        #region Methods

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

        public void SendCommandToModel(ICommand command)
        {
            OnCommandToModel?.Invoke(command);    
        }
        
        public void SendCommandToViewControl(ICommand command)
        {
            OnCommandToViewControl?.Invoke(command);
        }

        #endregion

    }
}
