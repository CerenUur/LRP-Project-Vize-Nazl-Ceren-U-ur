using Microsoft.EntityFrameworkCore;
using LRP_Project_Vize_Nazlý_Ceren_Uður.Data;
using LRP_Project_Vize_Nazlý_Ceren_Uður.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=lrp.db"));
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// 1. GÝRÝÞ YAPMA (Role Göre Bilgi Döner)
app.MapPost("/api/login", async (User loginUser, AppDbContext db) => {
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username && u.Password == loginUser.Password);
    if (user == null) return Results.Unauthorized();
    return Results.Ok(user);
});

// 2. LABORATUVARLARI LÝSTELE
app.MapGet("/api/labs", async (AppDbContext db) => {
    return await db.Laboratories.ToListAsync();
});

// 3. YENÝ BÝLGÝSAYAR EKLE, KOD ÜRET VE ÖÐRENCÝYE HESAP AÇ
app.MapPost("/api/computers", async (Computer pc, AppDbContext db) => {
    // Demirbaþ Kodu Üret
    var count = await db.Computers.CountAsync(c => c.LaboratoryId == pc.LaboratoryId);
    pc.InventoryCode = $"LAB{pc.LaboratoryId}-PC-{(count + 1):D2}";

    db.Computers.Add(pc);

    // Sorumluluk Atama: Öðrenci no girilmiþse otomatik kullanýcý oluþtur
    if (!string.IsNullOrEmpty(pc.StudentNo))
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username == pc.StudentNo);
        if (existingUser == null)
        {
            db.Users.Add(new User
            {
                FullName = pc.StudentName ?? "Yeni Öðrenci",
                Username = pc.StudentNo,
                Password = pc.StudentNo, // Þifre varsayýlan öðrenci no
                Role = "Student"
            });
        }
    }

    await db.SaveChangesAsync();
    return Results.Ok(pc);
});

// 4. LABORATUVARDAKÝ CÝHAZLARI LÝSTELE
app.MapGet("/api/labs/{labId}/computers", async (int labId, AppDbContext db) => {
    return await db.Computers.Where(c => c.LaboratoryId == labId).ToListAsync();
});

// VERÝTABANI BAÞLATMA
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Users.Any())
    {
        db.Users.Add(new User { FullName = "Ceren Uður", Username = "admin", Password = "123", Role = "Admin" });
    }
    if (!db.Laboratories.Any())
    {
        db.Laboratories.Add(new Laboratory { Name = "Yazýlým Laboratuvarý" });
        db.Laboratories.Add(new Laboratory { Name = "Bilgisayar Aðlarý Lab" });
    }
    db.SaveChanges();
}
// ÖÐRENCÝNÝN KENDÝ CÝHAZINI GETÝRMESÝ ÝÇÝN GEREKLÝ API
app.MapGet("/api/student/device/{studentNo}", async (string studentNo, AppDbContext db) => {
    // Veritabanýnda StudentNo sütununda bu öðrenciye ait bilgisayarý bul
    var device = await db.Computers.FirstOrDefaultAsync(c => c.StudentNo == studentNo);

    if (device == null)
    {
        return Results.NotFound(new { message = "Cihaz bulunamadý" });
    }

    return Results.Ok(device);
});
// YENÝ LABORATUVAR EKLE
app.MapPost("/api/labs", async (Laboratory lab, AppDbContext db) => {
    db.Laboratories.Add(lab);
    await db.SaveChangesAsync();
    return Results.Ok(lab);
});

// LABORATUVAR GÜNCELLE (Adýný Deðiþtirme)
app.MapPut("/api/labs/{id}", async (int id, Laboratory updatedLab, AppDbContext db) => {
    var lab = await db.Laboratories.FindAsync(id);
    if (lab == null) return Results.NotFound();
    lab.Name = updatedLab.Name;
    await db.SaveChangesAsync();
    return Results.Ok(lab);
});
app.Run();