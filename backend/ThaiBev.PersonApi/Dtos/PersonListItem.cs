namespace ThaiBev.PersonApi.Dtos;

/// <summary>แถวในหน้ารายการ (IT 01-1): birthDate เป็น ค.ศ. ISO yyyy-MM-dd — UI แปลงเป็น พ.ศ. เอง</summary>
public record PersonListItem(int Id, string FullName, string Address, DateOnly BirthDate, int Age);
