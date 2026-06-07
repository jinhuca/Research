using CrystalMonitor.Hardware;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace HardwareService.Tests;

// ── Tests ─────────────────────────────────────────────────────────────────────

public class HardwareObservableTests {
  // ── Snapshot metadata ─────────────────────────────────────────────────────

  [Fact]
  public async Task Snapshot_Timestamp_IsSetAtTimeOfPoll() {
    var before = DateTimeOffset.Now;
    var snapshot = await OneSnapshot();
    var after = DateTimeOffset.Now;

    Assert.InRange(snapshot.Timestamp, before, after);
  }

  [Fact]
  public async Task Snapshot_EmptyComputer_HasNoReadings() {
    var snapshot = await OneSnapshot(); // no hardware added
    Assert.Empty(snapshot.Readings);
  }

  // ── Reading field mapping ─────────────────────────────────────────────────

  [Fact]
  public async Task Reading_MapsHardwareName() {
    var snapshot = await OneSnapshot(hwName: "Intel Core i9-14900K",
        sensors: new[] { Sensor() });

    Assert.All(snapshot.Readings, r => Assert.Equal("Intel Core i9-14900K", r.HardwareName));
  }

  [Fact]
  public async Task Reading_MapsHardwareType() {
    var snapshot = await OneSnapshot(hwType: HardwareType.GpuNvidia,
        sensors: new[] { Sensor() });

    Assert.All(snapshot.Readings, r => Assert.Equal(HardwareType.GpuNvidia, r.HardwareType));
  }

