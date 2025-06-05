# CCP.RoleAccessScanner

ระบบบันทึก Controller, Action และ Role/Policy ที่ใช้ใน ASP.NET Core MVC Project ลงฐานข้อมูลกลาง
เพื่อช่วยตรวจสอบสิทธิ์การเข้าถึงหน้าเว็บของแต่ละโปรเจคได้อย่างมีประสิทธิภาพ

---

## ✨ คุณสมบัติหลัก

* สแกน Controller และ Action ทุกตัวในระบบ พร้อม Role/Policy
* รองรับการใช้ `[Authorize]` ทั้งระดับ Controller และ Method
* รองรับ `appsettings.json` Role Mapping
* รองรับ `[RemarkPage]` ระบุรายละเอียดหน้านั้น ๆ เพื่อสร้างบน View
* อัปเดตอัตโนมัติเมื่อรันโปรเจค
* รองรับการใช้ Model และ DbSet จากแต่ละโปรเจคโดยไม่ผูกกับ Model ภายในแพคเกจ

---

## ✅ โครงสร้างที่ต้องเตรียม

### 1. โพรไฟล์ NuGet Package

ใน Git Repo จะมีโฟลเดอร์แยกออกเป็น 2 ส่วน:

* `/CCP.RoleAccessScanner` → ตัวโปรเจคแพคเกจ (Library)
* `/TEST` → ตัวอย่างโปรเจคที่เรียกใช้ Package

---

## 📅 การติดตั้งแพคเกจในโปรเจคอื่น

### 📑 Visual Studio

1. ไปที่ `Tools > NuGet Package Manager > Package Sources`
2. เพิ่ม Source ใหม่:

   * **Name**: CCP.Local
   * **Path**: `\\ccpnas\dep-it\25.Programmer\MyNuGets\CCP.RoleAccessScanner`
3. ติดตั้งผ่าน Package Manager Console:

```bash
Install-Package CCP.RoleAccessScanner -Source CCP.Local
```

### 💻 VS Code / .NET CLI

```bash
dotnet add package CCP.RoleAccessScanner --source \\ccpnas\dep-it\25.Programmer\MyNuGets\CCP.RoleAccessScanner
```

---

## 📄 การตั้งค่าโปรเจคเพื่อใช้งาน Package

### 1. เพิ่ม Role Mapping ใน `appsettings.json`

```json
"AuthorizationPolicies": {
  "Admin": ["SystemAll", "Admin"],
  "Manager": ["SystemAll", "Manager"],
  "User": ["SystemAll", "Admin", "User"]
}
```

### 2.  เปลี่ยน Model ที่ได้จาก scaffold เป็น partial class และ Implement Interface `IRoleAccessRecord` 

```csharp
using CCP.RoleAccessScanner.Interfaces;

public partial class RoleAccessLog : IRoleAccessRecord
{
    public string ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Role { get; set; }
    public string? Type { get; set; }
    public string? Remark { get; set; }
    public DateTime LoggedAt { get; set; }
}
```

### 3. เพิ่ม DbSet ใน DbContext ของคุณ

```csharp
public DbSet<RoleAccessLog> RoleAccessLogs { get; set; }
```

### 4. เรียกใช้ใน `Program.cs`

```csharp
builder.Services.AddRoleAccessScanner<AppDbContext, RoleAccessLog>(
    configuration: builder.Configuration,
    projectId: "PROJ-001",
    projectName: "ระบบจัดการผู้ใช้งาน"
);
```

### 5. ใช้ Attribute `[RemarkPage]` เพื่อกำหนดรายละเอียดของแต่ละหน้า

```csharp
private readonly remarkPage = "รายละเอียดหน้าจัดการผู้ใช้";
[Authorize("Admin")]
[RemarkPage(remarkPage)]
public IActionResult Index() => View();
```

---

## 🏁 ประเภท Action ที่ระบบจะแยก

| ลักษณะ                                        | มี View | HTTP Method | เก็บเป็นประเภท |
| --------------------------------------------- | ------- | ----------- | -------------- |
| GET ที่มี View                                | ✔       | GET         | Page           |
| POST/PUT/DELETE มี View หรือชื่อมี button/btn | ✔/❌     | POST        | Button         |
| อื่น ๆ                                        | ❌       | -           | Event          |

> ถ้ามี Method ทั้ง `GET` และ `POST` ชื่อเดียวกัน มี View — ระบบจะเก็บแค่ `GET` เป็น Page เท่านั้น

---

## ⚖️ ตัวอย่างการเขียน Controller

```csharp
private readonly remarkPage = "รายละเอียดหน้าจัดการผู้ใช้";
[Authorize("Admin")]
public class EmployeeController : Controller
{
    [RemarkPage(remarkPage)]
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Save(Employee model)
    {
        // บันทึกข้อมูล
        return RedirectToAction("Index");
    }
}
```

---

## 🚀 การ Build และ Pack NuGet

```bash
dotnet pack -c Release
```

* ไฟล์ `.nupkg` จะอยู่ใน `bin/Release/`
* คัดลอกไปไว้ที่ `\\ccpnas\dep-it\25.Programmer\MyNuGets\CCP.RoleAccessScanner`

---

## 💬 สอบถามหรือรายงานปัญหา

ติดต่อทีม Programmer CCP โดยตรง หรือเปิด Issue ภายใน Git Repository ขององค์กร
