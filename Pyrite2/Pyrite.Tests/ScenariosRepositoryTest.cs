using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.Data;
using Pyrite.Exceptions;
using Pyrite.IOC;
using Pyrite.Scenarios;
using Pyrite.Scenarios.ScenarioTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Tests
{
    [TestClass]
    public class ScenariosRepositoryTest
    {
        [TestMethod]
        public void CreateAndSaveScenario()
        {
            Singleton.Add(new ExceptionsHandler());
            Singleton.Add(new FileSavior());
            var repository = new ScenariosRepository();
            Singleton.Add(repository);
            var testScen = new SingleActionScenario();
            testScen.Name = "testScen";
            testScen.Category = "category1";
            testScen.TargetAction = new ComplexCheckerActionTest.TestAction() {
                Value = "20"
            };
            repository.AddScenario(testScen);
        }

        [TestMethod]
        public void LoadScenario()
        {
            Singleton.Add(new ExceptionsHandler());
            Singleton.Add(new FileSavior());
            var repository = new ScenariosRepository();
            Singleton.Add(repository);
            var testScen = repository.Scenarios.Single(x => x.Name.Equals("testScen"));
            testScen.Execute("30", new System.Threading.CancellationToken());
        }
    }
}
