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
Добавлен параметр userName в RouteAttribute для корректной передачи имени пользователя в действие
```c#
[HttpPost("sign-in/{userName}")]
public async Task<IActionResult> Login(string userName)
{
    ...
}
```

Добавлена генерация аутентификационных куки в метод Login
```c#
var claims = new List<Claim> {
    new Claim(ClaimTypes.NameIdentifier, account.ExternalId),
    new Claim(ClaimTypes.Role, account.Role),
    new Claim(ClaimTypes.Name, account.UserName)
};
var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
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

Изменена сигнатура метода, на возврат Task< IActionResult >
```c#
public async Task<IActionResult> Login(string userName) {
...
}
```

В случае, если в результате поиска пользователя получили null, то действие возвращает код 404 с помощью метода NotFound()
```c#
return NotFound();
```

Если поиск вернул пользователя, то после добавление аутентификационных куки действие возвращает код 200
```c#
return Ok();
```

### 4) TODO 3

Добавлено получение userId из куки при помощи свойства Claims объекта User
```c#
[Authorize] 
[HttpGet]
public async ValueTask<Account> Get()
{
    var userId = User.Claims
        .Where(claim => claim.Type == ClaimTypes.NameIdentifier)
        .Select(claim => claim.Value)
        .FirstOrDefault();

    if (userId == null)
        return null;
    
    return await _accountService.LoadOrCreateAsync(userId /* TODO_ 3: Get user id from cookie */);
}
```
### 5) TODO 4

Было выснено, что неавторизованный пользователь, при обращении к защищенному контроллеру или действию, переходит на страницу, устанавливаемую свойством LoginPath. По умолчанию он не установлен, и при переходе выдается 404 Not Found

В LoginPath был установлен путь, при переходе на который возвращается 401 Unauthorized
```c#
.AddCookie(options => {
    options.LoginPath = "/api/denied";
});
```
Создано действие в контроллере LoginController, возвращающее 401 Unauthorized при переходе на него

```c#
[HttpGet("denied")]
public IActionResult AccessDenied() => Unauthorized();
```
Для проверки данной функциональности был написан юниттест TestUnauthorized

### 6) TODO 5

В атрибут Authorize было добавлен параметр Roles с значением "Admin", таким образом действие будет работать только для пользователей с ролью Admin

```c#
[Authorize(Roles = "Admin")]
```

При переходе пользователем, не имеющего роль Admin, данное действие совершает переход на адрес, установленный свойством AccessDeniedPath класса CookieAuthenticationOptions
```c#
.AddCookie(options => {
    options.LoginPath = "/api/denied";
    options.AccessDeniedPath = "/api/denied"; 
});
```
Для проверки данной функциональности был написан юниттест TestAdminUnauthorized
### 7) TODO 6

Обновлен метод GetFromCache класса AccountService. При отсутвии аккаунта в кэше, метод находит его в базе акаунтов и добавляет его в кэш для последующего использования

```c#
public async ValueTask<Account> GetFromCache(long id)
{
    if (_cache.TryGetValue(id, out var account))
        return account;

    account = await _db.GetOrCreateAccountAsync(id);
    _cache.AddOrUpdate(account);
    
    return account;
}
```
Из-за смены сигнатуры метода (Изменено возвращаемое значение с Account на ValueTask< Account >)
был изменен интерфейс IAccountService (изменено возвращаемое значение метода) и было изменено возвращаемое значение действия GetByInternalId класса AccountController

```c#
public interface IAccountService
{
    ValueTask<Account> GetFromCache(long id);
    ...
}
```

```c#
[Authorize(Roles = "Admin")]
[HttpGet("{id}")]
public ValueTask<Account> GetByInternalId([FromRoute] int id)
{
    return _accountService.GetFromCache(id);
}
```

В ходе поиска причин возникновения копии аккаунта при работе действия Get контроллера AccountController было выяснено, что серия методов "GetOrCreate..." 
класса AccountDatabaseStub возвращает не объект, а его копию. Из-за чего действие увеличения Counter производится над копией. 
Для решения данной проблемы надо либо возвращать оригинальный объект, либо сохранять объект (применять изменения) в базе данных

Было принято решение реализовать методы ручного сохранения. 
Данный способ, по моему мнению, даст больше гибкости в случае, если надо будет заменить реализацию IAccountDatabase с AccountDatabaseStub на реализацию, требующую "ручного" сохранения


**Изменения в коде:**

Изменения интерфейса:
```c#
public interface IAccountDatabase
{
    ...
    
    // Метод возвращает значение типа Task< bool > в зависимости от того, 
    // произошло ли сохранение. 
    // (Если записи с таким externalId не существует, то сохранение не происходит)
    Task<bool> SaveChangesAsync(Account account);
}
```
Реализация:
```c#
public Task<bool> SaveChangesAsync(Account account)
{
    lock (this)
    {
        if (_accounts.ContainsKey(account.ExternalId))
        {
            _accounts[account.ExternalId] = account;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
```
Изменения интерфейса:
```c#
public interface IAccountService
{
    ...
    ValueTask<bool> SaveChanges(Account account);
}
```
Реализация:
```c#
public async ValueTask<bool> SaveChanges(Account account)
{
    var wasSaved = await _db.SaveChangesAsync(account);
    if (wasSaved)
        _cache.AddOrUpdate(account);
    
    return wasSaved;
}
```

Итоговый код действия контроллера:
```c#
[Authorize]
[HttpPost("counter")]
public async Task UpdateAccount()
{
    //Update account in cache, don't bother saving to DB, this is not an objective of this task.
    var actionResult = await Get();
    if (actionResult.Result != NotFound())
    {
        var account = actionResult.Value;
        account.Counter++; 
        await _accountService.SaveChanges(account);
    }
}
```
