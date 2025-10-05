using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Communication;
using Module.Console.Models;
using Console;
using DataAccessLayer;
using Module.Console.Helpers;
using Moq;

namespace FieldServiceTool.ConsoleUnitTest
{
    [TestClass]
    public class MachineTests
    {
        private MachineModel _machineModel;
        private Machine _machine;

        [TestInitialize]
        public void Initialize()
        {
            _machine = new Machine(new CanBusCommunication(), new GeneralPurposeInputOutput());
            var consoleMonitor = new ConsoleMonitor(_machine);
            var dataAccess = new Mock<ICacheableDataAccess>();
            var data = new Data(dataAccess.Object);

            _machineModel = new MachineModel(_machine, consoleMonitor, data);
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public void MachinePropertyChanged_Test()
        {
            string actual = null;
            double expectedValue = 10;
            _machineModel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };
            _machineModel.PatientIGain = 10;
            Assert.IsNotNull(actual);
            Assert.AreEqual("PatientIGain", actual);
            Assert.AreEqual(expectedValue, _machineModel.PatientIGain);
        }
    }
}
