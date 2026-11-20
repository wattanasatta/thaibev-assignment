using System.ComponentModel.DataAnnotations;

namespace ThaiBev.PersonApi.Dtos;

public class CreatePersonRequest
{
    [Required(ErrorMessage = "กรุณากรอกชื่อ")]
    [StringLength(100, ErrorMessage = "ชื่อต้องไม่เกิน 100 ตัวอักษร")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกนามสกุล")]
    [StringLength(100, ErrorMessage = "นามสกุลต้องไม่เกิน 100 ตัวอักษร")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณาเลือกวันเกิด")]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "กรุณากรอกที่อยู่")]
    [StringLength(500, ErrorMessage = "ที่อยู่ต้องไม่เกิน 500 ตัวอักษร")]
    public string Address { get; set; } = string.Empty;
}
