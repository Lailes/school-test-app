# School Test App

**The point of exercise** is solve as many TODOs as you can. Please Feel free to send your solution if you completed at least 4 TODOs. Every task can have a lot of different solutions, think about your code style, performance, about balance between code style and performance :), and about possible bugs.



Now clone this repository and get on with the task. Good luck! ;)

### Tips and useful information
1. Solution contains 7 TODOs numbered from 0 to 6
2. Please attach to email with solution the description of reasons of TODO6 problems if you've solved it.
3. [Asp.Net core Authentication and Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-3.1). **Important!** You don't really need to use Identity Framework, use cookie authentication without identity instead.
4. [Asp.Net core Dependency injection](https://metanit.com/sharp/aspnet5/6.1.php)

*Information can be updated at some point of time. In that case you will be notified by email

## Solution of TODOes

### 1) TODO 0
Добавлена настройка
```c#
services.AddMvc(options => options.EnableEndpointRouting = false);
```

Добавлен синглтон AccountService, который требуется AccountController

```c#
services.AddSingleton<IAccountService, AccountService>();
```
За ненадобностью была отключена комментарием строка в Configure
```c#
app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"); });
```

### 2) TODO 1

Добавлена генерация аутентификационных куки в метод Login

```c#
var claims = new List<Claim> {
    new Claim(ClaimTypes.NameIdentifier, account.UserName),
    new Claim(ClaimTypes.Role, account.Role),
    new Claim(ClaimTypes.Name, account.ExternalId)
};
var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
await HttpContext.SignInAsync(claimsPrincipal);
```
Зарегестрирована система аутентификации в методе ConfigureServices в классе Sturtup

```c#
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
```
Добавлена аутентификация и авторизация в конвейер обработки запросов

```c#
app.UseAuthentication();
app.UseAuthorization();
```

### 3) TODO 2
### 4) TODO 3
### 5) TODO 4
### 6) TODO 5
### 7) TODO 6