using Microsoft.EntityFrameworkCore;
using ThaiBev.PersonApi.Data;
using ThaiBev.PersonApi.Dtos;
using ThaiBev.PersonApi.Services;

namespace ThaiBev.PersonApi.Tests;

public class PersonServiceTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"persons-{Guid.NewGuid()}")
            .Options);

    private static PersonService CreateService(AppDbContext db) =>
        new(db, TestTimeProvider.November2026);

    private static CreatePersonRequest ValidRequest() => new()
    {
        FirstName = "  สมชาย  ",
        LastName = " ใจดี ",
        BirthDate = new DateOnly(1995, 3, 21),
        Address = "  123 ถนนนิมมานเหมินท์ เชียงใหม่  ",
    };

    [Fact]
    public async Task CreateAsync_TrimsInput_AndReturnsDetailWithComputedAge()
    {
        await using var db = CreateDb();
        var service = CreateService(db);

        var detail = await service.CreateAsync(ValidRequest());

        Assert.Equal("สมชาย", detail.FirstName);
        Assert.Equal("ใจดี", detail.LastName);
        Assert.Equal("สมชาย ใจดี", detail.FullName);
        Assert.Equal("123 ถนนนิมมานเหมินท์ เชียงใหม่", detail.Address);
        Assert.Equal(31, detail.Age); // 2026 - 1995 ตามสูตรโจทย์
        Assert.True(detail.Id > 0);

        // บันทึกลง DB แบบ trim แล้ว
        var saved = await db.Persons.SingleAsync(p => p.Id == detail.Id);
        Assert.Equal("สมชาย", saved.FirstName);
        Assert.Equal(new DateOnly(1995, 3, 21), saved.BirthDate);
        Assert.True(saved.CreatedAt > DateTime.MinValue); // CreatedAt ต้องถูก set
    }

    [Fact]
    public async Task CreateAsync_AppendsAtEnd_OrderedByIdAscending()
    {
        await using var db = CreateDb();
        var service = CreateService(db);
        await service.CreateAsync(ValidRequest()); // id 1
        await service.CreateAsync(ValidRequest()); // id 2

        var list = await service.GetAllAsync();

        Assert.Equal(2, list.Count);
        Assert.True(list[0].Id < list[1].Id); // ข้อมูลใหม่ (id มากกว่า) อยู่ท้าย
    }

    [Fact]
    public async Task CreateAsync_FutureBirthDate_ThrowsValidation()
    {
        await using var db = CreateDb();
        var service = CreateService(db);
        var request = ValidRequest();
        request.BirthDate = new DateOnly(2026, 11, 2); // วันพรุ่งนี้ (นาฬิกา fix ที่ 1 พ.ย. 2026)

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => service.CreateAsync(request));

        Assert.True(ex.Errors.ContainsKey("birthDate"));
    }

    [Fact]
    public async Task CreateAsync_BirthDateOlderThan150Years_ThrowsValidation()
    {
        await using var db = CreateDb();
        var service = CreateService(db);
        var request = ValidRequest();
        request.BirthDate = new DateOnly(1800, 1, 1); // เก่ากว่า 2026-150=1876

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => service.CreateAsync(request));

        Assert.True(ex.Errors.ContainsKey("birthDate"));
    }

    [Theory]
    [InlineData("firstName")]
    [InlineData("lastName")]
    [InlineData("address")]
    public async Task CreateAsync_WhitespaceOnlyField_ThrowsValidation(string field)
    {
        await using var db = CreateDb();
        var service = CreateService(db);
        var request = ValidRequest();
        if (field == "firstName") request.FirstName = "   ";
        if (field == "lastName") request.LastName = "\t";
        if (field == "address") request.Address = " \n ";

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => service.CreateAsync(request));

        Assert.True(ex.Errors.ContainsKey(field));
    }

    [Fact]
    public async Task CreateAsync_MissingBirthDate_ThrowsValidation()
    {
        await using var db = CreateDb();
        var service = CreateService(db);
        var request = ValidRequest();
        request.BirthDate = default; // JSON ไม่ส่ง birthDate → default 0001-01-01

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => service.CreateAsync(request));

        Assert.True(ex.Errors.ContainsKey("birthDate"));
    }

    [Fact]
    public async Task GetAllAsync_ComposesFullName_AndComputesAgeFromToday()
    {
        await using var db = CreateDb();
        var service = CreateService(db);
        await service.CreateAsync(ValidRequest());

        var list = await service.GetAllAsync();

        var item = Assert.Single(list);
        Assert.Equal("สมชาย ใจดี", item.FullName);
        Assert.Equal(31, item.Age);
        Assert.Equal(new DateOnly(1995, 3, 21), item.BirthDate);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDetail()
    {
        await using var db = CreateDb();
        var service = CreateService(db);
        var created = await service.CreateAsync(ValidRequest());

        var detail = await service.GetByIdAsync(created.Id);

        Assert.NotNull(detail);
        Assert.Equal(created.Id, detail.Id);
        Assert.Equal(created.FullName, detail.FullName);
    }

    [Fact]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        await using var db = CreateDb();
        var service = CreateService(db);

        var detail = await service.GetByIdAsync(9999);

        Assert.Null(detail);
    }
}
