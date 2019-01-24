using System;

namespace Lazurite.MainDomain
{
    public class ScenarioExecutionException: Exception
    {
        public ScenarioExecutionException(ScenarioExecutionError error, string tag = "")
        {
            ErrorType = error;
            Tag = tag;
        }
        
        public string Tag { get; }

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
                    case ScenarioExecutionError.AccessDenied:
                        return "Доступ запрещен.";
                    case ScenarioExecutionError.NotAvailable:
                        return "Сценарий недоступен";
                    case ScenarioExecutionError.InvalidValue:
                        return string.Format("Значение [{0}] не может быть принято.", Tag);
                }
                throw new InvalidOperationException();
            }
        }
    }
}
