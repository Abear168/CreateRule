# CreateRule 项目增强计划：菜单栏、登录与权限功能

## 📋 项目概述

为现有的 ASP.NET Core Razor Pages 应用添加以下功能：
- **菜单栏**：多页面导航系统
- **登录功能**：用户认证系统
- **权限功能**：基于角色的访问控制（RBAC）

---

## 🎯 技术方案

### 核心技术栈
- **ASP.NET Core Identity**：内置的身份认证和授权系统
- **Entity Framework Core**：ORM框架，用于数据库操作
- **SQLite**：轻量级数据库（可轻松切换到SQL Server）
- **Bootstrap 5**：UI框架（项目已使用）

### 架构设计
```
用户 (User) ──┬── 角色 (Role) ──── 权限 (Permission)
              │
              └── 用户角色映射 (UserRole)
```

---

## 📝 详细实施步骤

### 阶段一：数据库与身份认证基础

#### 1. 安装必要的 NuGet 包
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="*" />
```

#### 2. 创建数据模型
- **ApplicationUser**：扩展用户模型（继承IdentityUser）
  - 真实姓名
  - 部门
  - 创建时间
  - 最后登录时间
  - **审核状态**（Pending/Approved/Rejected）
  - **审核时间**
  - **审核人ID**
  - **审核备注**

- **ApplicationRole**：角色模型（继承IdentityRole）
  - 角色名称
  - 角色描述
  - 创建时间

- **Permission**：权限模型
  - 权限代码（如 "user.create", "rule.generate"）
  - 权限名称
  - 权限描述
  - 所属模块

- **RolePermission**：角色权限关联表
  - 角色ID
  - 权限ID

- **AuditLog**：审核日志模型
  - 用户ID
  - 操作类型（注册审核/其他审核）
  - 操作时间
  - 操作人ID
  - 操作结果
  - 备注信息

#### 3. 创建数据库上下文
- **ApplicationDbContext**：继承 IdentityDbContext
- 配置实体映射关系
- 配置连接字符串（appsettings.json）

#### 4. 配置 Identity 服务
- 在 Program.cs 中配置 Identity
- 配置密码策略、登录策略
- 配置 Cookie 认证选项

#### 5. 数据库迁移
- 创建初始迁移
- 生成数据库表结构

---

### 阶段二：用户认证功能

#### 6. 创建登录页面
- **Pages/Account/Login.cshtml**：登录界面
- **Pages/Account/Login.cshtml.cs**：登录逻辑
- 功能：
  - 用户名/密码登录
  - 记住我功能
  - 登录失败提示
  - 登录成功跳转

#### 7. 创建注册页面（审核流程）
- **Pages/Account/Register.cshtml**：注册界面
- **Pages/Account/Register.cshtml.cs**：注册逻辑
- 功能：
  - 用户名、邮箱、密码输入
  - 真实姓名、部门信息输入
  - 密码强度验证
  - 邮箱格式验证
  - **提交后状态为"待审核"**
  - **提示用户等待审核**
  - **不允许自动登录**

#### 7.1 创建注册待审核提示页面
- **Pages/Account/RegisterPending.cshtml**：注册待审核提示
- 功能：
  - 显示"注册申请已提交，请等待审核"
  - 提供联系方式
  - 引导用户返回登录页

#### 8. 创建审核管理页面
- **Pages/Admin/Users/Pending.cshtml**：待审核用户列表
- **Pages/Admin/Users/Approve.cshtml**：审核用户详情
- 功能：
  - 查看所有待审核用户
  - 查看用户详细信息
  - **审核通过**：激活账号，发送邮件通知
  - **审核拒绝**：记录拒绝原因，可选择发送通知邮件
  - 批量审核操作
  - 审核历史记录

#### 9. 创建邮件发送服务
- **Services/EmailService.cs**：邮件发送服务
- 功能：
  - SMTP 配置（appsettings.json）
  - 发送注册成功邮件
  - 发送审核拒绝邮件
  - 邮件模板管理
  - 异步发送
  - 发送失败重试机制

#### 10. 创建邮件模板
- **EmailTemplates/RegistrationApproved.html**：注册成功邮件模板
- **EmailTemplates/RegistrationRejected.html**：审核拒绝邮件模板
- 内容：
  - 注册成功：欢迎信息、登录指引
  - 审核拒绝：拒绝原因、联系方式

#### 11. 配置邮件服务
- 在 **appsettings.json** 添加 SMTP 配置：
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@example.com",
    "SmtpPassword": "your-password",
    "EnableSsl": true,
    "SenderEmail": "noreply@example.com",
    "SenderName": "CreateRule System"
  }
}
```

