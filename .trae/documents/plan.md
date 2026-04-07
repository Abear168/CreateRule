# 工单号下拉框功能计划

## 1. 创建工单服务 `WorkOrderService.cs`

文件: `Services/WorkOrderService.cs`

### 1.1 Token获取
- 登录接口: `POST http://10.10.150.250:8080/hmmes/api/ac/login`
- 请求体: `{"account":"superadmin","password":"admin"}`
- 响应中提取 `tokenValue`

### 1.2 工单列表获取
- 接口: `POST http://10.10.150.250/hmmes/api/workOrder/page`
- 请求头: `tokenValue` (来自登录响应)
- 请求体:
```json
{
  "createUserId": "1",
  "modifyUserId": "1",
  "pageSize": 1000,
  "pageNumber": 1
}
```
- 响应中提取 `result.records[]` 的 `name` 字段

## 2. 创建 DTO 模型

### 2.1 LoginRequest / LoginResponse
- 登录请求和响应

### 2.2 WorkOrderRequest / WorkOrderResponse
- 工单列表请求和响应

### 2.3 WorkOrderItem
- 工单记录模型，只包含 name 字段

## 3. 修改内箱规则创建页面

### 3.1 添加下拉框
- 将工单号 input 改为 select2 下拉框
- 支持搜索、模糊匹配
- 支持直接输入

### 3.2 页面引入Select2
- 从CDN引入Select2 CSS/JS

### 3.3 JavaScript逻辑
- 页面加载时调用服务获取工单列表
- 填充下拉框
- 支持模糊搜索

## 4. 实现步骤

1. 创建 `Models/WorkOrderDto.cs` - DTO模型
2. 创建 `Services/WorkOrderService.cs` - API服务
3. 注册服务到 `Program.cs`
4. 修改 `Create.cshtml` - 改为下拉框
5. 修改 `Create.cshtml.cs` - 注入服务获取工单列表
6. 同样修改 `Edit.cshtml` 和 `Edit.cshtml.cs`
