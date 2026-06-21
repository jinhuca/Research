using CrystalMonitor.Hardware;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace HardwareService.Tests;

// ── Testable seam ─────────────────────────────────────────────────────────────

/// <summary>
/// Thin adapter over <see cref="HardwareObservable"/>'s internal
/// dependency-injected seams (<c>PollAllCore</c> / <c>QueryOnceCore</c>),
/// exposed to this assembly via <c>InternalsVisibleTo</c>. This calls the
/// REAL production polling/snapshot/error-handling logic — it does not
/// reimplement it — so these tests contribute real coverage to
/// HardwareService.dll and can't drift from production behavior.
/// </summary>
public static class TestableHardwareObservable {
  public static IObservable<HardwareSnapshot> PollWith(
      Func<IComputer> factory,
      TimeSpan? interval = null,
      IScheduler? scheduler = null)
      => HardwareObservable.PollAllCore(
          () => (factory(), NoOp),
          interval,
          scheduler);

  public static IObservable<HardwareSnapshot> QueryOnceWith(
      Func<IComputer> factory,
      IScheduler? scheduler = null)
      => HardwareObservable.QueryOnceCore(
          () => (factory(), NoOp),
          scheduler);

  public static IObservable<SensorReading> ReadingStream(
      Func<IComputer> factory, TimeSpan? interval = null, IScheduler? scheduler = null)
      => PollWith(factory, interval, scheduler).SelectMany(s => s.Readings);

  /// <summary>Fakes have no real teardown — they're not opened/closed like a real Computer.</summary>
  private static readonly Action NoOp = () => { };
}
