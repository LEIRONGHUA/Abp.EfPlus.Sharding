This project is a project that integrates [Abp](https://github.com/abpframework/abp) 、 [EntityFramework-Plus](https://github.com/zzzprojects/EntityFramework-Plus) and [sharding-core](https://github.com/dotnetcore/sharding-core) to reproduce the problem [issues #831](https://github.com/zzzprojects/EntityFramework-Plus/issues/831)

Steps to reproduce

1: Clone code

2: Modify the default connection string in the `test/Abp.EfPlus.Sharding.TestBase/appsettings.json` file.

3: Run the unit test case `test/Abp.EfPlus.Sharding.EfCore.Tests/EfCoreCustomer_Tests.cs`

4: See result
<img width="2560" height="1540" alt="image" src="https://github.com/user-attachments/assets/17c19dee-b474-4aa1-8518-9e8961bd805d" />

