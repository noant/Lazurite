using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class ScenarioExecutionException: Exception
    {
        public ScenarioExecutionException(ScenarioExecutionError error)
        {
            ErrorType = error;
        }
        
        public ScenarioExecutionError ErrorType { get; }

        public override string Message {
            get
            {
                switch (ErrorType)
                {
                    case ScenarioExecutionError.CircularReference:
                        return "Найдена цикличная ссылка на сценарий! Выполение будет прекращено.";
                    case ScenarioExecutionError.StackOverflow:
                        return "Стек переполнен! Выполение будет прекращено.";
                }
                throw new InvalidOperationException();
            }
        }
    }
}
