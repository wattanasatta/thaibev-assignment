using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ThaiBev.PersonApi.Entities;

namespace ThaiBev.PersonApi.Data;

/// <summary>
/// เตรียมฐานข้อมูลตอน startup:
/// - ใช้ EF migration เป็นเส้นทางหลัก
/// - ถ้าตาราง Person มีอยู่แล้ว (ผู้ใช้สร้างจาก SQL script ใน /database เอง) ให้ข้าม migration แทนการพัง
/// - seed ข้อมูลตัวอย่างเมื่อตารางว่าง
/// </summary>
public static class DbInitializer
{
    public static async Task ApplyAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        try
        {
            if (!await DatabaseExistsAsync(db))
            {
                // Migrate() จะสร้าง database + ตาราง + บันทึก migration history ให้เอง
                await db.Database.MigrateAsync();
                logger.LogInformation("สร้างฐานข้อมูลใหม่และ apply migrations ทั้งหมดสำเร็จ");
            }
            else
            {
                var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
                if (pending.Count > 0)
                {
                    if (await PersonTableExistsAsync(db))
                    {
                        logger.LogWarning(
                            "ตาราง Person มีอยู่แล้ว (น่าจะสร้างจาก SQL script) จึงข้าม migration {Count} รายการ: {Migrations}",
                            pending.Count, string.Join(", ", pending));
                    }
                    else
                    {
                        await db.Database.MigrateAsync();
                        logger.LogInformation("Applied {Count} migration(s): {Migrations}",
                            pending.Count, string.Join(", ", pending));
                    }
                }
            }

            await SeedAsync(db, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "เตรียมฐานข้อมูลไม่สำเร็จ — API ยังรันได้ แต่จะ error จนกว่าฐานข้อมูลจะพร้อม");
        }
    }

    private static async Task<bool> DatabaseExistsAsync(AppDbContext db)
    {
        var csb = new SqlConnectionStringBuilder(db.Database.GetConnectionString());
        var databaseName = csb.InitialCatalog;
        csb.InitialCatalog = "master"; // connect ไป master แทน เพราะ DB ปลายทางอาจยังไม่มีตัวตน

        await using var connection = new SqlConnection(csb.ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@dbName";
        parameter.Value = databaseName;
        command.Parameters.Add(parameter);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private static async Task<bool> PersonTableExistsAsync(AppDbContext db)
    {
        var connection = db.Database.GetDbConnection();
        await connection.OpenAsync();
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'Person' AND schema_id = SCHEMA_ID('dbo')";
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static async Task SeedAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Persons.AnyAsync())
        {
            return;
        }

        db.Persons.AddRange(SamplePeople);
        await db.SaveChangesAsync();
        logger.LogInformation("Seed ข้อมูลตัวอย่าง {Count} รายการ", SamplePeople.Count);
    }

    /// <summary>ข้อมูลตัวอย่าง 8 รายการ (เหมือนกับ 02_seed.sql) — ครอบคลุม leap day (29 ก.พ.) และอายุหลากหลายช่วง</summary>
    public static readonly List<Person> SamplePeople =
    [
        new() { FirstName = "สมชาย", LastName = "ใจดี", BirthDate = new DateOnly(1995, 3, 21), Address = "123 ถนนนิมมานเหมินท์ ต.สุเทพ อ.เมือง จ.เชียงใหม่" },
        new() { FirstName = "สมหญิง", LastName = "รักไทย", BirthDate = new DateOnly(1988, 7, 15), Address = "45 ถนนสุขุมวิท แขวงคลองตันเหนือ เขตวัฒนา กรุงเทพฯ" },
        new() { FirstName = "ปิยะพงษ์", LastName = "มั่นคง", BirthDate = new DateOnly(2001, 1, 1), Address = "8/9 หมู่ 2 ต.บางปู อ.เมืองสมุทรปราการ จ.สมุทรปราการ" },
        new() { FirstName = "มาลี", LastName = "บุญมาก", BirthDate = new DateOnly(1976, 12, 25), Address = "22 ซอยลาดพร้าว 71 แขวงวังทองหลาง เขตวังทองหลาง กรุงเทพฯ" },
        new() { FirstName = "ธนกฤต", LastName = "แสงทอง", BirthDate = new DateOnly(2010, 6, 6), Address = "99 ถนนเจริญเมือง ต.วัดเกต อ.เมือง จ.เชียงใหม่" },
        new() { FirstName = "อรุณี", LastName = "เจริญสุข", BirthDate = new DateOnly(1969, 5, 12), Address = "1/1 ถนนกาญจนวิเชษฐ์ ต.ในเมือง อ.เมือง จ.ขอนแก่น" },
        new() { FirstName = "กิตติ", LastName = "ศรีสุข", BirthDate = new DateOnly(2000, 2, 29), Address = "777 ถนนเพชรบุรี แขวงมักกะสัน เขตราชเทวี กรุงเทพฯ" },
        new() { FirstName = "วิภา", LastName = "ใจงาม", BirthDate = new DateOnly(2004, 11, 20), Address = "10 ถนนยุคล แขวงบางรัก เขตบางรัก กรุงเทพฯ" },
    ];
}