#### 12. 创建登出功能
- **Pages/Account/Logout.cshtml.cs**：登出处理
- 清除认证 Cookie
- 跳转到登录页

#### 13. 创建用户管理页面
- **Pages/Admin/Users/Index.cshtml**：用户列表
- **Pages/Admin/Users/Create.cshtml**：创建用户
- **Pages/Admin/Users/Edit.cshtml**：编辑用户
- **Pages/Admin/Users/Delete.cshtml**：删除用户
- 功能：
  - 用户列表展示（分页）
  - 搜索用户
  - 启用/禁用用户
  - 重置密码
  - 分配角色
  - **查看用户审核状态**
  - **筛选待审核/已审核/已拒绝用户**

---

### 阶段三：角色与权限管理

#### 14. 创建角色管理页面
- **Pages/Admin/Roles/Index.cshtml**：角色列表
- **Pages/Admin/Roles/Create.cshtml**：创建角色
- **Pages/Admin/Roles/Edit.cshtml**：编辑角色
- **Pages/Admin/Roles/Delete.cshtml**：删除角色
- 功能：
  - 角色列表展示
  - 创建新角色
  - 编辑角色信息
  - 为角色分配权限

#### 15. 创建权限管理页面
- **Pages/Admin/Permissions/Index.cshtml**：权限列表
- **Pages/Admin/Permissions/Create.cshtml**：创建权限
- **Pages/Admin/Permissions/Edit.cshtml**：编辑权限
- 功能：
  - 权限列表展示
  - 创建新权限
  - 编辑权限信息
  - 按模块分组显示

#### 16. 实现权限检查
- 创建自定义特性 **PermissionAttribute**
- 创建权限服务 **PermissionService**
- 在页面模型中应用权限检查
- 在 Razor 视图中应用权限检查

---

### 阶段四：菜单栏功能

#### 17. 设计菜单结构
- **菜单项模型**：
  - 菜单名称
  - 菜单图标
  - 链接地址
  - 父菜单ID（支持多级菜单）
  - 排序顺序
  - 所需权限代码

#### 18. 创建菜单数据
- 在数据库中创建菜单表
- 初始化默认菜单数据：
  - 首页
  - 规则管理
    - 规则导入（现有功能）
    - 规则设置（链接到外部）
  - 系统管理
    - 用户管理
    - 角色管理
    - 权限管理
    - **审核管理**（新增）

#### 19. 实现动态菜单
- 创建 **MenuService** 服务
- 根据用户权限动态生成菜单
- 在 _Layout.cshtml 中渲染菜单
- 支持多级菜单展开/折叠

#### 20. 更新布局页面
- 修改 **_Layout.cshtml**
- 添加侧边栏菜单
- 添加顶部导航栏（用户信息、登出按钮）
- 响应式设计（移动端适配）

---

### 阶段五：权限控制实施

#### 21. 保护现有页面
- **Index.cshtml**（规则导入）：需要 "rule.view" 权限
- **OnPostGenerateAsync**：需要 "rule.generate" 权限

#### 22. 创建权限常量类
```csharp
public static class Permissions
{
    public static class Rule
    {
        public const string View = "rule.view";
        public const string Create = "rule.create";
        public const string Generate = "rule.generate";
    }
    
    public static class User
    {
        public const string View = "user.view";
        public const string Create = "user.create";
        public const string Edit = "user.edit";
        public const string Delete = "user.delete";
        public const string Approve = "user.approve";  // 新增审核权限
    }
    
    // ... 其他权限
}
```

#### 23. 创建种子数据
- 创建默认管理员账户
- 创建默认角色（管理员、普通用户）
- 创建默认权限
- 分配默认权限给角色

---

### 阶段六：前端优化

#### 24. 美化登录页面
- 使用 Bootstrap 设计登录表单
- 添加背景样式
- 添加表单验证提示

#### 25. 优化菜单样式
- 侧边栏菜单样式
- 激活状态高亮
- 悬停效果
- 折叠动画

#### 26. 添加用户反馈
- 操作成功/失败提示
- 加载动画
- 确认对话框

#### 27. 优化审核页面
- 待审核用户列表样式
- 审核操作按钮设计
- 审核历史展示
- 批量操作界面

---

### 阶段七：安全增强

#### 28. 实现安全特性
- 密码强度验证
- 登录失败次数限制
- 账户锁定策略
- 密码加密存储（Identity自动处理）

