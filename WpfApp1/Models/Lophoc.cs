using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Lophoc
{
    public int Malop { get; set; }

    public string? Tenlop { get; set; }

    public string? Giangvien { get; set; }

    public virtual ICollection<Sinhvien> Sinhviens { get; set; } = new List<Sinhvien>();
}
