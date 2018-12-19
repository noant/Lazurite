using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public abstract string Current { get; }
    }
}
