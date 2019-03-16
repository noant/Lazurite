using Lazurite.Shared;

namespace MediaHost.Bases
{
    public abstract class MediaCommandBase
    {
        public string Name { get; protected set; }

        public abstract void Execute();

        public abstract void Execute(string param);

        /// <summary>
        /// Если true, то можно выполнять Execute(string param)
        /// </summary>
        public bool AllowParam { get; protected set; }

        /// <summary>
        /// Активация команды тредует активации всей панели
        /// </summary>
        public abstract bool ActivateWithPanelBase(string param = null);

        public event EventsHandler<string> Changed;

        public void RaiseValueChanged(string val) => Changed?.Invoke(this, new EventsArgs<string>(val));

        public void RaiseEvents() => RaiseValueChanged(Current);

        public abstract string Current { get; }

        public void TransferEvents(MediaCommandBase command)
        {
            command.Changed += Changed;
            Changed = null;
        }
    }
}