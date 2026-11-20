using Microsoft.EntityFrameworkCore;
using ThaiBev.PersonApi.Data;
using ThaiBev.PersonApi.Dtos;
using ThaiBev.PersonApi.Entities;

namespace ThaiBev.PersonApi.Services;

public class PersonService(AppDbContext db, TimeProvider clock) : IPersonService
{
    public async Task<IReadOnlyList<PersonListItem>> GetAllAsync(CancellationToken ct = default)
    {
        var today = AgeCalculator.TodayInBangkok(clock);
        var persons = await db.Persons.AsNoTracking()
            .OrderBy(p => p.Id) // ข้อมูลใหม่ (Id มาก) อยู่ท้ายตาราง
            .ToListAsync(ct);
        return persons.Select(p => ToListItem(p, today)).ToList();
    }

    public async Task<PersonDetail?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var person = await db.Persons.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return person is null ? null : ToDetail(person, AgeCalculator.TodayInBangkok(clock));
    }

    public async Task<PersonDetail> CreateAsync(CreatePersonRequest request, CancellationToken ct = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
        {
            throw new AppValidationException(errors);
        }

        var person = new Person
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            BirthDate = request.BirthDate,
            Address = request.Address.Trim(),
            CreatedAt = clock.GetUtcNow().UtcDateTime,
        };

        db.Persons.Add(person);
        await db.SaveChangesAsync(ct);

        return ToDetail(person, AgeCalculator.TodayInBangkok(clock));
    }

    private Dictionary<string, string[]> Validate(CreatePersonRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        void Check(string key, bool invalid, string message)
        {
            if (invalid)
            {
                errors[key] = [message];
            }
        }

        Check("firstName", string.IsNullOrWhiteSpace(request.FirstName), "กรุณากรอกชื่อ");
        Check("firstName", (request.FirstName?.Trim().Length ?? 0) > 100, "ชื่อต้องไม่เกิน 100 ตัวอักษร");
        Check("lastName", string.IsNullOrWhiteSpace(request.LastName), "กรุณากรอกนามสกุล");
        Check("lastName", (request.LastName?.Trim().Length ?? 0) > 100, "นามสกุลต้องไม่เกิน 100 ตัวอักษร");
        Check("address", string.IsNullOrWhiteSpace(request.Address), "กรุณากรอกที่อยู่");
        Check("address", (request.Address?.Trim().Length ?? 0) > 500, "ที่อยู่ต้องไม่เกิน 500 ตัวอักษร");

        var today = AgeCalculator.TodayInBangkok(clock);
        if (request.BirthDate == default) // ไม่ได้ส่ง birthDate มา (JSON default 0001-01-01)
        {
            errors["birthDate"] = ["กรุณาเลือกวันเกิด"];
        }
        else if (request.BirthDate > today)
        {
            errors["birthDate"] = ["วันเกิดต้องไม่เป็นวันที่ในอนาคต"];
        }
        else if (request.BirthDate < today.AddYears(-150))
        {
            errors["birthDate"] = ["วันเกิดต้องไม่เก่าเกิน 150 ปี"];
        }

        return errors;
    }

    private static PersonListItem ToListItem(Person p, DateOnly today) =>
        new(p.Id, $"{p.FirstName} {p.LastName}", p.Address, p.BirthDate, AgeCalculator.CalculateAge(p.BirthDate, today));

    private static PersonDetail ToDetail(Person p, DateOnly today) =>
        new(p.Id, p.FirstName, p.LastName, $"{p.FirstName} {p.LastName}", p.BirthDate,
            AgeCalculator.CalculateAge(p.BirthDate, today), p.Address);
}
