This is missing App.Config to ensure the connection strings are not checked in

ConnectionStings are the only node that is required, example:

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="display name" connectionString="Server=[server];Database=[database];Integrated Security=True" />  
    <add name="display name 2" connectionString="Server=[server];Database=[database];Integrated Security=True" />  
  </connectionStrings>
</configuration>