#### 29. 添加审计日志
- 记录用户登录/登出
- 记录关键操作（创建、修改、删除）
- 记录权限变更
- **记录审核操作**

#### 30. 实现防 CSRF 攻击
- 自动验证 Token（Razor Pages 默认支持）
- 确保所有 POST 请求都有 Token

---

## 🗂️ 文件结构规划

```
CreateRule/
├── Areas/
│   └── Identity/
│       └── Pages/           # Identity 相关页面（可选）
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Migrations/
│   └── DbInitializer.cs     # 种子数据
├── Models/
│   ├── ApplicationUser.cs
│   ├── ApplicationRole.cs
│   ├── Permission.cs
│   ├── RolePermission.cs
│   ├── MenuItem.cs
│   └── AuditLog.cs          # 新增
├── Services/
│   ├── PermissionService.cs
│   ├── MenuService.cs
│   └── EmailService.cs      # 新增
├── Attributes/
│   └── PermissionAttribute.cs
├── EmailTemplates/          # 新增
│   ├── RegistrationApproved.html
│   └── RegistrationRejected.html
├── Pages/
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   ├── RegisterPending.cshtml  # 新增
│   │   └── Logout.cshtml
│   ├── Admin/
│   │   ├── Users/
│   │   │   ├── Index.cshtml
│   │   │   ├── Create.cshtml
│   │   │   ├── Edit.cshtml
│   │   │   ├── Delete.cshtml
│   │   │   ├── Pending.cshtml      # 新增
│   │   │   └── Approve.cshtml      # 新增
│   │   ├── Roles/
│   │   └── Permissions/
│   ├── Shared/
│   │   ├── _Layout.cshtml   # 更新
│   │   ├── _LoginPartial.cshtml
│   │   └── _MenuPartial.cshtml
│   └── Index.cshtml         # 更新（添加权限检查）
└── wwwroot/
    ├── css/
    │   └── menu.css         # 菜单样式
    └── js/
        └── menu.js          # 菜单交互
```

---

## 🔐 默认权限设计

### 角色定义
1. **超级管理员 (SuperAdmin)**
   - 所有权限
   - 管理其他管理员

2. **管理员 (Admin)**
   - 用户管理
   - 角色管理
   - 规则管理

3. **普通用户 (User)**
   - 查看规则
   - 生成规则

### 权限矩阵
| 权限代码 | 权限名称 | 超级管理员 | 管理员 | 普通用户 |
|---------|---------|-----------|--------|---------|
| rule.view | 查看规则 | ✓ | ✓ | ✓ |
| rule.generate | 生成规则 | ✓ | ✓ | ✓ |
| user.view | 查看用户 | ✓ | ✓ | ✗ |
| user.create | 创建用户 | ✓ | ✓ | ✗ |
| user.edit | 编辑用户 | ✓ | ✓ | ✗ |
| user.delete | 删除用户 | ✓ | ✗ | ✗ |
| user.approve | 审核用户 | ✓ | ✓ | ✗ |
| role.manage | 管理角色 | ✓ | ✓ | ✗ |
| permission.manage | 管理权限 | ✓ | ✗ | ✗ |

---

## 📊 数据库表设计

### 用户表 (AspNetUsers)
- Id (主键)
- UserName
- Email
- PasswordHash
- RealName (自定义)
- Department (自定义)
- CreatedAt (自定义)
- LastLoginAt (自定义)
- **ApprovalStatus (自定义)** - 审核状态（Pending/Approved/Rejected）
- **ApprovedAt (自定义)** - 审核时间
- **ApprovedBy (自定义)** - 审核人ID
- **ApprovalRemark (自定义)** - 审核备注

### 角色表 (AspNetRoles)
- Id (主键)
- Name
- Description (自定义)
- CreatedAt (自定义)

### 用户角色关联表 (AspNetUserRoles)
- UserId
- RoleId

### 权限表 (Permissions)
- Id (主键)
- Code (唯一)
- Name
- Description
- Module

### 角色权限关联表 (RolePermissions)
- RoleId
- PermissionId

### 菜单表 (MenuItems)
- Id (主键)
- Name
- Icon
- Url
- ParentId
- Order
- PermissionCode

### 审核日志表 (AuditLogs)
- Id (主键)
- UserId (外键)
- ActionType (操作类型：注册审核/其他审核)
- ActionTime (操作时间)
- OperatorId (操作人ID)
- Result (操作结果：Approved/Rejected)
- Remark (备注信息)
- CreatedAt

---

## 🚀 实施顺序

