using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Prism;
using SmartAblationSystem;
using SmartAblationSystem.ViewModels;
using SmartAblationSystem.Models;
using Console;
using Communication;
using RS232Communication;
using Unity;

namespace SmartAblationSystemUnitTests
{
  [TestClass]
  public class ResetDASBalloonUnitTests
  {
    CommonViewModel commonViewModelMock;
    CryoTherapyViewModel cryoTherapyViewModel;
    Machine machineMock;
    ChangeBalloonTypeFSM changeBalloonTypeFSMMock;
    InflateDeflateBalloonModel inflateDeflateBalloonModelMock;
    Data dataMock = new Data();
    Mock<ICanBusCommunication> canBusCommunicationMock = new Mock<ICanBusCommunication>();
    Mock<IGeneralPurposeInputOutput> gpIOMock = new Mock<IGeneralPurposeInputOutput>();
    Mock<ISerialPortManager> serialPortManagerMock = new Mock<ISerialPortManager>();
    Mock<IUnityContainer> unityContainerMock = new Mock<IUnityContainer>();

    [TestInitialize]
    public void Setup()
    {
      machineMock = new Machine(canBusCommunicationMock.Object, gpIOMock.Object);
      commonViewModelMock = new CommonViewModel(machineMock, serialPortManagerMock.Object);
      inflateDeflateBalloonModelMock = new InflateDeflateBalloonModel(dataMock, machineMock);
      changeBalloonTypeFSMMock = new ChangeBalloonTypeFSM(inflateDeflateBalloonModelMock);

      cryoTherapyViewModel = new CryoTherapyViewModel(unityContainerMock.Object);
    }
    /*
     * Code we are testing:
          PressureSetPoint = 2.5;
          CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled = false;
          DASBalloonEnabled = false;
    */
    
    [TestMethod]
    public void TestReset()
    {
      cryoTherapyViewModel.DASBalloonEnabled = true;
      cryoTherapyViewModel.PressureSetPoint = It.IsAny<double>();
      CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled = true;

      Assert.IsTrue(cryoTherapyViewModel.DASBalloonEnabled);
      Assert.IsTrue(CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled);

      cryoTherapyViewModel.ResetDASBalloonSize();

      // Verify
      Assert.IsFalse(cryoTherapyViewModel.DASBalloonEnabled);
      Assert.IsTrue(cryoTherapyViewModel.PressureSetPoint == 2.5);
      Assert.IsFalse(CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled);
    }
  }
}
