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

### 1. ตั้งค่าระบบให้ใช้ NuGet Local Feed

ถ้ามีไฟล์ `.nupkg` ให้ copy ไปไว้ในโฟลเดอร์:

```bash
C:\NuGetFeed
