using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.IOC;

namespace Pyrite.Scenarios.ScenarioTypes
{
    public class RemoteScenario : ScenarioBase
    {
        private IClientFactory _clientFactory;
        private IServer _server;

        /// <summary>
        /// Target server ip or name
        /// </summary>
        public string AddressHost { get; set; }

        /// <summary>
        /// Target server port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Target server login
        /// </summary>
        public string UserLogin { get; set; }

        /// <summary>
        /// Target server password
        /// </summary>
        public string Password { get; set; }


        public override AbstractValueType ValueType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override string CalculateCurrentValue()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteInternal(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetAllUsedActionTypes()
        {
            throw new NotImplementedException();
        }

        public override string GetCurrentValue()
        {
            throw new NotImplementedException();
        }

        public override void SetCurrentValueInternal(string value)
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            _clientFactory = Singleton.Resolve<IClientFactory>();
            _server = _clientFactory.GetServer(AddressHost, Port, UserLogin, Password);
        }
    }
}
