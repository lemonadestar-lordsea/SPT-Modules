# Packet Sniffer

Patches are based on version 0.12.12.1.16069

## Deobfuscate

```bash
de4dot-x64.exe Assembly-CSharp.dll
de4dot-x64.exe --un-name "!^<>[a-z0-9]$&!^<>[a-z0-9]__.*$&![A-Z][A-Z]\$<>.*$&^[a-zA-Z_<{$][a-zA-Z_0-9<>{}$.`-]*$" "Assembly-CSharp-cleaned.dll" --strtyp delegate --strtok "YOUR TOKEN HERE"
```

## Assembly-CSharp.dll

### Save requests

```cs
// Token: 0x06001E99 RID: 7833 RVA: 0x001A54CC File Offset: 0x001A36CC
[postfix]
Class180.method_2()
{
    var uri = new Uri(url);
    var path = (System.IO.Directory.GetCurrentDirectory() + "\\HTTP_DATA\\").Replace("\\\\", "\\");
    var file = uri.LocalPath.Replace('/', '.').Remove(0, 1);
    var time = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    if (System.IO.Directory.CreateDirectory(path).Exists && obj != null)
    {
        System.IO.File.WriteAllText($@"{path}req.{file}_{time}.json", text);
    }
}
```

### Save responses

```cs
// Token: 0x06001EA5 RID: 7845 RVA: 0x001A5BB8 File Offset: 0x001A3DB8
[postfix]
Class180.method_12()
{
    // add this at the end, before "return text3;"
    var uri = new Uri(url);
    var path = (System.IO.Directory.GetCurrentDirectory() + "\\HTTP_DATA\\").Replace("\\\\", "\\");
    var file = uri.LocalPath.Replace('/', '.').Remove(0, 1);
    var time = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    if (System.IO.Directory.CreateDirectory(path).Exists)
    {
        // in case you turn this into a patch, text3 = __result
        System.IO.File.WriteAllText($@"{path}resp.{file}_{time}.json", text3);
    }
}

### Disable SSL certification

```cs
// Token: 0x06005017 RID: 20503 RVA: 0x0027CF90 File Offset: 0x0027B190
[prefix]
Class519.ValidateCertificate()
{
    return true;
}
```

### Battleye

```cs
// Token: 0x06006ABB RID: 27323 RVA: 0x002D3354 File Offset: 0x002D1554
[prefix]
Class833.RunValidation()
{
    this.Succeed = true;
}
```

## FilesChecker.dll

### Consistency multi

```cs
// Token: 0x06000054 RID: 84 RVA: 0x00002A38 File Offset: 0x00000C38
// target with return type: Task<ICheckResult>
[prefix]
ConsistencyController.EnsureConsistency()
{
    return Task.FromResult<ICheckResult>(ConsistencyController.CheckResult.Succeed(new TimeSpan()));
}
```

### Consistency single

```cs
// Token: 0x06000053 RID: 83 RVA: 0x000028D4 File Offset: 0x00000AD4
[prefix]
ConsistencyController.EnsureConsistencySingle()
{
    return Task.FromResult<ICheckResult>(ConsistencyController.CheckResult.Succeed(new TimeSpan()));
}
```