### 第一优先级（核心功能）
1. 安装包和配置数据库
2. 创建数据模型和上下文
3. 配置 Identity 服务
4. 创建登录/登出功能
5. 创建注册功能（含审核流程）
6. 创建邮件发送服务
7. 创建审核管理页面
8. 更新布局页面（基础菜单）

### 第二优先级（管理功能）
9. 创建用户管理页面
10. 创建角色管理页面
11. 创建权限管理页面
12. 实现权限检查逻辑

### 第三优先级（优化完善）
13. 美化界面
14. 添加审计日志
15. 完善错误处理
16. 测试和调试

---

## ⚠️ 注意事项

1. **数据迁移**
   - 现有数据不受影响
   - 新表会自动创建
   - 可以后期迁移到 SQL Server

2. **性能考虑**
   - 权限检查使用缓存
   - 菜单数据缓存
   - 数据库查询优化

3. **安全考虑**
   - 所有密码加密存储
   - 防止 SQL 注入
   - 防止 XSS 攻击
   - CSRF 保护

4. **兼容性**
   - 保持现有功能不变
   - 渐进式添加新功能
   - 向后兼容

---

## 📈 预期成果

完成后，项目将具备：
- ✅ 完整的用户认证系统
- ✅ **用户注册审核流程**
- ✅ **邮件通知功能**
- ✅ 基于角色的权限控制
- ✅ 动态菜单栏（根据权限显示）
- ✅ 用户管理界面
- ✅ **审核管理界面**
- ✅ 角色管理界面
- ✅ 权限管理界面
- ✅ 安全的密码存储
- ✅ 操作审计日志

---

## 🔄 审核流程详细设计

### 用户注册审核流程

```
用户提交注册
    ↓
创建账号（状态：Pending）
    ↓
显示"等待审核"提示
    ↓
管理员收到待审核通知
    ↓
管理员审核
    ├─→ 审核通过
    │       ↓
    │   激活账号（状态：Approved）
    │       ↓
    │   发送注册成功邮件
    │       ↓
    │   用户可以登录
    │
    └─→ 审核拒绝
            ↓
        记录拒绝原因（状态：Rejected）
            ↓
        可选：发送拒绝通知邮件
            ↓
        用户无法登录
```

### 邮件发送流程

#### 注册成功邮件
```
触发时机：管理员审核通过用户注册
收件人：用户注册邮箱
邮件内容：
  - 欢迎信息
  - 用户名
  - 登录地址
  - 初始密码提示（建议首次登录修改密码）
  - 联系方式
```

#### 审核拒绝邮件（可选）
```
触发时机：管理员审核拒绝用户注册
收件人：用户注册邮箱
邮件内容：
  - 审核结果通知
  - 拒绝原因
  - 联系方式（如有疑问）
```

### 审核管理界面设计

#### 待审核用户列表
- 显示字段：
  - 用户名
  - 真实姓名
  - 邮箱
  - 部门
  - 注册时间
  - 操作按钮（查看详情、审核通过、审核拒绝）

- 筛选功能：
  - 按部门筛选
  - 按注册时间范围筛选

#### 审核详情页面
- 用户信息展示
- 审核操作区：
  - 审核通过按钮
  - 审核拒绝按钮
  - 审核备注输入框
- 审核历史记录（如有）

### 邮件服务设计

#### EmailService 接口
```csharp
public interface IEmailService
{
    Task<bool> SendRegistrationApprovedEmailAsync(string email, string username);
    Task<bool> SendRegistrationRejectedEmailAsync(string email, string reason);
    Task<bool> SendEmailAsync(string to, string subject, string body);
}
```

#### 邮件模板变量
- `{{Username}}` - 用户名
- `{{LoginUrl}}` - 登录地址
- `{{Reason}}` - 拒绝原因
- `{{ContactEmail}}` - 联系邮箱
- `{{SystemName}}` - 系统名称

### 安全考虑

1. **邮箱验证**
   - 注册时验证邮箱格式
   - 发送邮件前验证邮箱有效性

2. **审核权限**
   - 只有拥有 `user.approve` 权限的用户才能审核
   - 记录审核人信息

3. **邮件发送安全**
   - SMTP 使用 TLS/SSL 加密
   - 邮件密码加密存储
   - 发送失败记录日志

4. **防止滥用**
   - 限制同一邮箱注册频率
   - 限制待审核账号有效期（如30天未审核自动清理）

---

## 🛠️ 技术债务

未来可以考虑的改进：
- 双因素认证（2FA）
- OAuth2.0 / OpenID Connect 集成
- 密码找回功能
- 用户头像上传
- 操作日志详细记录
- 数据导出功能
