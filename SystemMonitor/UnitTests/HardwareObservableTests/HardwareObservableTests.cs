using CrystalMonitor.Hardware;
using Microsoft.Reactive.Testing;
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

  [Fact]
  public async Task Reading_MapsSensorMinAndMax() {
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { Min = 10.5f, Max = 99.9f } });

    Assert.Equal(10.5f, snapshot.Readings[0].Min);
    Assert.Equal(99.9f, snapshot.Readings[0].Max);
  }

  [Fact]
  public async Task Reading_NullMinAndMax_ArePreserved() {
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { Min = null, Max = null } });

    Assert.Null(snapshot.Readings[0].Min);
    Assert.Null(snapshot.Readings[0].Max);
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
  [InlineData(SensorType.Energy, "mWh")]
  [InlineData(SensorType.Factor, "")]
  [InlineData(SensorType.Frequency, "Hz")]
  [InlineData(SensorType.Humidity, "%")]
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

  [Fact]
  public async Task Unit_TimeSpanSensorType_ReturnsNull() {
    // TimeSpan is a real, defined SensorType — unlike the cast-to-9999 case above —
    // but SensorReadingExtensions.UnitFor has no switch arm for it, so it falls
    // through to null along with genuinely-unknown values. This pins that current
    // behavior; if a unit for TimeSpan is intentionally added later, update this test.
    var snapshot = await OneSnapshot(sensors: new[]
        { new FakeSensor { SensorType = SensorType.TimeSpan } });

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

  [Fact]
  public async Task FilterBy_ChainedHardwareAndSensorType_PassesOnlyIntersection() {
    var computer = new FakeComputer();
    computer.FakeHardware.Add(new FakeHardware {
      Name = "CPU",
      HardwareType = HardwareType.Cpu,
      Sensors = new ISensor[] { new FakeSensor { SensorType = SensorType.Load } },
    });
    computer.FakeHardware.Add(new FakeHardware {
      Name = "GPU",
      HardwareType = HardwareType.GpuNvidia,
      Sensors = new ISensor[]
      {
        new FakeSensor { SensorType = SensorType.Load },
        new FakeSensor { SensorType = SensorType.Temperature },
      },
    });

    var results = await TestableHardwareObservable
        .ReadingStream(() => computer)
        .FilterBy(HardwareType.GpuNvidia)
        .FilterBy(SensorType.Load)
        .Take(1)
        .ToList();

    Assert.Single(results);
    Assert.Equal(HardwareType.GpuNvidia, results[0].HardwareType);
    Assert.Equal(SensorType.Load, results[0].SensorType);
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

  // ── Periodic polling (virtual time) ───────────────────────────────────────
  // Uses TestScheduler instead of real wall-clock waits so timing is exact
  // and tests run instantly regardless of the configured interval.

  [Fact]
  public void PollWith_VirtualTime_EmitsImmediatelyOnSubscribe() {
    var scheduler = new TestScheduler();
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });
    var emitCount = 0;

    using var sub = TestableHardwareObservable
        .PollWith(() => computer, TimeSpan.FromSeconds(1), scheduler)
        .Subscribe(_ => emitCount++);

    // No time has been advanced yet — only the StartWith(-1) seed should have fired.
    Assert.Equal(1, emitCount);
  }

  [Fact]
  public void PollWith_VirtualTime_EmitsOnceEveryInterval() {
    var scheduler = new TestScheduler();
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });
    var emitCount = 0;

    using var sub = TestableHardwareObservable
        .PollWith(() => computer, TimeSpan.FromSeconds(1), scheduler)
        .Subscribe(_ => emitCount++);

    scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
    Assert.Equal(2, emitCount); // initial + 1 tick

    scheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks);
    Assert.Equal(5, emitCount); // + 3 more ticks
  }

  [Fact]
  public void PollWith_VirtualTime_DoesNotEmitBeforeIntervalElapses() {
    var scheduler = new TestScheduler();
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });
    var emitCount = 0;

    using var sub = TestableHardwareObservable
        .PollWith(() => computer, TimeSpan.FromSeconds(10), scheduler)
        .Subscribe(_ => emitCount++);

    scheduler.AdvanceBy(TimeSpan.FromSeconds(9).Ticks);
    Assert.Equal(1, emitCount); // still just the initial seed
  }

  [Fact]
  public void PollWith_Factory_IsInvokedOnceRegardlessOfTickCount() {
    var scheduler = new TestScheduler();
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });
    var factoryCalls = 0;
    IComputer Factory() { factoryCalls++; return computer; }

    using var sub = TestableHardwareObservable
        .PollWith(Factory, TimeSpan.FromSeconds(1), scheduler)
        .Subscribe(_ => { });

    scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

    Assert.Equal(1, factoryCalls);
  }

  [Fact]
  public void PollWith_TwoSubscriptions_EachInvokesFactorySeparately() {
    var scheduler = new TestScheduler();
    var factoryCalls = 0;
    IComputer Factory() {
      factoryCalls++;
      return ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });
    }
    var observable = TestableHardwareObservable.PollWith(Factory, TimeSpan.FromSeconds(1), scheduler);

    using var sub1 = observable.Subscribe(_ => { });
    using var sub2 = observable.Subscribe(_ => { });

    Assert.Equal(2, factoryCalls); // one factory() call per subscription, not per source
  }

  // ── Error handling (production swallows + logs, never crashes the stream) ──
  // PollAllCore wraps TakeSnapshot in try/catch and logs via Serilog rather than
  // propagating OnError — a single bad tick (e.g. a transient driver hiccup)
  // shouldn't kill the whole polling subscription.

  [Fact]
  public void Snapshot_ComputerAcceptThrows_SkipsTickWithoutTerminatingStream() {
    var scheduler = new TestScheduler();
    var computer = new FakeComputer { ThrowOnAccept = new InvalidOperationException("driver fault") };
    var emitCount = 0;
    var completed = false;
    Exception? observedError = null;

    using var sub = TestableHardwareObservable
        .PollWith(() => computer, TimeSpan.FromSeconds(1), scheduler)
        .Subscribe(_ => emitCount++, err => observedError = err, () => completed = true);

    scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

    Assert.Equal(0, emitCount);  // every tick threw, so nothing was ever emitted
    Assert.Null(observedError);  // ...but the error was caught and logged, not propagated
    Assert.False(completed);     // ...and the subscription is still alive, not terminated
  }

  [Fact]
  public void Snapshot_ComputerAcceptThrows_RecoversOnNextSuccessfulTick() {
    var scheduler = new TestScheduler();
    var computer = new FakeComputer {
      ThrowOnAccept = new InvalidOperationException("transient driver fault"),
    };
    var snapshots = new List<HardwareSnapshot>();

    using var sub = TestableHardwareObservable
        .PollWith(() => computer, TimeSpan.FromSeconds(1), scheduler)
        .Subscribe(snapshots.Add);

    scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);
    Assert.Empty(snapshots); // initial seed + first tick both failed

    computer.ThrowOnAccept = null; // driver "recovers"
    scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

    Assert.Single(snapshots); // the next tick succeeds and is emitted normally
  }

  [Fact]
  public async Task PollWith_FactoryThrows_PropagatesAsOnError() {
    // Contrast with the tests above: a failure constructing/opening the computer
    // itself (outside the per-tick try/catch) is NOT swallowed — Observable.Create
    // routes synchronous exceptions in the subscribe delegate to OnError as usual.
    IComputer Factory() => throw new InvalidOperationException("could not open device");

    var task = TestableHardwareObservable
        .PollWith(Factory)
        .Take(1)
        .ToTask();

    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => task);
    Assert.Equal("could not open device", ex.Message);
  }

  // ── QueryOnce ─────────────────────────────────────────────────────────────

  [Fact]
  public async Task QueryOnce_CompletesOnInitialTick_RegardlessOfInterval() {
    // QueryOnce hardcodes its underlying interval to 1s and ignores any caller
    // preference; this only "works" because Take(1) grabs the StartWith(-1) seed,
    // which fires synchronously on subscribe, before any interval tick is due.
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });

    var snapshot = await TestableHardwareObservable
        .QueryOnceWith(() => computer)
        .FirstAsync()
        .ToTask()
        .WaitAsync(TimeSpan.FromMilliseconds(500));

    Assert.Single(snapshot.Readings);
  }

  // ── Direct snapshot builder (no Rx involved) ────────────────────────────────
  // TakeSnapshot is internal now too, so the core mapping logic can be exercised
  // without going through the reactive pipeline at all.

  [Fact]
  public void TakeSnapshot_Direct_BuildsSnapshotFromComputer() {
    var computer = ComputerWith(new FakeHardware {
      Name = "Direct CPU",
      Sensors = new ISensor[] { new FakeSensor { Name = "Direct Sensor", Value = 55f } },
    });

    var snapshot = HardwareObservable.TakeSnapshot(computer);

    Assert.Single(snapshot.Readings);
    Assert.Equal("Direct CPU", snapshot.Readings[0].HardwareName);
    Assert.Equal("Direct Sensor", snapshot.Readings[0].SensorName);
    Assert.Equal(55f, snapshot.Readings[0].Value);
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

  [Fact]
  public void Dispose_VirtualTime_StopsFurtherEmissionsImmediately() {
    var scheduler = new TestScheduler();
    var computer = ComputerWith(new FakeHardware { Sensors = new ISensor[] { Sensor() } });
    var emitCount = 0;

    var sub = TestableHardwareObservable
        .PollWith(() => computer, TimeSpan.FromSeconds(1), scheduler)
        .Subscribe(_ => emitCount++);

    scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);
    var countAtDispose = emitCount;

    sub.Dispose();
    scheduler.AdvanceBy(TimeSpan.FromSeconds(10).Ticks);

    Assert.Equal(countAtDispose, emitCount); // nothing further fires post-dispose
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