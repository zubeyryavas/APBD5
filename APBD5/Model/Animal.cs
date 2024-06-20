using System;
using System.ComponentModel.DataAnnotations;

namespace APBD5.Model;
public class Animal{

    public int IdAnimal { get; set; }
    [Required]
    public required string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    public required string Category { get; set; }

    [Required]
    public required string Area { get; set; }
}
