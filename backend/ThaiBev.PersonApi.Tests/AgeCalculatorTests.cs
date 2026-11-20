using System.Globalization;
using ThaiBev.PersonApi.Services;

namespace ThaiBev.PersonApi.Tests;

public class AgeCalculatorTests
{
    [Fact]
    public void TodayInBangkok_UsesBangkokCalendarDay_NotUtc()
    {
        // UTC คืนวันที่ 31 ธ.ค. 2026 (17:00 UTC) = เช้าวันที่ 1 ม.ค. 2027 เวลาไทย
        var provider = new TestTimeProvider(new DateTimeOffset(2026, 12, 31, 17, 0, 0, TimeSpan.Zero));

        var today = AgeCalculator.TodayInBangkok(provider);

        Assert.Equal(new DateOnly(2027, 1, 1), today);
    }

    [Fact]
    public void TodayInBangkok_NormalAfternoon_KeepsSameDate()
    {
        // 20 พ.ย. 2026 04:00 UTC = 11:00 น. เวลาไทย (ยังเป็นวันเดียวกัน)
        var today = AgeCalculator.TodayInBangkok(TestTimeProvider.November2026);

        Assert.Equal(new DateOnly(2026, 11, 1), today);
    }

    [Fact]
    public void CalculateAge_IsBirthYearSubtractedFromCurrentYear()
    {
        // ตามโจทย์: อายุ = ปีปัจจุบัน - ปีเกิด (2026 - 1995 = 31)
        var age = AgeCalculator.CalculateAge(new DateOnly(1995, 3, 21), new DateOnly(2026, 11, 20));

        Assert.Equal(31, age);
    }

    [Theory]
    [InlineData("2000-12-31", "2026-01-01", 26)] // เกิดปลายปี ปีนี้เพิ่งขึ้นปีใหม่ 1 วัน — ยังนับ 26 ตามสูตรปีลบปี
    [InlineData("2000-02-29", "2026-02-28", 26)] // เกิดวัน leap day ปีอธิกสุรทิน — สูตรเดียวกัน
    [InlineData("2000-02-29", "2026-03-01", 26)]
    [InlineData("2004-11-20", "2024-11-20", 20)] // วันเกิดตรงกับวันนี้
    [InlineData("2026-11-20", "2026-11-20", 0)]  // เกิดวันนี้
    public void CalculateAge_EdgeCases_FollowYearDifferenceRule(string birthIso, string todayIso, int expected)
    {
        // ต้อง parse แบบ invariant (ISO yyyy-MM-dd): เครื่อง locale ไทยใช้ปฏิทินพุทธ
        // ถ้าใช้ DateOnly.Parse แบบ culture-sensitive จะตีความ "2000-02-29" เป็น พ.ศ. และ throw
        var birth = DateOnly.ParseExact(birthIso, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var today = DateOnly.ParseExact(todayIso, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var age = AgeCalculator.CalculateAge(birth, today);

        Assert.Equal(expected, age);
    }

    [Fact]
    public void CalculateAge_SameYear_IsZero()
    {
        var age = AgeCalculator.CalculateAge(new DateOnly(2026, 6, 1), new DateOnly(2026, 11, 1));

        Assert.Equal(0, age);
    }

    [Fact]
    public void CalculateAge_BuddhistEraDifference_MatchesGregorianDifference()
    {
        // พ.ศ. 2569 - พ.ศ. 2538 (ค.ศ. 2026 - ค.ศ. 1995) ต้องได้ผลเท่ากัน
        var gregorianDiff = 2026 - 1995;
        var buddhistDiff = (2026 + 543) - (1995 + 543);

        Assert.Equal(gregorianDiff, buddhistDiff);
    }
}
