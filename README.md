# CCP.RoleAccessScanner

ระบบ Role Access Scanner สำหรับ ASP.NET Core MVC  
ช่วยตรวจจับว่าแต่ละ Role สามารถเข้าถึงหน้าหรือฟังก์ชันใดบ้างในระบบ แล้วบันทึกลงฐานข้อมูลโดยอัตโนมัติ

---

## ✅ ความสามารถหลัก

- ตรวจจับ `[Authorize(Roles = "...")]` หรือ `[Authorize(Policy = "...")]` หรือ `[Authorize("...")]` ทั้งระดับ Controller และ Action
- จำแนกสิทธิ์ว่าเป็น:
  - `Page` → มี View `.cshtml` ตรงกับ Action
  - `Action` → มีลักษณะเป็น POST หรือมีคำว่า Save/Delete/Update
  - `Unknown` → กรณีไม่เข้าเงื่อนไข
- บันทึกลง `RoleAccessLogs` ในฐานข้อมูลผ่าน `EF Core`
- ทำงานอัตโนมัติเมื่อแอปเริ่มทำงาน

---

## 📦 ติดตั้ง Package

### กรณีใช้ Visual Studio
1. คลิกขวาที่โปรเจกต์ของคุณ → เลือก **Manage NuGet Packages**
2. ไปที่แท็บ **Browse**
3. คลิกไอคอน ⚙️ (Settings) ที่มุมขวาบน
4. ในหน้าต่าง **NuGet Package Manager**, เพิ่ม **Package source** ใหม่:
   - **Name**: `CCPLocal`
   - **Source**: `\\ccpnas\Programmer\MyNuGets\CCP.RoleAccessScanner`
5. คลิก **Update** หรือ **OK** เพื่อบันทึก
6. กลับมาที่แท็บ **Browse**
   - เปลี่ยน **Package source** เป็น `CCPLocal`
   - ค้นหา `CCP.RoleAccessScanner`
   - เลือกและคลิก **Install**
   
### กรณีใช้ VS Code
  1. dotnet add package CCP.RoleAccessScanner --source \\ccpnas\Programmer\MyNuGets\CCP.RoleAccessScanner
## หมายเหตุ
- ให้แน่ใจว่าเครือข่ายสามารถเข้าถึง path `\\ccpnas\Programmer`
- หากไม่พบแพ็กเกจ ให้คลิกปุ่ม **Refresh** (ไอคอนลูกศรวน)

## 🧪 วิธีใช้งานในโปรเจกต์
  1. using CCP.RoleAccessScanner.Extensions;
  2. builder.Services.AddRoleAccessScanner<AppDbContext>("PROJECT_ID", "PROJECT_NAME");
  3. DbContext ของคุณ ต้องรวม RoleAccessLog => public DbSet<**RoleAccessLog**> RoleAccessLogs { get; set; }
  4. การเพิ่ม [RemarkPage] => [RemarkPage("หน้าจัดการผู้ใช้")] ข้างบน method

##📝 **ตัวอย่างการใช้งาน**
![image](https://github.com/user-attachments/assets/928e62dc-6c30-4bc5-b7dd-52c0666854f4)


![image](https://github.com/user-attachments/assets/d58f7bf3-98cc-4393-aa7f-f17152aa4c1e)


![image](https://github.com/user-attachments/assets/c01aee26-e928-41e9-af8a-4b9a992def6d)


![image](https://github.com/user-attachments/assets/a14d5728-6f31-4f32-bd45-6861a3cb68e1)


![image](https://github.com/user-attachments/assets/f29739df-8159-4955-9976-b4e205d68340)



```bash
C:\NuGetFeed