  [Fact]
  public async Task Reading_MapsSensorName() {
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { Name = "CPU Package" } });

    Assert.Equal("CPU Package", snapshot.Readings[0].SensorName);
  }

  [Fact]
  public async Task Reading_MapsSensorType() {
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { SensorType = SensorType.Power } });

    Assert.Equal(SensorType.Power, snapshot.Readings[0].SensorType);
  }

  [Fact]
  public async Task Reading_MapsSensorValue() {
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { Value = 73.4f } });

    Assert.Equal(73.4f, snapshot.Readings[0].Value);
  }

  [Fact]
  public async Task Reading_NullSensorValue_IsPreserved() {
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { Value = null } });

    Assert.Null(snapshot.Readings[0].Value);
  }

  // ── Sensor count ──────────────────────────────────────────────────────────

  [Fact]
  public async Task Snapshot_ReadingCount_MatchesSensorCount() {
    var snapshot = await OneSnapshot(sensors: new[] { Sensor(), Sensor(), Sensor() });
    Assert.Equal(3, snapshot.Readings.Count);
  }

  // ── Unit mapping — covers every branch of the switch ─────────────────────

  [Theory]
  [InlineData(SensorType.Temperature, "°C")]
  [InlineData(SensorType.Load, "%")]
  [InlineData(SensorType.Clock, "MHz")]
  [InlineData(SensorType.Power, "W")]
  [InlineData(SensorType.Voltage, "V")]
  [InlineData(SensorType.Current, "A")]
  [InlineData(SensorType.Fan, "RPM")]
  [InlineData(SensorType.Flow, "L/h")]
  [InlineData(SensorType.Control, "%")]
  [InlineData(SensorType.Level, "%")]
  [InlineData(SensorType.Data, "GB")]
  [InlineData(SensorType.SmallData, "MB")]
  [InlineData(SensorType.Throughput, "B/s")]
  [InlineData(SensorType.TimeSpan, "s")]
  [InlineData(SensorType.Energy, "mWh")]
  public async Task Unit_KnownSensorType_ReturnsCorrectUnit(SensorType type, string expected) {
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { SensorType = type } });

    Assert.Equal(expected, snapshot.Readings[0].Unit);
  }

  [Fact]
  public async Task Unit_UnknownSensorType_ReturnsNull() {
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { SensorType = (SensorType)9999 } });

    Assert.Null(snapshot.Readings[0].Unit);
  }

  // ── Sub-hardware recursion ────────────────────────────────────────────────

  [Fact]
  public async Task SubHardware_OneLevelDeep_SensorsAreCollected() {
    var child = new FakeHardware {
      Name = "SuperIO",
      Sensors = new ISensor[] { new FakeSensor { Name = "Fan #1" } },
    };
    var parent = new FakeHardware {
      Name = "ASUS ROG",
      Sensors = Array.Empty<ISensor>(),
      SubHardware = new IHardware[] { child },
    };

    var snapshot = await Poll(ComputerWith(parent));

    Assert.Single(snapshot.Readings);
    Assert.Equal("Fan #1", snapshot.Readings[0].SensorName);
  }

  [Fact]
  public async Task SubHardware_TwoLevelsDeep_SensorsAreCollected() {
    var grandchild = new FakeHardware { Sensors = new ISensor[] { new FakeSensor { Name = "Deep Sensor" } } };
    var child = new FakeHardware { Sensors = Array.Empty<ISensor>(), SubHardware = new IHardware[] { grandchild } };
    var root = new FakeHardware { Sensors = Array.Empty<ISensor>(), SubHardware = new IHardware[] { child } };

    var snapshot = await Poll(ComputerWith(root));

    Assert.Single(snapshot.Readings);
    Assert.Equal("Deep Sensor", snapshot.Readings[0].SensorName);
  }

  [Fact]
  public async Task SubHardware_ParentAndChildSensors_AreBothCollected() {
    var child = new FakeHardware { Sensors = new ISensor[] { new FakeSensor { Name = "Child Sensor" } } };
    var parent = new FakeHardware {
      Sensors = new ISensor[] { new FakeSensor { Name = "Parent Sensor" } },
      SubHardware = new IHardware[] { child },
    };

    var snapshot = await Poll(ComputerWith(parent));

    Assert.Equal(2, snapshot.Readings.Count);
    Assert.Contains(snapshot.Readings, r => r.SensorName == "Parent Sensor");
    Assert.Contains(snapshot.Readings, r => r.SensorName == "Child Sensor");
  }

  // ── Multiple top-level hardware nodes ─────────────────────────────────────

  [Fact]
  public async Task MultipleHardwareNodes_AllReadingsPresent() {
    var computer = new FakeComputer();
    computer.FakeHardware.Add(new FakeHardware {
      Name = "CPU",
      Sensors = new ISensor[] { new FakeSensor { Name = "Temp" } }
    });
    computer.FakeHardware.Add(new FakeHardware {
      Name = "GPU",
      HardwareType = HardwareType.GpuNvidia,
      Sensors = new ISensor[] { new FakeSensor { Name = "Core Load", SensorType = SensorType.Load } }
    });

    var snapshot = await Poll(computer);

    Assert.Equal(2, snapshot.Readings.Count);
    Assert.Contains(snapshot.Readings, r => r.HardwareName == "CPU");
    Assert.Contains(snapshot.Readings, r => r.HardwareName == "GPU");
  }

  // ── Stream behaviour ──────────────────────────────────────────────────────

  [Fact]
  public async Task ReadingStream_EmitsOneFlatReadingPerSensor() {
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor(), Sensor(), Sensor() } });

    var readings = await TestableHardwareObservable
        .ReadingStream(() => computer)
        .Take(3)
        .ToList();

    Assert.Equal(3, readings.Count);
  }

  // ── FilterBy ──────────────────────────────────────────────────────────────

  [Fact]
  public async Task FilterBy_SensorType_PassesMatchingAndDropsRest() {
    var computer = ComputerWith(new FakeHardware {
      Sensors = new ISensor[]
        {
                new FakeSensor { SensorType = SensorType.Temperature },
                new FakeSensor { SensorType = SensorType.Load },
                new FakeSensor { SensorType = SensorType.Clock },
        }
    });

    var results = await TestableHardwareObservable
        .ReadingStream(() => computer)
        .FilterBy(SensorType.Temperature)
        .Take(1)
        .ToList();

    Assert.Single(results);
    Assert.Equal(SensorType.Temperature, results[0].SensorType);
  }

  [Fact]
  public async Task FilterBy_HardwareType_PassesMatchingAndDropsRest() {
    var computer = new FakeComputer();
    computer.FakeHardware.Add(new FakeHardware { Name = "CPU", HardwareType = HardwareType.Cpu, Sensors = new ISensor[] { Sensor() } });
    computer.FakeHardware.Add(new FakeHardware { Name = "GPU", HardwareType = HardwareType.GpuNvidia, Sensors = new ISensor[] { Sensor() } });

    var results = await TestableHardwareObservable
        .ReadingStream(() => computer)
        .FilterBy(HardwareType.GpuNvidia)
        .Take(1)
        .ToList();

    Assert.Single(results);
    Assert.Equal(HardwareType.GpuNvidia, results[0].HardwareType);
  }

  [Fact]
  public async Task FilterBy_SensorType_NoMatch_EmitsNothing() {
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { new FakeSensor { SensorType = SensorType.Temperature } } });

    var results = await TestableHardwareObservable
        .ReadingStream(() => computer)
        .FilterBy(SensorType.Fan)   // nothing matches
        .Timeout(TimeSpan.FromMilliseconds(300))
        .Catch<SensorReading, TimeoutException>(_ => Observable.Empty<SensorReading>())
        .ToList();

    Assert.Empty(results);
  }

  // ── Take(1) / QueryOnce equivalent ────────────────────────────────────────

  [Fact]
  public async Task Take1_CompletesAfterExactlyOneSnapshot() {
    var emitCount = 0;
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });

    await TestableHardwareObservable
        .PollWith(() => computer)
        .Do(_ => emitCount++)
        .Take(1)
        .ToTask();

    Assert.Equal(1, emitCount);
  }

  // ── Teardown ──────────────────────────────────────────────────────────────

  [Fact]
  public void Dispose_BeforeFirstEmit_DoesNotThrow() {
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });

    var sub = TestableHardwareObservable
        .PollWith(() => computer, TimeSpan.FromHours(1))
        .Subscribe(_ => { });

    Assert.Null(Record.Exception(() => sub.Dispose()));
  }

  [Fact]
  public void Dispose_AfterFirstEmit_DoesNotThrow() {
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });
    using var ready = new System.Threading.ManualResetEventSlim(false);

    var sub = TestableHardwareObservable
        .PollWith(() => computer, TimeSpan.FromMilliseconds(50))
        .Subscribe(_ => ready.Set());

    ready.Wait(TimeSpan.FromSeconds(2));

    Assert.Null(Record.Exception(() => sub.Dispose()));
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  private static FakeSensor Sensor(
      string name = "Sensor",
      SensorType type = SensorType.Temperature,
      float value = 0f)
      => new() { Name = name, SensorType = type, Value = value };

  private static FakeComputer ComputerWith(FakeHardware hw) {
    var c = new FakeComputer();
    c.FakeHardware.Add(hw);
    return c;
  }

  /// <summary>One snapshot with optional hardware populated.</summary>
  private static Task<HardwareSnapshot> OneSnapshot(
      string hwName = "Fake CPU",
      HardwareType hwType = HardwareType.Cpu,
      FakeSensor[]? sensors = null) {
    var computer = new FakeComputer();

    if (sensors is { Length: > 0 })
      computer.FakeHardware.Add(new FakeHardware {
        Name = hwName,
        HardwareType = hwType,
        Sensors = sensors,
      });

    return Poll(computer);
  }

  private static Task<HardwareSnapshot> Poll(FakeComputer computer)
      => TestableHardwareObservable
          .PollWith(() => computer)
          .Take(1)
          .FirstAsync()
          .ToTask();
}