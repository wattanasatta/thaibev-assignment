namespace ThaiBev.PersonApi.Services;

/// <summary>
/// คำนวณอายุตามโจทย์: อายุ = ปีปัจจุบัน - ปีเกิด (ไม่ใช่อายุเต็มปีตามวัน/เดือน)
/// ปี พ.ศ. ลบกับปี พ.ศ. ได้ผลเท่ากับ ค.ศ. ลบ ค.ศ. จึงคำนวณด้วย ค.ศ. ได้เลย
/// </summary>
public static class AgeCalculator
{
    private static readonly TimeZoneInfo BangkokTimeZone = ResolveBangkokTimeZone();

    /// <summary>"วันนี้" ตามเวลาประเทศไทย (กันกรณี server เป็น UTC คืนวันที่ 31 ธ.ค.)</summary>
    public static DateOnly TodayInBangkok(TimeProvider timeProvider) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), BangkokTimeZone).Date);

    public static int CalculateAge(DateOnly birthDate, DateOnly today) =>
        today.Year - birthDate.Year;

    private static TimeZoneInfo ResolveBangkokTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
        }
        catch (Exception)
        {
            // Windows เก่า ๆ ที่ไม่รู้จัก IANA id ให้ใช้ Windows id ของเวลาไทย (UTC+7, ไม่มี DST) แทน
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ไม่พบ timezone ของประเทศไทย (Asia/Bangkok) บนเครื่องนี้", ex);
            }
        }
    }
}
