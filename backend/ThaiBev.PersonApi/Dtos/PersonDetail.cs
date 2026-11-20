namespace ThaiBev.PersonApi.Dtos;

/// <summary>ข้อมูลเต็มสำหรับ View modal (IT 01-3) และ response ของ POST</summary>
public record PersonDetail(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    DateOnly BirthDate,
    int Age,
    string Address);
