namespace ThaiBev.PersonApi.Tests;

/// <summary>TimeProvider หลอกสำหรับเทสต์ — กำหนดเวลาปัจจุบันแบบ fix เพื่อทดสอบการคำนวณอายุ</summary>
public sealed class TestTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;

    /// <summary>1 พ.ย. 2026 เวลา 04:00 UTC = 11:00 น. เวลาไทย (เกินเที่ยงวัน จึงไม่เจอ edge case เปลี่ยนวัน)</summary>
    public static TestTimeProvider November2026 { get; } =
        new(new DateTimeOffset(2026, 11, 1, 4, 0, 0, TimeSpan.Zero));
}
